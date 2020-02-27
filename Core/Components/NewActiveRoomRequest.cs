using Unity.Entities;

namespace RoomSystem.Core.Components
{
	public struct NewActiveRoomRequest : IComponentData
	{
		public Entity RoomEntity;
	}
}