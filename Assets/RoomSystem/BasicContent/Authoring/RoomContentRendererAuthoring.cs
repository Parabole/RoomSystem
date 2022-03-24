using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using UnityEngine;

namespace Parabole.RoomSystem.BasicContent.Authoring
{
	public class RoomContentRendererAuthoring : MonoBehaviour, IConvertGameObjectToEntity
	{
		[SerializeField] private GameObject[] rendererParents = null;

		public GameObject[] RendererParents => rendererParents;

		public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
			var rendererList = new List<Renderer>();
			for (int i = 0; i < rendererParents.Length; i++)
			{
				var parent = rendererParents[i];
				if (parent == null)
				{
					Debug.LogError($"Unassigned renderer parent in {gameObject.name}", gameObject);
					continue;
				}
				var currentRenderers = rendererParents[i].GetComponentsInChildren<Renderer>();
				rendererList.AddRange(currentRenderers);
			}
			
			AssignArray(entity, dstManager, rendererList);
		}

		private static void AssignArray(Entity entity, EntityManager dstManager, List<Renderer> rendererList)
		{
			var renderers = rendererList.Distinct().ToArray();

			for (int i = 0; i < renderers.Length; i++)
			{
				renderers[i].enabled = false;
			}

			dstManager.AddComponentData(entity, new RoomContentRenderer
			{
				Renderers = renderers,
			});
		}
	}
}