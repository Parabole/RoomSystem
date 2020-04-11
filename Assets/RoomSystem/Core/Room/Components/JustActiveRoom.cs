using Unity.Entities;

namespace Parabole.RoomSystem.Core.Room.Components
{
	public struct JustActiveRoom : IComponentData
	{
		
	}

	public class JustActiveRoomResetSystem : BaseRoomEventsResetSystem<JustActiveRoom>
	{
		
	}
}