using UnityEngine;
using System.Collections;

public class ParticleScript : MonoBehaviour {
	
	public float timeOn = 0.5f;
	float timer = 0.0f;
	
	public float lifeTime = 8.0f;
	float lifeTimer = 0.0f;
	
	public bool lightOn = false;
	public int bounceCount = 1;
	
	public bool isCharge = false;
	
	
	private AudioClip SND_drip1;
	private AudioClip SND_drip2;
	private AudioClip SND_drip3;
	private AudioClip SND_drip4;
	private AudioClip SND_drip5;
	private AudioSource AudioSource_Particle;
	
	
	// Use this for initialization
	void Start () {
		//lifeTime = Random.Range(4.0f, 6.0f);
		
		SND_drip1 = Resources.Load("SND_drip1", typeof(AudioClip)) as AudioClip;
		SND_drip2 = Resources.Load("SND_drip2", typeof(AudioClip)) as AudioClip;
		SND_drip3 = Resources.Load("SND_drip3", typeof(AudioClip)) as AudioClip;
		SND_drip4 = Resources.Load("SND_drip4", typeof(AudioClip)) as AudioClip;
		SND_drip5 = Resources.Load("SND_drip5", typeof(AudioClip)) as AudioClip;
		
		AudioSource_Particle = gameObject.GetComponent<AudioSource>();
		
		
		
		//gameObject.light.color = new Color(Mathf.FloorToInt(Random.value * 255), Mathf.FloorToInt(Random.value * 255), Mathf.FloorToInt(Random.value * 255));
		gameObject.light.color = new Color(Mathf.FloorToInt(Random.value * 255), Mathf.FloorToInt(Random.value * 255), 255);
		
		//gameObject.light.color = new Color(255, 255, 255);
		
		
	}
	
	// Update is called once per frame
	void Update () {
		
		if (lightOn) {
			timer += Time.deltaTime;
			
			//gameObject.light.intensity = 0.01f - timer/10;
			
			if (timer > timeOn)
			{
				lightOn = false;
				gameObject.light.enabled = false;
				gameObject.light.intensity = 0;
			}
		}
		
		lifeTimer += Time.deltaTime;
		if (lifeTimer > lifeTime || bounceCount == 0)
		{
			Destroy(this.gameObject);	
		}
	}
	
	public void ShowBounce(){
		
		//other.collider.gameObject.rigidbody.angularVelocity = -other.collider.gameObject.rigidbody.angularVelocity;
		gameObject.light.enabled = true;
		lightOn = true;
		timer = 0.0f;
		gameObject.light.intensity = 0.001f;
		bounceCount--;
		
		
		switch(Mathf.FloorToInt(Random.value * 5)){
			case 0:
				PlaySound(SND_drip1);
				break;
			case 1:
				PlaySound(SND_drip2);
				break;
			case 2:
				PlaySound(SND_drip3);
				break;
			case 3:
				PlaySound(SND_drip4);
				break;
			case 4:
				PlaySound(SND_drip5);
				break;
		}
		
	}
	
	
	private void PlaySound(AudioClip _audioClip){
		AudioSource_Particle.clip = _audioClip;
		AudioSource_Particle.Play();
		
	}
}
