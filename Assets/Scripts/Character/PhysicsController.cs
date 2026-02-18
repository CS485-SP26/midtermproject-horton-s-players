using UnityEngine;

// TODO: Consider the benefits of refactoring to namespace Movement
namespace Character
{
    public class PhysicsMovement : MovementController
    {
        [SerializeField] float drag = 0.5f;
        [SerializeField] float rotationSpeed = 0.1f;
        [SerializeField] float jumpForce = 5f;
        int remainingJumps = 1;
        bool isGrounded = true;

        
        protected override void Start()
        {
            base.Start();
            rb.linearDamping = drag;
        }

        public override float GetHorizontalSpeedPercent()
        {
            Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            return Mathf.Clamp01(horizontalVelocity.magnitude / maxVelocity);
        }

        public override void Jump() 
        { 
            // TODO: integrate jump support from week 2-3 
            if(remainingJumps!= 0 && isGrounded){
                ApplyJump();   
                remainingJumps--;
            }
        }

        protected override void FixedUpdate()
        {
            ApplyMovement();
            ClampVelocity();
            ApplyRotation();
        }
        
        void ApplyMovement()
        {
            // TODO integrate your physics from week 2-3 
            Vector3 movement = new Vector3(moveInput.x, 0f, moveInput.y);
            movement *= Time.deltaTime * acceleration;
            rb.AddForce(movement, ForceMode.Force);
        }

        void ApplyJump()
        {
            // TODO integrate your jump logic from week 2-3 
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        // TODO integrate collision support from week 2-3 
        void OnCollisionEnter(Collision collision)
         {
            isGrounded = true;
            remainingJumps = 1;
         }
        
        void OnCollisionExit(Collision collision)
        {
            isGrounded = false;
        } 
        
        void ClampVelocity()
        {
            // Clamp horizontal velocity while preserving vertical (for jumping/falling)
            Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            
            if (horizontalVelocity.magnitude > maxVelocity)
            {
                horizontalVelocity = horizontalVelocity.normalized * maxVelocity;
                rb.linearVelocity = new Vector3(horizontalVelocity.x, rb.linearVelocity.y, horizontalVelocity.z);
            }
        }

        void ApplyRotation()
        {
            Vector3 direction = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            if (direction.magnitude > 0.5f)
            {
                // 1. Calculate the target rotation (where we WANT to look)
                Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);

                // 2. Smoothly rotate from our current rotation toward the target
                transform.rotation = Quaternion.Slerp(
                    transform.rotation, 
                    targetRotation, 
                    rotationSpeed * Time.fixedDeltaTime
                );
            }
        }
    }
}
