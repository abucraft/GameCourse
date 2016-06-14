using UnityEngine;
using System.Collections;
using CnControls;

namespace MemoryTrap
{
    public class PlayerManager : MonoBehaviour
    {

        public Animator animator;

        public enum PlayerState
        {
            Idle, Walk, Attack
        }

        public float velocity;

        private Vector3 targetPoint;
        private PlayerState currState;
        private Transform mainCameraTransform;
        private Rigidbody _rigidbody;
        public Camera mainCamera;

        private int isWalking = 0;

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
            }
        }

        void FixedUpdate()
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

            _rigidbody.velocity = transform.forward * velocity * isWalking;
        }
    }

}