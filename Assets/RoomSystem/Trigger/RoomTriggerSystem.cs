using Parabole.InteractionSystem.Runtime.Triggers;
using Parabole.RoomSystem.Core.Room;
using Parabole.RoomSystem.Core.Room.Components;
using Unity.Entities;
using Unity.Jobs;

namespace Parabole.RoomSystem.TriggerIntegration
{
	[AlwaysUpdateSystem]
	[UpdateInGroup(typeof(RoomUpdateGroup))]
	public class RoomTriggerSystem : SystemBase
	{
		private EntityQuery currentlyActiveQuery;

		protected override void OnCreate()
		{
			currentlyActiveQuery = GetEntityQuery(ComponentType.ReadOnly<ActiveRoomSelected>());
		}

		protected override void OnUpdate()
		{
			var newSelectedEntity = Entity.Null;
			var isAlreadyActive = false;

			Entities.WithoutBurst().WithAny<TriggerStay>().ForEach((in RoomTrigger trigger) =>
			{
				newSelectedEntity = trigger.RoomEntity;
				if (EntityManager.HasComponent<ActiveRoomSelected>(newSelectedEntity)) isAlreadyActive = true;
			}).Run();

			RequestNewRoom(isAlreadyActive, newSelectedEntity);
		}

		private void RequestNewRoom(bool isAlreadyActive, Entity newEntity)
		{
			if (isAlreadyActive)
			{
				return;
			}

			EntityManager.RemoveComponent<ActiveRoomSelected>(currentlyActiveQuery);

			if (newEntity != Entity.Null)
			{
				EntityManager.AddComponent<ActiveRoomSelected>(newEntity);
			}
		}
	}
}