using UnityEngine;
using System.Collections;
using Assets.Scripts.Managers;

public class ExplodingProjectile : MonoBehaviour
{
    [Header("Projectile Dependencies")]
    [Tooltip("Check layers bullet will hit")]
    public LayerMask targetLayerMask;
    
    public GameObject impactPrefab;
    public GameObject explosionPrefab;

    public Rigidbody thisRigidbody;

    public GameObject particleKillGroup;
    private Collider thisCollider;

    [Header("Projectile Settings")]
    public float thrust;
    public bool LookRotation = true;
    public float projectileSpeed;
    public float projectileSpeedMultiplier;

    public bool ignorePrevRotation = false;

    public bool explodeOnTimer = false;
    public float explosionTimer;
    float timer;

    private Vector3 previousPosition;

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start()
    {
        thisRigidbody = GetComponent<Rigidbody>();
        thisCollider = GetComponent<Collider>();
        previousPosition = transform.position;
    }
    
    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        /*if(Input.GetButtonUp("Fire2"))
        {
            Explode();
        }*/

        timer += Time.deltaTime;
        if (timer >= explosionTimer && explodeOnTimer == true)
        {
            Explode();
        }

    }

    /// <summary>
    /// 
    /// </summary>
    void FixedUpdate()
    {
        if (LookRotation && timer >= 0.05f)
        {
            transform.rotation = Quaternion.LookRotation(thisRigidbody.velocity);
        }

        CheckCollision(previousPosition);

        previousPosition = transform.position;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="prevPos"></param>
    void CheckCollision(Vector3 prevPos)
    {
        RaycastHit hit;
        Vector3 direction = transform.position - prevPos;
        Ray ray = new Ray(prevPos, direction);
        float dist = Vector3.Distance(transform.position, prevPos);

        if (Physics.Raycast(ray, out hit, dist, targetLayerMask))
        {
            transform.position = hit.point;
            Quaternion rot = Quaternion.FromToRotation(Vector3.forward, hit.normal);
            Vector3 pos = hit.point;
            Instantiate(impactPrefab, pos, rot);

            if (!explodeOnTimer)
            {
                DuelingManager.Instance.KillOpponent();
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="collision"></param>
    //void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.tag != "FX" && collision.gameObject.tag != "Player" && collision.gameObject.tag != "Weapon")
    //    {
    //        ContactPoint contact = collision.contacts[0];
    //        Quaternion rot = Quaternion.FromToRotation(Vector3.forward, contact.normal);
    //        if (ignorePrevRotation)
    //        {
    //            rot = Quaternion.Euler(0, 0, 0);
    //        }
    //        Vector3 pos = contact.point;
    //        Instantiate(impactPrefab, pos, rot);
    //
    //        if (!explodeOnTimer)
    //        {
    //            Destroy(gameObject);
    //        }
    //    }
    //}

    /// <summary>
    /// 
    /// </summary>
    void Explode()
    {
        Instantiate(explosionPrefab, gameObject.transform.position, Quaternion.Euler(0, 0, 0));
        Destroy(gameObject);
    }

}