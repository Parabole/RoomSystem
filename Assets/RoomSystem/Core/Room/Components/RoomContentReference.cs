using Unity.Entities;

namespace Parabole.RoomSystem.Core.Room.Components
{
	public struct RoomContentReference : IBufferElementData
	{
		public Entity Entity;
	}
}