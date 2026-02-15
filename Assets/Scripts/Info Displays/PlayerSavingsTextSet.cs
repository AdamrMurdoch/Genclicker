using UnityEngine;
using TMPro;

public class PlayerSavingsTextSet : MonoBehaviour
{
    public TextMeshProUGUI SavingsDisplay;
    public long MoneyValue;

    void Update()
    {
        SavingsDisplay.text = string.Format("${0:n0}", MoneyValue);
    }
}
