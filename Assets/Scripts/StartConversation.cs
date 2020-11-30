using Assets.Scripts.Managers;
using BrendonBanville.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartConversation : MonoBehaviour
{
    FBasic_Demo_CoversationSwitcher ConvoSwitcher;
    SoundManager SM;

    public bool Outside = true;
    public bool Inside = false;

    [Space(10)]
    public GameObject Player;
    public GameObject TPCamera;
    public GameObject DialogueCamera;

    public bool StartOnSceneLoad = true;

    // Start is called before the first frame update
    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        ConvoSwitcher = GetComponent<FBasic_Demo_CoversationSwitcher>();
        SM = GameObject.FindGameObjectWithTag("AM").GetComponent<SoundManager>();

        if (Outside)
        {
            SM.Outside.TransitionTo(1.0f);
        }
        else if (Inside)
        {
            SM.Inside.TransitionTo(1.0f);
        }

        if (StartOnSceneLoad)
        {
            ConvoSwitcher.StartConversation();
        }
    }

    private void Update()
    {
        
    }

    public void InitiateConversation()
    {
        //Cursor.visible = true;
        //Cursor.lockState = CursorLockMode.None;

        Player.GetComponent<ThirdPersonUserControl>().readPlayerInput = false;
        Player.GetComponentInChildren<SkinnedMeshRenderer>().gameObject.SetActive(false);
        TPCamera.SetActive(false);
        DialogueCamera.SetActive(true);

        ConvoSwitcher.Conversation.PlayerReference = Player.transform;
        ConvoSwitcher.StartConversation();
    }
}
