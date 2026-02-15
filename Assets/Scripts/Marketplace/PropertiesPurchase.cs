using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PropertiesPurchase : MonoBehaviour
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

    [SerializeField] private int EastDrivePrice = 320000;
    [SerializeField] private int BlessfrontAvenuePrice = 10000000;
    [SerializeField] private int DartbackAvenuePrice = 83000;

    private int PurchasePrice;
    private string PopupExtraDescription;
    private string PopupExtra2Description;

    private void Start()
    {
        blurCanvasGroup = PopupBlur.GetComponent<CanvasGroup>();
        popupBoxRect = PopupBox.GetComponent<RectTransform>();
        SetPopupVisibility(false);
    }

    public void PurchaseProperty(string ItemID)
    {
        switch(ItemID)
        {
            case "31 East Drive":
                {
                    int price = EastDrivePrice;
                    int perClickPrice = 43000;
                    if(SaveSingleton.Instance.Save.PlayerMoney >= price)
                    {
                        SaveSingleton.Instance.Save.PlayerMoney -= price;
                        Header.text = $"{ItemID} Purchased!";
                        PopupExtraDescription = $"You successfully purchased {ItemID} for ";
                        PopupExtra2Description = $"This property will now provide you with {string.Format("${0:n0}", perClickPrice)} per click.";
                    }
                    else
                    {
                        Header.text = "Insufficient Savings";
                        PopupExtraDescription = $"You attempted to purchase {ItemID} for ";
                        PopupExtra2Description = $"Please try again once you have sufficient savings for this purchase.";
                    }
                    break;
                }
            case "536 Blessfront Avenue":
                {
                    int price = BlessfrontAvenuePrice;
                    int perClickPrice = 200000;
                    if(SaveSingleton.Instance.Save.PlayerMoney >= price)
                    {
                        SaveSingleton.Instance.Save.PlayerMoney -= price;
                        Header.text = $"{ItemID} Purchased!";
                        PopupExtraDescription = $"You successfully purchased {ItemID} for ";
                        PopupExtra2Description = $"This property will now provide you with {string.Format("${0:n0}", perClickPrice)} per click.";
                    }
                    else
                    {
                        Header.text = "Insufficient Savings";
                        PopupExtraDescription = $"You attempted to purchase {ItemID} for ";
                        PopupExtra2Description = $"Please try again once you have sufficient savings for this purchase.";
                    }
                    break;
                }
            case "202 Dartback Avenue":
                {
                    int price = DartbackAvenuePrice;
                    int perClickPrice = 7000;
                    if(SaveSingleton.Instance.Save.PlayerMoney >= price)
                    {
                        SaveSingleton.Instance.Save.PlayerMoney -= price;
                        Header.text = $"{ItemID} Purchased!";
                        PopupExtraDescription = $"You successfully purchased {ItemID} for ";
                        PopupExtra2Description = $"This property will now provide you with {string.Format("${0:n0}", perClickPrice)} per click.";
                    }
                    else
                    {
                        Header.text = "Insufficient Savings";
                        PopupExtraDescription = $"You attempted to purchase {ItemID} for ";
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
