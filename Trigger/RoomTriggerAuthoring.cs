using Parabole.InteractionSystem.Triggers;
using Unity.Entities;
using UnityEngine;

namespace Parabole.RoomSystem.Core.Room.Authoring
{
	[RequiresEntityConversion]
	public class RoomTriggerAuthoring : MonoBehaviour, IConvertGameObjectToEntity
	{
		[SerializeField] private TriggerAuthoring triggerAuthoring;
		
		public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
			dstManager.AddComponentObject(entity, triggerAuthoring.transform);
		}
	}
}