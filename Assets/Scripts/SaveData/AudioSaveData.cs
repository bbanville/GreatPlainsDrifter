/*******************************************************************************
File:      AudioSaveData.cs
Author:    Brendon Banville
DP Email:  b.banville@digipen.edu
Date:      4/8/2019
Course:    CS175
Section:   A

Description:
    Contains the serializable data for the pause menu.  
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Xml;
using System.Xml.Serialization;
using System;

[System.Serializable]
public class AudioSaveData
{
    public float musicVol;
    public float sfxVol;
    public float uiVol;
}