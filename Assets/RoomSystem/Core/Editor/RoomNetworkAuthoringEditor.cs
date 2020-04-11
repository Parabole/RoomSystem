using Parabole.RoomSystem.Core.ExcludePortal;
using Parabole.RoomSystem.Core.Network.Authoring;
using Parabole.RoomSystem.Core.Portal.Authoring;
using Parabole.RoomSystem.Core.Room.Authoring;
using UnityEditor;
using UnityEngine;

namespace Parabole.RoomSystem.Core.Editor
{
	[CustomEditor(typeof(RoomNetworkAuthoring))]
	public class RoomNetworkAuthoringEditor : UnityEditor.Editor
	{
		private RoomNetworkAuthoring authoring;
		void OnEnable()
		{
			authoring = (RoomNetworkAuthoring)target;

			var gameObject = authoring.gameObject;
		}

		
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			
			CheckCreateRoom();
			CheckCreatePortal();
			CheckCreateExcludePortal();
		}
		
		private void CheckCreateRoom()
		{
			if (GUILayout.Button("Create New Room"))
			{
				var gameObject = new GameObject("New Room", typeof(RoomAuthoring));
				gameObject.transform.SetParent(authoring.transform, false);
				Selection.activeGameObject = gameObject;
			}
		}
		
		private void CheckCreatePortal()
		{
			if (GUILayout.Button("Create New Room Portal"))
			{
				var gameObject = new GameObject("New Portal", typeof(RoomPortalAuthoring));
				gameObject.transform.SetParent(authoring.transform, false);
				Selection.activeGameObject = gameObject;
			}
		}
		
		private void CheckCreateExcludePortal()
		{
			if (GUILayout.Button("Create New Room Exclude Portal"))
			{
				var gameObject = new GameObject("New Exclude Portal", typeof(RoomExcludePortalAuthoring));
				gameObject.transform.SetParent(authoring.transform, false);
				Selection.activeGameObject = gameObject;
			}
		}
	}
}