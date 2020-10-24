/*******************************************************************************
File: PlayerAmmo.cs
Author: Brendon Banville
DP Email: b.banville@digipen.edu
Date: 9/17/2019
Course: CS176
Section: A
Description:
    Tracks player ammo as well as the available player weapons.
*******************************************************************************/

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PlayerAmmo : MonoBehaviour
    {
        public GameObject Hand;
        public GameObject CurrentWeapon;
        public List<GameObject> WeaponList;
        public List<bool> CollectedWeapons;

        private Dictionary<AmmoType, int> _ammo = new Dictionary<AmmoType, int>();

        void Start()
        {
            _ammo = new Dictionary<AmmoType, int>
            {
                { AmmoType.Infinite, 666 }
            };

            if (CurrentWeapon == null)
            {
                CurrentWeapon = Instantiate(WeaponList.First());
                CurrentWeapon.transform.SetParent(Hand.transform, false);
            }
        }

        /// <summary>
        /// Switches the equipped weapon if there is a weapon in the slot
        /// </summary>
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) && HasWeapon(0))
            {
                Destroy(CurrentWeapon);
                CurrentWeapon = Instantiate(WeaponList[0]);
                CurrentWeapon.transform.SetParent(Hand.transform, false);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2) && HasWeapon(1))
            {
                Destroy(CurrentWeapon);
                CurrentWeapon = Instantiate(WeaponList[1]);
                CurrentWeapon.transform.SetParent(Hand.transform, false);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3) && HasWeapon(2))
            {
                Destroy(CurrentWeapon);
                CurrentWeapon = Instantiate(WeaponList[2]);
                CurrentWeapon.transform.SetParent(Hand.transform, false);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4) && HasWeapon(3))
            {
                Destroy(CurrentWeapon);
                CurrentWeapon = Instantiate(WeaponList[3]);
                CurrentWeapon.transform.SetParent(Hand.transform, false);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5) && HasWeapon(4))
            {
                Destroy(CurrentWeapon);
                CurrentWeapon = Instantiate(WeaponList[4]);
                CurrentWeapon.transform.SetParent(Hand.transform, false);
            }
        }

        /// <summary>
        /// Adds a weapon to your list of collected weapons
        /// </summary>
        /// <param name="weaponId"></param>
        public void CollectWeapon(int weaponId)
        {
            CollectedWeapons[weaponId] = true;

            var highestId = CollectedWeapons.Select((s, i) => new {i, s})
                .Where(t => t.s)
                .Select(t => t.i)
                .Max();

            if (weaponId >= highestId)
            {
                Destroy(CurrentWeapon);
                CurrentWeapon = Instantiate(WeaponList[weaponId]);
                CurrentWeapon.transform.SetParent(Hand.transform, false);
            }
        }

        /// <summary>
        /// Checks if you have a specific weapon
        /// </summary>
        /// <param name="weaponId"></param>
        /// <returns></returns>
        public bool HasWeapon(int weaponId)
        {
            return CollectedWeapons.Count > weaponId && CollectedWeapons[weaponId];
        }

        /// <summary>
        /// Gets how much ammo of a certain type the player has
        /// </summary>
        /// <param name="ammoType"></param>
        /// <returns></returns>
        public int GetAmmo(AmmoType ammoType)
        {
            return _ammo.ContainsKey(ammoType) ? _ammo[ammoType] : 0;
        }

        /// <summary>
        /// Checks if the player has ammo
        /// </summary>
        /// <param name="ammoType"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public bool HasAmmo(AmmoType ammoType, int amount)
        {
            return ammoType == AmmoType.Infinite || GetAmmo(ammoType) > amount;
        }

        /// <summary>
        /// Gives the player ammo of a certain type
        /// </summary>
        /// <param name="ammoType"></param>
        /// <param name="amount"></param>
        public void AddAmmo(AmmoType ammoType, int amount)
        {
            if (_ammo.ContainsKey(ammoType))
            {
                _ammo[ammoType] += amount;
            }
            else
            {
                _ammo.Add(ammoType, amount);
            }
        }

        /// <summary>
        /// Takes ammo from the player of a certain type
        /// </summary>
        /// <param name="ammoType"></param>
        /// <param name="amount"></param>
        public void RemoveAmmo(AmmoType ammoType, int amount)
        {
            if (_ammo.ContainsKey(ammoType))
            {
                _ammo[ammoType] -= amount;
            }
        }
    }
}