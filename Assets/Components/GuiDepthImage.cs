using UnityEngine;
using System.Collections;


public class GuiDepthImage : MonoBehaviour {

	public KinectManager m_Manager;
	public Texture m_Texture;

	// Use this for initialization
	void Start () {
		if(m_Manager == null)
			m_Manager = (KinectManager) FindObjectOfType(typeof(KinectManager));
	}
	
	void Update()
	{

		m_Texture = m_Manager.DepthStream;
	}

	
	void OnGUI()
	{
		PlayerManager Manager = (PlayerManager)FindObjectOfType(typeof(PlayerManager));
		
		GUI.Label(new Rect(240, 500, 300, 50), Manager.TotalPlayers.ToString());
		GUIUtility.ScaleAroundPivot(new Vector2(-1,-1), new Vector2(m_Texture.width / 2, m_Texture.height / 2));
		GUI.Label(new Rect(-640,-240,m_Texture.width,m_Texture.height),m_Texture);
		
	}
}