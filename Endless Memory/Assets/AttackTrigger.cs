using UnityEngine;
using System.Collections;

public class AttackTrigger : MonoBehaviour {

    private bool isActive = false;
    public int life = 10;
    public int age = 0;
    public int damage = 10;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (isActive)
        {
            age += 1;
            if (age >= life)
                Destroy(gameObject);
        }
	}

    public void SetDamage(int d)
    {
        isActive = true;
        damage = d;
    }
}
