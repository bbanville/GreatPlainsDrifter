using Assets.Scripts.Managers;
using BrendonBanville.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FocusChecker : MonoBehaviour
{
    [Range(0.0f, 1.0f)] public float increaseFocusAmount = 0.01f;
    public float increaseFocusRate;

    [ReadOnly] public bool isOver = false;
    private bool increasingFocus = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isOver && !increasingFocus)
        {
            if (DuelingManager.Instance.playerFocus < 100)
            {
                StartCoroutine(IncreaseFocus());
            }
        }
    }

    public void IsOver()
    {
        isOver = true;
    }

    public void IsNotOver()
    {
        isOver = false;
    }

    IEnumerator IncreaseFocus()
    {
        increasingFocus = true;
        DuelingManager.Instance.IncreasePlayerFocus(increaseFocusAmount);

        yield return new WaitForSeconds(increaseFocusRate);
        increasingFocus = false;
    }
}
