using Unity.Entities;

namespace Parabole.RoomSystem.Core.Room.Components
{
	public struct RoomExcludePortalReference : IBufferElementData
	{
		public Entity PortalEntity;
		public Entity LinkedRoomEntity;
	}
}