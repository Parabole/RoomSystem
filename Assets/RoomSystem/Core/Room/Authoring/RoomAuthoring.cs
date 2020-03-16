using System.Linq;
using Parabole.RoomSystem.Core.Content.Authoring;
using Parabole.RoomSystem.Core.Helper;
using Parabole.RoomSystem.Core.Room.Components;
using Unity.Entities;
using UnityEngine;

namespace Parabole.RoomSystem.Core.Room.Authoring
{
	[RequiresEntityConversion]
	public class RoomAuthoring : MonoBehaviour, IConvertGameObjectToEntity
	{
		[SerializeField] private string roomName = null;
		[SerializeField] private bool seeThrough = false;

		public bool IsNameValid => !string.IsNullOrEmpty(roomName);
		public string RoomName => roomName;

		public bool SeeThrough => seeThrough;

		public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
			dstManager.AddComponentData(entity, new RoomDefinition
			{
				NameHash = HashHelper.GetHash(roomName),
			});
			
			// Makes sure the buffer exists for the archetype, even if RoomPortalAuthoring adds it too
			dstManager.AddBuffer<RoomPortalReference>(entity);
			
			AddContentReferences(entity, dstManager, conversionSystem);
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