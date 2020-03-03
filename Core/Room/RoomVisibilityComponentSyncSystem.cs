using Parabole.RoomSystem.Core.Room;
using Parabole.RoomSystem.Core.Room.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace RoomSystem.Core.Room
{
	[UpdateInGroup(typeof(RoomUpdateRequestGroup))]
	[UpdateAfter(typeof(RoomVisibilityListingSystem))]
	public class RoomVisibilityComponentSyncSystem : SystemBase
	{
		private RoomVisibilityListingSystem listingSystem;
		
		private NativeList<Entity> removeVisibleList = new NativeList<Entity>(Allocator.Persistent);
		private NativeList<Entity> removeStandbyList = new NativeList<Entity>(Allocator.Persistent);
		private NativeList<Entity> addVisibleList = new NativeList<Entity>(Allocator.Persistent);
		private NativeList<Entity> addStandbyList = new NativeList<Entity>(Allocator.Persistent);
		
		protected override void OnCreate()
		{
			listingSystem = World.GetOrCreateSystem<RoomVisibilityListingSystem>();
		}

		protected override void OnDestroy()
		{
			removeVisibleList.Dispose();
			removeStandbyList.Dispose();
			addVisibleList.Dispose();
			addStandbyList.Dispose();
		}

		protected override void OnUpdate()
		{
			var inputDependencies = Dependency;
			
			var visibleHandle = StartVisibleJobs(inputDependencies);
			var standbyHandle = StartStandbyJobs(inputDependencies);
			
			JobHandle.CombineDependencies(visibleHandle, standbyHandle).Complete();

			AddAndRemoveComponents();
		}

		private JobHandle StartVisibleJobs(JobHandle inputDependencies)
		{
			var newEntities = listingSystem.VisibleEntities;
			
			var localRemoveList = removeVisibleList;
			var localAddList = addVisibleList;
						
			localRemoveList.Clear();
			localAddList.Clear();
			
			var removeHandle = Entities.WithAll<RoomDefinition, VisibleRoom>().WithReadOnly(newEntities)
				.ForEach((Entity entity, DynamicBuffer<RoomContentReference> buffer) =>
				{
					CheckRemove(entity, buffer, newEntities, localRemoveList);
				}).Schedule(inputDependencies);

			var addHandle = Entities.WithAll<RoomDefinition>().WithNone<VisibleRoom>().WithReadOnly(newEntities)
				.ForEach((Entity entity, DynamicBuffer<RoomContentReference> buffer) =>
				{
					CheckAdd(entity, buffer, newEntities, localAddList);
				}).Schedule(inputDependencies);
			
			return JobHandle.CombineDependencies(removeHandle, addHandle);
		}

		private JobHandle StartStandbyJobs(JobHandle inputDependencies)
		{
			var newEntities = listingSystem.StandbyEntities;

			var localRemoveList = removeStandbyList;
			var localAddList = addStandbyList;
			
			localRemoveList.Clear();
			localAddList.Clear();
			
			var removeHandle = Entities.WithAll<RoomDefinition, StandbyRoom>().WithReadOnly(newEntities)
				.ForEach((Entity entity, DynamicBuffer<RoomContentReference> buffer) =>
				{
					CheckRemove(entity, buffer, newEntities, localRemoveList);
				}).Schedule(inputDependencies);

			var addHandle = Entities.WithAll<RoomDefinition>().WithNone<StandbyRoom>().WithReadOnly(newEntities)
				.ForEach((Entity entity, DynamicBuffer<RoomContentReference> buffer) =>
				{
					CheckAdd(entity, buffer, newEntities, localAddList);
				}).Schedule(inputDependencies);
			
			return JobHandle.CombineDependencies(removeHandle, addHandle);
		}

		private static void CheckRemove(Entity entity, DynamicBuffer<RoomContentReference> buffer, 
			NativeList<Entity> newEntities, NativeList<Entity> removeList)
		{
			if (newEntities.Contains(entity))
			{
				return;
			}
			
			removeList.Add(entity);
			AddBufferEntitiesToList(buffer, removeList);
		}

		private static void CheckAdd(Entity entity, DynamicBuffer<RoomContentReference> buffer, 
			NativeList<Entity> newEntities, NativeList<Entity> addList)
		{
			if (!newEntities.Contains(entity))
			{
				return;
			}
			
			addList.Add(entity);
			AddBufferEntitiesToList(buffer, addList);
		}

		private static void AddBufferEntitiesToList(DynamicBuffer<RoomContentReference> buffer, NativeList<Entity> list)
		{
			for (int i = 0; i < buffer.Length; i++)
			{
				list.Add(buffer[i].Entity);
			}
		}

		private void AddAndRemoveComponents()
		{
			EntityManager.AddComponent<VisibleRoom>(addVisibleList.AsArray());
			EntityManager.AddComponent<JustVisibleRoom>(addVisibleList.AsArray());

			EntityManager.AddComponent<StandbyRoom>(addStandbyList.AsArray());
			EntityManager.AddComponent<JustStandbyRoom>(addStandbyList.AsArray());

			EntityManager.RemoveComponent<VisibleRoom>(removeVisibleList.AsArray());
			EntityManager.AddComponent<JustNotVisibleRoom>(removeVisibleList.AsArray());

			EntityManager.RemoveComponent<StandbyRoom>(removeStandbyList.AsArray());
			EntityManager.AddComponent<JustNotStandbyRoom>(removeStandbyList.AsArray());
		}
	}
}