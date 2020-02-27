using Unity.Entities;

namespace RoomSystem.Core
{
	[UpdateInGroup(typeof(RoomUpdateGroup))]
	[UpdateBefore(typeof(NewActiveRoomRequestSystem))]
	public class RoomEventsResetSystem : SystemBase
	{
		protected override void OnCreate()
		{
			
		}

		protected override void OnUpdate()
		{
		}
	}
}