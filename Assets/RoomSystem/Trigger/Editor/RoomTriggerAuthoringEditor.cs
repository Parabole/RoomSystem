using Parabole.InteractionSystem.Triggers;
using Parabole.RoomSystem.Core.Room.Authoring;
using UnityEditor;
using UnityEngine;

namespace RoomSystem.Trigger.Editor
{
	[CustomEditor(typeof(RoomTriggerAuthoring))]
	public class RoomTriggerAuthoringEditor : UnityEditor.Editor
	{
		private RoomTriggerAuthoring authoring;
		
		void OnEnable()
		{
			authoring = (RoomTriggerAuthoring)target;
		}
		
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			CheckCreateTrigger();
		}
		
		private void CheckCreateTrigger()
		{
			if (authoring.TriggerAuthoring != null) { return; }

			if (GUILayout.Button("Create Trigger"))
			{
				var roomName = authoring.GetComponent<RoomAuthoring>().RoomName;
				var gameObject = new GameObject($"RoomTrigger_{roomName}");
				gameObject.transform.position = authoring.transform.position;
				gameObject.AddComponent<TriggerAdapter>();
				authoring.TriggerAuthoring = gameObject.GetComponentInChildren<TriggerAuthoring>();

				Selection.activeGameObject = gameObject;
			}
		}
	}
}