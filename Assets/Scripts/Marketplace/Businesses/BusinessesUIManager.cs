using UnityEngine;
using TMPro;

public class BusinessesUIManager : MonoBehaviour
{
    [SerializeField] private string ItemName;
    [SerializeField] private TextMeshProUGUI ItemStatusText;

    void Update()
    {
        switch(ItemName.ToLower())
        {
            case "casino business":
            {
                if(SaveSingleton.Instance.Save.CasinoBusinessPurchased == true)
                {
                    ItemStatusText.text = "Owned";
                }
                else
                {
                    ItemStatusText.text = "$200,000";
                }
                break;
            }
            case "car dealership":
            {
                if(SaveSingleton.Instance.Save.CarDealershipBusinessPurchased == true)
                {
                    ItemStatusText.text = "Owned";
                }
                else
                {
                    ItemStatusText.text = "$500,000";
                }
                break;
            }
            case "restraunt business":
            {
                if(SaveSingleton.Instance.Save.RestrauntBusinessPurchased == true)
                {
                    ItemStatusText.text = "Owned";
                }
                else
                {
                    ItemStatusText.text = "$1,000,000";
                }
                break;
            }
        }
    }
}
