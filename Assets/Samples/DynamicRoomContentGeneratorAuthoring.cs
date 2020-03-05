using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Samples
{
	public class DynamicRoomContentGeneratorAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
	{
		[SerializeField] private GameObject dynamicContentPrefab = null;
		
		public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
			dstManager.AddComponentData(entity, new DynamicRoomContentGenerator
			{
				PrefabEntity = conversionSystem.GetPrimaryEntity(dynamicContentPrefab),
			});
		}

		public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
		{
			referencedPrefabs.Add(dynamicContentPrefab);
		}
	}
}