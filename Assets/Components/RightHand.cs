using UnityEngine;
using System.Collections;

public class RightHand : MonoBehaviour {
	
	
	private KinectManager m_Manager;
	
	// Use this for initialization
	void Start () {
		m_Manager = (KinectManager)FindObjectOfType(typeof(KinectManager));
	}
	
	// Update is called once per frame
	void Update () {
		if(m_Manager != null)
		{
			KinectInterface.NUI_SKELETON_DATA data;
			if(m_Manager.GetFirstTrackedSkeleton(0, out data))
			{
				Vector3 vector = new Vector3(
				data.SkeletonPositions[(int)KinectInterface.NUI_SKELETON_POSITION_INDEX.NUI_SKELETON_POSITION_HAND_RIGHT].X * 5,
		    	data.SkeletonPositions[(int)KinectInterface.NUI_SKELETON_POSITION_INDEX.NUI_SKELETON_POSITION_HAND_RIGHT].Y * 5,
				(-data.SkeletonPositions[(int)KinectInterface.NUI_SKELETON_POSITION_INDEX.NUI_SKELETON_POSITION_HAND_RIGHT].Z * 2));
				
				
				
				gameObject.transform.localPosition = vector + new Vector3(0,0,3.5f);
			}
		}
	}
}
