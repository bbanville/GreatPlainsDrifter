/****************************************************************************
File: Pickup.cs
Author: Brendon Banville
DP Email: b.banville@digipen.edu
Date: 9/18/2019
Course: CS176
Section: A
Description:
    The code for the switch interactble.
*******************************************************************************/

using Assets.Scripts.Player;
using UnityEngine;

namespace Assets.Scripts.Environment.Interactable
{
    public class Pickup : MonoBehaviour
    {
        public string Name = "Red Key";
        public GameObject PickupModel;
        public float OffsetY = .25f;
        public float Speed = 4;
        public float MaxSway = .125f;
        public AudioClip Take;

        private float _swayRatio;

        void Start()
        {
            _swayRatio = 1/MaxSway;
            GetComponent<SphereCollider>().center = new Vector3(0, OffsetY, 0);
        }

        void Update()
        {
            PickupModel.transform.localPosition = new Vector3(0, OffsetY + Mathf.Sin(Time.fixedTime * Speed) / _swayRatio, 0);
        }

        void OnTriggerEnter(Collider col)
        {
            if (col.name == "Player")
            {
                AudioSource.PlayClipAtPoint(Take, transform.position);
                //PlayerInventory.AddItem(Name);
                gameObject.SetActive(false);
            }
        }

        public virtual void ActivatePickup()
        {

        }
    }
}