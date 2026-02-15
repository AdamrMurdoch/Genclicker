using UnityEngine;
using System.Collections.Generic;

public class ObjectEnable : MonoBehaviour
{
    public List<GameObject> EnabledObjects = new List<GameObject>();

    private void Start()
    {
        foreach (GameObject obj in EnabledObjects)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }
    }
}