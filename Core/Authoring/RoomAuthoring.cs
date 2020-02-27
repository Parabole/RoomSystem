using Unity.Entities;
using UnityEngine;

namespace RoomSystem.Core.Authoring
{
	[RequiresEntityConversion]
	public class RoomAuthoring : MonoBehaviour, IConvertGameObjectToEntity
	{
		[SerializeField] private string roomName;

		public bool IsNameValid => !string.IsNullOrEmpty(roomName);
		public string RoomName => roomName;


		public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
			dstManager.AddComponentData(entity, new Room());
		}
	}
}