using Unity.Entities;

namespace Parabole.RoomSystem.Core.Content.Components
{
	public struct RoomContentDynamicLink : IBufferElementData
	{
		public int NameHash;
		public bool IsLinked;
	}
}