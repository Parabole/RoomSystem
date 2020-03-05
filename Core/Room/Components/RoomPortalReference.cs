using Unity.Entities;

namespace Parabole.RoomSystem.Core.Room.Components
{
	public struct RoomPortalReference : IBufferElementData
	{
		public Entity PortalEntity;
		public Entity LinkedRoomEntity;
	}
}