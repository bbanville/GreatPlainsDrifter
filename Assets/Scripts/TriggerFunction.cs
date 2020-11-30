using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace BrendonBanville.Tools
{
    [System.Serializable]
    public class TriggerFunction : MonoBehaviour
    {
        public string EventName;
        public UnityEvent FunctionsToTrigger;

        [Header("Activation Methods")]
        public bool ActivateOnTriggerEnter = false;

        private void OnTriggerEnter(Collider other)
        {
            if (ActivateOnTriggerEnter)
            {
                FunctionsToTrigger.Invoke();
            }
        }
    }
}
