using System;
using System.Collections.Generic;
using System.Linq;
using Parabole.EditorTools;
using Parabole.RoomSystem.Core.Content.Authoring;
using Parabole.RoomSystem.Core.ExcludePortal;
using Parabole.RoomSystem.Core.Network.Authoring;
using Parabole.RoomSystem.Core.Portal.Authoring;
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
			authoring = (RoomAuthoring) target;

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
			DrawConnectedRoomsPortals();
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
				gameObject.GetComponent<RoomContentDynamicLinkAuthoring>().RoomNames = new[] {roomName};
				gameObject.transform.position = authoring.transform.position;

				Selection.activeGameObject = gameObject;
			}
		}

		private void DrawConnectedRoomsPortals()
		{
			var linkedRooms = GetLinkedRooms();

			if (linkedRooms.Any())
			{
				GUILayout.Space(20);
				EditorGUILayout.LabelField("Rooms Linked By Portals", EditorStyles.boldLabel);
			}
			
			foreach (var linkedRoomData in GetLinkedRooms())
			{
				GUILayout.BeginHorizontal();
				
				var isSeeThrough = linkedRoomData.SeeThroughRoom != null;
				var isExclude = linkedRoomData.Exclude != null;
				
				var labelText = linkedRoomData.OtherRoom.RoomName;
				
				if (linkedRoomData.Exclude != null)
				{
					labelText += " (Excluded)";
				}
				else if (linkedRoomData.Portal != null && linkedRoomData.Portal.IsLimited)
				{
					labelText += " (Limited)";
				}
				GUILayout.Label(labelText);

				if (isSeeThrough && !isExclude)
				{
					CheckExcludeButton(linkedRoomData.OtherRoom);
				}
				
				CheckSmallSelectionButton("Room", linkedRoomData.OtherRoom.gameObject, Color.green);
				
				if (isExclude)
				{
					CheckSmallSelectionButton("Exclude", linkedRoomData.Exclude.gameObject, Color.red);
				}
				else if (isSeeThrough)
				{
					CheckSmallSelectionButton("Through", linkedRoomData.SeeThroughRoom.gameObject, Color.green);
				}
				else
				{
					CheckSmallSelectionButton("Portal", linkedRoomData.Portal.gameObject, Color.blue);
				}
				
				GUILayout.EndHorizontal();
			}
		}

		private void CheckSmallSelectionButton(string label, GameObject gameObject, Color color)
		{
			var previousColor = GUI.backgroundColor;
			GUI.backgroundColor = color;
			if (GUILayout.Button(label, GUILayout.MaxWidth(80)))
			{
				Selection.activeGameObject = gameObject;
			}
			GUI.backgroundColor = previousColor;
		}
		
		private void CheckExcludeButton(RoomAuthoring otherRoomAuthoring)
		{
			var previousColor = GUI.backgroundColor;
			GUI.backgroundColor = Color.red;
			if (GUILayout.Button("x", GUILayout.MaxWidth(20)))
			{
				var networkAuthoring = authoring.GetComponentInParent<RoomNetworkAuthoring>();
				var parent = networkAuthoring.transform;
				
				var gameObject = new GameObject("New Exclude Portal", typeof(RoomExcludePortalAuthoring));
				Undo.RegisterCreatedObjectUndo(gameObject, "Create Exclude Portal");
				gameObject.transform.SetParent(parent, false);

				var exclude = gameObject.GetComponent<RoomExcludePortalAuthoring>();
				var excludeSerializedObject = new SerializedObject(exclude);

				excludeSerializedObject.FindProperty("roomAuthoringA").objectReferenceValue = authoring;
				excludeSerializedObject.FindProperty("roomAuthoringB").objectReferenceValue = otherRoomAuthoring;

				excludeSerializedObject.ApplyModifiedProperties();
				
				GameObjectEditorIconHelper.SetIcon(gameObject, GameObjectEditorIconHelper.LabelIcon.Red);
				
				gameObject.name = exclude.GetPortalName();
			}
			GUI.backgroundColor = previousColor;
		}

		private IEnumerable<LinkedRoomData> GetLinkedRooms()
		{
			var allPortals = FindObjectsOfType<RoomPortalAuthoring>();
			var allExcludePortals = FindObjectsOfType<RoomExcludePortalAuthoring>();
			List<LinkedRoomData> resultList = new List<LinkedRoomData>();

			foreach (var portalAuthoring in allPortals)
			{
				if (GetIsContainedInPortal(portalAuthoring))
				{
					var otherRoomAuthoring = GetOtherRoomInPortal(portalAuthoring);
					resultList.Add(new LinkedRoomData
					{
						Portal = portalAuthoring,
						OtherRoom = otherRoomAuthoring,
					});
				}
			}

			var enumerable = resultList.AsEnumerable();
			var dataComparer = new LinkedRoomDataRoomComparer();
			foreach (var linkedRoomData in resultList)
			{
				if (linkedRoomData.Portal.IsLimited)
				{
					continue;
				}
				
				var seeThroughEnumerable = AddRoomsLinkedBySeeThrough(linkedRoomData.OtherRoom, allPortals);

				enumerable = enumerable.Union(seeThroughEnumerable, dataComparer);
			}

			resultList = enumerable.ToList();

			foreach (var linkedRoomData in resultList)
			{
				CheckForExcludePortal(allExcludePortals, linkedRoomData);
			}
			
			resultList.Sort();
			return resultList;
		}

		private IEnumerable<LinkedRoomData> AddRoomsLinkedBySeeThrough(RoomAuthoring otherRoomAuthoring, RoomPortalAuthoring[] allPortals)
		{
			if (!otherRoomAuthoring.SeeThrough)
			{
				yield break;
			}
			
			foreach (var portal in allPortals)
			{
				if (GetIsContainedInPortal(portal, otherRoomAuthoring))
				{
					var thirdRoom = GetOtherRoomInPortal(portal, otherRoomAuthoring);
					if (thirdRoom != authoring)
					{
						yield return new LinkedRoomData
						{
							OtherRoom = thirdRoom,
							SeeThroughRoom = otherRoomAuthoring,
						};
					}
				}
			}
		}

		private string GetAutomaticContentName()
		{
			return RoomContentAuthoringEditor.GetAutomaticName(authoring.RoomName);
		}
		
		private bool GetIsContainedInPortal(RoomPortalAuthoring portalAuthoring)
		{
			return GetIsContainedInPortal(portalAuthoring, authoring);
		}
		
		private bool GetIsContainedInPortal(RoomPortalAuthoring portalAuthoring, RoomAuthoring roomAuthoring)
		{
			return portalAuthoring.RoomAuthoringA == roomAuthoring || portalAuthoring.RoomAuthoringB == roomAuthoring;
		}
		
		private bool GetIsContainedInExclude(RoomExcludePortalAuthoring excludeAuthoring, RoomAuthoring roomAuthoring)
		{
			return excludeAuthoring.RoomAuthoringA == roomAuthoring || excludeAuthoring.RoomAuthoringB == roomAuthoring;
		}
		
		private RoomAuthoring GetOtherRoomInPortal(RoomPortalAuthoring portalAuthoring)
		{
			return GetOtherRoomInPortal(portalAuthoring, authoring);
		}
		
		private RoomAuthoring GetOtherRoomInPortal(RoomPortalAuthoring portalAuthoring, RoomAuthoring roomAuthoring)
		{
			if (portalAuthoring.RoomAuthoringA == roomAuthoring)
			{
				return portalAuthoring.RoomAuthoringB;
			}
			
			if (portalAuthoring.RoomAuthoringB == roomAuthoring)
			{
				return portalAuthoring.RoomAuthoringA;
			}

			throw new ArgumentException("Room not contained in portal");
		}

		private void CheckForExcludePortal(RoomExcludePortalAuthoring[] excludeAuthorings, LinkedRoomData roomData)
		{
			foreach (var excludeAuthoring in excludeAuthorings)
			{
				var hasThisRoom = GetIsContainedInExclude(excludeAuthoring, authoring);
				var hasOtherRoom = GetIsContainedInExclude(excludeAuthoring, roomData.OtherRoom);

				if (hasThisRoom && hasOtherRoom)
				{
					roomData.Exclude = excludeAuthoring;
				}
			}
		}
		
		private class LinkedRoomData : IComparable<LinkedRoomData>
		{
			public RoomAuthoring OtherRoom;
			public RoomPortalAuthoring Portal;
			public RoomAuthoring SeeThroughRoom;
			public RoomExcludePortalAuthoring Exclude;

			private int GetPriority()
			{
				if (Exclude != null)
				{
					return 2;
				}
				
				if (SeeThroughRoom != null)
				{
					return 1;
				}

				// Is regular portal
				return 0;
			}
			
			public int CompareTo(LinkedRoomData other)
			{
				var thisPriority = GetPriority();
				var otherPriority = other.GetPriority();

				if (thisPriority != otherPriority)
				{
					return thisPriority - otherPriority;
				}

				return String.Compare(OtherRoom.RoomName, other.OtherRoom.RoomName, StringComparison.Ordinal);
			}
		}
		
		private class LinkedRoomDataRoomComparer : IEqualityComparer<LinkedRoomData>
		{
			public bool Equals(LinkedRoomData x, LinkedRoomData y)
			{
				return x.OtherRoom == y.OtherRoom;
			}

			public int GetHashCode(LinkedRoomData obj)
			{
				return obj.OtherRoom.GetHashCode();
			}
		}
	}
}