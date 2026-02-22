using UnityEngine;
using TMPro;

public class PlayerSavingsTextSet : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI SavingsDisplay;
    public long MoneyValue;

    void Update()
    {
        SavingsDisplay.text = string.Format("${0:n0}", SaveSingleton.Instance.Save.PlayerMoney);
    }
}
