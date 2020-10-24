using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DrunkLevel
{
    public string LevelName;
    public Material DrunkMaterial;

    public DrunkLevel(string levelName, Material drunkMaterial)
    {
        LevelName = levelName;
        DrunkMaterial = drunkMaterial;
    }
}
