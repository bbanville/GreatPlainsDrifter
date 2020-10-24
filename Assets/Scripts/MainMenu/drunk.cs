using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BrendonBanville.Tools;

public class drunk : MonoBehaviour 
{
	public Material activeMaterial;

    public void SetMaterial(Material newMaterial)
    {
        activeMaterial = newMaterial;
    }

    void OnRenderImage (RenderTexture source, RenderTexture destination) 
	{
		Graphics.Blit (source, destination, activeMaterial);
	}
}