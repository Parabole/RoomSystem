using Parabole.RoomSystem.Core.Helper;
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
		
		public RoomAuthoring RoomAuthoringA => roomAuthoringA;
		public RoomAuthoring RoomAuthoringB => roomAuthoringB;

		private bool wasChainChecked;
		private bool isChain;
		
		public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
			if (!GetIsFullyAssigned())
			{
				Debug.LogError($"Rooms in portal {gameObject.name} are not correctly assigned");
				return;
			}

			var roomEntityA = conversionSystem.GetPrimaryEntity(roomAuthoringA);
			var roomEntityB = conversionSystem.GetPrimaryEntity(roomAuthoringB);
			
			RoomPortalAuthoringHelper.CreatePortal(entity, roomEntityA, roomEntityB, dstManager);
			
			dstManager.AddComponentData(entity, new RoomPortalId
			{
				Value = HashHelper.GetHash(GetPortalName()),
			});
		}

		public bool GetIsChain()
		{
			if (!wasChainChecked)
			{
				isChain = GetComponent<RoomPortalChainAuthoring>() != null;
				wasChainChecked = true;
			}
			return isChain;
		}

		public bool GetIsFullyAssigned()
		{
			return roomAuthoringA != null && 
					roomAuthoringB != null && 
					roomAuthoringA != roomAuthoringB;
		}

		public string GetPortalName()
		{
			return $"Portal_{RoomAuthoringA.RoomName}_{RoomAuthoringB.RoomName}";
		}
		
#if UNITY_EDITOR
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