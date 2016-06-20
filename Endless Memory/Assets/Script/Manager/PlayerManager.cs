using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CnControls;

namespace MemoryTrap
{
    public class PlayerManager : MonoBehaviour
    {

        public Animator animator;

        public enum PlayerState
        {
            Idle, Walk, Attack, AttackOver
        }

        public float velocity;

        private Vector3 targetPoint;
        private PlayerState currState;
        private Transform mainCameraTransform;
        private Rigidbody _rigidbody;
        public Camera mainCamera;

        private int isWalking = 0;
        private bool isAttacking = false;
        private int attackCount = 0;
        private int attackFrame = 23;
        private int hp = 45;
        public List<GameObject> enemyList = new List<GameObject>();

        // Use this for initialization
        void Start()
        {
            animator = GetComponent<Animator>();
            mainCameraTransform = mainCamera.GetComponent<Transform>();
            _rigidbody = GetComponent<Rigidbody>();
            velocity = 2f;
            currState = PlayerState.Idle;

            _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }


        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(KeyCode.Mouse1))
            {
                animator.Play("Attack", -1, 0f);
                isAttacking = true;
            }
            if (isAttacking)
            {
                if (attackCount == 22)
                {
                    currState = PlayerState.AttackOver;
                    attackCount++;
                }
                else if (attackCount < attackFrame)
                    attackCount++;
                else
                {
                    attackCount = 0;
                    currState = PlayerState.Idle;
                    isAttacking = false;
                }
            }

            _rigidbody.velocity = transform.forward * velocity * isWalking;
        }

        void FixedUpdate()
        {
            if (!isAttacking)
            {
                Vector3 inputVector = new Vector3(CnInputManager.GetAxis("Horizontal"), 0, CnInputManager.GetAxis("Vertical"));
                Vector3 movementVector = Vector3.zero;
                if (inputVector.sqrMagnitude > 0.001f)
                {
                    if (currState == PlayerState.Idle)
                    {
                        animator.SetBool("isWalking", true);
                        animator.Play("Walk", -1, 0f);
                        isWalking = 1;
                    }
                    currState = PlayerState.Walk;
                    movementVector = inputVector;
                    transform.LookAt(transform.position + movementVector);
                }
                else
                {
                    animator.SetBool("isWalking", false);
                    currState = PlayerState.Idle;
                    isWalking = 0;
                }
            }

           // _rigidbody.velocity = transform.forward * velocity * isWalking;
        }

        void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Monster"))
                enemyList.Add(other.gameObject);
        }
        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Monster"))
                enemyList.Remove(other.gameObject);
        }
        void HandleAttack()
        {
            if(currState == PlayerState.AttackOver)
            {
                foreach(GameObject enemy in enemyList)
                {

                }
            }
        }
        public void DecreaseHp(int hit)
        {
            if (hp - hit <= 0)
                hp = 0;
            else
                hp -= hit;
        }
        public void playAttack()
        {
            isAttacking = true;
            animator.SetBool("isWalking", false);
            animator.Play("Attack");
        }
        
    }

}