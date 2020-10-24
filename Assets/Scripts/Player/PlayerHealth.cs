/*******************************************************************************
File: PlayerHealth.cs
Author: Brendon Banville
DP Email: b.banville@digipen.edu
Date: 9/15/2019
Course: CS176
Section: A
Description:
    Contains the methods and functions for updating player health both in the
    system and the UI.
*******************************************************************************/

using Assets.Scripts.People;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Assets.Scripts.Player
{
    public class PlayerHealth : MonoBehaviour, IDamageBehavior
    {
        public TextMeshProUGUI HealthTextBox;
        public Image HealthImage;
        public GameObject Weapon;

        private HealthBehavior _healthBehavior;

        void Start()
        {
            _healthBehavior = GetComponent<HealthBehavior>();
        }

        void Update()
        {
            HealthTextBox.text = "" + _healthBehavior.Health;
            //HealthImage.color = new Color(255, 0, 0, 1 - _healthBehavior.Health / 100f);
        }

        public void TakeDamage(int damage)
        {
            _healthBehavior.Health -= damage;
        }

        public void OnHit(HitContext hitContext)
        {
            // Flash screen red or some shit here
        }

        public void OnDeath(HitContext hitContext)
        {
            var rigidBody = GetComponent<Rigidbody>();
            rigidBody.constraints = RigidbodyConstraints.None;
            rigidBody.AddExplosionForce(hitContext.Force * 2, transform.position - hitContext.Direction, 1f, 3f, ForceMode.Impulse);

            gameObject.GetComponent<BoxCollider>().center = Vector3.zero;
            gameObject.GetComponent<BoxCollider>().size = new Vector3(.4f,.3f, .27f);

            Destroy(GetComponent<CapsuleCollider>());
            Destroy(GetComponentInChildren<MouseRotation>());
            Destroy(GetComponentInChildren<KeyboardMovement>());
            Destroy(GetComponentInChildren<Bobber>());
            Destroy(GetComponentInChildren<Tilter>());
            Destroy(GetComponentInChildren<PlayerInteractions>());
            Destroy(gameObject.GetComponent<PlayerGun>());
            Destroy(Weapon);
        }
    }
}