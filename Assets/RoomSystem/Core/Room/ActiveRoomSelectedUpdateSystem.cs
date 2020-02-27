using Parabole.RoomSystem.Core.Room.Components;
using Unity.Entities;

namespace Parabole.RoomSystem.Core.Room
{
	[UpdateInGroup(typeof(RoomUpdateGroup))]
	public class NewActiveRoomRequestSystem : SystemBase
	{
		protected override void OnUpdate()
		{
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