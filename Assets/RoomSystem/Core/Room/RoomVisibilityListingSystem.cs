using Parabole.RoomSystem.Core.Helper;
using Parabole.RoomSystem.Core.Portal;
using Parabole.RoomSystem.Core.Portal.Components;
using Parabole.RoomSystem.Core.Room.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace Parabole.RoomSystem.Core.Room
{
	/// <summary>
	/// Fills lists for needed visible/standby rooms.
	/// Since both rooms and content use the same sets of components (RoomVisible, RoomJustVisible, etc),
	/// the system updates them all at the same time and adds them in the same lists.
	/// </summary>
	[AlwaysUpdateSystem]
	[UpdateAfter(typeof(RoomPortalUpdateGroup))]
	[UpdateInGroup(typeof(RoomUpdateRequestGroup))]
	public class RoomVisibilityListingSystem : SystemBase
	{
		private EntityQuery newActiveQuery;
		
		private NativeList<Entity> visibleEntities = new NativeList<Entity>(Allocator.Persistent);
		private NativeList<Entity> standbyEntities = new NativeList<Entity>(Allocator.Persistent);
		
		public NativeList<Entity> VisibleEntities => visibleEntities;
		public NativeList<Entity> StandbyEntities => standbyEntities;

		public JobHandle FillJobHandle = default;

		protected override void OnCreate()
		{
			newActiveQuery = GetEntityQuery(
				ComponentType.ReadOnly<RoomDefinition>(),
				ComponentType.ReadOnly<ActiveRoom>(),
				ComponentType.ReadOnly<RoomPortalReference>());
		}

		protected override void OnDestroy()
		{
			visibleEntities.Dispose();
			standbyEntities.Dispose();
		}

		protected override void OnUpdate()
		{
			visibleEntities.Clear();
			standbyEntities.Clear();

			if (newActiveQuery.CalculateEntityCount() > 0)
			{
				FillLists();
				FillJobHandle = Dependency;
			}
			else
			{
				FillJobHandle = default;
			}
		}

		private void FillLists()
		{
			var portalFromEntity = GetComponentDataFromEntity<RoomPortal>(true);

			FillVisible(visibleEntities, portalFromEntity);
			FillStandby(visibleEntities, standbyEntities, portalFromEntity);
			
			var contentReferenceFromEntity = GetBufferFromEntity<RoomContentReference>(true);
			AddContentsToRoomList(visibleEntities, contentReferenceFromEntity);
			AddContentsToRoomList(standbyEntities, contentReferenceFromEntity);
		}

		private void FillVisible(NativeList<Entity> localVisibleEntities, 
			ComponentDataFromEntity<RoomPortal> portalFromEntity)
		{
			// Only select rooms with RoomDefinition, and not contents
			Entities.WithoutBurst().WithAll<RoomDefinition, ActiveRoom>().WithReadOnly(portalFromEntity).ForEach((Entity entity,
				DynamicBuffer<RoomPortalReference> portalReferences, 
				DynamicBuffer<RoomExcludePortalReference> excludeReferences) =>
				{
					var excludedRoomEntities = new NativeArray<Entity>(excludeReferences.Length, Allocator.Temp);

					for (int i = 0; i < excludedRoomEntities.Length; i++)
					{
						excludedRoomEntities[i] = excludeReferences[i].LinkedRoomEntity;
					}
					
					localVisibleEntities.AddUnion(entity);
					CheckVisible(localVisibleEntities, excludedRoomEntities, portalReferences, portalFromEntity);
					
					excludedRoomEntities.Dispose();
				}).Schedule();
		}

		// TODO: Standby rooms are currently every room connected to a visible room, find a way to check proximity too.
		private void FillStandby(NativeList<Entity> localVisibleEntities, NativeList<Entity> localStandbyEntities, 
			ComponentDataFromEntity<RoomPortal> portalFromEntity)
		{
			var portalReferencesFromEntity = GetBufferFromEntity<RoomPortalReference>(true);
			
			Job.WithReadOnly(portalReferencesFromEntity).WithReadOnly(portalFromEntity).WithCode(() =>
			{
				for (int i = 0; i < localVisibleEntities.Length; i++)
				{
					var visibleEntity = localVisibleEntities[i];
					var portalReferences = portalReferencesFromEntity[visibleEntity];
					localStandbyEntities.AddUnion(visibleEntity);
					CheckStandby(localStandbyEntities, portalReferences, portalFromEntity);
				}
			}).Schedule();
		}

		private void AddContentsToRoomList(NativeList<Entity> entities, 
			BufferFromEntity<RoomContentReference> contentReferencesFromEntity)
		{
			Job.WithReadOnly(contentReferencesFromEntity).WithCode(() =>
			{
				var roomEntities = entities.ToArray(Allocator.Temp);
				for (int i = 0; i < roomEntities.Length; i++)
				{
					var roomEntity = roomEntities[i];
					var buffer = contentReferencesFromEntity[roomEntity];
					AddEntitiesInContentReferences(entities, buffer);
				}
				roomEntities.Dispose();
			}).Schedule();
		}

		private static void AddEntitiesInContentReferences(NativeList<Entity> entities,
			DynamicBuffer<RoomContentReference> buffer)
		{
			for (int i = 0; i < buffer.Length; i++)
			{
				var reference = buffer[i];
				entities.AddUnion(reference.Entity);
			}
		}

		private static void CheckVisible(NativeList<Entity> visibleEntities,
			NativeArray<Entity> excludedRoomEntities,
			DynamicBuffer<RoomPortalReference> portalReferences,
			ComponentDataFromEntity<RoomPortal> portalFromEntity)
		{
			for (int i = 0; i < portalReferences.Length; i++)
			{
				var portalReference = portalReferences[i];
				var portalEntity = portalReference.PortalEntity;
				var linkedRoomEntity = portalReference.LinkedRoomEntity;
				
				if (!excludedRoomEntities.Contains(linkedRoomEntity))
				{
					var portal = portalFromEntity[portalEntity];
					if (portal.IsOpen)
					{
						visibleEntities.AddUnion(linkedRoomEntity);
					}
				}
			}
		}

		private static void CheckStandby(NativeList<Entity> standbyEntities, 
			DynamicBuffer<RoomPortalReference> portalReferences, 
			ComponentDataFromEntity<RoomPortal> portalFromEntity)
		{
			for (int i = 0; i < portalReferences.Length; i++)
			{
				var portalEntity = portalReferences[i].PortalEntity;
                var portal = portalFromEntity[portalEntity];
                if (portal.IsAccessible)
                {
	                standbyEntities.AddUnion(portalReferences[i].LinkedRoomEntity);
                }
			}
		}
	}
}