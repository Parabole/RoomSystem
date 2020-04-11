using UnityEngine;

namespace Parabole.RoomSystem.Core.Helper
{
	public static class HashHelper
	{
		public static int GetHash(string value)
		{
			return Animator.StringToHash(value);
		}
	}
}