using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoreUI : MonoBehaviour
{
    [SerializeField] private Button purchaseSeedsButton;
    [SerializeField] private Button sellTomatoesButton;
    [SerializeField] private TMP_Text seedsText;
    [SerializeField] private TMP_Text tomatoesText;
    [SerializeField] private int seedPrice = 10;
    [SerializeField] private int tomatoPrice = 100;
    void Start()
    {
        // when the Purchase Seeds button is clicked, trigger the PurchaseSeeds() function
        purchaseSeedsButton.onClick.AddListener(PurchaseSeeds); 
        sellTomatoesButton.onClick.AddListener(SellTomatoes);
        
        // Update displays
        UpdateStoreDisplay();
        UpdateStoreButtons();
    }
    private void PurchaseSeeds() 
    {
        // first check if the player has enough funds to purchase seeds
        if (GameManager.Instance.getFunds() >= seedPrice)
        {
            GameManager.Instance.AddFunds(-seedPrice); // subtract seedprice because the player bought a seed
            GameManager.Instance.AddSeeds(1); // add a seed

            // Update displays
            UpdateStoreDisplay();
            UpdateStoreButtons();
            FundsUI.Instance.UpdateFundsDisplay();
        }
    }
    private void UpdateStoreDisplay()
    {
        seedsText.text = "Seeds: " + GameManager.Instance.getSeeds();
        tomatoesText.text = "Tomatoes: " + GameManager.Instance.getTomatoes();

    }
    private void UpdateStoreButtons()
    {
        // grey the button out if the player does not have enough funds to purchase seeds
        purchaseSeedsButton.interactable = GameManager.Instance.getFunds() >= seedPrice;
        sellTomatoesButton.interactable = GameManager.Instance.getTomatoes() > 0;
    }
    private void SellTomatoes()
    {
        if (GameManager.Instance.getTomatoes() > 0)
        {
            GameManager.Instance.AddFunds(tomatoPrice);
            GameManager.Instance.AddTomatoes(-1);
        }
        UpdateStoreDisplay();
        UpdateStoreButtons();
        FundsUI.Instance.UpdateFundsDisplay();
    }
    
}
