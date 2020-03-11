using System.Linq;
using Unity.Entities;
using UnityEngine;

namespace Parabole.RoomSystem.BasicContent.Authoring
{
	[RequiresEntityConversion]
	public class RoomContentRendererAuthoring : MonoBehaviour, IConvertGameObjectToEntity
	{
		[SerializeField] private GameObject[] rendererParents = null;
		
		public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
			var renderers = rendererParents.SelectMany(x => x.GetComponentsInChildren<Renderer>()).Distinct().ToArray();

			for (int i = 0; i < renderers.Length; i++)
			{
				renderers[i].enabled = false;
			}
			
			var rendererContent = new RoomContentRenderer
			{
				Renderers = renderers,
			};

			dstManager.AddComponentData(entity, rendererContent);
		}
	}
}