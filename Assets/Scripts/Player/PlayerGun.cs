/*******************************************************************************
File: PlayerInteractions.cs
Author: Brendon Banville
DP Email: b.banville@digipen.edu
Date: 9/17/2019
Course: CS176
Section: A
Description:
    Allows the player to fire their equipped weapon.
*******************************************************************************/

using Assets.Scripts.Weapons;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PlayerGun : MonoBehaviour
    {
        private BaseGun _gun;

        void Start()
        {
            _gun = GetComponent<BaseGun>();
        }

        void Update()
        {
            if (Input.GetMouseButton(0))
            {
                _gun.Fire();
            }
        }
    }
}
