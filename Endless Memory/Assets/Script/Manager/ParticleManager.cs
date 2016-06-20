using UnityEngine;
using System.Collections;

public class ParticleManager : MonoBehaviour {

    public GameObject slimeParticle;
    public float upForce = 0f;

    private GameObject particle;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void emitSlimeParticle(Vector3 pos, Vector3 tar)
    {
        Vector3 shotPos = pos;
        shotPos.y = 0.5f;
        Vector3 dir = tar - pos; 
        particle = Instantiate(slimeParticle, shotPos, Quaternion.Euler(dir)) as GameObject;
        Vector3 shotDir = dir + new Vector3(0, upForce, 0);
        particle.GetComponent<Rigidbody>().AddForce(shotDir, ForceMode.VelocityChange);
    }
}
