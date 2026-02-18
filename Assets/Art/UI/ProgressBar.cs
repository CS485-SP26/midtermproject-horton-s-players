using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private TextMeshProUGUI fillText;
    public float Fill { set { fillImage.fillAmount = value; } } 
    public void SetText(string text)
    {
        fillText.text = text;
    }

    
}
