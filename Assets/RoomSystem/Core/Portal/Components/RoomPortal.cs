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

		/// <summary>
		/// Limit the creation of other portals passing through this one
		/// </summary>
		public bool IsLimited;

		public Entity EntityRoomA;
		
		public Entity EntityRoomB;
	}
}