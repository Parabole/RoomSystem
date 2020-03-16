using System.Collections.Generic;
using Parabole.RoomSystem.Core.Helper;
using Parabole.RoomSystem.Core.Portal.Components;
using Parabole.RoomSystem.Core.Room.Authoring;
using Parabole.RoomSystem.Core.Room.Components;
using Unity.Collections;
using Unity.Entities;

namespace Parabole.RoomSystem.Core.Portal.Authoring
{
	[UpdateInGroup(typeof(GameObjectAfterConversionGroup))]
	public class RoomSeeThroughPortalConversionSystem : GameObjectConversionSystem
	{
		private List<PortalChainElement> chainElements = new List<PortalChainElement>();
		
		protected override void OnUpdate()
		{
			FillChainElements();
			CreatePortalChains();
		}

		private void FillChainElements()
		{
			chainElements.Clear();

			Entities.ForEach((RoomAuthoring roomAuthoring) =>
			{
				if (roomAuthoring.SeeThrough)
				{
					var roomEntity = GetPrimaryEntity(roomAuthoring);
					var portalReferences = DstEntityManager.GetBuffer<RoomPortalReference>(roomEntity);

					for (int i = 0; i < portalReferences.Length; i++)
					{
						LinkToOtherPortals(roomAuthoring, portalReferences, i);
					}
				}
			});
		}

		private void LinkToOtherPortals(RoomAuthoring roomAuthoring, 
			DynamicBuffer<RoomPortalReference> portalReferences, int currentIndex)
		{
			var length = portalReferences.Length;
			if (currentIndex + 1 >= length)
			{
				return;
			}

			var portalReferenceA = portalReferences[currentIndex];
			
			for (int i = currentIndex + 1; i < length; i++)
			{
				var portalReferenceB = portalReferences[i];

				var portalEntityA = portalReferenceA.PortalEntity;
				var portalEntityB = portalReferenceB.PortalEntity;
				
				var element = new PortalChainElement
				{
					RoomAuthoring = roomAuthoring,
					RoomA = portalReferenceA.LinkedRoomEntity,
					RoomB = portalReferenceB.LinkedRoomEntity,
					PortalA = portalEntityA,
					PortalB = portalEntityB,
				};

				// Can't chain other chains
				if (EntityManager.HasComponent<RoomPortalChain>(portalEntityA) ||
				    EntityManager.HasComponent<RoomPortalChain>(portalEntityB))
				{
					continue;
				}
				
				chainElements.Add(element);
			}
		}

		private void CreatePortalChains()
		{
			for (int i = 0; i < chainElements.Count; i++)
			{
				var element = chainElements[i];
				var portalEntity = CreateAdditionalEntity(element.RoomAuthoring);

				RoomPortalAuthoringHelper.CreatePortal(portalEntity, element.RoomA, element.RoomB, DstEntityManager);

				DstEntityManager.AddComponentData(portalEntity, new RoomPortalChain
				{
					PortalEntityA = element.PortalA,
					PortalEntityB = element.PortalB,
				});
			}
		}
		
		private struct PortalChainElement
		{
			public RoomAuthoring RoomAuthoring;
			public Entity RoomA;
			public Entity RoomB;
			public Entity PortalA;
			public Entity PortalB;
		}
	}
}