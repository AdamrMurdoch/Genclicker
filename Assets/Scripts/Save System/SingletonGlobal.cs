using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SingletonGlobal
{
    // Private instance of the singleton.
    private static SingletonGlobal instance;

    // Public property to access the singleton instance.
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