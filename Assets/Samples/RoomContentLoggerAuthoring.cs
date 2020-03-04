using Unity.Entities;
using UnityEngine;

namespace Samples
{
	public class RoomContentLoggerAuthoring : MonoBehaviour, IConvertGameObjectToEntity
	{
		public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
			dstManager.AddComponentData(entity, new RoomContentLogger
			{
				GameObjectName = gameObject.name,
			});
		}
	}
}