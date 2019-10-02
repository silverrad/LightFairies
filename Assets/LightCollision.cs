using UnityEngine;
using System.Collections;

public class LightCollision : MonoBehaviour {
	
	bool spawnLight = false;
	float timeOn = 0.0f;
	public float timeLight = 0.4f;

	// Use this for initialization
	void Start () {
		//this.gameObject.AddComponent<Light>();
		//gameObject.light.color = Color.yellow;
		//gameObject.light.type = LightType.Point;
		//gameObject.light.enabled = true;
	}
	
	// Update is called once per frame
	void Update () {
		
		if (spawnLight)
		{
			timeOn += Time.deltaTime;
			if (timeOn > timeLight)
			{
				timeOn = 0.0f;
				spawnLight = false;
				gameObject.light.enabled = false;
			}
		}
	}
	
	void OnCollisionEnter(Collision other) {
		
		other.collider.gameObject.GetComponent<ParticleScript>().ShowBounce();
		
	}
	
	void OnParticleCollision() {
		
		//Ray ray = new Ray(other.transform.position, this.transform.position - other.transform.position);
		//RaycastHit hit = new RaycastHit();
		
		//if (this.collider.Raycast(ray, out hit, 100.0f))
		//{
		//	gameObject.light.transform.position = hit.point;
		//}
		
		//spawnLight = true;
		//gameObject.light.enabled = true;
	}
}
