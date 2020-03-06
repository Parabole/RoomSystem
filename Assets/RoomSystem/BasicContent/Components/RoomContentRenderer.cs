using Unity.Entities;
using UnityEngine;

namespace Parabole.RoomSystem.BasicContent.Components
{
	public class RoomContentRenderer : IComponentData
	{
		public Renderer[] Renderers;
	}
}