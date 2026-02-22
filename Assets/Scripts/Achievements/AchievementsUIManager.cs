using UnityEngine;

public class AchievementsUIManager : MonoBehaviour
{
    [SerializeField] private int AchievementIndex;
    [SerializeField] private GameObject AchievementCompletionStatus;

    void Start()
    {

        if(SaveSingleton.Instance.Save.PlayerMoney >= 1000000)
        {
            SaveSingleton.Instance.Save.Achievement1Unlocked = true;
        }
        if(SaveSingleton.Instance.Save.PlayerMoney >= 10000000)
        {
            SaveSingleton.Instance.Save.Achievement2Unlocked = true;
        }
        if(SaveSingleton.Instance.Save.PlayerMoney >= 50000000)
        {
            SaveSingleton.Instance.Save.Achievement3Unlocked = true;
        }
        if(SaveSingleton.Instance.Save.PlayerMoney >= 100000000)
        {
            SaveSingleton.Instance.Save.Achievement4Unlocked = true;
        }

        switch(AchievementIndex)
        {
            case 1:
            {
                if(SaveSingleton.Instance.Save.Achievement1Unlocked == true)
                {
                    AchievementCompletionStatus.SetActive(true);
                }
                else
                {
                    AchievementCompletionStatus.SetActive(false);
                }
                break;
            }
            case 2:
            {
                if(SaveSingleton.Instance.Save.Achievement2Unlocked == true)
                {
                    AchievementCompletionStatus.SetActive(true);
                }
                else
                {
                    AchievementCompletionStatus.SetActive(false);
                }
                break;
            }
            case 3:
            {
                if(SaveSingleton.Instance.Save.Achievement3Unlocked == true)
                {
                    AchievementCompletionStatus.SetActive(true);
                }
                else
                {
                    AchievementCompletionStatus.SetActive(false);
                }
                break;
            }
            case 4:
            {
                if(SaveSingleton.Instance.Save.Achievement4Unlocked == true)
                {
                    AchievementCompletionStatus.SetActive(true);
                }
                else
                {
                    AchievementCompletionStatus.SetActive(false);
                }
                break;
            }
        }
    }
}
