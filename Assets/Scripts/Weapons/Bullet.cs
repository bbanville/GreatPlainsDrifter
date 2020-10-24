using Assets.Scripts.Managers;
using Assets.Scripts.People;
using UnityEngine;

namespace Assets.Scripts.Weapons
{
    public class Bullet : MonoBehaviour
    {

        public float Speed;
        public int Damage = 10;

        public bool IsFriendly;

        void Update()
        {
            transform.position += (transform.forward.normalized * Speed * Time.deltaTime);
        }

        private void OnTriggerEnter(Collider col)
        {
            Destroy(gameObject);
        }
    }
}
