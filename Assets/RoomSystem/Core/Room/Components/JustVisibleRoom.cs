using Unity.Entities;

namespace Parabole.RoomSystem.Core.Room.Components
{
	public struct JustVisibleRoom : IComponentData
	{
		
	}

	public class JustVisibleRoomResetSystem : BaseRoomEventsResetSystem<JustVisibleRoom>
	{
		
	}
}