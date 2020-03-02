using System.Linq;
using Parabole.RoomSystem.Core.Content.Authoring;
using Parabole.RoomSystem.Core.Room.Components;
using Unity.Entities;
using UnityEngine;

namespace Parabole.RoomSystem.Core.Room.Authoring
{
	[RequiresEntityConversion]
	public class RoomAuthoring : MonoBehaviour, IConvertGameObjectToEntity
	{
		[SerializeField] private string roomName = null;

		public bool IsNameValid => !string.IsNullOrEmpty(roomName);
		public string RoomName => roomName;
		
		public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
			dstManager.AddComponentData(entity, new RoomDefinition());
		}

		private void AddContentReferences(Entity entity, EntityManager dstManager, 
			GameObjectConversionSystem conversionSystem)
		{
			var buffer = dstManager.AddBuffer<RoomContentReference>(entity);

			var implicitAuthorings = GetComponentsInChildren<RoomContentAuthoring>();

			var explicitReferenceAuthoring = GetComponent<RoomExplicitContentReferenceAuthoring>();
			if (explicitReferenceAuthoring == null)
			{
				for (int i = 0; i < implicitAuthorings.Length; i++)
				{
					AddContentReference(buffer, implicitAuthorings[i], conversionSystem);
				}
			}
			else
			{
				var explicitAuthorings = explicitReferenceAuthoring.Contents;
				foreach (var roomContentAuthoring in implicitAuthorings.Union(explicitAuthorings))
				{
					AddContentReference(buffer, roomContentAuthoring, conversionSystem);
				}
			}
		}

		private static void AddContentReference(DynamicBuffer<RoomContentReference> buffer, 
			RoomContentAuthoring authoring, GameObjectConversionSystem conversionSystem)
		{
			var contentEntity = conversionSystem.GetPrimaryEntity(authoring);
			buffer.Add(new RoomContentReference
			{
				Entity = contentEntity,
			});
		}
	}
}