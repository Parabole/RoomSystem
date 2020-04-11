using Unity.Entities;

namespace Parabole.RoomSystem.TriggerIntegration
{
	public struct RoomTrigger : IComponentData
	{
		public Entity RoomEntity;
		public int Priority;
	}
}