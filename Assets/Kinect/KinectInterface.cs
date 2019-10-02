using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using System.Text;



	public enum FilterFlags
	{
		NSFILTER_SKELETON_SMOOTHING = 0,
		NSFILTER_SKELETON_MIRRORING,
		NSFILTER_COUNT
	};

public class KinectInterface : MonoBehaviour {
	
	//----------------------------------------------------------------------------------------------------
	// CONSTANTS
	//----------------------------------------------------------------------------------------------------
	public const int NUI_SKELETON_COUNT = 6;
	public const int NUI_CAMERA_ELEVATION_MAXIMUM  = 27;
	public const int NUI_CAMERA_ELEVATION_MINIMUM = -27;
	
	private const int POS_COUNT = (int)NUI_SKELETON_POSITION_INDEX.NUI_SKELETON_POSITION_COUNT;
	public const int PACK_BYTES = 8;
	


	
	public enum NuiImageStream
	{
		NSM_IMAGE_STREAM_DEPTH_640x480,
		NSM_IMAGE_STREAM_DEPTH_320x240,
	    NSM_IMAGE_STREAM_DEPTH_320x240_IN_COLOR_SPACE,
		NSM_IMAGE_STREAM_DEPTH_80x60,
		NSM_IMAGE_STREAM_DEPTH_AND_PLAYERINDEX_640x480,
		NSM_IMAGE_STREAM_DEPTH_AND_PLAYERINDEX_320x240,
	    NSM_IMAGE_STREAM_DEPTH_AND_PLAYERINDEX_320x240_IN_COLOR_SPACE,
		NSM_IMAGE_STREAM_DEPTH_AND_PLAYERINDEX_80x60,
		NSM_IMAGE_STREAM_COLOR_640x480,
		NSM_IMAGE_STREAM_COLOR_IN_DEPTH_SPACE_640x480,
		NSM_IMAGE_STREAM_COLOR_YUV_640x480,
		NSM_IMAGE_STREAM_COUNT
	};
	
	/// <summary>
	/// Skeleton Joints
	/// </summary>
    public enum NUI_SKELETON_POSITION_INDEX
    {
        NUI_SKELETON_POSITION_HIP_CENTER,
        NUI_SKELETON_POSITION_SPINE,
        NUI_SKELETON_POSITION_SHOULDER_CENTER,
        NUI_SKELETON_POSITION_HEAD,
        NUI_SKELETON_POSITION_SHOULDER_LEFT,
        NUI_SKELETON_POSITION_ELBOW_LEFT,
        NUI_SKELETON_POSITION_WRIST_LEFT,
        NUI_SKELETON_POSITION_HAND_LEFT,
        NUI_SKELETON_POSITION_SHOULDER_RIGHT,
        NUI_SKELETON_POSITION_ELBOW_RIGHT,
        NUI_SKELETON_POSITION_WRIST_RIGHT,
        NUI_SKELETON_POSITION_HAND_RIGHT,
        NUI_SKELETON_POSITION_HIP_LEFT,
        NUI_SKELETON_POSITION_KNEE_LEFT,
        NUI_SKELETON_POSITION_ANKLE_LEFT,
        NUI_SKELETON_POSITION_FOOT_LEFT,
        NUI_SKELETON_POSITION_HIP_RIGHT,
        NUI_SKELETON_POSITION_KNEE_RIGHT,
        NUI_SKELETON_POSITION_ANKLE_RIGHT,
        NUI_SKELETON_POSITION_FOOT_RIGHT,
        NUI_SKELETON_POSITION_COUNT
    };
	
	/// <summary>
	/// Skeleton Tracking State 
	/// </summary>
	public enum NUI_SKELETON_TRACKING_STATE
    {
        NUI_SKELETON_NOT_TRACKED = 0,
        NUI_SKELETON_POSITION_ONLY,
        NUI_SKELETON_TRACKED
    };
	
	/// <summary>
	/// Skeleton Position Tracking state 
	/// </summary>
    public enum NUI_SKELETON_POSITION_TRACKING_STATE
    {
        NUI_SKELETON_POSITION_NOT_TRACKED = 0,
        NUI_SKELETON_POSITION_INFERRED,
        NUI_SKELETON_POSITION_TRACKED
    };
	
	/// <summary>
	/// Should match SkeletonDataType enum in KinectPlugin.h
	/// </summary>
	public enum SkeletonDataType
	{
		KP_SKELETON_DATA_TYPE_UNFILTERED = 0,
		KP_SKELETON_DATA_TYPE_FILTERED,
		KP_SKELETON_DATA_TYPE_COUNT,
	};
		
	//----------------------------------------------------------------------------------------------------
	// STRUCTS
	//----------------------------------------------------------------------------------------------------
	
	/// <summary>
	/// Vector4 defined for NuiStream 
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Pack=PACK_BYTES)]
    public struct Vector4
    {	
		public float X;
        public float Y;
        public float Z;
        public float W;
    };
	
	/// <summary>
	/// Skeleton data 
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Pack=PACK_BYTES)]
    public struct NUI_SKELETON_DATA
    {
        public NUI_SKELETON_TRACKING_STATE eTrackingState;
        public int dwTrackingID;
        public int dwEnrollmentIndex;
        public int dwUserIndex;
        public Vector4 Position;

        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = POS_COUNT)]
        public Vector4[] SkeletonPositions;

        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = POS_COUNT)]
        public NUI_SKELETON_POSITION_TRACKING_STATE[] eSkeletonPositionTrackingState;

        public int dwQualityFlags;

        // Must add some padding so that we match up with the NUI data structs exactly.
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 12)]
        public byte[] m_padding;
    };
	
	/// <summary>
	/// Skeleton frame data 
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Pack=PACK_BYTES)]
    public struct NUI_SKELETON_FRAME
    {		
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        public uint liTimeStampLowPart;
        public uint liTimeStampHighPart;
#else
        public uint liTimeStampHighPart;
        public uint liTimeStampLowPart;
#endif
        public int dwFrameNumber;
        public int dwFlags;
        public Vector4 vFloorClipPlane;
        public Vector4 vNormalToGravity;

        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = NUI_SKELETON_COUNT)]
        public NUI_SKELETON_DATA[] SkeletonData;
    };
	
	[System.Runtime.InteropServices.DllImportAttribute("Assets\\Plugins\\UnityGAK.dll", EntryPoint = "GAKInit")]
	public static extern int GAKInit();
	
	[System.Runtime.InteropServices.DllImportAttribute("Assets\\Plugins\\UnityGAK.dll", EntryPoint = "GAKUpdate")]
	public static extern int GAKUpdate(float dt);
	
	[System.Runtime.InteropServices.DllImportAttribute("Assets\\Plugins\\UnityGAK.dll", EntryPoint = "GAKClose")]
	public static extern int GAKClose();
	
	//[System.Runtime.InteropServices.DllImportAttribute("Assets\\Plugins\\UnityGAK.dll", EntryPoint = "GAKCopyImageDataToTexture")]
	//public static extern int GAKCopyImageDataToTexture(int textureId, NuiImageStream dataStreamType);
	
	[System.Runtime.InteropServices.DllImportAttribute("Assets\\Plugins\\UnityGAK.dll", EntryPoint = "GAKCopyImageDataToTexturePtr")]
	public static extern int GAKCopyImageDataToTexturePtr(IntPtr TextureMemory, NuiImageStream dataStreamType);
	
	[System.Runtime.InteropServices.DllImportAttribute("Assets\\Plugins\\UnityGAK.dll", EntryPoint = "GAKGetSkeletonData")]
	public static extern int GAKGetSkeletonData( out NUI_SKELETON_FRAME skeletonFrameData);
	
	[System.Runtime.InteropServices.DllImportAttribute("Assets\\Plugins\\UnityGAK.dll", EntryPoint = "GAKInitStreamFilter")]
	public static extern int GAKInitStreamFilter( uint FilterFlag, float Priority);
	
	[System.Runtime.InteropServices.DllImportAttribute("Assets\\Plugins\\UnityGAK.dll", EntryPoint = "GAKSetFilterData")]
	public static extern int GAKSetFilterData(uint FilterFlag, IntPtr Data);
	
	[System.Runtime.InteropServices.DllImportAttribute("Assets\\Plugins\\UnityGAK.dll", EntryPoint = "GAKGetFilterData")]
	public static extern int GAKGetFilterData(uint FilterFlag, IntPtr Data);
	
	[System.Runtime.InteropServices.DllImportAttribute("Assets\\Plugins\\UnityGAK.dll", EntryPoint = "GAKSetPlayerAddedCallback")]
	public static extern int GAKSetPlayerAddedCallback(IntPtr Delegate);
	
	[System.Runtime.InteropServices.DllImportAttribute("Assets\\Plugins\\UnityGAK.dll", EntryPoint = "GAKSetPlayerRemovedCallback")]
	public static extern int GAKSetPlayerRemovedCallback(IntPtr Delegate);
	
	[System.Runtime.InteropServices.DllImportAttribute("Assets\\Plugins\\UnityGAK.dll", EntryPoint = "GAKSetPlayerRemappedCallback")]
	public static extern int GAKSetPlayerRemappedCallback(IntPtr Delegate);
	
	[System.Runtime.InteropServices.DllImportAttribute("Assets\\Plugins\\UnityGAK.dll", EntryPoint = "GAKSetPlayerCount")]
	public static extern int GAKSetPlayerCount(uint Count);
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	/*using UnityEngine;
using System;
using System.Runtime.InteropServices;

public class KinectTexture : MonoBehaviour {
	
    //Link to the UnityTexture plugin and call the UpdateTexture function there.
    //[DllImport ("UnityTest")]
	[System.Runtime.InteropServices.DllImportAttribute("UnityTest.DLL", EntryPoint = "UpdateColorStream")]
    private static extern void UpdateColorStream (IntPtr pBits, int width, int height);
	
	//[DllImport ("UnityTest")]
	[System.Runtime.InteropServices.DllImportAttribute("UnityTest.DLL", EntryPoint = "StartKinect")]
    private static extern void StartKinect();
	
	//[DllImport ("UnityTest")]
    [System.Runtime.InteropServices.DllImportAttribute("UnityTest.DLL", EntryPoint = "ShutdownKinect")]
	private static extern void ShutdownKinect();
	
	public int width = 640;
    public int height = 480;
    private Texture2D m_Texture;

    private Color32[] m_Pixels;
    private GCHandle m_PixelsHandle;
        
    void Start () {
        // Create texture that will be updated in the plugin
        m_Texture = new Texture2D (width, height, TextureFormat.ARGB32, false);
        // Create the pixel array for the plugin to write into at startup    
        m_Pixels = m_Texture.GetPixels32(0);
        // "pin" the array in memory, so we can pass direct pointer to it's data to the plugin,
        // without costly marshaling of array of structures.
        m_PixelsHandle = GCHandle.Alloc(m_Pixels, GCHandleType.Pinned);
		
		StartKinect();
		
        // Assign texture to the renderer
        if (renderer)
            renderer.material.mainTexture = m_Texture;
        // or gui texture
        else if (GetComponent(typeof(GUITexture)))
        {
            GUITexture gui = GetComponent(typeof(GUITexture)) as GUITexture;
            gui.texture = m_Texture;
        }
        else
        {
            Debug.Log("Game object has no renderer or gui texture to assign the generated texture to!");
        }
    }
    
    void OnDisable() {
        // Free the pinned array handle.
        m_PixelsHandle.Free();
		ShutdownKinect();
    }


    
    // Now we can simply call UpdateTexture which gets routed directly into the plugin
    void Update () {
		
        UpdateColorStream (m_PixelsHandle.AddrOfPinnedObject(), m_Texture.width, m_Texture.height);
        m_Texture.SetPixels32(m_Pixels, 0);
        m_Texture.Apply ();
		
    }
}*/

}
