using Character;
using UnityEngine;

namespace Environment
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SphereCollider))]
    [RequireComponent(typeof(LineRenderer))]
    public class StoreSceneTrigger : MonoBehaviour
    {
        [SerializeField] private string storeSceneName = "Store";
        [SerializeField] private float requiredStaySeconds = 2f;
        [SerializeField] private bool oneTimeTrigger = true;

        [Header("Circle Visual")]
        [SerializeField] private bool showWhiteCircle = true;
        [SerializeField] private int circleSegments = 64;
        [SerializeField] private float circleLineWidth = 0.06f;

        private bool hasTriggered = false;
        private bool playerInside = false;
        private float stayTimer = 0f;
        private SphereCollider triggerCollider;
        private LineRenderer circleRenderer;

        void Reset()
        {
            triggerCollider = GetComponent<SphereCollider>();
            if (triggerCollider != null)
            {
                triggerCollider.isTrigger = true;
            }

            circleRenderer = GetComponent<LineRenderer>();
            SetupCircleRenderer();
            DrawCircle();
        }

        void Awake()
        {
            triggerCollider = GetComponent<SphereCollider>();
            circleRenderer = GetComponent<LineRenderer>();

            if (triggerCollider != null && !triggerCollider.isTrigger)
            {
                triggerCollider.isTrigger = true;
                Debug.LogWarning("StoreSceneTrigger requires trigger mode. Collider was automatically set to Is Trigger.");
            }

            SetupCircleRenderer();
            DrawCircle();
        }

        void OnTriggerEnter(Collider other)
        {
            if (!IsPlayer(other))
            {
                return;
            }

            playerInside = true;
            stayTimer = 0f;
        }

        void OnTriggerStay(Collider other)
        {
            if (oneTimeTrigger && hasTriggered)
            {
                return;
            }

            if (!IsPlayer(other))
            {
                return;
            }

            if (!playerInside)
            {
                playerInside = true;
                stayTimer = 0f;
            }

            stayTimer += Time.deltaTime;
            if (stayTimer < requiredStaySeconds)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(storeSceneName))
            {
                Debug.LogError("StoreSceneTrigger has an empty scene name.");
                return;
            }

            hasTriggered = true;
            GameManager.Instance.LoadScenebyName(storeSceneName);
        }

        void OnTriggerExit(Collider other)
        {
            if (!IsPlayer(other))
            {
                return;
            }

            playerInside = false;
            stayTimer = 0f;
        }

        void OnValidate()
        {
            requiredStaySeconds = Mathf.Max(0f, requiredStaySeconds);
            circleSegments = Mathf.Max(8, circleSegments);
            circleLineWidth = Mathf.Max(0.005f, circleLineWidth);

            triggerCollider = GetComponent<SphereCollider>();
            circleRenderer = GetComponent<LineRenderer>();

            if (triggerCollider != null)
            {
                triggerCollider.isTrigger = true;
            }

            SetupCircleRenderer();
            DrawCircle();
        }

        void SetupCircleRenderer()
        {
            if (circleRenderer == null)
            {
                return;
            }

            circleRenderer.enabled = showWhiteCircle;
            circleRenderer.useWorldSpace = false;
            circleRenderer.loop = true;
            circleRenderer.positionCount = circleSegments;
            circleRenderer.widthMultiplier = circleLineWidth;
            circleRenderer.startColor = Color.white;
            circleRenderer.endColor = Color.white;

            if (circleRenderer.sharedMaterial == null)
            {
                Shader spriteShader = Shader.Find("Sprites/Default");
                if (spriteShader != null)
                {
                    circleRenderer.sharedMaterial = new Material(spriteShader);
                }
            }
        }

        void DrawCircle()
        {
            if (circleRenderer == null || triggerCollider == null)
            {
                return;
            }

            if (!showWhiteCircle)
            {
                circleRenderer.enabled = false;
                return;
            }

            circleRenderer.enabled = true;
            circleRenderer.positionCount = circleSegments;

            float radius = triggerCollider.radius;
            Vector3 offset = triggerCollider.center;

            for (int i = 0; i < circleSegments; i++)
            {
                float t = (float)i / circleSegments;
                float angle = t * Mathf.PI * 2f;
                float x = Mathf.Cos(angle) * radius;
                float z = Mathf.Sin(angle) * radius;
                circleRenderer.SetPosition(i, offset + new Vector3(x, 0.02f, z));
            }
        }

        bool IsPlayer(Collider other)
        {
            return other.GetComponent<PlayerController>() != null
                || other.GetComponentInParent<PlayerController>() != null;
        }
    }
}
