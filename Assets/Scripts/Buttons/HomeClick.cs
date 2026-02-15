using UnityEngine;

public class HomeClick : MonoBehaviour
{
    [SerializeField] private long AddMoneyPerClick;
    public void HomeClickActivated()
    {
        SaveSingleton.Instance.Save.PlayerMoney += AddMoneyPerClick;
    }
}
