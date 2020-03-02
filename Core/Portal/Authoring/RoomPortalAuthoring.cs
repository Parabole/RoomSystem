using Parabole.RoomSystem.Core.Portal.Components;
using Parabole.RoomSystem.Core.Room.Authoring;
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
			dstManager.AddComponentData(entity, new RoomPortal());
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