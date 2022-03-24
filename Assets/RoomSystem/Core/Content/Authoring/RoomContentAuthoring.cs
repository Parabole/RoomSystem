using Parabole.RoomSystem.Core.Content.Components;
using Unity.Entities;
using UnityEngine;

namespace Parabole.RoomSystem.Core.Content.Authoring
{
	public class RoomContentAuthoring : MonoBehaviour, IConvertGameObjectToEntity
	{
		[SerializeField] private string contentName = null;

		public string ContentName
		{
			get => contentName;
			set => contentName = value;
		}

		public bool IsNameValid => !string.IsNullOrEmpty(contentName);

		public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
			dstManager.AddComponentData(entity, new RoomContent());
		}
	}
}