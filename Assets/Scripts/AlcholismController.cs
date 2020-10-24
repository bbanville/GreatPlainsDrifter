using BrendonBanville.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlcholismController : MonoBehaviour
{
    [ReadOnly] public drunk DrunkScript;
    [ReadOnly] public int DrunkLevel = 0;

    [Space(10)]
    public float AlchoholTolerance = 8.0f;
    public List<Material> DrunkLevels;

    private bool SoberingUp = false;

    protected void Start()
    {
        DrunkScript = GetComponent<drunk>();
    }

    protected void Update()
    {
        if (DrunkLevel == 0)
        {
            DrunkScript.enabled = false;
        }
        else
        {
            DrunkScript.enabled = true;
        }

        if (DrunkLevel == 1)
        {
            DrunkScript.activeMaterial = DrunkLevels[0];
        }
        if (DrunkLevel == 2)
        {
            DrunkScript.activeMaterial = DrunkLevels[1];
        }
        if (DrunkLevel == 3)
        {
            DrunkScript.activeMaterial = DrunkLevels[2];
        }

        if (DrunkLevel > 0 && !SoberingUp)
        {
            StartCoroutine(SoberUp());
        }
    }

    public void IncreaseDrunkLevel()
    {
        if (DrunkLevel < 4)
        {
            DrunkLevel++;
        }
    }

    public void DecreaseDrunkLevel()
    {
        if (DrunkLevel > 0)
        {
            DrunkLevel--;
        }
    }

    IEnumerator SoberUp()
    {
        SoberingUp = true;

        yield return new WaitForSeconds(AlchoholTolerance);

        DecreaseDrunkLevel();
        SoberingUp = false;
    }
}
