using UnityEngine;
using System.Collections;

using System.Collections.Generic;
using System.Runtime.InteropServices;
using Filters;
using System;
//Singleton class that handles the connection between Unity and the Plugin

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void PlayerAddedCallback(UInt32 PlayerMask, UInt32 SkeletonIndex);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void PlayerRemovedCallback(UInt32 PlayerMask);

[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate void PlayerRemappedCallback(UInt32 OldPlayer, UInt32 NewPlayer);

public class KinectManager : MonoBehaviour {
	
	bool m_ColorStreamEnabled;
	bool m_DepthStreamEnabled;
	
	Texture2D m_ColorTexture;
	Color32[] m_ColorArray;
	GCHandle m_ColorHandle;
	
	Texture2D m_DepthTexture;
	Color32[] m_DepthArray;
	GCHandle m_DepthHandle;
	
	private const int KP_TEXTURE_WIDTH = 1024;
	private const int KP_TEXTURE_HEIGHT = 512;
	
	
	public bool ColorStreamEnabled
	{
		get
		{
			return m_ColorStreamEnabled;
		}
		set
		{
			m_ColorStreamEnabled = value;
		}		
	}
	
	public bool DepthStreamEnabled
	{
		get
		{
			return m_DepthStreamEnabled;
		}
		set
		{
			m_DepthStreamEnabled = value;
		}
	}
	
	public Texture2D ColorStream
	{
		get
		{
			return m_ColorTexture;
		}
	}
	
	public Texture2D DepthStream
	{
		get
		{
			return m_DepthTexture;
		}
	}
	
	// Use this for initialization
	void Start () {
		//Start Kinect
		KinectInterface.GAKInit();
		//KinectInterface.GAKInitStreamFilter((uint)FilterFlags.NSFILTER_SKELETON_MIRRORING, 1.0f);
		KinectInterface.GAKInitStreamFilter((uint)FilterFlags.NSFILTER_SKELETON_SMOOTHING, 2.0f);
		
		PlayerManager Manager = (PlayerManager)FindObjectOfType(typeof(PlayerManager));
		
		SetPlayerAddedCallback(Manager.PlayerAdded);
		SetPlayerRemovedCallback(Manager.PlayerRemoved);
		//SetPlayerRemappedCallback(PlayerManager.PlayerReplaced);	
		
		m_ColorTexture = new Texture2D(640, 480, TextureFormat.RGBA32, false);
		m_ColorArray = m_ColorTexture.GetPixels32(0); 
		m_ColorHandle = GCHandle.Alloc(m_ColorArray, GCHandleType.Pinned);
		
		m_DepthTexture = new Texture2D(320, 240, TextureFormat.RGBA32, false);
		m_DepthArray = m_DepthTexture.GetPixels32();
		m_DepthHandle = GCHandle.Alloc(m_DepthArray, GCHandleType.Pinned);
		
		Manager.MaxPlayers = 2;
		
	}
	
	
	public int SetFilterData<T>(uint FilterFlag, T Data)
	{
		GCHandle Temp = GCHandle.Alloc(Data, GCHandleType.Pinned);
		int RetVal = KinectInterface.GAKSetFilterData(FilterFlag, Temp.AddrOfPinnedObject());
		Temp.Free();
		return RetVal;
	}
	
	public int GetFilterData<T>(uint FilterFlag, ref T Data) where T : struct
	{
		var ArrayBox = new T[1]{Data};
		
		GCHandle Temp = GCHandle.Alloc(ArrayBox, GCHandleType.Pinned);
		
		int RetVal = KinectInterface.GAKGetFilterData(FilterFlag, Temp.AddrOfPinnedObject());
		
		Data = ArrayBox[0];
		
		Temp.Free();
		
		return RetVal;
	}
	                  
	
	// Update is called once per frame
	void Update () {
	
		//Update Kinect
		KinectInterface.GAKUpdate(.1f);
		//GetRawImage();
		//GetDepthImage();
	}
	
	void OnDisable() {
        
    }
	
	void OnApplicationQuit()
    {
		
  		KinectInterface.GAKClose();
		// Free the pinned array handle.
        m_ColorHandle.Free();
		m_DepthHandle.Free();
    }
	
	public Texture GetRawImage()
	{

		
		KinectInterface.GAKCopyImageDataToTexturePtr(m_ColorHandle.AddrOfPinnedObject(), KinectInterface.NuiImageStream.NSM_IMAGE_STREAM_COLOR_640x480);
		m_ColorTexture.SetPixels32(m_ColorArray);
		m_ColorTexture.Apply();
		
		return m_ColorTexture;
	}
	
	public Texture GetDepthImage()
	{
		KinectInterface.GAKCopyImageDataToTexturePtr(m_DepthHandle.AddrOfPinnedObject(), KinectInterface.NuiImageStream.NSM_IMAGE_STREAM_DEPTH_AND_PLAYERINDEX_320x240);
		m_DepthTexture.SetPixels32(m_DepthArray);
		m_DepthTexture.Apply();
		
		return m_DepthTexture;
	}
	
	public bool GetFirstTrackedSkeleton(int frameIndex, out KinectInterface.NUI_SKELETON_DATA skeletonData)
    {
		KinectInterface.NUI_SKELETON_FRAME frameData;
       	KinectInterface.GAKGetSkeletonData( out frameData );
		
        skeletonData = new KinectInterface.NUI_SKELETON_DATA();  
		for (int skeletonIndex = 0; skeletonIndex < KinectInterface.NUI_SKELETON_COUNT; skeletonIndex++)
        {
             if (frameData.SkeletonData[skeletonIndex].eTrackingState == KinectInterface.NUI_SKELETON_TRACKING_STATE.NUI_SKELETON_TRACKED)
             {
				skeletonData = frameData.SkeletonData[skeletonIndex];
                return true;
             }				
        }
        return false;
    }
	
	public void SetPlayerAddedCallback(PlayerAddedCallback Delegate)
	{
		IntPtr ptr = Marshal.GetFunctionPointerForDelegate(Delegate);
		KinectInterface.GAKSetPlayerAddedCallback(ptr);
	}
	
	public void SetPlayerRemovedCallback(PlayerRemovedCallback Delegate)
	{
		IntPtr ptr = Marshal.GetFunctionPointerForDelegate(Delegate);
		KinectInterface.GAKSetPlayerRemovedCallback(ptr);
	}
	public void SetPlayerRemappedCallback(PlayerRemappedCallback Delegate)
	{
		IntPtr ptr = Marshal.GetFunctionPointerForDelegate(Delegate);
		KinectInterface.GAKSetPlayerRemappedCallback(ptr);
	}
	
	
}
