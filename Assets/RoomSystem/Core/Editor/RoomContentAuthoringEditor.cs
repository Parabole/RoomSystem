using Parabole.EditorTools;
using Parabole.RoomSystem.Core.Content.Authoring;
using UnityEditor;

namespace Parabole.RoomSystem.Core.Editor
{
	[UnityEditor.CustomEditor(typeof(RoomContentAuthoring))]
	public class RoomContentAuthoringEditor : UnityEditor.Editor
	{
		private RoomContentAuthoring authoring;
		
		void OnEnable()
		{
			authoring = (RoomContentAuthoring)target;
		}

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			CheckName();
		}

		private void CheckName()
		{
			if (!authoring.IsNameValid)
			{
				return;
			}
			
			var automaticName = GetAutomaticName(authoring.ContentName);
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

		public static string GetAutomaticName(string contentName)
		{
			return $"RoomContent_{contentName}";
		}
	}
}