using Parabole.InteractionSystem.Runtime.Triggers;
using RoomSystem.Core;
using Unity.Entities;
using Unity.Jobs;

namespace DefaultNamespace
{
	[UpdateBefore(typeof(RoomUpdateGroup))]
	[AlwaysSynchronizeSystem]
	public class RoomTriggerSystem : JobComponentSystem
	{
		
		
		protected override void OnCreate()
		{
			
		}

		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			Entities.WithStructuralChanges().WithAny<TriggerStay>().ForEach((in RoomTrigger trigger) =>
			{
				// TODO: Check if changed
			}).Run();

			return default;
		}
	}
}