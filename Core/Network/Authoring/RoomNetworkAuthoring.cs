using Unity.Entities;
using UnityEngine;

namespace Parabole.RoomSystem.Core.Network.Authoring
{
	[RequiresEntityConversion]
	public class RoomNetworkAuthoring : MonoBehaviour, IConvertGameObjectToEntity
	{
		public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
			
		}
	}
}