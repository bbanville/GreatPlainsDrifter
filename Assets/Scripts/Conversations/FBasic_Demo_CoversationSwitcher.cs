using BrendonBanville.Tools;
using FIMSpace.FBasics;
using UnityEngine;

public class FBasic_Demo_CoversationSwitcher : MonoBehaviour
{
    public bool ButtonActivated = false;
    [Condition("ButtonActivated", true)]
    public CanvasGroup Button;

    public FBasic_Conversation Conversation;
    public StartConversation startConvo;

	public void StartConversation()
    {
        Conversation.ShowConversation();
	}

    private void Start()
    {
        startConvo = GetComponent<StartConversation>();
    }

    public void Update()
    {
        if (Button)
        {
            if (Conversation.IsWorking && ButtonActivated)
            {
                Button.alpha -= Time.deltaTime * 5f;
                Button.interactable = false;
            }
            else if (ButtonActivated)
            {
                Button.alpha += Time.deltaTime * 5f;
                Button.interactable = true;
            }
        }

        if (!Conversation.IsWorking && !ButtonActivated && startConvo.StartOnSceneLoad)
        {
            StartConversation();
        }
    }
}
