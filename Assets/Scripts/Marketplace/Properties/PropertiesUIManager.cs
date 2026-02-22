using UnityEngine;
using TMPro;

public class PropertiesUIManager : MonoBehaviour
{
    [SerializeField] private string ItemName;
    [SerializeField] private TextMeshProUGUI ItemStatusText;

    void Update()
    {
        switch(ItemName.ToLower())
        {
            case "east drive":
            {
                if(SaveSingleton.Instance.Save.EastDrivePropertyPurchased == true)
                {
                    ItemStatusText.text = "Owned";
                }
                else
                {
                    ItemStatusText.text = "$320,000";
                }
                break;
            }
            case "blessfront avenue":
            {
                if(SaveSingleton.Instance.Save.BlessfrontAvenuePropertyPurchased == true)
                {
                    ItemStatusText.text = "Owned";
                }
                else
                {
                    ItemStatusText.text = "$1,000,000";
                }
                break;
            }
            case "dartback avenue":
            {
                if(SaveSingleton.Instance.Save.DartbackAvenuePropertyPurchased == true)
                {
                    ItemStatusText.text = "Owned";
                }
                else
                {
                    ItemStatusText.text = "$83,000";
                }
                break;
            }
        }
    }
}
