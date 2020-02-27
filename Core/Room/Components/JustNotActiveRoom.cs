using Unity.Entities;

namespace Parabole.RoomSystem.Core.Room.Components
{
	public struct JustNotActiveRoom : IComponentData
	{
		
	}

	public class JustNotActiveRoomResetSystem : BaseRoomEventsResetSystem<JustNotActiveRoom>
	{
		
	}
}