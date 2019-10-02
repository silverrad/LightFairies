using UnityEngine;
using System.Collections;
using System;
using System.Diagnostics;

public class PlayerManager : MonoBehaviour {
	
	public struct EnrolledPlayer
	{
		public bool Active;
		public UInt32 PlayerID;
		public UInt32 SkeletonID;
	};
	
	public const int NUI_SKELETON_MAX_TRACKED_COUNT = 2;
	
	private uint maxPlayers = 1;
	
//	static public void PlayerAdded(UInt32 Player, UInt32 SkeletonIndex)
//	{
//		UnityEngine.Debug.Log("Player " + Player.ToString() + " added at index" + SkeletonIndex + " .");	
//	}
//	
//	static public void PlayerRemoved(UInt32 Player)
//	{
//		UnityEngine.Debug.Log("Player " + Player.ToString() + " removed.");
//	}
//	
//	static public void PlayerReplaced(UInt32 OldPlayer, UInt32 NewPlayer)
//	{
//		UnityEngine.Debug.Log("Player " + OldPlayer.ToString() + " is now Player " + NewPlayer.ToString() + " .");
//	}
//	
	
	public PlayerAddedCallback PlayerAdded;
	public PlayerRemovedCallback PlayerRemoved;
	
	public uint TotalPlayers = 0;
	private EnrolledPlayer[] Players = new EnrolledPlayer[NUI_SKELETON_MAX_TRACKED_COUNT];
	
	public uint MaxPlayers
	{
		get{ return maxPlayers;}
		set
		{ 
			if(value <= NUI_SKELETON_MAX_TRACKED_COUNT)
			{
				maxPlayers = value;
				KinectInterface.GAKSetPlayerCount(maxPlayers);
			}
		}
	}
	
	
	// Use this for initialization
	void Start () {
	
		PlayerAdded = (Player, SkeletonIndex) =>
		{
			if(TotalPlayers < MaxPlayers)
			{
				Players[TotalPlayers].Active = true;
				Players[TotalPlayers].PlayerID = Player;
				Players[TotalPlayers].SkeletonID = SkeletonIndex;
				TotalPlayers++;
			}
		};
		
		PlayerRemoved = (Player) =>
		{
			for(int I = 0; I < TotalPlayers; ++I)
			{
				if(Players[I].Active && Players[I].PlayerID == Player)
				{
					Players[I].Active = false;
					for(int J = I + 1; J < TotalPlayers; ++J)
					{
						if(Players[J].Active)
						{
							Players[J-1] = Players[J];
							Players[J].Active = false;
						}
					}
					TotalPlayers--;
				}
			}
		};
		
		
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
