using Parabole.RoomSystem.Core.Room.Components;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;

namespace Parabole.RoomSystem.Core.Room
{
	[UpdateInGroup(typeof(RoomUpdateGroup))]
	public class ActiveRoomSelectedUpdateSystem : SystemBase
	{
		protected override void OnUpdate()
		{
			EntityManager.CreateEntity(ComponentType.ReadWrite<RoomUpdateRequest>());

			Entities.WithStructuralChanges().WithAll<ActiveRoom>().WithNone<ActiveRoomSelected>()
				.ForEach((Entity entity) =>
				{
					EntityManager.RemoveComponent<ActiveRoom>(entity);
					EntityManager.AddComponent<JustNotActiveRoom>(entity);
				}).Run();

			Entities.WithStructuralChanges().WithNone<ActiveRoom>().WithAll<ActiveRoomSelected>()
				.ForEach((Entity entity) =>
				{
					EntityManager.AddComponent<ActiveRoom>(entity);
					EntityManager.AddComponent<JustActiveRoom>(entity);
				}).Run();
		}
	}
}