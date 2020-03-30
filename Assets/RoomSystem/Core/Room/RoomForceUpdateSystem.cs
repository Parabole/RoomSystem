using Parabole.RoomSystem.Core.Room;
using Parabole.RoomSystem.Core.Room.Components;
using Unity.Entities;

namespace Parabole.RoomSystem.Core.Room
{
	[UpdateInGroup(typeof(RoomUpdateGroup))]
	[UpdateAfter(typeof(ActiveRoomSelectedUpdateSystem))]
	public class RoomForceUpdateSystem : SystemBase
	{
		private EntityQuery forceRequestQuery;
		private EntityQuery requestQuery;

		protected override void OnCreate()
		{
			requestQuery = GetEntityQuery(ComponentType.ReadWrite<RoomUpdateRequest>());
			forceRequestQuery = GetEntityQuery(ComponentType.ReadWrite<RoomForceUpdateRequest>());
			
			RequireForUpdate(forceRequestQuery);
		}

		protected override void OnUpdate()
		{
			if (requestQuery.CalculateEntityCount() == 0)
			{
				EntityManager.CreateEntity(ComponentType.ReadWrite<RoomUpdateRequest>());
			}
			
			EntityManager.DestroyEntity(forceRequestQuery);
		}
	}
}