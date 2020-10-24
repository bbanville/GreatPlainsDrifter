using Assets.Scripts.Managers;
using BrendonBanville.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuelingUserControl : Singleton<DuelingUserControl>
{
    [Tooltip("This button will begin drawing the player's weapon")]
    public KeyCode drawWeapon = KeyCode.Mouse1;

    [Tooltip("This button will fire the player's weapon")]
    public KeyCode fireWeapon = KeyCode.Mouse0;

    private Animator _playerAnimator;
    private Animator _opponentAnimator;

    // Start is called before the first frame update
    void Start()
    {
        _playerAnimator = DuelingManager.Instance._player.GetComponent<Animator>();
        _opponentAnimator = DuelingManager.Instance._opponent.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(drawWeapon) && !DuelingManager.Instance.gunDrawn)
        {
            DuelingManager.Instance.DrawGun();
            DuelingManager.Instance.DrawPrompt.GetComponentInParent<Animator>().Play("PromptDrawDissapear");
        }

        if (Input.GetKeyDown(fireWeapon))
        {
            if (DuelingManager.Instance.gunDrawn
                && DuelingManager.Instance.endState != DuelingManager.EndStates.PlayerWin
                && DuelingManager.Instance._playerGun.gameObject.activeInHierarchy)
            {
                DuelingManager.Instance._playerGun.Fire();
                //DuelingManager.Instance.KillOpponent();
            }
        }
    }
}
