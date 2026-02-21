using UnityEngine;
using TMPro;
using Character;
namespace Farming
{

[RequireComponent(typeof(AnimatedController))]
    public class Farmer : MonoBehaviour
    {

        [SerializeField] private GameObject Hoe;
        [SerializeField] private GameObject waterCan;
        [SerializeField] private ProgressBar waterLevelUI; //eventually refactor this to a watering can'
        [SerializeField] private TMP_Text fundsText;
        [SerializeField] private float waterLevel = 1f;
        [SerializeField] private float waterPerUse = 0.1f;
        AnimatedController animatedController;
        private WinCondition winCondition;

        void Start()
        {
            animatedController = GetComponent<AnimatedController>();
            winCondition = FindAnyObjectByType<WinCondition>();
            Debug.Assert(animatedController, "Farmer requires an animatedController");
            Debug.Assert(winCondition, "Farmer requires a wincondition");
            Debug.Assert(waterCan, "Farmer requires a waterCan");
            Debug.Assert(Hoe, "Farmer requires a Hoe");
            Debug.Assert(waterLevelUI, "Farmer requires a water level");
            Debug.Assert(fundsText, "Farmer requires a fundsText");
            SetTool("None");
            waterLevelUI.Fill = waterLevel;
            waterLevelUI.SetText("Water Level");
            fundsText.text = "Funds: $" + GameManager.Instance.getFunds();
            
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
                    animatedController.SetTrigger("Till"); 
                        Debug.Log("SetTrigger TILL"); 
                        tile.Interact();
                        break;
                    case FarmTile.Condition.Tilled: 
                        if (waterLevel > waterPerUse)
                        {
                            animatedController.SetTrigger("Water"); 
                            Debug.Log("SetTrigger WATER"); 
                            tile.Interact();
                            waterLevel -= waterPerUse;
                            waterLevelUI.Fill = waterLevel;

                        }
                        break;
                    default: break;
                }
                winCondition.countWateredTiles();
                UpdateFundsDisplay();
            }
        void UpdateFundsDisplay()
        {
            fundsText.text = "Funds: $" + GameManager.Instance.getFunds();
        }
    }
}