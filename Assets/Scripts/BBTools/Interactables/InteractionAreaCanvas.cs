using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace BrendonBanville.Tools.Interactables
{
    /// <summary>
    /// FM: Script for creating interaction area and canvas with text viewed on object and handling choosed event
    /// </summary>
    public class InteractionAreaCanvas : InteractionAreaBase
    {
        public static Transform InteractionCanvasesContainer;

        [Space(3f)]
        public KeyCode InteractionKey = KeyCode.E;

        [Space(10f)]
        public Vector3 canvasObjectOffset;

        [Space(10f)]
        public UnityEvent EventOnInteraction;

        public bool displayTextInCanvas = true;
        [Condition("displayTextInCanvas", true)]
        public TMP_FontAsset textFont;
        [Condition("displayTextInCanvas", true)]
        public string textInCanvas = "Interact";

        protected Canvas viewCanvas;
        protected RectTransform canvasRect;
        protected CanvasGroup canvasGroup;

        protected TextMeshProUGUI viewText;

        protected override void Start()
        {
            base.Start();

            // Creating canvas to view text on it 
            GameObject canvasObject = new GameObject("CanvasInteraction-" + name);
            canvasObject.transform.position = transform.position + transform.TransformVector(canvasObjectOffset);
            viewCanvas = canvasObject.AddComponent<Canvas>();
            canvasRect = canvasObject.GetComponent<RectTransform>();
            viewCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

            // Just canvas group to fade everything
            canvasGroup = canvasObject.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;

            // Creating Text object and assigning base variables
            GameObject textObject = new GameObject("CanvasInteraction-Text-" + name);
            textObject.transform.SetParent(canvasObject.transform);
            textObject.transform.position = canvasObject.transform.position;
            viewText = textObject.AddComponent<TextMeshProUGUI>();
            viewText.font = textFont;
            viewText.alignment = TextAlignmentOptions.Center;
            viewText.rectTransform.sizeDelta = new Vector2(500f, 350f);

            if (displayTextInCanvas)
            {
                viewText.text = "[" + InteractionKey + "] " + textInCanvas;
            }

            if (InteractionCanvasesContainer == null )
            {
                InteractionCanvasesContainer = new GameObject("Interaction Canvases-Container").transform;
            }

            canvasObject.transform.SetParent(InteractionCanvasesContainer, true);

            toLookPositionOffset = canvasObjectOffset;
        }

        protected override void UpdateIn()
        {
            if ( !Focused )
            {
                canvasGroup.alpha = 0f;
                return;
            }

            // Waiting for input to invoke actions defined in event
            if (Input.GetKeyDown(InteractionKey))
            {
                if (EventOnInteraction != null)
                {
                    EventOnInteraction.Invoke();
                }
            }



            // Setting position of text to be viewed on object in 3D space on 2D canvas
            Vector3 targetPos = VectorMethods.GetUIPositionFromWorldPosition(transform.position + transform.TransformVector(canvasObjectOffset), Camera.main, canvasRect);

            if (targetPos.z > 0f)
            {
                // Fading in quickly canvas
                canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 1.05f, Time.deltaTime * 5f);

                targetPos.z = 0f;
                viewText.rectTransform.anchoredPosition = targetPos;
            }
            else
            {
                canvasGroup.alpha = 0f;
            }
        }

        protected override void OnEnter()
        {
            base.OnEnter();

            viewCanvas.gameObject.transform.position = transform.position + transform.TransformVector(canvasObjectOffset);

            if ( InteractionKey != KeyCode.None ) viewText.text = "[" + InteractionKey + "] " + textInCanvas; else viewText.text = textInCanvas;
        }

        protected override void OnExit()
        {
            canvasGroup.alpha = 0f;
            base.OnExit();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.1f, 0.8f, 0.1f, 0.5f);
            Gizmos.DrawSphere(transform.position + transform.TransformVector(canvasObjectOffset), 0.25f);
        }
    }
}
