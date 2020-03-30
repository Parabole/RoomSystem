using Parabole.EditorTools;
using Parabole.RoomSystem.Core.ExcludePortal;
using Parabole.RoomSystem.Core.Portal.Authoring;
using UnityEditor;
using UnityEngine;

namespace Parabole.RoomSystem.Core.Editor
{
	[UnityEditor.CustomEditor(typeof(RoomExcludePortalAuthoring))]
	public class RoomExcludePortalAuthoringEditor : UnityEditor.Editor
	{
		private RoomExcludePortalAuthoring authoring;
		
		void OnEnable()
		{
			authoring = (RoomExcludePortalAuthoring)target;

			var gameObject = authoring.gameObject;
			if (!GameObjectEditorIconHelper.GetHasIcon(gameObject))
			{
				GameObjectEditorIconHelper.SetIcon(gameObject, GameObjectEditorIconHelper.LabelIcon.Red);
			}
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			if (!authoring.GetIsFullyAssigned())
			{
				DrawErrorLabel();
				return;
			}
			
			CheckName();
			CheckPlaceAtCenter();
		}

		private static void DrawErrorLabel()
		{
			var defaultColor = GUI.contentColor;
			GUI.contentColor = Color.red;
			GUILayout.Label("Not correctly assigned");
			GUI.contentColor = defaultColor;
		}

		private void CheckPlaceAtCenter()
		{
			if (GUILayout.Button("Place At Center"))
			{
				authoring.transform.position = authoring.GetCenterPosition();
			}
		}

		private void CheckName()
		{
			if (!AreNamesValid)
			{
				return;
			}

			var automaticName = authoring.GetPortalName();
			var gameObject = authoring.gameObject;
			if (automaticName != gameObject.name)
			{
				var gameObjectSerializedObject = new SerializedObject(authoring.gameObject);
				var serializedProperty = gameObjectSerializedObject.FindProperty("m_Name");
				gameObjectSerializedObject.ApplyModifiedPropertiesWithoutUndo();

				serializedProperty.stringValue = automaticName;

				gameObjectSerializedObject.ApplyModifiedPropertiesWithoutUndo();
			}
		}

		private bool AreNamesValid => authoring.AreNamesValid;
	}
}