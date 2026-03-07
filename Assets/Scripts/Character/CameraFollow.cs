using UnityEngine;
using UnityEngine.InputSystem;

namespace Character 
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private Vector3 targetOffset = new(0f, 1.5f, 0f);
        [SerializeField, Min(0.5f)] private float followDistance = 5f;
        [SerializeField, Min(0f)] private float positionSmoothTime = 0.08f;
        [SerializeField] private float lookSensitivityX = 140f;
        [SerializeField] private float lookSensitivityY = 90f;
        [SerializeField] private float minPitch = -20f;
        [SerializeField] private float maxPitch = 65f;

        [Header("Cursor Steering")]
        [SerializeField] private bool steerWithCursorPosition = true;
        [SerializeField, Range(0f, 0.9f)] private float cursorDeadZone = 0.1f;

        private Vector2 lookInput;
        private float yaw;
        private float pitch = 20f;
        private Vector3 smoothVelocity;

        public Transform FollowTarget => player;

        void Start()
        {
            if (!player)
            {
                PlayerController playerController = FindAnyObjectByType<PlayerController>();
                if (playerController)
                {
                    player = playerController.transform;
                }
            }

            Debug.Assert(player, "CameraFollow requires a player target.");
            if (player)
            {
                yaw = player.eulerAngles.y;
            }
        }

        void LateUpdate()
        {
            if (!player)
            {
                return;
            }

            Vector2 lookDirection = lookInput;
            if (steerWithCursorPosition)
            {
                lookDirection += GetCursorLookDirection();
            }

            lookDirection = Vector2.ClampMagnitude(lookDirection, 1f);
            lookDirection.y = 0f;

            if  (lookDirection.x == 0f)
            {
                yaw = player.eulerAngles.y;
            }

            yaw += lookDirection.x * lookSensitivityX * Time.deltaTime;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

            Vector3 pivot = player.TransformPoint(targetOffset);
            Quaternion lookRotation = Quaternion.Euler(pitch, yaw, 0f);
            Vector3 desiredPosition = pivot - (lookRotation * Vector3.forward * followDistance);

            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref smoothVelocity, positionSmoothTime);
            transform.rotation = lookRotation;
        }

        public void SetPlayer(Transform target)
        {
            player = target;
            if (player)
            {
                yaw = player.eulerAngles.y;
            }
        }

        public void SetLookInput(Vector2 value)
        {
            lookInput = value;
        }

        private Vector2 GetCursorLookDirection()
        {
            if (Mouse.current == null)
            {
                return Vector2.zero;
            }

            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Vector2 halfScreen = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
            if (halfScreen.x <= 0f || halfScreen.y <= 0f)
            {
                return Vector2.zero;
            }

            Vector2 centered = mousePosition - halfScreen;
            Vector2 normalized = new Vector2(centered.x / halfScreen.x, centered.y / halfScreen.y);
            normalized = Vector2.ClampMagnitude(normalized, 1f);

            if (normalized.magnitude < cursorDeadZone)
            {
                return Vector2.zero;
            }

            return normalized;
        }
    }
}
