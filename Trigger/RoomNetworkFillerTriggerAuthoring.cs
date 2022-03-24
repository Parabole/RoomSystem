using Parabole.RoomSystem.TriggerIntegration;
using Unity.Entities;
using UnityEngine;

namespace DefaultNamespace
{
	public class RoomNetworkFillerTriggerAuthoring : MonoBehaviour, IConvertGameObjectToEntity
	{
		public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
			dstManager.AddComponent<RoomNetworkFillerTrigger>(entity);
		}
	}
}