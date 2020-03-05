using Unity.Entities;

namespace Parabole.RoomSystem.Core.Room.Components
{
	/// <summary>
	/// Base room component
	/// </summary>
	public struct RoomDefinition : IComponentData
	{
		public int NameHash;
	}
}