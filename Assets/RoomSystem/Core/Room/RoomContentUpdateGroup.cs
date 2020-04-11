using Unity.Entities;
using UnityEngine.Scripting;

namespace Parabole.RoomSystem.Core.Room
{
	[UpdateInGroup(typeof(RoomUpdateRequestGroup))]
	[UpdateAfter(typeof(RoomVisibilityComponentSyncSystem))]
	public class RoomContentUpdateGroup : ComponentSystemGroup
	{
		[Preserve]
		public RoomContentUpdateGroup()
		{
			
		}
	}
}