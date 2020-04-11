using Parabole.RoomSystem.Core.Content.Components;
using Parabole.RoomSystem.Core.Helper;
using Unity.Entities;
using UnityEngine;

namespace Parabole.RoomSystem.Core.Content.Authoring
{
	[RequiresEntityConversion]
	public class RoomContentDynamicLinkAuthoring : MonoBehaviour, IConvertGameObjectToEntity
	{
		[SerializeField] private string[] roomNames = null;

		public string[] RoomNames
		{
			get => roomNames;
			set => roomNames = value;
		}

		public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
			var buffer = dstManager.AddBuffer<RoomContentDynamicLink>(entity);

			for (int i = 0; i < RoomNames.Length; i++)
			{
				buffer.Add(new RoomContentDynamicLink
				{
					NameHash = HashHelper.GetHash(RoomNames[i]),
				});
			}
		}
	}
}