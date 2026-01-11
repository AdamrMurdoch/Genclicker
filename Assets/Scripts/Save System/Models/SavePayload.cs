using System;

[Serializable]
public class SavePayload
{
    public int version = 1;
    public SaveData save;
}