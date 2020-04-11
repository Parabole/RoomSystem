using Unity.Entities;
using UnityEngine;

namespace Parabole.RoomSystem.BasicContent
{
	public class RoomContentLight : IComponentData
	{
		public Light[] Lights;
	}
}