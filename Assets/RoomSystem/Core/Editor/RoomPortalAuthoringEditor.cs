using Parabole.EditorTools;
using Parabole.RoomSystem.Core.Portal.Authoring;
using UnityEditor;
using UnityEngine;

namespace Parabole.RoomSystem.Core.Editor
{
	[UnityEditor.CustomEditor(typeof(RoomPortalAuthoring))]
	public class RoomPortalAuthoringEditor : UnityEditor.Editor
	{
		private RoomPortalAuthoring authoring;
		
		void OnEnable()
		{
			authoring = (RoomPortalAuthoring)target;

			var gameObject = authoring.gameObject;
			if (!GameObjectEditorIconHelper.GetHasIcon(gameObject))
			{
				Debug.Log("Set");
				GameObjectEditorIconHelper.SetIcon(gameObject, GameObjectEditorIconHelper.LabelIcon.Blue);
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
			if (!AreNameValids)
			{
				return;
			}
			
			var automaticName = $"Portal_{authoring.RoomAuthoringA.RoomName}_{authoring.RoomAuthoringB.RoomName}";
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

		private bool AreNameValids => authoring.RoomAuthoringA.IsNameValid && authoring.RoomAuthoringB.IsNameValid;
	}
}