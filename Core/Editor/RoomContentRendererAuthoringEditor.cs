using System.Collections.Generic;
using Parabole.RoomSystem.BasicContent.Authoring;
using UnityEditor;
using UnityEngine;

namespace RoomSystem.Core.Editor
{
	[CustomEditor(typeof(RoomContentRendererAuthoring))]
	public class RoomContentRendererAuthoringEditor : UnityEditor.Editor
	{
		private List<GameObject> gameObjectsToSkip = new List<GameObject>();

		private bool wasAlreadyDisabled;
		
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			var authoring = (RoomContentRendererAuthoring)target;
			
			if (!wasAlreadyDisabled && GUILayout.Button("Test Deactivate Content"))
			{
				wasAlreadyDisabled = true;
				gameObjectsToSkip.Clear();
				foreach (var parent in authoring.RendererParents)
				{
					if (parent.activeSelf)
					{
						parent.SetActive(false);
					}
					else
					{
						Debug.LogWarning($"{parent.name} already disabled.");
						gameObjectsToSkip.Add(parent);
					}
				}
			}

			if (GUILayout.Button("Reset Test"))
			{
				wasAlreadyDisabled = false;
				foreach (var parent in authoring.RendererParents)
				{
					if (!gameObjectsToSkip.Contains(parent))
					{
						parent.SetActive(true);
					}
				}
			}
		}
	}
}