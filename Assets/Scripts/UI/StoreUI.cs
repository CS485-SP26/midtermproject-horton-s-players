using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StoreUI : MonoBehaviour
{
    [SerializeField] private Button purchaseSeedsButton;
    [SerializeField] private TMP_Text seedsText;
    [SerializeField] private int seedPrice = 10;
    void Start()
    {
        // when the Purchase Seeds button is clicked, trigger the PurchaseSeeds() function
        purchaseSeedsButton.onClick.AddListener(PurchaseSeeds); 
        // Update displays
        UpdateSeedsDisplay();
        UpdateSeedsButton();
    }
    private void PurchaseSeeds() 
    {
        // first check if the player has enough funds to purchase seeds
        if (GameManager.Instance.getFunds() >= seedPrice)
        {
            GameManager.Instance.AddFunds(-seedPrice); // subtract seedprice because the player bought a seed
            GameManager.Instance.AddSeeds(1); // add a seed

            // Update displays
            UpdateSeedsDisplay();
            UpdateSeedsButton();
            FundsUI.Instance.UpdateFundsDisplay();
        }
    }
    private void UpdateSeedsDisplay()
    {
        seedsText.text = "Seeds: " + GameManager.Instance.getSeeds();
    }
    private void UpdateSeedsButton()
    {
        // grey the button out if the player does not have enough funds to purchase seeds
        purchaseSeedsButton.interactable = GameManager.Instance.getFunds() >= seedPrice;
    }
    
}
