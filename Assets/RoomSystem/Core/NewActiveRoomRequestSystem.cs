using RoomSystem.Core.Components;
using Unity.Entities;
using UnityEngine;

namespace RoomSystem.Core
{
	[UpdateInGroup(typeof(RoomUpdateGroup))]
	public class NewActiveRoomRequestSystem : SystemBase
	{
		private EntityQuery currentActiveQuery;
		private EntityQuery requestQuery;
		
		protected override void OnCreate()
		{
			currentActiveQuery = GetEntityQuery(ComponentType.ReadWrite<ActiveRoom>());
			
			requestQuery = GetEntityQuery(ComponentType.ReadWrite<NewActiveRoomRequest>());
			RequireForUpdate(requestQuery);
		}
		
		protected override void OnUpdate()
		{
			Entities.WithStructuralChanges().ForEach((Entity requestEntity, ref NewActiveRoomRequest request) =>
			{
				var newRoomEntity = request.RoomEntity;
				if (EntityManager.HasComponent<ActiveRoom>(newRoomEntity))
				{
					Debug.LogError("Requested to set already active room to new active room");
				}
				else
				{
					ClearCurrentActive();
					SetNewRoomActive(newRoomEntity);
				}
			}).Run();
			
			EntityManager.DestroyEntity(requestQuery);
		}

		private void SetNewRoomActive(Entity newRoomEntity)
		{
			EntityManager.AddComponent<ActiveRoom>(newRoomEntity);
		}

		private void ClearCurrentActive()
		{
			EntityManager.RemoveComponent<ActiveRoom>(currentActiveQuery);
		}
	}
}