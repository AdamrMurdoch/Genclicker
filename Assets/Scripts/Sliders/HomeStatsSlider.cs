using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HomeStatsSlider : MonoBehaviour
{
    public Slider Energy;
    public Slider Achievements;
    public Slider Wealth;

    public float smoothTime = 0.2f;
    private float EnergyVelocity = 0f;
    private float AchievementsVelocity = 0f;
    private float WealthVelocity = 0f;

    void Update()
    {
        SaveSingleton.Instance.Save.AchievementsCompleted = Mathf.Clamp(SaveSingleton.Instance.Save.AchievementsCompleted, 0, 100);
        SaveSingleton.Instance.Save.PlayerEnergy = Mathf.Clamp(SaveSingleton.Instance.Save.PlayerEnergy, 0, 100);
        SaveSingleton.Instance.Save.WealthScore = Mathf.Clamp(SaveSingleton.Instance.Save.WealthScore, 0, 100);

        UpdateSlider(Achievements, SaveSingleton.Instance.Save.AchievementsCompleted, ref AchievementsVelocity);
        UpdateSlider(Energy, SaveSingleton.Instance.Save.PlayerEnergy, ref EnergyVelocity);
        UpdateSlider(Wealth, SaveSingleton.Instance.Save.WealthScore, ref WealthVelocity);
    }

    void UpdateSlider(Slider slider, float targetValue, ref float velocity)
    {
        slider.value = Mathf.SmoothDamp(slider.value, targetValue, ref velocity, smoothTime);
    }
}
