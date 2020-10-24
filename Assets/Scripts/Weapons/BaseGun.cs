using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Managers;
using Assets.Scripts.People;
using Assets.Scripts.Player;
using BrendonBanville.Tools;
using UnityEngine;

namespace Assets.Scripts.Weapons
{
    public class BaseGun : MonoBehaviour
    {
        public enum UserTypes { Player, AI };
        public UserTypes userType = UserTypes.Player;

        public AmmoType AmmunitionType;
        public GameObject Bullet;
        public GameObject MuzzleFlash;
        public bool IsMelee;
        public Transform Tip;
        public List<AudioClip> AttackSound;
        public int gunDamage = 25;
        public float ShotDelay = .1f;
        public int magazineSize = 6;
        public int ammoInMagazine;
        public float reloadTime = 1.0f;

        [HideInInspector] public Vector3 shotDirection;
        public bool setShotDirectionally = true;
        public bool setShotStaticTarget = false;

        [Condition("setShotStaticTarget", true)]
        public Transform staticTarget;
        private Transform defaultTarget;

        private Animator _animator;
        private AudioSource _audioSource;
        private float _lastShot;
        private PlayerAmmo _ammo;
        private UnityEngine.Camera _viewCamera;

        bool reloading;

        void Start()
        {
            _ammo = FindObjectOfType<PlayerAmmo>();
            _animator = GetComponent<Animator>();
            _audioSource = GetComponent<AudioSource>();
            _viewCamera = Camera.main;

            ammoInMagazine = magazineSize;

            defaultTarget = _viewCamera.transform;
        }

        void Update()
        {

        }

        public virtual void Fire()
        {
            if (CanFire())
            {
                if (IsMelee)
                {
                    MeleeAttack();
                }
                else
                {
                    _animator.Play("GunFire");

                    ammoInMagazine--;

                    RangedAttack();
                }
            }
            else if (ammoInMagazine <= 0)
            {
                Reload();
            }
        }

        public virtual void MeleeAttack()
        {
            _lastShot = Time.fixedTime;

            var ray = _viewCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && Vector3.Distance(transform.position, hit.point) < .5f)
            {
                if (hit.collider.tag == "Enemy")
                {
                    var hitContext = new HitContext
                    {
                        Damage = Random.Range(0, 2),
                        Direction = transform.forward,
                        Force = 1,
                        IsMelee = true                    
                    };

                    hit.collider.GetComponent<HealthBehavior>().TakeDamage(hitContext);
                }
            }
        }

        public virtual void RangedAttack()
        {
            if (AttackSound.Count > 0)
            {
                _audioSource.PlayOneShot(AttackSound.Random());
            }

            var bullet = Instantiate(Bullet);

            if (Tip != null)
            {
                var ray = _viewCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
                RaycastHit hit;

                if (setShotDirectionally)
                {
                    if (Physics.Raycast(ray, out hit))
                    {
                        shotDirection = (hit.point - Tip.transform.position).normalized;
                    }
                    else
                    {
                        shotDirection = defaultTarget.forward;
                    }
                }
                else if (setShotStaticTarget)
                {
                    Tip.LookAt(staticTarget);
                    shotDirection = Tip.forward;
                }
                else
                {
                    shotDirection = Tip.forward;
                }

                bullet.transform.position = Tip.transform.position;
                bullet.transform.rotation = Quaternion.LookRotation(shotDirection);
            }
            else
            {
                bullet.transform.position = transform.position;
                bullet.transform.rotation = transform.rotation;
            }

            bullet.GetComponent<Bullet>().Damage = gunDamage;

            _lastShot = Time.fixedTime;

            if (AmmunitionType != AmmoType.Infinite)
            {
                _ammo.RemoveAmmo(AmmoType.Pistol, 1);
            }
        }

        public bool CanFire()
        {
            var ammunitionIsNotEmpty = (AmmunitionType == AmmoType.Infinite || _ammo.HasAmmo(AmmunitionType, 1));
            var magazineIsNotEmpty = ammoInMagazine > 0;
            var itHasBeenLongEnoughSinceTheLastShot = Time.fixedTime - _lastShot > ShotDelay;

            return ammunitionIsNotEmpty && itHasBeenLongEnoughSinceTheLastShot && magazineIsNotEmpty;
        }

        public void Reload()
        {
            reloading = true;
            StartCoroutine(ReloadTime(reloadTime));
        }

        IEnumerator ReloadTime(float time)
        {
            yield return new WaitForSeconds(time);

            reloading = false;
            ammoInMagazine = magazineSize;
        }
    }
}