using Parabole.RoomSystem.Core.Portal.Components;
using Parabole.RoomSystem.Core.Room;
using Parabole.RoomSystem.Core.Room.Components;
using RoomSystem.Core.Portal;
using Unity.Collections;
using Unity.Entities;

namespace RoomSystem.Core.Room
{
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
				ComponentType.ReadOnly<JustActiveRoom>(),
				ComponentType.ReadOnly<RoomPortalReference>());
			
			RequireForUpdate(newActiveQuery);
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
			Entities.WithAll<JustActiveRoom>()
				.ForEach((Entity entity, DynamicBuffer<RoomPortalReference> portalReferences) =>
				{
					AddUnion(localVisibleEntities, entity);
					for (int i = 0; i < portalReferences.Length; i++)
					{
						var portalEntity = portalReferences[i].Entity;
						var portal = portalFromEntity[portalEntity];
						if (portal.IsOpen)
						{
							AddUnion(localVisibleEntities, portalEntity);
						}
					}
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
					AddUnion(localStandbyEntities, visibleEntity);
					CheckStandby(localStandbyEntities, portalReferences, portalFromEntity);
				}
			}).Run();
		}

		private static void CheckStandby(NativeList<Entity> standbyEntities, 
			DynamicBuffer<RoomPortalReference> portalReferences, 
			ComponentDataFromEntity<RoomPortal> portalFromEntity)
		{
			for (int i = 0; i < portalReferences.Length; i++)
			{
				var portalEntity = portalReferences[i].Entity;
                var portal = portalFromEntity[portalEntity];
                if (portal.IsAccessible)
                {
                	AddUnion(standbyEntities, portalEntity);
                }
			}
		}
		
		private static void AddUnion(NativeList<Entity> entities, Entity entity)
		{
			if (!entities.Contains(entity))
			{
				entities.Add(entity);
			}
		}
	}
}