using UnityEngine;
using System.Collections;
using MemoryTrap;

public class BulletController : MonoBehaviour {

    public int damage = 10;
    public int g = 0;
    private float life = 10f;
    private float age = 0f;
	// Use this for initialization
	void Start () {
	   
	}
	
	// Update is called once per frame
	void Update () {
        age += Time.deltaTime;
        if (age >= life)
            Destroy(gameObject);
	}

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" || other.tag == "Ground")
        {
            if(other.tag == "Player")
                other.GetComponent<PlayerManager>().DecreaseHp(damage);
            // other.GetComponent<PlayerManager>()
            Destroy(gameObject);
        }
    }
}
