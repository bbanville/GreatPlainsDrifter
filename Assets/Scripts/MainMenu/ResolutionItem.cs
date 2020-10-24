/*******************************************************************************
File:      ResolutionItem.cs
Author:    Brendon Banville
DP Email:  b.banville@digipen.edu
Date:      4/8/2019
Course:    CS175
Section:   A

Description:
    Contains the class that stores the data for a resolution item used in the
    pause menu.  
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResolutionItem
{
    public int width;
    public int height;

    public ResolutionItem(int newWidth, int newHeight)
    {
        width = newWidth;
        height = newHeight;
    }
}