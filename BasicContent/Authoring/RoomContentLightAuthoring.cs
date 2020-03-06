using System.Linq;
using Parabole.RoomSystem.BasicContent.Components;
using Unity.Entities;
using UnityEngine;

namespace Parabole.RoomSystem.BasicContent.Authoring
{
	[RequiresEntityConversion]
	public class RoomContentLightAuthoring : MonoBehaviour, IConvertGameObjectToEntity
	{
		[SerializeField] private GameObject[] lightParents;
		
		public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
			var lights = lightParents.SelectMany(x => x.GetComponentsInChildren<Light>()).Distinct().ToArray();

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