using UnityEngine;

public class SingletonGlobal
{
    private static SingletonGlobal instance;
    public static SingletonGlobal Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new SingletonGlobal();
            }
            return instance;
        }
    }

    public bool IsSceneTransitioning;
}