using Parabole.RoomSystem.Core.Room.Components;
using RoomSystem.Core.Room;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Parabole.RoomSystem.Core.Room
{
	[UpdateInGroup(typeof(RoomUpdateGroup))]
	[UpdateAfter(typeof(ActiveRoomSelectedUpdateSystem))]
	[UpdateAfter(typeof(RoomForceUpdateSystem))]
	public class RoomUpdateRequestGroup : ComponentSystemGroup
	{
		[Preserve]
		public RoomUpdateRequestGroup()
		{
			
		}

		protected override void OnCreate()
		{
			var query = GetEntityQuery(ComponentType.ReadWrite<RoomUpdateRequest>());
			
			RequireForUpdate(query);
		}
	}
}