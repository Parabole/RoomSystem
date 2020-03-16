using Parabole.RoomSystem.Core.Portal.Components;
using Parabole.RoomSystem.Core.Room.Components;
using Unity.Entities;

namespace Parabole.RoomSystem.Core.Helper
{
	public static class RoomPortalAuthoringHelper
	{
		public static void CreatePortal(Entity portalEntity, Entity roomEntityA, 
			Entity roomEntityB, EntityManager dstManager)
		{
			dstManager.AddComponentData(portalEntity, new RoomPortal
			{
				EntityRoomA = roomEntityA,
				EntityRoomB = roomEntityB,
			});
			
			AddReferenceToRoom(portalEntity, roomEntityA, roomEntityB, dstManager);
			AddReferenceToRoom(portalEntity, roomEntityB, roomEntityA, dstManager);
		}
		
		private static void AddReferenceToRoom(Entity portalEntity, Entity roomEntity, Entity linkedRoomEntity, 
			EntityManager dstManager)
		{
			// Make sure the buffer exists by calling add buffer
			var buffer = dstManager.AddBuffer<RoomPortalReference>(roomEntity);
			buffer.Add(new RoomPortalReference
			{
				PortalEntity = portalEntity,
				LinkedRoomEntity = linkedRoomEntity,
			});
		}
	}
}