using Parabole.InteractionSystem.Runtime.Triggers;
using Parabole.RoomSystem.Core.Room;
using Parabole.RoomSystem.Core.Room.Components;
using Unity.Entities;
using Unity.Jobs;

namespace Parabole.RoomSystem.TriggerIntegration
{
	[AlwaysUpdateSystem]
	[AlwaysSynchronizeSystem]
	[UpdateInGroup(typeof(RoomUpdateGroup))]
	public class RoomTriggerSystem : JobComponentSystem
	{
		private EntityQuery currentlyActiveQuery;

		protected override void OnCreate()
		{
			currentlyActiveQuery = GetEntityQuery(ComponentType.ReadOnly<ActiveRoomSelected>());
		}

		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			var newSelectedEntity = Entity.Null;
			var isAlreadyActive = false;

			Entities.WithoutBurst().WithAny<TriggerStay>().ForEach((in RoomTrigger trigger) =>
			{
				var currentEntity = trigger.RoomEntity;
				if (EntityManager.HasComponent<ActiveRoom>(currentEntity)) isAlreadyActive = true;
			}).Run();

			RequestNewRoom(isAlreadyActive, newSelectedEntity);

			return default;
		}

		private void RequestNewRoom(bool isAlreadyActive, Entity newEntity)
		{
			if (isAlreadyActive) return;

			EntityManager.RemoveComponent<ActiveRoomSelected>(currentlyActiveQuery);

			if (newEntity != Entity.Null) EntityManager.AddComponent<ActiveRoomSelected>(currentlyActiveQuery);
		}
	}
}