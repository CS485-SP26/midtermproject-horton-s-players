using UnityEngine;
using TMPro;

public class FundsUI : MonoBehaviour
{
    [SerializeField] private TMP_Text fundsText;

    public static FundsUI Instance {get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void UpdateFundsDisplay()
    {
        if (fundsText != null)
        {
            fundsText.text = "Funds: $" + GameManager.Instance.getFunds();
        }
    }
    public void Start()
    {
        UpdateFundsDisplay();
    }
}
