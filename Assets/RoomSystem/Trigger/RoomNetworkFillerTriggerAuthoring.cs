using RoomSystem.Core.Network.Authoring;
using Unity.Entities;
using UnityEngine;

namespace DefaultNamespace
{
	[RequiresEntityConversion]
	public class RoomNetworkFillerTriggerAuthoring : MonoBehaviour, IConvertGameObjectToEntity
	{
		public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
			dstManager.AddComponent<RoomNetworkAuthoring>(entity);
		}
	}
}