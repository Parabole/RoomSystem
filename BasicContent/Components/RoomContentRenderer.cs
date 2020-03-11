using Unity.Entities;
using UnityEngine;

namespace Parabole.RoomSystem.BasicContent
{
	public class RoomContentRenderer : IComponentData
	{
		public Renderer[] Renderers;
	}
}