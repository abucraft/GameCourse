using UnityEngine;
using System.Collections;

public class MonsterController : MonoBehaviour {

    public enum MonsterState
    {
        stroll, move, attack, dead
    }

    public Transform target;
    public float maxSight;
    public Vector3 velocity;
    public float hitPoint;
    public float attackPoint;

    public static string targetTag = "Player";
    public float coolDown;
    public MonsterState currState;
    private bool nearPlayer;

	// Use this for initialization
	void Start () {
        target = GameObject.FindGameObjectWithTag(targetTag).transform;
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    void SwitchState()
    {

    }
}
