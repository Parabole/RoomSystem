using Parabole.RoomSystem.Core.Helper;
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
			var neededEntities = listingSystem.VisibleEntities;
			
			var localRemoveList = removeVisibleList;
			var localAddList = addVisibleList;
						
			localRemoveList.Clear();
			localAddList.Clear();
			
			var removeHandle = Entities.WithAll<RoomDefinition, VisibleRoom>().WithReadOnly(neededEntities)
				.ForEach((Entity entity, DynamicBuffer<RoomContentReference> buffer) =>
				{
					CheckRemove(entity, buffer, neededEntities, localRemoveList);
				}).Schedule(inputDependencies);

			var addHandle = Entities.WithAll<RoomDefinition>().WithNone<VisibleRoom>().WithReadOnly(neededEntities)
				.ForEach((Entity entity, DynamicBuffer<RoomContentReference> buffer) =>
				{
					CheckAdd(entity, buffer, neededEntities, localAddList);
				}).Schedule(inputDependencies);
			
			return JobHandle.CombineDependencies(removeHandle, addHandle);
		}

		private JobHandle StartStandbyJobs(JobHandle inputDependencies)
		{
			var neededEntities = listingSystem.StandbyEntities;

			var localRemoveList = removeStandbyList;
			var localAddList = addStandbyList;
			
			localRemoveList.Clear();
			localAddList.Clear();
			
			var removeHandle = Entities.WithAll<RoomDefinition, StandbyRoom>().WithReadOnly(neededEntities)
				.ForEach((Entity entity, DynamicBuffer<RoomContentReference> buffer) =>
				{
					CheckRemove(entity, buffer, neededEntities, localRemoveList);
				}).Schedule(inputDependencies);

			var addHandle = Entities.WithAll<RoomDefinition>().WithNone<StandbyRoom>().WithReadOnly(neededEntities)
				.ForEach((Entity entity, DynamicBuffer<RoomContentReference> buffer) =>
				{
					CheckAdd(entity, buffer, neededEntities, localAddList);
				}).Schedule(inputDependencies);
			
			return JobHandle.CombineDependencies(removeHandle, addHandle);
		}

		private static void CheckRemove(Entity entity, DynamicBuffer<RoomContentReference> buffer, 
			NativeList<Entity> neededEntities, NativeList<Entity> removeList)
		{
			if (neededEntities.Contains(entity))
			{
				return;
			}
			
			removeList.AddUnion(entity);
			AddBufferEntitiesToList(buffer, removeList);
		}

		private static void CheckAdd(Entity entity, DynamicBuffer<RoomContentReference> buffer, 
			NativeList<Entity> neededEntities, NativeList<Entity> addList)
		{
			if (!neededEntities.Contains(entity))
			{
				return;
			}
			
			addList.AddUnion(entity);
			AddBufferEntitiesToList(buffer, addList);
		}

		private static void AddBufferEntitiesToList(DynamicBuffer<RoomContentReference> buffer, NativeList<Entity> list)
		{
			for (int i = 0; i < buffer.Length; i++)
			{
				list.AddUnion(buffer[i].Entity);
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