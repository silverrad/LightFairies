using UnityEngine;
using System.Collections;

public class LightEat : MonoBehaviour {
	
	public float absorbRange = 100.0f;
	
	public float lightAmount = 0.0f;
	
	public float lightAmountLimit = 255.0f;
	
	private Light lightSource;
	
	public float lightScaleRate = 1.0f;
	
	public Color lightColor;
	
	
	// Use this for initialization
	void Start () {
		lightSource = this.gameObject.GetComponent<Light>();
		
		lightSource.color = lightColor;
		
	}
	
	// Update is called once per frame
	void Update () {
		ParticleScript[] p_script = FindObjectsOfType(typeof(ParticleScript)) as ParticleScript[];
		foreach(ParticleScript p in p_script)
		{
			if (Vector3.Distance(this.transform.position, p.transform.position) < absorbRange && !p.isCharge)
			{
				float speed = p.rigidbody.velocity.magnitude;
				p.rigidbody.velocity = (this.transform.position - p.transform.position).normalized;
				p.rigidbody.velocity *= speed;
			}
		}
		
		if(lightAmount > 0){
			lightAmount -= Time.deltaTime * 10;
		}else{
			lightAmount = 0;
		}
		
		lightSource.range = lightAmount;
		
		//float illumAmount = lightAmount / (lightAmountLimit * 2);
		
		//renderer.material.color = new Color(illumAmount, illumAmount, illumAmount);
		renderer.material.color = new Color(MapToRange(lightAmount, 255, lightColor.r), MapToRange(lightAmount, 255, lightColor.g), MapToRange(lightAmount, 255, lightColor.b));
		
		//interpolate between 0 to goal illum light color for each r/g/b
		
		
		
		
		//Debug.Log(lightAmount);
		
	}
	
	void OnCollisionEnter(Collision other) {
		
		lightAmount += 1.0f * lightScaleRate;
		
		if(lightAmount >= lightAmountLimit){
			lightAmount = lightAmountLimit;
		}
		
		
		
		//print(lightAmount);
		//other.collider.gameObject.GetComponent<ParticleScript>().ShowBounce();
		Destroy(other.collider.gameObject);
		
	}
	
	
	public float MapToRange (float num, float initialRange, float finalRange)
	{
		
		float result = num;
		result /= initialRange;
		result *= finalRange;
		return result;
	}
	
	
	
}
