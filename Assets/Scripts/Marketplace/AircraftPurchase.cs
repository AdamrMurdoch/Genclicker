using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AircraftPurchase : MonoBehaviour
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

    [SerializeField] private int SmallPlanePrice = 130000;
    [SerializeField] private int PrivateJetPrice = 1690000;
    [SerializeField] private int CommercialPlanePrice = 8100000;

    private int PurchasePrice;
    private string PopupExtraDescription;
    private string PopupExtra2Description;

    private void Start()
    {
        blurCanvasGroup = PopupBlur.GetComponent<CanvasGroup>();
        popupBoxRect = PopupBox.GetComponent<RectTransform>();
        SetPopupVisibility(false);
    }

    public void PurchaseSmallPlane()
    {
        PurchaseAircraft("Small Plane");
    }

    public void PurchasePrivateJet()
    {
        PurchaseAircraft("Private Jet");
    }

    public void PurchaseCommercialPlane()
    {
        PurchaseAircraft("Commercial Plane");
    }

    private void PurchaseAircraft(string ItemID)
    {
        switch(ItemID)
        {
            case "Small Plane":
                {
                    int price = SmallPlanePrice;
                    int perClickPrice = 15000;
                    if(SaveSingleton.Instance.Save.PlayerMoney >= price)
                    {
                        SaveSingleton.Instance.Save.PlayerMoney -= price;
                        Header.text = "Small Plane Purchased!";
                        PopupExtraDescription = $"You successfully purchased a {ItemID} for ";
                        PopupExtra2Description = $"This asset will now provide you with {string.Format("${0:n0}", perClickPrice)} per click.";
                    }
                    else
                    {
                        Header.text = "Insufficient Savings";
                        PopupExtraDescription = $"You attempted to purchase a {ItemID} for ";
                        PopupExtra2Description = $"Please try again once you have sufficient savings for this purchase.";
                    }
                    break;
                }
            case "Private Jet":
                {
                    int price = PrivateJetPrice;
                    int perClickPrice = 132000;
                    if(SaveSingleton.Instance.Save.PlayerMoney >= price)
                    {
                        SaveSingleton.Instance.Save.PlayerMoney -= price;
                        Header.text = "Private Jet Purchased!";
                        PopupExtraDescription = $"You successfully purchased a {ItemID} for ";
                        PopupExtra2Description = $"This asset will now provide you with {string.Format("${0:n0}", perClickPrice)} per click.";
                    }
                    else
                    {
                        Header.text = "Insufficient Savings";
                        PopupExtraDescription = $"You attempted to purchase a {ItemID} for ";
                        PopupExtra2Description = $"Please try again once you have sufficient savings for this purchase.";
                    }
                    break;
                }
            case "Commercial Plane":
                {
                    int price = CommercialPlanePrice;
                    int perClickPrice = 210000;
                    if(SaveSingleton.Instance.Save.PlayerMoney >= price)
                    {
                        SaveSingleton.Instance.Save.PlayerMoney -= price;
                        Header.text = "Commercial Plane Purchased!";
                        PopupExtraDescription = $"You successfully purchased a {ItemID} for ";
                        PopupExtra2Description = $"This asset will now provide you with {string.Format("${0:n0}", perClickPrice)} per click.";
                    }
                    else
                    {
                        Header.text = "Insufficient Savings";
                        PopupExtraDescription = $"You attempted to purchase a {ItemID} for ";
                        PopupExtra2Description = $"Please try again once you have sufficient savings for this purchase.";
                    }
                    break;
                }
        }
        SetupPopup();
    }

    private void SetupPopup()
    {
        BodyText.text = $"{PopupExtraDescription} {string.Format("${0:n0}", PurchasePrice)}.\n\n{PopupExtra2Description}";
        Button1.SetActive(true);
        Button2.SetActive(false);
        Button3.SetActive(false);
        ButtonText1.text = "Continue";
        ButtonText2.text = "";
        ButtonText3.text = "";

        ConfigurePopup();
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
