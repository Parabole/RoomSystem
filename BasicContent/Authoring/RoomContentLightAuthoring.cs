using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using UnityEngine;

namespace Parabole.RoomSystem.BasicContent.Authoring
{
	public class RoomContentLightAuthoring : MonoBehaviour, IConvertGameObjectToEntity
	{
		[SerializeField] private GameObject[] lightParents = null;
		
		public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
			var lightList = new List<Light>();
			for (int i = 0; i < lightParents.Length; i++)
			{
				var parent = lightParents[i];
				if (parent == null)
				{
					Debug.LogError($"Unassigned light parent in {gameObject.name}", gameObject);
					continue;
				}
				var currentRenderers = lightParents[i].GetComponentsInChildren<Light>();
				lightList.AddRange(currentRenderers);
			}

			AssignLightArray(entity, dstManager, lightList);
		}

		private static void AssignLightArray(Entity entity, EntityManager dstManager, List<Light> lightList)
		{
			var lights = lightList.Distinct().ToArray();
			for (int i = 0; i < lights.Length; i++)
			{
				lights[i].enabled = false;
			}

			var lightContent = new RoomContentLight
			{
				Lights = lights,
			};

			dstManager.AddComponentData(entity, lightContent);
		}
	}
}