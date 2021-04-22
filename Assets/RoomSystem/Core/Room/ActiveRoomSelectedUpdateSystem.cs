using Parabole.RoomSystem.Core.Room.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;

namespace Parabole.RoomSystem.Core.Room
{
	[UpdateInGroup(typeof(RoomUpdateGroup))]
	public class ActiveRoomSelectedUpdateSystem : SystemBase
	{
		private NativeList<Entity> entitiesToSetActive = new NativeList<Entity>(Allocator.Persistent);
		private NativeList<Entity> entitiesToSetNotActive = new NativeList<Entity>(Allocator.Persistent);

		private EntityArchetype requestArchetype;
		
		protected override void OnCreate()
		{
			requestArchetype = EntityManager.CreateArchetype(ComponentType.ReadWrite<RoomUpdateRequest>());
		}
		protected override void OnDestroy()
		{
			entitiesToSetActive.Dispose();
			entitiesToSetNotActive.Dispose();
		}

		protected override void OnUpdate()
		{
			EntityManager.CreateEntity(requestArchetype);
			
			entitiesToSetActive.Clear();
			entitiesToSetNotActive.Clear();
			
			CheckSetNotActive(entitiesToSetNotActive);
			CheckSetActive(entitiesToSetActive, entitiesToSetNotActive);

			SetNotActive();
			SetActive();
		}

		private void CheckSetNotActive(NativeList<Entity> notActiveEntities)
		{
			Entities.WithAll<ActiveRoom>().WithNone<ActiveRoomSelected>()
				.ForEach((Entity entity, DynamicBuffer<RoomContentReference> buffer) =>
				{
					notActiveEntities.Add(entity);

					for (int i = 0; i < buffer.Length; i++)
					{
						notActiveEntities.Add(buffer[i].Entity);
					}
				}).Run();
		}

		private void CheckSetActive(NativeList<Entity> activeEntities, NativeList<Entity> notActiveEntities)
		{
			Entities.WithNone<ActiveRoom>().WithAll<ActiveRoomSelected>()
				.ForEach((Entity entity, DynamicBuffer<RoomContentReference> buffer) =>
				{
					activeEntities.Add(entity);

					for (int i = 0; i < buffer.Length; i++)
					{
						var contentEntity = buffer[i].Entity;
						
						// As there may be multiple contents per room, and multiple rooms per contents,
						// we have to check if we're not setting an entity to not active and then to active again
						var index = notActiveEntities.IndexOf(contentEntity);
						if (index != -1)
						{
							notActiveEntities.RemoveAtSwapBack(index);
						}
						else
						{
							activeEntities.Add(buffer[i].Entity);
						}
					}
				}).Run();
		}

		private void SetNotActive()
		{
			for (int i = 0; i < entitiesToSetNotActive.Length; i++)
			{
				SetNotActive(entitiesToSetNotActive[i]);
			}
		}

		private void SetActive()
		{
			for (int i = 0; i < entitiesToSetActive.Length; i++)
			{
				SetActive(entitiesToSetActive[i]);
			}
		}

		private void SetNotActive(Entity entity)
		{
			EntityManager.RemoveComponent<ActiveRoom>(entity);
			EntityManager.AddComponent<JustNotActiveRoom>(entity);
		}

		private void SetActive(Entity entity)
		{
			EntityManager.AddComponent<ActiveRoom>(entity);
			EntityManager.AddComponent<JustActiveRoom>(entity);
		}
	}
}