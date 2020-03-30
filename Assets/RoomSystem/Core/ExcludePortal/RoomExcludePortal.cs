using Parabole.RoomSystem.Core.Room.Authoring;
using Unity.Entities;
using UnityEngine;

namespace Parabole.RoomSystem.Core.ExcludePortal
{
	public struct RoomExcludePortal : IComponentData
	{
		public Entity EntityRoomA;
		public Entity EntityRoomB;
	}
}