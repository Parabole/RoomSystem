using System;
using Parabole.InteractionSystem.Runtime.Triggers;
using Parabole.RoomSystem.Core.Room;
using Parabole.RoomSystem.Core.Room.Components;
using Unity.Collections;
using Unity.Entities;

namespace Parabole.RoomSystem.TriggerIntegration
{
	[AlwaysUpdateSystem]
	[UpdateInGroup(typeof(RoomUpdateGroup))]
	public class RoomTriggerSystem : SystemBase
	{
		private EntityQuery currentlyActiveQuery;
		private EntityQuery fillerQuery;

		protected override void OnCreate()
		{
			currentlyActiveQuery = GetEntityQuery(ComponentType.ReadOnly<ActiveRoomSelected>());
			
			fillerQuery = GetEntityQuery(
				ComponentType.ReadOnly<RoomNetworkFillerTrigger>(),
				ComponentType.ReadOnly<TriggerStay>());
		}

		protected override void OnUpdate()
		{
			var newSelectedEntity = Entity.Null;
			var isAlreadyActive = false;

			int currentPriority = Int32.MaxValue;
			Entities.WithoutBurst().WithAny<TriggerStay>().ForEach((in RoomTrigger trigger) =>
			{
				if (trigger.Priority <= currentPriority)
				{
					currentPriority = trigger.Priority;
					newSelectedEntity = trigger.RoomEntity;
					if (EntityManager.HasComponent<ActiveRoomSelected>(newSelectedEntity))
					{
						isAlreadyActive = true;
					}
				}
			}).Run();

			RequestNewRoom(isAlreadyActive, newSelectedEntity);
		}

		private void RequestNewRoom(bool isAlreadyActive, Entity newEntity)
		{
			if (isAlreadyActive)
			{
				return;
			}
			
			if (newEntity != Entity.Null)
			{
				RemoveActive();
				EntityManager.AddComponent<ActiveRoomSelected>(newEntity);
			}
			else if (fillerQuery.CalculateEntityCount() == 0)
			{
				RemoveActive();
			}
		}

		private void RemoveActive()
		{
			EntityManager.RemoveComponent<ActiveRoomSelected>(currentlyActiveQuery);
		}
	}
}