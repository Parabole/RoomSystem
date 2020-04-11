using Unity.Entities;

namespace Parabole.RoomSystem.Core.Room.Components
{
	public struct JustNotStandbyRoom : IComponentData
	{
		
	}

	public class JustNotStandbyRoomResetSystem : BaseRoomEventsResetSystem<JustNotStandbyRoom>
	{
		
	}
}