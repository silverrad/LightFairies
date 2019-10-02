using UnityEngine;
using System.Collections;

public class ParticleSpawn : MonoBehaviour {
	
	public GameObject g_Object;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		if (Input.GetKey(KeyCode.P))
		{
			for (int i = 0; i < 1000; i++)
			{
				GameObject g = (GameObject)Instantiate(g_Object, this.transform.position + new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), 
							Quaternion.identity);		
				g.rigidbody.AddExplosionForce(200.0f, this.transform.position, 10.0f);
			}
		}
	}
}
