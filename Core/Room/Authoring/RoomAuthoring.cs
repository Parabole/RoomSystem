using Unity.Entities;
using UnityEngine;

namespace Parabole.RoomSystem.Core.Room.Authoring
{
	[RequiresEntityConversion]
	public class RoomAuthoring : MonoBehaviour, IConvertGameObjectToEntity
	{
		public bool IsNameValid => !string.IsNullOrEmpty(RoomName);
		[SerializeField] public string RoomName { get; } = null;


		public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
			dstManager.AddComponentData(entity, new global::Parabole.RoomSystem.Core.Room.Components.Room());
		}
	}
}