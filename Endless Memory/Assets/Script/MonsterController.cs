using UnityEngine;
using System.Collections;

namespace MemoryTrap
{
    public class MonsterController : MonoBehaviour
    {
        public GameObject monster;
        public int hp = 100;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        /*
        void OnCollisionEnter(Collision collision)
        {
            if(collision.gameObject.CompareTag("PlayerAttack"))
            {
                monster.GetComponent<Animation>().Play("Damage");
                DecreaseHp(collision.gameObject.GetComponent<AttackTrigger>().damage);
            }
        }
        */
        void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("PlayerAttack"))
            {
                monster.GetComponent<Animator>().Play("Hit");
                DecreaseHp(other.gameObject.GetComponent<AttackTrigger>().damage);
            }
        }

        void DecreaseHp(int hitPoint)
        {
            if (hp - hitPoint <= 0)
                hp = 0;
            else
                hp -= hitPoint;
        }
    }
}