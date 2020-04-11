using System;
using Unity.Collections;
using Unity.Entities;

namespace Parabole.RoomSystem.Core.Helper
{
	public static class NativeCollectionHelper
	{
		public static void AddUnion<T>(this NativeList<T> list, T element) where T : struct, IEquatable<T>
		{
			if (!list.Contains(element))
			{
				list.Add(element);
			}
		}
	}
}