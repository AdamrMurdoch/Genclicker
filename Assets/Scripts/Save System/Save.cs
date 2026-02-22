using UnityEngine;
using System.Collections;

public class Save : MonoBehaviour
{
    private int SaveInterval;
    private void Start()
    {
        SaveInterval = 0;
        StartCoroutine(AutoSaveCoroutine());
    }

    private IEnumerator AutoSaveCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            SaveInterval++;

            if (SaveInterval >= 10)
            {
                StartCoroutine(SaveSingleton.Instance.SaveAllData());
                SaveInterval = 0;
            }
        }
    }

    private void OnApplicationQuit()
    {
        StartCoroutine(SaveSingleton.Instance.SaveAllData());
    }

    private void OnApplicationPause(bool pause)
    {
        StartCoroutine(SaveSingleton.Instance.SaveAllData());
    }
}
