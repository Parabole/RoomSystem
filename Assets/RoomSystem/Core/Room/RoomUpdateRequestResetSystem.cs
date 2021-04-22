using Parabole.RoomSystem.Core.Room.Components;
using Unity.Entities;

namespace Parabole.RoomSystem.Core.Room
{
	/// <summary>
	/// Just before checking for new request, remove last frame's request, if any
	/// </summary>
	[UpdateBefore(typeof(ActiveRoomSelectedUpdateSystem))]
	[UpdateBefore(typeof(RoomContentDynamicLinkSystem))]
	[UpdateInGroup(typeof(RoomUpdateGroup))]
	public class RoomUpdateRequestResetSystem : SystemBase
	{
		private EntityQuery query;
		
		protected override void OnCreate()
		{
			query = GetEntityQuery(ComponentType.ReadWrite<RoomUpdateRequest>());
			
			RequireForUpdate(query);
		}

		protected override void OnUpdate()
		{
			EntityManager.DestroyEntity(query);
		}
	}
}