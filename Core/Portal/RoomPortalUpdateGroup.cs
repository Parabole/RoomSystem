using Parabole.RoomSystem.Core.Room;
using Parabole.RoomSystem.Core.Room.Components;
using Unity.Entities;
using UnityEngine.Scripting;

namespace Parabole.RoomSystem.Core.Portal
{
	[UpdateInGroup(typeof(RoomUpdateRequestGroup))]
	public class RoomPortalUpdateGroup : ComponentSystemGroup
	{
		[Preserve]
		public RoomPortalUpdateGroup()
		{
			
		}
	}
}