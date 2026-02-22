using UnityEngine;
using TMPro;

public class AircraftUIManager : MonoBehaviour
{
    [SerializeField] private string ItemName;
    [SerializeField] private TextMeshProUGUI ItemStatusText;

    void Update()
    {
        switch(ItemName.ToLower())
        {
            case "small plane":
            {
                if(SaveSingleton.Instance.Save.SmallPlanePurchased == true)
                {
                    ItemStatusText.text = "Owned";
                }
                else
                {
                    ItemStatusText.text = "$130,000";
                }
                break;
            }
            case "private jet":
            {
                if(SaveSingleton.Instance.Save.PrivateJetPurchased == true)
                {
                    ItemStatusText.text = "Owned";
                }
                else
                {
                    ItemStatusText.text = "$1,690,000";
                }
                break;
            }
            case "commercial plane":
            {
                if(SaveSingleton.Instance.Save.CommercialPlanePurchased == true)
                {
                    ItemStatusText.text = "Owned";
                }
                else
                {
                    ItemStatusText.text = "$8,100,000";
                }
                break;
            }
        }
    }
}
