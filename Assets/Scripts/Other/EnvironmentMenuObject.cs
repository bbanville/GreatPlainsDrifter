using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnvironmentMenuObject
{
    public string Name;
    public GameObject EnvObject;
    public Animator ObjectAnimator;

    public EnvironmentMenuObject(string name, GameObject envObject, Animator objectAnimator)
    {
        Name = name;
        EnvObject = envObject;
        ObjectAnimator = objectAnimator;
    }
}
