using Parabole.RoomSystem.Core.Portal.Components;
using Parabole.RoomSystem.Core.Room;
using RoomSystem.Core.Room;
using Unity.Entities;

namespace RoomSystem.Core.Portal
{
	[UpdateInGroup(typeof(RoomUpdateRequestGroup))]
	[UpdateAfter(typeof(RoomPortalUpdateGroup))]
	[UpdateBefore(typeof(RoomVisibilityListingSystem))]
	public class RoomPortalChainSystem : SystemBase
	{
		protected override void OnUpdate()
		{
			var portalFromEntity = GetComponentDataFromEntity<RoomPortal>(true);
			
			Entities.WithReadOnly(portalFromEntity).ForEach((ref RoomPortalChain portalChain) =>
			{
				var portalA = portalFromEntity[portalChain.PortalEntityA];
				var portalB = portalFromEntity[portalChain.PortalEntityB];
					
				portalChain.ArePortalsOpen = portalA.IsOpen && portalB.IsOpen;
				portalChain.ArePortalsAccessible = portalA.IsAccessible && portalB.IsAccessible;
			}).ScheduleParallel();
			
			Entities.ForEach((ref RoomPortal portal, in RoomPortalChain portalChain) =>
			{
				portal.IsOpen &= portalChain.ArePortalsOpen;
				portal.IsAccessible &= portalChain.ArePortalsAccessible;
			}).ScheduleParallel();
		}
	}
}