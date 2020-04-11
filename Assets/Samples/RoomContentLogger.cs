using Unity.Collections;
using Unity.Entities;

namespace Samples
{
	public struct RoomContentLogger : IComponentData
	{
		public FixedString32 GameObjectName;
	}
}