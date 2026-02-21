using Character;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Environment
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Collider))]
    public class StoreSceneTrigger : MonoBehaviour
    {
        [SerializeField] private string storeSceneName = "Store";
        [SerializeField] private float triggerDelaySeconds = 2f;
        [SerializeField] private bool oneTimeTrigger = true;
        [SerializeField] private bool autoHandlePlayerReturnPosition = true;
        [SerializeField] private bool savePlayerPositionBeforeLoad = true;
        [SerializeField] private bool restoreSavedPlayerPositionOnLoad = false;

        private bool hasTriggered = false;
        private float playerStayTimer = 0f;
        private PlayerController trackedPlayer = null;

        void Reset()
        {
            Collider sceneTrigger = GetComponent<Collider>();
            if (sceneTrigger != null)
            {
                sceneTrigger.isTrigger = true;
            }
        }

        void Awake()
        {
            Collider sceneTrigger = GetComponent<Collider>();
            if (sceneTrigger != null && !sceneTrigger.isTrigger)
            {
                sceneTrigger.isTrigger = true;
                Debug.LogWarning("StoreSceneTrigger requires trigger mode. Collider was automatically set to Is Trigger.");
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (oneTimeTrigger && hasTriggered)
            {
                return;
            }

            PlayerController player = GetPlayerController(other);
            if (player == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(storeSceneName))
            {
                Debug.LogError("StoreSceneTrigger has an empty scene name.");
                return;
            }

            trackedPlayer = player;
            playerStayTimer = 0f;
        }

        void OnTriggerStay(Collider other)
        {
            if (oneTimeTrigger && hasTriggered)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(storeSceneName))
            {
                return;
            }

            PlayerController player = GetPlayerController(other);
            if (player == null)
            {
                return;
            }

            if (trackedPlayer != player)
            {
                trackedPlayer = player;
                playerStayTimer = 0f;
            }

            playerStayTimer += Time.deltaTime;
            if (playerStayTimer < triggerDelaySeconds)
            {
                return;
            }

            LoadStoreScene(player.transform);
        }

        void OnTriggerExit(Collider other)
        {
            PlayerController player = GetPlayerController(other);
            if (player == null || player != trackedPlayer)
            {
                return;
            }

            trackedPlayer = null;
            playerStayTimer = 0f;
        }

        void LoadStoreScene(Transform playerTransform)
        {
            if (oneTimeTrigger && hasTriggered)
            {
                return;
            }

            bool shouldSavePlayerPosition = savePlayerPositionBeforeLoad;
            bool shouldRestorePlayerPosition = restoreSavedPlayerPositionOnLoad;

            if (autoHandlePlayerReturnPosition)
            {
                string activeSceneName = SceneManager.GetActiveScene().name;
                bool isEnteringTargetScene = activeSceneName != storeSceneName;
                shouldSavePlayerPosition = isEnteringTargetScene;
                shouldRestorePlayerPosition = !isEnteringTargetScene;
            }

            if (shouldSavePlayerPosition)
            {
                if (playerTransform != null)
                {
                    GameManager.Instance.SavePlayerPosition(playerTransform.position);
                }
            }

            if (shouldRestorePlayerPosition)
            {
                GameManager.Instance.RestoreSavedPlayerPositionOnNextSceneLoad();
            }

            hasTriggered = true;
            trackedPlayer = null;
            playerStayTimer = 0f;
            GameManager.Instance.LoadScenebyName(storeSceneName);
        }

        bool IsPlayer(Collider other)
        {
            return other.GetComponent<PlayerController>() != null
                || other.GetComponentInParent<PlayerController>() != null;
        }

        PlayerController GetPlayerController(Collider other)
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                return player;
            }

            return other.GetComponentInParent<PlayerController>();
        }

        Transform GetPlayerTransform(Collider other)
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                return player.transform;
            }

            player = other.GetComponentInParent<PlayerController>();
            return player != null ? player.transform : null;
        }
    }
}
