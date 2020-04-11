using Parabole.RoomSystem.Core.Content.Components;
using Parabole.RoomSystem.Core.Helper;
using Parabole.RoomSystem.Core.Room.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace Parabole.RoomSystem.Core.Room
{
	/// <summary>
	/// Sets the right components by comparing the currently visible/standby room and contents.
	/// Since both rooms and content use the same sets of components (RoomVisible, RoomJustVisible, etc),
	/// the system updates them all at the same time.
	/// </summary>
	[AlwaysUpdateSystem]
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
			var inputDeps = JobHandle.CombineDependencies(Dependency, listingSystem.FillJobHandle);

			removeVisibleList.Clear();
			addVisibleList.Clear();
			removeStandbyList.Clear();
			addStandbyList.Clear();
			
			// TODO: Find a way to run those in parallel
			var visibleHandle = StartVisibleJobs(inputDeps);
			var standbyHandle = StartStandbyJobs(visibleHandle);
			
			JobHandle.CombineDependencies(visibleHandle, standbyHandle).Complete();

			AddAndRemoveComponents();
		}

		private JobHandle StartVisibleJobs(JobHandle inputDependencies)
		{
			var neededEntities = listingSystem.VisibleEntities;
			
			var localRemoveList = removeVisibleList;
			var localAddList = addVisibleList;
			
			var removeHandle = Entities.WithAny<RoomContent, RoomDefinition>().WithAll<VisibleRoom>()
				.WithReadOnly(neededEntities).ForEach((Entity entity) =>
			{
				CheckRemove(entity, neededEntities, localRemoveList);
			}).Schedule(inputDependencies);

			var addHandle = Entities.WithAny<RoomContent, RoomDefinition>().WithNone<VisibleRoom>()
				.WithReadOnly(neededEntities).ForEach((Entity entity) =>
			{
				CheckAdd(entity, neededEntities, localAddList);
			}).Schedule(inputDependencies);
			
			return JobHandle.CombineDependencies(addHandle, removeHandle);
		}

		private JobHandle StartStandbyJobs(JobHandle inputDependencies)
		{
			var neededEntities = listingSystem.StandbyEntities;

			var localRemoveList = removeStandbyList;
			var localAddList = addStandbyList;
			
			var removeHandle = Entities.WithAny<RoomContent, RoomDefinition>().WithAll<StandbyRoom>()
				.WithReadOnly(neededEntities).ForEach((Entity entity) =>
				{
					CheckRemove(entity, neededEntities, localRemoveList);
				}).Schedule(inputDependencies);

			var addHandle = Entities.WithAny<RoomContent, RoomDefinition>().WithNone<StandbyRoom>()
				.WithReadOnly(neededEntities).ForEach((Entity entity) =>
				{
					CheckAdd(entity, neededEntities, localAddList);
				}).Schedule(inputDependencies);
			
			return JobHandle.CombineDependencies(addHandle, removeHandle);
		}

		private static void CheckRemove(Entity entity, NativeList<Entity> neededEntities, NativeList<Entity> removeList)
		{
			if (neededEntities.Contains(entity))
			{
				return;
			}
			
			removeList.AddUnion(entity);
		}

		private static void CheckAdd(Entity entity, NativeList<Entity> neededEntities, NativeList<Entity> addList)
		{
			if (!neededEntities.Contains(entity))
			{
				return;
			}
			
			addList.AddUnion(entity);
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