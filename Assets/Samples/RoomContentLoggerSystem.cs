using Parabole.RoomSystem.Core.Room.Components;
using Unity.Entities;
using UnityEngine;

namespace Samples
{
	public class RoomContentLoggerSystem : SystemBase
	{
		protected override void OnCreate()
		{

		}

		protected override void OnUpdate()
		{
			Entities.WithoutBurst().WithAll<JustActiveRoom>().ForEach((in RoomContentLogger logger) =>
			{
				Debug.Log(logger.GameObjectName + "JustActiveRoom");
			}).Run();
			
			Entities.WithoutBurst().WithAll<JustNotActiveRoom>().ForEach((in RoomContentLogger logger) =>
			{
				Debug.Log(logger.GameObjectName + "JustNotActiveRoom");
			}).Run();
			
			Entities.WithoutBurst().WithAll<JustStandbyRoom>().ForEach((in RoomContentLogger logger) =>
			{
				Debug.Log(logger.GameObjectName + "JustStandbyRoom");
			}).Run();
			
			Entities.WithoutBurst().WithAll<JustNotStandbyRoom>().ForEach((in RoomContentLogger logger) =>
			{
				Debug.Log(logger.GameObjectName + "JustNotStandbyRoom");
			}).Run();
			
			Entities.WithoutBurst().WithAll<JustVisibleRoom>().ForEach((in RoomContentLogger logger) =>
			{
				Debug.Log(logger.GameObjectName + "JustVisibleRoom");
			}).Run();
			
			Entities.WithoutBurst().WithAll<JustNotVisibleRoom>().ForEach((in RoomContentLogger logger) =>
			{
				Debug.Log(logger.GameObjectName + "JustNotVisibleRoom");
			}).Run();
		}
	}
}