using Assets.Scripts.Weapons;
using UnityEngine;

namespace Assets.Scripts.People
{
    public class HealthBehavior : MonoBehaviour
    {
        public int Health = 5;
        private IDamageBehavior _damageBehavior;

        Bullet bullet;

        void Start()
        {
            _damageBehavior = GetComponent<IDamageBehavior>();
        }

        void OnTriggerEnter(Collider col)
        {
            if (col.tag == "FX")
            {
                bullet = col.GetComponent<Bullet>();

                if (bullet != null && ShouldCollideWith(bullet))
                {
                    //Debug.Log("Hit Registered");

                    HitContext hitContext = new HitContext
                    {
                        Direction = bullet.transform.forward,
                        Force = bullet.Speed,
                        Damage = bullet.Damage,
                        IsMelee = false
                    };

                    TakeDamage(hitContext);

                    /// Temporary solution
                    if (GetComponent<EnemyFollow>() != null)
                    {
                        var enemyBehaviour = GetComponent<EnemyFollow>();

                        enemyBehaviour.enemyAggroed = true;
                    }
                    ///
                }
            }
        }

        public void TakeDamage(HitContext context)
        {
            Health -= context.Damage;

            if (Health <= 0)
            {
                Health = 0;

                /// Temporary solution
                Destroy(this.gameObject);
                ///

                //_damageBehavior.OnDeath(context);
            }
            else
            {
                //_damageBehavior.OnHit(context);
            }
        }

        private bool ShouldCollideWith(Bullet bullet)
        {
            return ((bullet.IsFriendly && transform.tag != "Player") || (!bullet.IsFriendly && transform.tag == "Player"));
        }
    }
}