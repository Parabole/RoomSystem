using Unity.Entities;
using UnityEngine;

namespace Parabole.RoomSystem.BasicContent.Components
{
	public class RoomContentLight : IComponentData
	{
		public Light[] Lights;
	}
}