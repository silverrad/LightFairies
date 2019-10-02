using UnityEngine;
using System.Collections;

public class DebugSkeletonNodeRoot : MonoBehaviour {
	
	public int frameIndex = 0;
	public bool enableSkeleton = true;
	
	public KinectInterface.NUI_SKELETON_POSITION_INDEX BodyPart;
	public KinectManager m_Manager;
	
	
	public GameObject GO_handR;
	public GameObject GO_handL;
	
	public GameObject fairyLeft;
	public GameObject fairyRight;
	
	public GameObject nearestObj;
	public GameObject target;
	
	public GameObject g_Object;
	
	
	private AudioClip SND_particleExplosion;
	private AudioClip SND_particleDirectional;
	
	
	public enum POS
    {
        HIP_CENTER,
        SPINE,
        SHOULDER_CENTER,
        HEAD,
        SHOULDER_LEFT,
        ELBOW_LEFT,
        WRIST_LEFT,
        HAND_LEFT,
        SHOULDER_RIGHT,
        ELBOW_RIGHT,
        WRIST_RIGHT,
        HAND_RIGHT,
        HIP_LEFT,
        KNEE_LEFT,
        ANKLE_LEFT,
        FOOT_LEFT,
        HIP_RIGHT,
        KNEE_RIGHT,
        ANKLE_RIGHT,
        FOOT_RIGHT,
        COUNT
    };
	
	private bool clapLock = false;
	private bool transport = true;
	private float distanceThreshold = 75f;
	
	private float DimmingSec;
	private float distance;
	
	private AudioSource audioSource;
	
	private ArrayList pendingParticles = new ArrayList();
	
	
	private float energy = 0;
	private const float MAX_ENERGY = 60.0f;
	
	// Use this for initialization
	void Start () {
		if(m_Manager == null)
		{
			m_Manager = (KinectManager)FindObjectOfType(typeof(KinectManager));
		}
		
		SND_particleExplosion = Resources.Load("SND_particleExplosion", typeof(AudioClip)) as AudioClip;
		SND_particleDirectional = Resources.Load("SND_particleDirectional", typeof(AudioClip)) as AudioClip;
		
		audioSource = this.GetComponent<AudioSource>();
	}
	
	private Vector3 rHandPos = Vector3.zero;
	private Vector3 lHandPos = Vector3.zero;
	
	float spawnTimer = 0.0f;
	
	
	private Vector3 clapStartingPos;
	private float clapStartDistance = 30.0f;
	
	private float explosionStartDistance = 20.0f;
	
	private float posScale = 150.0f;
	
	// Update is called once per frame
	void Update () {
		KinectInterface.NUI_SKELETON_DATA skeletonData;
		
		if(m_Manager.GetFirstTrackedSkeleton(0, out skeletonData))
		{
			
			Vector3 nextRHandPos = new Vector3 (skeletonData.SkeletonPositions[(int)POS.HAND_RIGHT].X * posScale, skeletonData.SkeletonPositions[(int)POS.HAND_RIGHT].Y * posScale, skeletonData.SkeletonPositions[(int)POS.HAND_RIGHT].Z * (-posScale) * 2);
			Vector3 nextLHandPos = new Vector3 (skeletonData.SkeletonPositions[(int)POS.HAND_LEFT].X * posScale, skeletonData.SkeletonPositions[(int)POS.HAND_LEFT].Y * posScale, skeletonData.SkeletonPositions[(int)POS.HAND_LEFT].Z * (-posScale) * 2);
			
			rHandPos = Vector3.Lerp(rHandPos, nextRHandPos, Time.deltaTime * 6);
			lHandPos = Vector3.Lerp(lHandPos, nextLHandPos, Time.deltaTime * 6);
			
			fairyRight.transform.LookAt(new Vector3(0,0,0));
			fairyLeft.transform.LookAt(new Vector3(0,0,0));
			
			//CLAP
			
			float fHandsDistance = Vector3.Distance (rHandPos, lHandPos);
			
			
			if (fHandsDistance < 15 && !clapLock) {
				clapLock = true;
				//clapStartingPos = Vector3.Lerp(lHandPos, rHandPos, 0.5f);
				
				
				//Debug.Log ("Clap Detected");;
				//print("clapStartingPos : " + clapStartingPos);
				
			} 
			
			if(fHandsDistance < 20){
				clapStartingPos = (rHandPos + lHandPos) / 2.0f;
				
			}else if(clapLock){
				if (Vector3.Distance(clapStartingPos, rHandPos) > explosionStartDistance && Vector3.Distance(clapStartingPos, lHandPos) > explosionStartDistance){
					//If both hands equally break apart, emit spherical particle explosion
					PlaySound(SND_particleExplosion);
					
					for (int i = 0; i < 100; i++){
						GameObject g = (GameObject)Instantiate(g_Object, GO_handR.transform.position + new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), Quaternion.identity);
						g.rigidbody.AddExplosionForce(700.0f, GO_handR.transform.position, 300.0f);
					}
					
					energy = 0;
					clapLock = false;
				}else if (Vector3.Distance(clapStartingPos, rHandPos) > clapStartDistance){
					//Shoot toward right hand
					PlaySound(SND_particleDirectional);
					
					for (int i = 0; i < 50 + energy; i++){
						GameObject g = (GameObject)Instantiate(g_Object, clapStartingPos + new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), Quaternion.identity);
						pendingParticles.Add(g);
					}
					
					StartCoroutine(DirectParticles(clapStartingPos, rHandPos));
					energy = 0;
					clapLock = false;
				}else if (Vector3.Distance(clapStartingPos, lHandPos) > clapStartDistance){
					//Shoot toward left hand
					PlaySound(SND_particleDirectional);
					
					for (int i = 0; i < 50 + energy; i++){
						GameObject g = (GameObject)Instantiate(g_Object, clapStartingPos + new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)), Quaternion.identity);
						pendingParticles.Add(g);
					}
					
					StartCoroutine(DirectParticles(clapStartingPos, lHandPos));
					energy = 0;
					clapLock = false;
				}
			}
			
			
			//Fairy instruction
			if( fHandsDistance < 50){
				fairyRight.transform.LookAt(fairyLeft.transform.position);
				fairyLeft.transform.LookAt(fairyRight.transform.position);
			}
			
			if(clapLock){
				
				if(energy < MAX_ENERGY){
					energy += Time.deltaTime * 30;
				}
				//Debug.Log("E : " + energy);
				spawnTimer += energy * 0.25f; //Time.deltaTime * spawnSpeed;
				if (spawnTimer >= 1.0f)
				{
					float spawnAmt = spawnTimer;
					for (int i = 0; i < spawnAmt; i++)
					{
						GameObject g = (GameObject)Instantiate(g_Object, GO_handR.transform.position + Random.insideUnitSphere * 50 * energy/MAX_ENERGY, 
									Quaternion.identity);		
						//g.rigidbody.AddExplosionForce(700.0f, GO_handR.transform.position, 300.0f);
						g.rigidbody.velocity = (GO_handR.transform.position - g.transform.position).normalized * 100.0f;
						g.GetComponent<ParticleScript>().bounceCount = 1;
						g.GetComponent<ParticleScript>().lifeTime = 0.25f;
						g.GetComponent<ParticleScript>().isCharge = true;
						
						if (!transport)
						{
							g.renderer.material.color = Color.red;	
						}
						spawnTimer--;
					}
				}
			}
			else
			{
				spawnTimer = 0.0f;
			}
			//END CLAP
		
		
				
		//TRANSPORT
			
			if(clapLock){
				//Case 1 : Hands were previously apart and the user has clapped on an object
				if(transport){
					target = findNearestTaggedObj(GO_handR.transform.position, "transport");
					if(target != null){
							transport = false;
							target.transform.renderer.enabled = false;	
					}
				}
				//Case 2: Hands were previously together and the user has released an object
				else{
					if(!transport){	
						target.transform.renderer.enabled = true;	
						if(findNearestTaggedObj(GO_handR.transform.position, "dock") != null){
							target.transform.position = findNearestTaggedObj(GO_handR.transform.position, "dock").transform.position;
							Debug.Log("level complete!!!!!!!!!!!!!!!");
						}
						target.transform.position = GO_handR.transform.position;
						transport = true;
					}
				}	
			}
			//END TRANSPORT OBJECT
			
		//GO_handR.transform.position = new Vector3(rHandPos.x, rHandPos.y, rHandPos.z * (-4) + 500);
		//GO_handL.transform.position = new Vector3(lHandPos.x, lHandPos.y, lHandPos.z * (-4) + 500);
			
		
		GO_handR.transform.position = rHandPos;
		GO_handL.transform.position = lHandPos;
			
			
			//GO_handR.transform.LookAt(nextRHandPos);
			//GO_handL.transform.LookAt(nextLHandPos);
			
			//fairyRight.transform.LookAt(new Vector3(0,0,0));
			//fairyLeft.transform.LookAt(new Vector3(0,0,0));
			
			
			
			
			
	}
	
}
	
	/*Finds the closest tagged object to given hand position within a given threshold*/
	GameObject findNearestTaggedObj (Vector3 handPosition, string searchTag)
	{
		nearestObj=null; distance = distanceThreshold; 
		GameObject[] taggedGameObjects = GameObject.FindGameObjectsWithTag(searchTag); 
		foreach (GameObject obj in taggedGameObjects){
			float distanceSqr = Vector3.Distance(obj.transform.position, handPosition);
			//set minimal distance gameObject
        	if (distanceSqr < distance) {
            	nearestObj = obj;
            	distance = distanceSqr;
        	}
    	}
    	return nearestObj;
	}

	
	
	IEnumerator DirectParticles (Vector3 _StartPos, Vector3 _EndPos)
	{
		
		Vector3 StartPos = _StartPos;
		//yield return new WaitForSeconds(0.1f);
		Vector3 EndPos = _EndPos;
		
		//Debug.Log("StartPos : " + StartPos);
		//Debug.Log("EndPos : " + EndPos);
		
		//Vector3 result = Vector3.Project(StartPos, EndPos);
		//Debug.Log("Projected : " + result);
		
		
		Vector3 ForceToAdd = (EndPos - StartPos) * 100;
		//ForceToAdd.z = ForceToAdd.z * (-1);
			
		foreach(GameObject p in pendingParticles){
			//p.rigidbody.Sleep
			if(p != null){
				p.rigidbody.AddForce(ForceToAdd);
			}
			//p.rigidbody.AddForce((StartPos - EndPos) * 100);
		}
		pendingParticles.Clear();
		
		
		yield return null;
		//EndPos - StartPos
	}
		
	private void PlaySound(AudioClip _audioClip){
		audioSource.clip = _audioClip;
		audioSource.Play();
	}
		
}
