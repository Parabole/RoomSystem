using Unity.Entities;

namespace Parabole.RoomSystem.Core.Content.Authoring
{
	public struct RoomContentDynamicLink : IBufferElementData
	{
		public int NameHash;
		public bool IsLinked;
	}
}