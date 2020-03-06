using Unity.Entities;
using UnityEngine;

namespace RoomSystem.Core.Network.Authoring
{
	public class RoomNetworkAuthoring : MonoBehaviour, IConvertGameObjectToEntity
	{
		public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
			
		}
	}
}