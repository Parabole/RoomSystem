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

		private EntityArchetype requestArchetype;
		
		protected override void OnCreate()
		{
			requestArchetype = EntityManager.CreateArchetype(ComponentType.ReadWrite<RoomUpdateRequest>());
			
			requestQuery = GetEntityQuery(ComponentType.ReadWrite<RoomUpdateRequest>());
			forceRequestQuery = GetEntityQuery(ComponentType.ReadWrite<RoomForceUpdateRequest>());
			
			RequireForUpdate(forceRequestQuery);
		}

		protected override void OnUpdate()
		{
			if (requestQuery.IsEmpty)
			{
				EntityManager.CreateEntity(requestArchetype);
			}
			
			EntityManager.DestroyEntity(forceRequestQuery);
		}
	}
}