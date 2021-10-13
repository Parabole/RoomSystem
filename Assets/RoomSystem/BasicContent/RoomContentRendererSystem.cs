using Parabole.RoomSystem.Core.Room;
using Parabole.RoomSystem.Core.Room.Components;
using Unity.Entities;
using UnityEngine;

namespace Parabole.RoomSystem.BasicContent
{
	[UpdateInGroup(typeof(RoomContentUpdateGroup))]
	public class RoomContentRendererSystem : SystemBase
	{
		protected override void OnUpdate()
		{
			Entities.WithoutBurst().WithAll<JustVisibleRoom>().ForEach((Entity entity, RoomContentRenderer content) =>
			{
				var renderers = content.Renderers;
				for (int i = 0; i < renderers.Length; i++)
				{
					SetRenderer(entity, renderers[i], true);
				}
			}).Run();
			
			Entities.WithoutBurst().WithAll<JustNotVisibleRoom>().ForEach((Entity entity, RoomContentRenderer content) =>
			{
				var renderers = content.Renderers;
				for (int i = 0; i < renderers.Length; i++)
				{
					SetRenderer(entity, renderers[i], false);
				}
			}).Run();
		}

		private void SetRenderer(Entity entity, Renderer renderer, bool value)
		{
			if (renderer == null)
			{
				var name = GetName(EntityManager, entity);
				Debug.LogError($"Renderer on room content has been destroyed on entity {name}");
				return;
			}
			
			renderer.enabled = value;
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