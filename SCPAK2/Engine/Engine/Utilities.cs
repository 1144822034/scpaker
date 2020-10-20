using Android.App;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using static Android.App.ActivityManager;

namespace Engine
{
	public static class Utilities
	{
		public static void Swap<T>(ref T a, ref T b)
		{
			T val = a;
			a = b;
			b = val;
		}

		public static int SizeOf<T>()
		{
			return Marshal.SizeOf(typeof(T));
		}

		public static T PtrToStructure<T>(IntPtr ptr)
		{
			return (T)Marshal.PtrToStructure(ptr, typeof(T));
		}

		public static T ArrayToStructure<T>(Array array)
		{
			GCHandle gCHandle = GCHandle.Alloc(array, GCHandleType.Pinned);
			try
			{
				return PtrToStructure<T>(gCHandle.AddrOfPinnedObject());
			}
			finally
			{
				gCHandle.Free();
			}
		}

		public static byte[] StructureToArray<T>(T structure)
		{
			byte[] array = new byte[SizeOf<T>()];
			GCHandle gCHandle = GCHandle.Alloc(structure, GCHandleType.Pinned);
			try
			{
				Marshal.Copy(gCHandle.AddrOfPinnedObject(), array, 0, array.Length);
				return array;
			}
			finally
			{
				gCHandle.Free();
			}
		}

		public static void Dispose<T>(ref T disposable) where T : IDisposable
		{
			if (disposable != null)
			{
				disposable.Dispose();
				disposable = default(T);
			}
		}

		public static void DisposeCollection<T>(ICollection<T> disposableCollection) where T : IDisposable
		{
			if (disposableCollection != null)
			{
				foreach (T item in disposableCollection)
				{
					item?.Dispose();
				}
				if (!disposableCollection.IsReadOnly)
				{
					disposableCollection.Clear();
				}
			}
		}

		public static long GetTotalAvailableMemory()
		{
			//IL_000f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001a: Expected O, but got Unknown
			ActivityManager val = (ActivityManager)Window.Activity.GetSystemService("activity");
			MemoryInfo val2 = (MemoryInfo)(object)new MemoryInfo();
			val.GetMemoryInfo(val2);
			return val2.TotalMem;
		}
	}
}
