using UnityEngine;
using TMPro;
using Character;
using Environment;
namespace Farming
{

[RequireComponent(typeof(AnimatedController))]
    public class Farmer : MonoBehaviour
    {

        [SerializeField] private GameObject Hoe;
        [SerializeField] private GameObject waterCan;
        [SerializeField] private ProgressBar energyLevelUI;
        [SerializeField] private ProgressBar waterLevelUI; //eventually refactor this to a watering can'
        [SerializeField] private TMP_Text fundsText;
        [Header("Resource Values")]
        [SerializeField, Range(0f, 1f)] private float energyLevel = 1f;
        [SerializeField] private float waterLevel = 1f;
        [SerializeField, Min(0f)] private float energyPerDig = 0.12f;
        [SerializeField, Min(0f)] private float energyPerWaterUse = 0f;
        [SerializeField] private float waterPerUse = 0.1f;
        [Header("Resource Restoration")]
        [SerializeField, Min(0f)] private float idleRecoveryDelay = 2f;
        [SerializeField, Min(0f)] private float energyRecoveryPerSecond = 0.2f;
        [SerializeField, Min(0f)] private float waterRecoveryPerSecond = 0.1f;
        [SerializeField] private bool restoreResourcesAtStartOfDay = true;
        AnimatedController animatedController;
        private WinCondition winCondition;
        private DayController dayController;
        private float lastActionTime;
        private bool isMoving;

        void Start()
        {
            animatedController = GetComponent<AnimatedController>();
            winCondition = FindAnyObjectByType<WinCondition>();
            Debug.Assert(animatedController, "Farmer requires an animatedController");
            Debug.Assert(winCondition, "Farmer requires a wincondition");
            Debug.Assert(waterCan, "Farmer requires a waterCan");
            Debug.Assert(Hoe, "Farmer requires a Hoe");
            Debug.Assert(energyLevelUI, "Farmer requires an energy level");
            Debug.Assert(waterLevelUI, "Farmer requires a water level");
            Debug.Assert(fundsText, "Farmer requires a fundsText");
            dayController = FindAnyObjectByType<DayController>();

            SetTool("None");
            energyLevel = Mathf.Clamp01(energyLevel);
            waterLevel = Mathf.Clamp01(waterLevel);
            RefreshResourceUI();
            fundsText.text = "Funds: $" + GameManager.Instance.getFunds();
            lastActionTime = Time.time;
            
        }

        void OnEnable()
        {
            if (dayController)
            {
                dayController.dayPassedEvent.AddListener(OnDayPassed);
            }
        }

        void OnDisable()
        {
            if (dayController)
            {
                dayController.dayPassedEvent.RemoveListener(OnDayPassed);
            }
        }

        void Update()
        {
            TryRestoreResources(Time.deltaTime);
        }

        public void SetTool(string tool)
            {
                waterCan.SetActive(false);
                Hoe.SetActive(false);
                switch (tool)
                {
                    case "Hoe": Hoe.SetActive(true); Debug.Log("SetActive HOE"); break;
                    case "waterCan": waterCan.SetActive(true); Debug.Log("SetActive WATERCAN"); break;
                }
            }
        public void TryTileInteract(FarmTile tile)
            {
                if (tile==null) return;           
                switch (tile.GetCondition)
                {
                    case FarmTile.Condition.Grass: 
                        if (!TrySpendEnergy(energyPerDig)) return;
                        animatedController.SetTrigger("Till"); 
                        Debug.Log("SetTrigger TILL"); 
                        tile.Interact();
                        break;
                    case FarmTile.Condition.Tilled: 
                        if (waterLevel >= waterPerUse && TrySpendEnergy(energyPerWaterUse))
                        {
                            animatedController.SetTrigger("Water"); 
                            Debug.Log("SetTrigger WATER"); 
                            tile.Interact();
                            waterLevel -= waterPerUse;
                            TouchActionTime();
                            RefreshResourceUI();

                        }
                        break;
                    case FarmTile.Condition.Watered:
                        tile.Interact();
                        TouchActionTime();
                        break;
                    default: break;
                }
                winCondition.countWateredTiles();
                FundsUI.Instance?.UpdateFundsDisplay();
            }

        public void SetIsMoving(bool moving)
        {
            isMoving = moving;
            if (moving)
            {
                TouchActionTime();
            }
        }

        public void RefillWaterToFull()
        {
            waterLevel = 1f;
            RefreshResourceUI();
            TouchActionTime();
        }

        private bool TrySpendEnergy(float amount)
        {
            if (amount <= 0f)
            {
                TouchActionTime();
                return true;
            }

            if (energyLevel < amount)
            {
                return false;
            }

            energyLevel -= amount;
            energyLevel = Mathf.Clamp01(energyLevel);
            TouchActionTime();
            RefreshResourceUI();
            return true;
        }

        private void TryRestoreResources(float deltaTime)
        {
            if (isMoving) return;
            if (Time.time - lastActionTime < idleRecoveryDelay) return;

            bool changed = false;

            if (energyRecoveryPerSecond > 0f && energyLevel < 1f)
            {
                energyLevel = Mathf.Min(1f, energyLevel + energyRecoveryPerSecond * deltaTime);
                changed = true;
            }

            if (waterRecoveryPerSecond > 0f && waterLevel < 1f)
            {
                waterLevel = Mathf.Min(1f, waterLevel + waterRecoveryPerSecond * deltaTime);
                changed = true;
            }

            if (changed)
            {
                RefreshResourceUI();
            }
        }

        private void TouchActionTime()
        {
            lastActionTime = Time.time;
        }

        private void RefreshResourceUI()
        {
            if (energyLevelUI)
            {
                energyLevelUI.Fill = energyLevel;
                energyLevelUI.SetText("Player Energy");
            }

            if (waterLevelUI)
            {
                waterLevelUI.Fill = waterLevel;
                waterLevelUI.SetText("Water Level");
            }
        }

        private void OnDayPassed()
        {
            if (!restoreResourcesAtStartOfDay) return;

            energyLevel = 1f;
            waterLevel = 1f;
            RefreshResourceUI();
            TouchActionTime();
        }
    }
}