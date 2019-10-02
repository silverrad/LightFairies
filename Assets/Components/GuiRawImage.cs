using UnityEngine;
using System.Collections;


public class GuiRawImage : MonoBehaviour {

	public KinectManager m_Manager;
	public Texture m_Texture;

	// Use this for initialization
	void Start () {
		if(m_Manager == null)
			m_Manager = (KinectManager) FindObjectOfType(typeof(KinectManager));
	}
	
	void Update()
	{

		m_Texture = m_Manager.ColorStream;
	}

	
	void OnGUI()
	{
		GUIUtility.ScaleAroundPivot(new Vector2(-1,-1), new Vector2(m_Texture.width / 2, m_Texture.height / 2));
		GUI.Label(new Rect(0,0,m_Texture.width,m_Texture.height),m_Texture);
	}
}
