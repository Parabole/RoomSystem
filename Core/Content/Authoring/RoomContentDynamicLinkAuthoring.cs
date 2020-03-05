using Parabole.RoomSystem.Core.Helper;
using Unity.Entities;
using UnityEngine;

namespace Parabole.RoomSystem.Core.Content.Authoring
{
	[RequiresEntityConversion]
	public class RoomContentDynamicLinkAuthoring : MonoBehaviour, IConvertGameObjectToEntity
	{
		[SerializeField] private string[] roomNames = null;

		public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
			var buffer = dstManager.AddBuffer<RoomContentDynamicLink>(entity);

			for (int i = 0; i < roomNames.Length; i++)
			{
				buffer.Add(new RoomContentDynamicLink
				{
					NameHash = HashHelper.GetHash(roomNames[i]),
				});
			}
		}
	}
}