using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Parabole.EditorTools
{
	/// <summary>
	/// Found at https://answers.unity.com/questions/542890/scene-color-object-marking.html
	/// </summary>
	public static class GameObjectEditorIconHelper
	{
		public enum Icon
		{
			CircleGray = 0,
			CircleBlue,
			CircleTeal,
			CircleGreen,
			CircleYellow,
			CircleOrange,
			CircleRed,
			CirclePurple,
			DiamondGray,
			DiamondBlue,
			DiamondTeal,
			DiamondGreen,
			DiamondYellow,
			DiamondOrange,
			DiamondRed,
			DiamondPurple
		}

		public enum LabelIcon
		{
			Gray = 0,
			Blue,
			Teal,
			Green,
			Yellow,
			Orange,
			Red,
			Purple
		}

		private static GUIContent[] labelIcons;
		private static GUIContent[] largeIcons;

		public static void SetIcon(GameObject gameObject, LabelIcon icon)
		{
			if (labelIcons == null)
			{
				labelIcons = GetTextures("sv_label_", string.Empty, 0, 8);
			}

			SetIcon(gameObject, labelIcons[(int) icon].image as Texture2D);
		}

		public static void SetIcon(GameObject gameObject, Icon icon)
		{
			if (largeIcons == null)
			{
				largeIcons = GetTextures("sv_icon_dot", "_pix16_gizmo", 0, 16);
			}

			SetIcon(gameObject, largeIcons[(int) icon].image as Texture2D);
		}

		public static bool GetHasIcon(GameObject gameObject)
		{
			var type = typeof(EditorGUIUtility);
			var methodInfo = type.GetMethod("GetIconForObject", BindingFlags.NonPublic | BindingFlags.Static);
			return (methodInfo.Invoke(null, new object[] {gameObject}) as Texture2D) != null;
		}
		
		private static void SetIcon(GameObject gameObject, Texture2D texture)
		{
			var type = typeof(EditorGUIUtility);
			var methodInfo = type.GetMethod("SetIconForObject", BindingFlags.NonPublic | BindingFlags.Static);
			methodInfo.Invoke(null, new object[] {gameObject, texture});
		}

		private static GUIContent[] GetTextures(string baseName, string postFix, int startIndex, int count)
		{
			var guiContents = new GUIContent[count];

			for (var i = 0; i < count; i++)
			{
				guiContents[i] = EditorGUIUtility.IconContent(baseName + (startIndex + i) + postFix);
			}

			return guiContents;
		}
	}
}