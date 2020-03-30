using Parabole.RoomSystem.Core.Helper;
using Parabole.RoomSystem.Core.Room.Authoring;
using Unity.Entities;
using UnityEngine;

namespace Parabole.RoomSystem.Core.ExcludePortal
{
	public class RoomExcludePortalAuthoring : MonoBehaviour, IConvertGameObjectToEntity
	{
		[SerializeField] private RoomAuthoring roomAuthoringA = null;
		[SerializeField] private RoomAuthoring roomAuthoringB = null;
		
		public RoomAuthoring RoomAuthoringA => roomAuthoringA;
		public RoomAuthoring RoomAuthoringB => roomAuthoringB;
		
		public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
			var entityRoomA = conversionSystem.GetPrimaryEntity(roomAuthoringA);
			var entityRoomB = conversionSystem.GetPrimaryEntity(roomAuthoringB);
			
			RoomPortalAuthoringHelper.CreateExcludePortal(entity, entityRoomA, entityRoomB, dstManager);
		}

		public bool GetIsFullyAssigned()
		{
			return roomAuthoringA != null && 
					roomAuthoringB != null && 
					roomAuthoringA != roomAuthoringB;
		}

		public string GetPortalName()
		{
			return $"Exclude_{roomAuthoringA.RoomName}_{roomAuthoringB.RoomName}";
		}
		
		public bool AreNamesValid => roomAuthoringA.IsNameValid && roomAuthoringB.IsNameValid;

#if UNITY_EDITOR
		public Vector3 GetCenterPosition()
		{
			return (roomAuthoringA.transform.position + roomAuthoringB.transform.position) / 2;
		} 
		
		private void OnDrawGizmos()
		{
			if (roomAuthoringA == null || roomAuthoringB == null)
			{
				return;
			}
			
			var portalPosition = transform.position;
			
			Gizmos.DrawLine(portalPosition, roomAuthoringA.transform.position);
			Gizmos.DrawLine(portalPosition, roomAuthoringB.transform.position);
		}
#endif
	}
}