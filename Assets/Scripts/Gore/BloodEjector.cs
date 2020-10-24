/*******************************************************************************
File: PlayerHealth.cs
Author: Brendon Banville
DP Email: b.banville@digipen.edu
Date: 9/15/2019
Course: CS176
Section: A
Description:
    produces part of a blood splatter effect.
*******************************************************************************/

using Assets.Scripts.People;
using BrendonBanville.Tools;
using UnityEngine;

namespace Assets.Scripts.Gore
{
    public class BloodEjector : MonoBehaviour
    {
        public GameObject BloodDroplet;

        public float MinForce;
        public float MaxForce;

        /// <summary>
        /// Activates blood splat on hit
        /// </summary>
        /// <param name="hitContext"></param>
        public void Eject(HitContext hitContext)
        {
            // initial burst
            for (var i = 0; i < 10; i++)
            {
                var blood = Instantiate(BloodDroplet);
                blood.transform.position = transform.position + new Vector3(0, .2f, 0);
                blood.GetComponent<Rigidbody>().AddForce(Random.onUnitSphere.PositiveY() * Random.Range(MinForce, MaxForce));
            }

            // follow bullet path
            for (var i = 0; i < 10; i++)
            {
                var blood = Instantiate(BloodDroplet);
                blood.transform.position = transform.position + new Vector3(0, .2f, 0);
                var path = hitContext.Direction*Random.Range(MinForce, MaxForce)*hitContext.Force;
                path.y = Random.Range(0f, 250f);
                blood.GetComponent<Rigidbody>().AddForce(path);
            }
        }
    }
}