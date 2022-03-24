using Parabole.RoomSystem.Core.Content.Authoring;
using Unity.Entities;
using UnityEngine;

namespace Parabole.RoomSystem.Core.Room.Authoring
{
	public class RoomExplicitContentReferenceAuthoring : MonoBehaviour
	{
		[SerializeField] private RoomContentAuthoring[] contents = null;

		public RoomContentAuthoring[] Contents => contents;
	}
}