using Unity.Entities;

namespace Parabole.RoomSystem.Core.Room.Components
{
	public struct JustNotVisibleRoom : IComponentData
	{
		
	}

	public class JustNotVisibleRoomResetSystem : BaseRoomEventsResetSystem<JustNotVisibleRoom>
	{
		
	}
}