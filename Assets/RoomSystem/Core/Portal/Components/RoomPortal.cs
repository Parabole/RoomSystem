using Unity.Entities;

namespace Parabole.RoomSystem.Core.Portal.Components
{
	public struct RoomPortal : IComponentData
	{
		/// <summary>
		/// The portal can be seen through
		/// </summary>
		public bool IsOpen;
		
		/// <summary>
		/// The portal can be opened in the near future
		/// </summary>
		public bool IsAccessible;

		public Entity EntityRoomA;
		
		public Entity EntityRoomB;
	}
}