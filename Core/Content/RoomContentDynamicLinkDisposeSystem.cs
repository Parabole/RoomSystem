using Parabole.RoomSystem.Core.Content.Authoring;
using Parabole.RoomSystem.Core.Content.Components;
using Parabole.RoomSystem.Core.Room.Components;
using Unity.Collections;
using Unity.Entities;

namespace Parabole.RoomSystem.Core.Room
{
	[UpdateInGroup(typeof(RoomUpdateGroup))]
	[UpdateBefore(typeof(RoomUpdateRequestGroup))]
	public class RoomContentDynamicLinkDisposeSystem : SystemBase
	{
		private EntityQuery removedQuery;
		
		protected override void OnCreate()
		{
			removedQuery = GetEntityQuery(
				ComponentType.ReadWrite<RoomContentDynamicLinkSystemState>(),
				ComponentType.Exclude<RoomContent>());
			
			RequireForUpdate(removedQuery);
		}

		protected override void OnUpdate()
		{
			var removedContents = removedQuery.ToEntityArray(Allocator.TempJob);
			
			EntityManager.RemoveComponent<RoomContentDynamicLinkSystemState>(removedQuery);
			
			Entities.WithDisposeOnCompletion(removedContents)
				.ForEach((DynamicBuffer<RoomContentReference> references) =>
				{
					for (int i = references.Length - 1; i >= 0; i--)
					{
						var reference = references[i];

						if (removedContents.Contains(reference.Entity))
						{
							references.RemoveAt(i);
						}
					}
				}).ScheduleParallel();
		}
	}
}