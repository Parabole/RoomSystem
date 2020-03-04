using Parabole.RoomSystem.Core.Room.Authoring;
using UnityEditor;

namespace RoomSystem.Trigger.Editor
{
	[CustomEditor(typeof(RoomTriggerAuthoring))]
	public class RoomTriggerAuthoringEditor : UnityEditor.Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
		}
	}
}