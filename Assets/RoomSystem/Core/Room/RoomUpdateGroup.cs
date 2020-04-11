using Unity.Entities;
using UnityEngine.Scripting;

namespace Parabole.RoomSystem.Core.Room
{
	public class RoomUpdateGroup : ComponentSystemGroup
	{
		[Preserve]
		public RoomUpdateGroup()
		{
		}
	}
}