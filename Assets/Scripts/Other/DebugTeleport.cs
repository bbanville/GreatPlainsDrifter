using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class DebugTeleport : MonoBehaviour
    {
        [SerializeField] List<debugTele> debugTeleports;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            foreach (debugTele teleport in debugTeleports)
            {
                if (Input.GetKeyDown(teleport.buttonToUse))
                {
                    this.transform.position = teleport.pointToTeleportTo.position;
                    this.transform.rotation = teleport.pointToTeleportTo.rotation;
                }
            }
        }
    }

    [Serializable]
    public class debugTele
    {
        public string nameOfTeleport;
        public KeyCode buttonToUse = KeyCode.Keypad1;
        public Transform pointToTeleportTo;
    }
}
