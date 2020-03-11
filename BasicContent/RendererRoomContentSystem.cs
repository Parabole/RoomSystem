using Parabole.RoomSystem.Core.Room;
using Parabole.RoomSystem.Core.Room.Components;
using Unity.Entities;

namespace Parabole.RoomSystem.BasicContent
{
	[UpdateInGroup(typeof(RoomContentUpdateGroup))]
	public class RendererRoomContentSystem : SystemBase
	{
		protected override void OnUpdate()
		{
			Entities.WithoutBurst().WithAll<JustVisibleRoom>().ForEach((RoomContentRenderer content) =>
			{
				var renderers = content.Renderers;
				for (int i = 0; i < renderers.Length; i++)
				{
					renderers[i].enabled = true;
				}
			}).Run();
			
			Entities.WithoutBurst().WithAll<JustNotVisibleRoom>().ForEach((RoomContentRenderer content) =>
			{
				var renderers = content.Renderers;
				for (int i = 0; i < renderers.Length; i++)
				{
					renderers[i].enabled = false;
				}
			}).Run();
		}
	}
}