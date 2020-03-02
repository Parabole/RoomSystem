using Parabole.EditorTools;
using Parabole.RoomSystem.Core.Content.Authoring;
using Parabole.RoomSystem.Core.Room.Authoring;
using UnityEditor;
using UnityEngine;

namespace Parabole.RoomSystem.Core.Editor
{
	[UnityEditor.CustomEditor(typeof(RoomAuthoring))]
	public class RoomAuthoringEditor : UnityEditor.Editor
	{
		private RoomAuthoring authoring;
		
		void OnEnable()
		{
			authoring = (RoomAuthoring)target;

			var gameObject = authoring.gameObject;
			if (!GameObjectEditorIconHelper.GetHasIcon(gameObject))
			{
				GameObjectEditorIconHelper.SetIcon(gameObject, GameObjectEditorIconHelper.LabelIcon.Green);
			}
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			CheckName();
			CheckAddContent();
		}

		private void CheckAddContent()
		{
			if (GUILayout.Button("Add Child Content"))
			{
				var gameObjectName = RoomContentAuthoringEditor.GetAutomaticName(authoring.RoomName);
				var gameObject = new GameObject(gameObjectName, typeof(RoomContentAuthoring));
				gameObject.transform.SetParent(authoring.transform, false);
				gameObject.GetComponent<RoomContentAuthoring>().ContentName = authoring.RoomName;
			}
		}

		private void CheckName()
		{
			if (!authoring.IsNameValid)
			{
				return;
			}
			
			var automaticName = $"Room_{authoring.RoomName}";
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
	}
}