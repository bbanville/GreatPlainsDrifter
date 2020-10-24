using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace BrendonBanville.Tools
{
    public class CountUpToTarget : MonoBehaviour
    {
        public void CountToTarget(TextMeshProUGUI field, int targetVal, float duration, float delay = 0f, string prefix = "", string suffix = "")
        {
            StartCoroutine(ExecuteCountToTarget(field, targetVal, duration, delay, prefix));
        }

        IEnumerator ExecuteCountToTarget(TextMeshProUGUI field, int targetVal, float duration, float delay = 0f, string prefix = "", string suffix = "")
        {
            if (delay > 0)
            {
                yield return new WaitForSeconds(delay);
            }

            float current = 0;
            while (current < targetVal)
            {
                // step by amount that will get us to the target value within the duration
                current += (int)(targetVal / (duration / Time.deltaTime));
                current = Mathf.Clamp(current, 0, targetVal);
                field.text = prefix + current + suffix;
                yield return null;
            }
        }
    }
}