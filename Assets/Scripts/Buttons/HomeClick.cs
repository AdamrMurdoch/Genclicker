using UnityEngine;
using TMPro;

public class HomeClick : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI PerClickAmountText;
    private long TotalClickAmount;

    public void HomeClickActivated()
    {
        SaveSingleton.Instance.Save.PlayerMoney += TotalClickAmount;
    }

    void Start()
    {
        TotalClickAmount = 5000;

        if(SaveSingleton.Instance.Save.EastDrivePropertyPurchased == true)
        {
            TotalClickAmount += 43000;
        }
        if(SaveSingleton.Instance.Save.BlessfrontAvenuePropertyPurchased == true)
        {
            TotalClickAmount += 200000;
        }
        if(SaveSingleton.Instance.Save.DartbackAvenuePropertyPurchased == true)
        {
            TotalClickAmount += 7000;
        }
        if(SaveSingleton.Instance.Save.SmallPlanePurchased == true)
        {
            TotalClickAmount += 15000;
        }
        if(SaveSingleton.Instance.Save.PrivateJetPurchased == true)
        {
            TotalClickAmount += 132000;
        }
        if(SaveSingleton.Instance.Save.CommercialPlanePurchased == true)
        {
            TotalClickAmount += 210000;
        }
        if(SaveSingleton.Instance.Save.HatchbackPurchased == true)
        {
            TotalClickAmount += 5000;
        }
        if(SaveSingleton.Instance.Save.SedanPurchased == true)
        {
            TotalClickAmount += 10000;
        }
        if(SaveSingleton.Instance.Save.SUVPurchased == true)
        {
            TotalClickAmount += 14000;
        }
        if(SaveSingleton.Instance.Save.SportsCarPurchased == true)
        {
            TotalClickAmount += 23000;
        }
        if(SaveSingleton.Instance.Save.SuperCarPurchased == true)
        {
            TotalClickAmount += 51000;
        }
        if(SaveSingleton.Instance.Save.HyperCarPurchased == true)
        {
            TotalClickAmount += 80000;
        }
        if(SaveSingleton.Instance.Save.CasinoBusinessPurchased == true)
        {
            TotalClickAmount += 15000;
        }
        if(SaveSingleton.Instance.Save.CarDealershipBusinessPurchased == true)
        {
            TotalClickAmount += 30000;
        }
        if(SaveSingleton.Instance.Save.RestrauntBusinessPurchased == true)
        {
            TotalClickAmount += 50000;
        }

        PerClickAmountText.text = string.Format("+${0:n0}", TotalClickAmount);
    }
}
