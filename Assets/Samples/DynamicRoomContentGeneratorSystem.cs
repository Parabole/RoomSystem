using Parabole.RoomSystem.Core.Content.Components;
using Unity.Entities;
using UnityEngine;

namespace Samples
{
	[AlwaysUpdateSystem]
	public class DynamicRoomContentGeneratorSystem : SystemBase
	{
		private EntityQuery dynamicQuery;

		protected override void OnCreate()
		{
			dynamicQuery = GetEntityQuery(
				ComponentType.ReadWrite<RoomContentDynamicLinkSystemState>(),
				ComponentType.ReadWrite<RoomContent>());
		}

		protected override void OnUpdate()
		{
			if (Input.GetKeyDown(KeyCode.L))
			{
				Entities.WithStructuralChanges().ForEach((ref DynamicRoomContentGenerator generator) =>
				{
					EntityManager.Instantiate(generator.PrefabEntity);
				}).Run();
			}

			if (Input.GetKeyDown(KeyCode.K))
			{
				EntityManager.DestroyEntity(dynamicQuery);
			}
			
		}
	}
}