using Parabole.RoomSystem.Core.Portal.Components;
using Parabole.RoomSystem.Core.Room.Authoring;
using Parabole.RoomSystem.Core.Room.Components;
using Unity.Entities;
using UnityEngine;

namespace Parabole.RoomSystem.Core.Portal.Authoring
{
	[RequiresEntityConversion]
	public class RoomPortalAuthoring : MonoBehaviour, IConvertGameObjectToEntity
	{
		[SerializeField] private RoomAuthoring roomAuthoringA;
		[SerializeField] private RoomAuthoring roomAuthoringB;
		
		public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
			if (!GetIsFullyAssigned())
			{
				Debug.LogError($"Rooms in portal {gameObject.name} are not correctly assigned");
				return;
			}

			var entityRoomA = conversionSystem.GetPrimaryEntity(roomAuthoringA);
			var entityRoomB = conversionSystem.GetPrimaryEntity(roomAuthoringB);
			
			dstManager.AddComponentData(entity, new RoomPortal
			{
				EntityRoomA = entityRoomA,
				EntityRoomB = entityRoomB,
			});
			
			AddReferenceToRoom(entity, entityRoomA, entityRoomB, dstManager);
			AddReferenceToRoom(entity, entityRoomB, entityRoomA, dstManager);
		}

		private static void AddReferenceToRoom(Entity portalEntity, Entity roomEntity, 
			Entity linkedRoomEntity, EntityManager dstManager)
		{
			// Make sure the buffer exists by calling add buffer
			var buffer = dstManager.AddBuffer<RoomPortalReference>(roomEntity);
			buffer.Add(new RoomPortalReference
			{
				PortalEntity = portalEntity,
				LinkedRoomEntity = linkedRoomEntity,
			});
		}

		public bool GetIsFullyAssigned()
		{
			return roomAuthoringA != null && 
					roomAuthoringB != null && 
					roomAuthoringA != roomAuthoringB;
		}

		#if UNITY_EDITOR
		public RoomAuthoring RoomAuthoringA => roomAuthoringA;
		public RoomAuthoring RoomAuthoringB => roomAuthoringB;

		public Vector3 GetCenterPosition()
		{
			return (RoomAuthoringA.transform.position + RoomAuthoringB.transform.position) / 2;
		} 
		
		private void OnDrawGizmos()
		{
			if (RoomAuthoringA == null || RoomAuthoringB == null)
			{
				return;
			}
			
			var portalPosition = transform.position;
			
			Gizmos.DrawLine(portalPosition, RoomAuthoringA.transform.position);
			Gizmos.DrawLine(portalPosition, RoomAuthoringB.transform.position);
		}
#endif
	}
}