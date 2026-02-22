using TMPro;
using UnityEngine;

public class VehiclesUIManager : MonoBehaviour
{
    [SerializeField] private string ItemName;
    [SerializeField] private TextMeshProUGUI ItemStatusText;

    void Update()
    {
        switch(ItemName.ToLower())
        {
            case "hatchback":
            {
                if(SaveSingleton.Instance.Save.HatchbackPurchased == true)
                {
                    ItemStatusText.text = "Owned";
                }
                else
                {
                    ItemStatusText.text = "$50,000";
                }
                break;
            }
            case "sedan":
            {
                if(SaveSingleton.Instance.Save.SedanPurchased == true)
                {
                    ItemStatusText.text = "Owned";
                }
                else
                {
                    ItemStatusText.text = "$85,000";
                }
                break;
            }
            case "suv":
            {
                if(SaveSingleton.Instance.Save.SUVPurchased == true)
                {
                    ItemStatusText.text = "Owned";
                }
                else
                {
                    ItemStatusText.text = "$110,000";
                }
                break;
            }
            case "sports car":
            {
                if(SaveSingleton.Instance.Save.SportsCarPurchased == true)
                {
                    ItemStatusText.text = "Owned";
                }
                else
                {
                    ItemStatusText.text = "$230,000";
                }
                break;
            }
            case "super car":
            {
                if(SaveSingleton.Instance.Save.SuperCarPurchased == true)
                {
                    ItemStatusText.text = "Owned";
                }
                else
                {
                    ItemStatusText.text = "$460,000";
                }
                break;
            }
            case "hyper car":
            {
                if(SaveSingleton.Instance.Save.HyperCarPurchased == true)
                {
                    ItemStatusText.text = "Owned";
                }
                else
                {
                    ItemStatusText.text = "$1,810,000";
                }
                break;
            }
        }
    }
}
