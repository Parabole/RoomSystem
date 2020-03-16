using Unity.Entities;

namespace Parabole.RoomSystem.Core.Portal.Components
{
	public struct RoomPortalChain : IComponentData
	{
		public Entity PortalEntityA;
		public Entity PortalEntityB;
		public bool ArePortalsOpen;
		public bool ArePortalsAccessible;
	}
}