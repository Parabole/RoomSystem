using Parabole.RoomSystem.Core.Room;
using Parabole.RoomSystem.Core.Room.Components;
using Unity.Entities;
using UnityEngine;

namespace Parabole.RoomSystem.BasicContent
{
	[UpdateInGroup(typeof(RoomContentUpdateGroup))]
	public class RoomContentLightSystem : SystemBase
	{
		protected override void OnUpdate()
		{
			Entities.WithoutBurst().WithAll<JustVisibleRoom>().ForEach((Entity entity, RoomContentLight content) =>
			{
				var lights = content.Lights;
				for (int i = 0; i < lights.Length; i++)
				{
					SetLight(entity, lights[i], true);
				}
			}).Run();
			
			Entities.WithoutBurst().WithAll<JustNotVisibleRoom>().ForEach((Entity entity, RoomContentLight content) =>
			{
				var lights = content.Lights;
				for (int i = 0; i < lights.Length; i++)
				{
					SetLight(entity, lights[i], false);
				}
			}).Run();
		}

		private void SetLight(Entity entity, Light light, bool value)
		{
			if (light == null)
			{
				var name = GetName(EntityManager, entity);
				Debug.LogError($"Light on room content has been destroyed on entity {name}");
				return;
			}

			light.enabled = value;
		}
		
		private static string GetName(EntityManager manager, Entity entity)
		{
#if UNITY_EDITOR
			return manager.GetName(entity);
#else
			return entity.ToString();
#endif
		}
	}
}