using Unity.Entities;

namespace Parabole.RoomSystem.Core.Room.Components
{
	public struct JustStandbyRoom : IComponentData
	{
		
	}

	public class JustStandbyRoomResetSystem : BaseRoomEventsResetSystem<JustStandbyRoom>
	{
		
	}
}