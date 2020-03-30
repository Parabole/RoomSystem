using Parabole.RoomSystem.Core.Content.Authoring;
using Parabole.RoomSystem.Core.Content.Components;
using Parabole.RoomSystem.Core.Room;
using Parabole.RoomSystem.Core.Room.Components;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Parabole.RoomSystem.Core.Room
{
	[UpdateInGroup(typeof(RoomUpdateGroup))]
	[UpdateBefore(typeof(ActiveRoomSelectedUpdateSystem))]
	public class RoomContentDynamicLinkSystem : SystemBase
	{
		private NativeHashMap<int, Entity> roomMap = new NativeHashMap<int, Entity>(64, Allocator.Persistent);
		private NativeList<LinkOperation> linkOperations = new NativeList<LinkOperation>(Allocator.Persistent);
		private NativeList<Entity> fullyLinkedContents = new NativeList<Entity>(Allocator.Persistent);

		private EntityQuery query;

		/// Used for delay warning
		private int runCount = 0;
		
		protected override void OnCreate()
		{
			RequireForUpdate(query);
		}

		protected override void OnDestroy()
		{
			roomMap.Dispose();
			linkOperations.Dispose();
			fullyLinkedContents.Dispose();
		}

		protected override void OnStartRunning()
		{
			runCount = 0;
		}

		protected override void OnUpdate()
		{
			CheckForLogLinkDelay();

			roomMap.Clear();
			linkOperations.Clear();
			fullyLinkedContents.Clear();

			UpdateMap(roomMap);
			
			GetLinkOperations(linkOperations, fullyLinkedContents, roomMap);

			SetComponents();
		}

		private void CheckForLogLinkDelay()
		{
			runCount++;
			if (runCount == 100)
			{
				Entities.WithoutBurst().WithNone<RoomContentDynamicLinkSystemState>()
					.ForEach((Entity entity, ref DynamicBuffer<RoomContentDynamicLink> links) =>
					{
						for (int i = links.Length - 1; i >= 0; i--)
						{
							if (!links[i].IsLinked)
							{
								LogWarning(entity, i);
							}
						}
					}).Run();
			}
		}

		private void UpdateMap(NativeHashMap<int, Entity> map)
		{
			Entities.ForEach((Entity entity, in RoomDefinition roomDefinition) =>
			{
				map.Add(roomDefinition.NameHash, entity);
			}).Run();
		}

		private void GetLinkOperations(NativeList<LinkOperation> linkOperations, NativeList<Entity> fullyLinkedContents, 
			NativeHashMap<int, Entity> roomMap)
		{
			Entities.WithStoreEntityQueryInField(ref query).WithNone<RoomContentDynamicLinkSystemState>()
				.ForEach((Entity entity, ref DynamicBuffer<RoomContentDynamicLink> links) =>
				{
					bool isFullyFilled = true;
					for (int i = links.Length - 1; i >= 0; i--)
					{
						if (!TryLink(entity, i, linkOperations, links, roomMap))
						{
							isFullyFilled = false;
						}
					}

					if (isFullyFilled)
					{
						fullyLinkedContents.Add(entity);
					}
				}).Run();
		}

		private static bool TryLink(Entity entity, int index, NativeList<LinkOperation> linkOperations,
			DynamicBuffer<RoomContentDynamicLink> links, NativeHashMap<int, Entity> roomMap)
		{
			var link = links[index];
			
			if (link.IsLinked)
			{
				return true;
			}

			if (roomMap.TryGetValue(link.NameHash, out var roomEntity))
			{
				linkOperations.Add(new LinkOperation
				{
					RoomEntity = roomEntity,
					ContentEntity = entity,
				});
				
				links[index] = new RoomContentDynamicLink
				{
					IsLinked = true,
					NameHash = link.NameHash,
				};
				return true;
			}
			return false;
		}

		private void SetComponents()
		{
			for (int i = 0; i < linkOperations.Length; i++)
			{
				var operation = linkOperations[i];
				LinkContentToRoom(operation);
			}

			for (int i = 0; i < fullyLinkedContents.Length; i++)
			{
				var entity = fullyLinkedContents[i];
				EntityManager.AddComponent<RoomContentDynamicLinkSystemState>(entity);
			}
		}

		private void LinkContentToRoom(LinkOperation operation)
		{
			var roomEntity = operation.RoomEntity;
			var contentEntity = operation.ContentEntity;
			var buffer = EntityManager.GetBuffer<RoomContentReference>(operation.RoomEntity);
			buffer.Add(new RoomContentReference
			{
				Entity = contentEntity,
			});

			if (EntityManager.HasComponent<ActiveRoom>(roomEntity))
			{
				EntityManager.AddComponent<JustActiveRoom>(contentEntity);
				EntityManager.AddComponent<ActiveRoom>(contentEntity);
			}

			if (EntityManager.HasComponent<VisibleRoom>(roomEntity))
			{
				EntityManager.AddComponent<JustVisibleRoom>(contentEntity);
				EntityManager.AddComponent<VisibleRoom>(contentEntity);
			}

			if (EntityManager.HasComponent<StandbyRoom>(roomEntity))
			{
				EntityManager.AddComponent<JustStandbyRoom>(contentEntity);
				EntityManager.AddComponent<StandbyRoom>(contentEntity);
			}
		}

		private void LogWarning(Entity entity, int index)
		{
			string entityDescription;
#if UNITY_EDITOR
			entityDescription = $"{EntityManager.GetName(entity)}, {entity}";
#else
			entityDescription = $"{entity}";
#endif
			string message = $"Can't dynamically find room on content entity {entityDescription}. " + 
							$"This is logged after 100 frames of searching. " +
							$"This will happen if the assigned name at index {index} in " +
							$"RoomContentDynamicLinkAuthoring is wrong or if the room's entity is not yet created";
			
			Debug.LogWarning(message);
		}

		private struct LinkOperation
		{
			public Entity ContentEntity;
			public Entity RoomEntity;
		}
	}
}