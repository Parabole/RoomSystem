using Parabole.RoomSystem.Core.Room;
using Parabole.RoomSystem.Core.Room.Components;
using Unity.Entities;

namespace Parabole.RoomSystem.BasicContent
{
	[UpdateInGroup(typeof(RoomContentUpdateGroup))]
	public class RoomContentLightSystem : SystemBase
	{
		protected override void OnUpdate()
		{
			Entities.WithoutBurst().WithAll<JustVisibleRoom>().ForEach((RoomContentLight content) =>
			{
				var lights = content.Lights;
				for (int i = 0; i < lights.Length; i++)
				{
					lights[i].enabled = true;
				}
			}).Run();
			
			Entities.WithoutBurst().WithAll<JustNotVisibleRoom>().ForEach((RoomContentLight content) =>
			{
				var lights = content.Lights;
				for (int i = 0; i < lights.Length; i++)
				{
					lights[i].enabled = false;
				}
			}).Run();
		}
	}
}