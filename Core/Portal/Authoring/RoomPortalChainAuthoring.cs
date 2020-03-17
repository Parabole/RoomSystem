using Parabole.RoomSystem.Core.Portal.Components;
using Unity.Entities;
using UnityEngine;

namespace Parabole.RoomSystem.Core.Portal.Authoring
{
	public class RoomPortalChainAuthoring : MonoBehaviour, IConvertGameObjectToEntity
	{
		[SerializeField] private RoomPortalAuthoring portalA = null;
		[SerializeField] private RoomPortalAuthoring portalB = null;
		
		public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
			if (!CheckIsValid())
			{
				return;
			}

			dstManager.AddComponentData(entity, new RoomPortalChain
			{
				PortalEntityA = conversionSystem.GetPrimaryEntity(portalA),
				PortalEntityB = conversionSystem.GetPrimaryEntity(portalB),
			});
		}

		private bool CheckIsValid()
		{
			if (portalA == null || portalB == null || portalA == portalB)
			{
				Debug.Log("RoomPortalChainAuthoring not setupped correctly", this);
				return false;
			}

			if (portalA.GetIsChain() || portalB.GetIsChain())
			{
				Debug.Log("Assigned a room portal chain in a room portal chain, this is not allowed", this);
				return false;
			}

			return true;
		}
	}
}