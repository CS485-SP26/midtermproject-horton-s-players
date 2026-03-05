using UnityEngine;

namespace Character {
    [RequireComponent(typeof(Rigidbody))]
    public class MovementController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] protected float acceleration = 20f;
        [SerializeField] protected float maxVelocity = 5f;
        [SerializeField] protected Transform movementReference;
        protected Rigidbody rb;
        protected Vector2 moveInput;

        protected virtual void Awake()
        {
            if (!movementReference && Camera.main)
            {
                movementReference = Camera.main.transform;
            }
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        protected virtual void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        public void Move(Vector2 lateralInput)
        {
            moveInput = lateralInput;
        }

        public void Stop()
        {
            rb.linearVelocity = Vector3.zero;
            moveInput = Vector2.zero;
        }

        public virtual void Jump() { /* NO JUMP SUPPORT */ }

        public virtual float GetHorizontalSpeedPercent()
        {
            return moveInput == Vector2.zero ? 0f : 1f;
        }

        public void SetMovementReference(Transform reference)
        {
            movementReference = reference;
        }

        protected Vector3 GetPlanarMoveDirection()
        {
            if (!movementReference)
            {
                movementReference = Camera.main ? Camera.main.transform : null;
            }

            if (!movementReference)
            {
                return new Vector3(moveInput.x, 0f, moveInput.y).normalized;
            }

            Vector3 forward = movementReference.forward;
            Vector3 right = movementReference.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();

            Vector3 moveDirection = (right * moveInput.x) + (forward * moveInput.y);
            return moveDirection.sqrMagnitude > 1f ? moveDirection.normalized : moveDirection;
        }

        protected virtual void FixedUpdate()
        {
            SimpleMovement();
        }

        void SimpleMovement()
        {
            Vector3 movement = GetPlanarMoveDirection();
            movement *= Time.deltaTime * acceleration;
            rb.MovePosition(rb.position + movement);
        }
    }
}