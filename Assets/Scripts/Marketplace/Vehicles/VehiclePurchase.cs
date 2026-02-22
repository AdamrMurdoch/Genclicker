using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class VehiclePurchase : MonoBehaviour
{
    [SerializeField] private ContentSizeFitter ParentFitter;
    [SerializeField] private SceneTransitionManager scenetransitionmanager;
    [SerializeField] private GameObject PopupBlur, PopupBox;
    [SerializeField] private GameObject Button1, Button2, Button3;
    [SerializeField] private TextMeshProUGUI Header, BodyText, ButtonText1, ButtonText2, ButtonText3;
    
    private CanvasGroup blurCanvasGroup;
    private RectTransform popupBoxRect;
    [SerializeField] private PopupAnimation popupAnimation;
    [SerializeField] private CanvasGroup popupCanvasGroup;

    [SerializeField] private int HatchbackPrice = 50000;
    [SerializeField] private int SedanPrice = 85000;
    [SerializeField] private int SUVPrice = 110000;
    [SerializeField] private int SportsCarPrice = 230000;
    [SerializeField] private int SuperCarPrice = 460000;
    [SerializeField] private int HyperCarPrice = 1810000;

    private int PurchasePrice;
    private string PopupExtraDescription;
    private string PopupExtra2Description;
    private int Price;
    private string ItemName;
    private int PerClickAmount;
    private bool PurchaseSuccess;

    private void Start()
    {
        blurCanvasGroup = PopupBlur.GetComponent<CanvasGroup>();
        popupBoxRect = PopupBox.GetComponent<RectTransform>();
        SetPopupVisibility(false);
    }

    public void PurchaseHatchback()
    {
        Price = HatchbackPrice;
        ItemName = "Hatchback";
        PerClickAmount = 5000;
        SetupPopup();
    }

    public void PurchaseSedan()
    {
        Price = SedanPrice;
        ItemName = "Sedan";
        PerClickAmount = 10000;
        SetupPopup();
    }

    public void PurchaseSUV()
    {
        Price = SUVPrice;
        ItemName = "SUV";
        PerClickAmount = 14000;
        SetupPopup();
    }

    public void PurchaseSportsCar()
    {
        Price = SportsCarPrice;
        ItemName = "Sports Car";
        PerClickAmount = 23000;
        SetupPopup();
    }

    public void PurchaseSuperCar()
    {
        Price = SuperCarPrice;
        ItemName = "Super Car";
        PerClickAmount = 51000;
        SetupPopup();
    }

    public void PurchaseHyperCar()
    {
        Price = HyperCarPrice;
        ItemName = "Hyper Car";
        PerClickAmount = 80000;
        SetupPopup();
    }

    private void SetupPopup()
    {
        if(SaveSingleton.Instance.Save.PlayerMoney >= Price)
        {
            PurchaseSuccess = true;
            SaveSingleton.Instance.Save.PlayerMoney -= Price;
            Header.text = $"{ItemName} Purchased!";
            PopupExtraDescription = $"You successfully purchased a {ItemName} for ";
            PopupExtra2Description = $"This asset will now provide you with {string.Format("${0:n0}", PerClickAmount)} per click.";
        }
        else
        {
            PurchaseSuccess = false;
            Header.text = "Insufficient Savings";
            PopupExtraDescription = $"You attempted to purchase a {ItemName} for ";
            PopupExtra2Description = $"Please try again once you have sufficient savings for this purchase.";
        }

        BodyText.text = $"{PopupExtraDescription} {string.Format("${0:n0}", PurchasePrice)}.\n\n{PopupExtra2Description}";
        Button1.SetActive(true);
        Button2.SetActive(false);
        Button3.SetActive(false);
        ButtonText1.text = "Continue";
        ButtonText2.text = "";
        ButtonText3.text = "";

        ConfigurePopup();

        if(PurchaseSuccess == true)
        {
            OwnItem();
        }
    }

    private void OwnItem()
    {
        switch(ItemName)
        {
            case "Hatchback":
            {
                SaveSingleton.Instance.Save.HatchbackPurchased = true;
                break;
            }
            case "Sedan":
            {
                SaveSingleton.Instance.Save.SedanPurchased = true;
                break;
            }
            case "SUV":
            {
                SaveSingleton.Instance.Save.SUVPurchased = true;
                break;
            }
            case "Sports Car":
            {
                SaveSingleton.Instance.Save.SportsCarPurchased = true;
                break;
            }
            case "Super Car":
            {
                SaveSingleton.Instance.Save.SuperCarPurchased = true;
                break;
            }
            case "Hyper Car":
            {
                SaveSingleton.Instance.Save.HyperCarPurchased = true;
                break;
            }
        }
    }

    private void ConfigurePopup()
    {
        // Unity Content Size Fitters are annoying af. This forces them to update and adjust to dynamic content within the popup. One of course isn't enough for reliable results.
        LayoutRebuilder.ForceRebuildLayoutImmediate(ParentFitter.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(ParentFitter.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(ParentFitter.GetComponent<RectTransform>());

        StartCoroutine(ShowPopupWithFade());
    }

    private IEnumerator ShowPopupWithFade()
    {
        yield return popupAnimation.AnimatePopup(blurCanvasGroup, popupCanvasGroup, 0, 1, 0, 1);
    }

    private IEnumerator HidePopupWithFadeAndShowOutcome()
    {
        yield return popupAnimation.AnimatePopup(blurCanvasGroup, popupCanvasGroup, 1, 0, 1, 0);
    }

    public void OnButtonPress(int buttonIndex)
    {
        StartCoroutine(HidePopupWithFadeAndShowOutcome());
    }

    private void SetPopupVisibility(bool visible)
    {
        float alpha = visible ? 1 : 0;
        blurCanvasGroup.alpha = alpha;
        blurCanvasGroup.blocksRaycasts = visible;
        popupCanvasGroup.alpha = alpha;
        popupCanvasGroup.blocksRaycasts = visible;
        popupBoxRect.anchoredPosition = new Vector2(0, 0);
    }
}
