using Parabole.InteractionSystem.Triggers;
using Parabole.RoomSystem.TriggerIntegration;
using Unity.Entities;
using UnityEngine;

namespace Parabole.RoomSystem.Core.Room.Authoring
{
	[RequiresEntityConversion]
	public class RoomTriggerAuthoring : MonoBehaviour, IConvertGameObjectToEntity
	{
		[SerializeField] private TriggerAuthoring triggerAuthoring = null;
		
		public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
			var triggerEntity = conversionSystem.GetPrimaryEntity(triggerAuthoring);
			dstManager.AddComponentData(triggerEntity, new RoomTrigger
			{
				RoomEntity = entity,
			});
		}
	}
}