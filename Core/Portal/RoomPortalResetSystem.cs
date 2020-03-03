using Parabole.RoomSystem.Core.Portal.Components;
using Parabole.RoomSystem.Core.Room;
using Unity.Entities;

namespace RoomSystem.Core.Portal
{
	[UpdateInGroup(typeof(RoomUpdateRequestGroup))]
	[UpdateBefore(typeof(RoomPortalUpdateGroup))]
	public class RoomPortalResetSystem : SystemBase
	{
		protected override void OnUpdate()
		{
			Entities.ForEach((ref RoomPortal roomPortal) =>
			{
				roomPortal.IsAccessible = true;
				roomPortal.IsOpen = true;
			}).Schedule();
		}
	}
}