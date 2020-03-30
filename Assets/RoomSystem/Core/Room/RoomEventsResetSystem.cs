using Unity.Entities;

namespace Parabole.RoomSystem.Core.Room
{
	[UpdateInGroup(typeof(RoomUpdateGroup))]
	[UpdateBefore(typeof(ActiveRoomSelectedUpdateSystem))]
	[UpdateBefore(typeof(RoomContentDynamicLinkSystem))]
	public class BaseRoomEventsResetSystem<T> : SystemBase where T : struct, IComponentData
	{
		private EntityQuery query;

		protected override void OnCreate()
		{
			query = GetEntityQuery(ComponentType.ReadWrite<T>());
		}

		protected override void OnUpdate()
		{
			EntityManager.RemoveComponent<T>(query);
		}
	}
}