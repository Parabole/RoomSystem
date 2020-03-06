using Parabole.EditorTools;
using Parabole.RoomSystem.Core.Content.Authoring;
using Parabole.RoomSystem.Core.Room.Authoring;
using Unity.Entities;
using UnityEditor;
using UnityEngine;

namespace Parabole.RoomSystem.Core.Editor
{
	[CustomEditor(typeof(RoomAuthoring))]
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
			CheckCreateChildContent();
			CheckCreateDynamicContent();
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

		private void CheckCreateChildContent()
		{
			if (GUILayout.Button("Create New Child Content"))
			{
				var gameObjectName = GetAutomaticContentName();
				var gameObject = new GameObject(gameObjectName, typeof(RoomContentAuthoring));
				gameObject.transform.SetParent(authoring.transform, false);
				gameObject.GetComponent<RoomContentAuthoring>().ContentName = authoring.RoomName;
				
				Selection.activeGameObject = gameObject;
			}
		}
		
		private void CheckCreateDynamicContent()
		{
			if (GUILayout.Button("Create New Dynamic Room Content"))
			{
				var gameObjectName = GetAutomaticContentName();
				var gameObject = new GameObject(gameObjectName, typeof(ConvertToEntity), 
					typeof(RoomContentAuthoring), typeof(RoomContentDynamicLinkAuthoring));

				var roomName = authoring.RoomName;
				gameObject.GetComponent<RoomContentAuthoring>().ContentName = roomName;
				gameObject.GetComponent<RoomContentDynamicLinkAuthoring>().RoomNames = new[] { roomName };
				gameObject.transform.position = authoring.transform.position;
				
				Selection.activeGameObject = gameObject;
			}
		}

		private string GetAutomaticContentName()
		{
			return RoomContentAuthoringEditor.GetAutomaticName(authoring.RoomName);
		}
	}
}