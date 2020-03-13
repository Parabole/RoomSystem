using Parabole.RoomSystem.Core.Helper;
using Parabole.RoomSystem.Core.Portal.Components;
using Parabole.RoomSystem.Core.Room;
using Parabole.RoomSystem.Core.Room.Components;
using RoomSystem.Core.Portal;
using Unity.Collections;
using Unity.Entities;

namespace RoomSystem.Core.Room
{
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
			}
		}

		private void FillLists()
		{
			var localVisibleEntities = visibleEntities;
			var localStandbyEntities = standbyEntities;
			
			var portalFromEntity = GetComponentDataFromEntity<RoomPortal>(true);

			FillVisible(localVisibleEntities, portalFromEntity);

			FillStandby(localVisibleEntities, localStandbyEntities, portalFromEntity);
		}

		private void FillVisible(NativeList<Entity> localVisibleEntities, 
			ComponentDataFromEntity<RoomPortal> portalFromEntity)
		{
			// Only select rooms with RoomDefinition, and not contents
			Entities.WithAll<RoomDefinition, ActiveRoom>()
				.ForEach((Entity entity, DynamicBuffer<RoomPortalReference> portalReferences) =>
				{
					localVisibleEntities.AddUnion(entity);
					CheckVisible(localVisibleEntities, portalReferences, portalFromEntity);
				}).Run();
		}

		private void FillStandby(NativeList<Entity> localVisibleEntities, NativeList<Entity> localStandbyEntities, 
			ComponentDataFromEntity<RoomPortal> portalFromEntity)
		{
			var portalReferencesFromEntity = GetBufferFromEntity<RoomPortalReference>(true);
			
			Job.WithCode(() =>
			{
				for (int i = 0; i < localVisibleEntities.Length; i++)
				{
					var visibleEntity = localVisibleEntities[i];
					var portalReferences = portalReferencesFromEntity[visibleEntity];
					localStandbyEntities.AddUnion(visibleEntity);
					CheckStandby(localStandbyEntities, portalReferences, portalFromEntity);
				}
			}).Run();
		}

		private static void CheckVisible(NativeList<Entity> visibleEntities,
			DynamicBuffer<RoomPortalReference> portalReferences, 
			ComponentDataFromEntity<RoomPortal> portalFromEntity)
		{
			for (int i = 0; i < portalReferences.Length; i++)
			{
				var portalEntity = portalReferences[i].PortalEntity;
				var portal = portalFromEntity[portalEntity];
				if (portal.IsOpen)
				{
					visibleEntities.AddUnion(portalReferences[i].LinkedRoomEntity);
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