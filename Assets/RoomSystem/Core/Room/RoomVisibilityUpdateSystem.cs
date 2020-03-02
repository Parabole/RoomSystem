using Parabole.RoomSystem.Core.Portal.Components;
using Parabole.RoomSystem.Core.Room;
using Parabole.RoomSystem.Core.Room.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace RoomSystem.Core.Room
{
	[UpdateAfter(typeof(ActiveRoomSelectedUpdateSystem))]
	[UpdateInGroup(typeof(RoomUpdateGroup))]
	public class RoomVisibilityUpdateSystem : SystemBase
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
			var localVisibleEntities = visibleEntities;
			var localStandbyEntities = standbyEntities;
			
			localVisibleEntities.Clear();
			localStandbyEntities.Clear();

			var portalFromEntity = GetComponentDataFromEntity<RoomPortal>(true);
			var portalReferencesFromEntity = GetBufferFromEntity<RoomPortalReference>(true);
			
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
		
		private static void AddUnion(NativeList<Entity> entities, Entity entity)
		{
			if (!entities.Contains(entity))
			{
				entities.Add(entity);
			}
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
	}
}