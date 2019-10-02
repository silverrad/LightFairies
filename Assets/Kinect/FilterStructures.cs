using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;


namespace Filters
{
	[StructLayout(LayoutKind.Sequential, Pack=KinectInterface.PACK_BYTES)]
	public struct SmoothingFilterSet
	{
		public int I;
		public float F;	
	}
	
	[StructLayout(LayoutKind.Sequential, Pack=KinectInterface.PACK_BYTES)]
	public struct SmoothingFilterGet
	{
		public int I;
		public float F;	
	}
}