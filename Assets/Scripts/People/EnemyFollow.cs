using BrendonBanville.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.People;

public class EnemyFollow : MonoBehaviour
{
    public GameObject _player;
    Rigidbody _rigidbody;
    float _currentMoveSpeed;
    Vector3 _lastDirection;
    enum EnemyBehaviourTypes { LookAtOnly, Follow }

    [Header("Enemy Settings")]
    [ReadOnly] public GameObject _enemy;

    public float TargetingRange = 1;
    public float EnemySpeed = 6;
    public float EnemyAcceleration = 1;

    float AttackTrigger;
    float TargetDistance;
    RaycastHit Shot;

    [Header("Behaviour Settings")]
    [SerializeField] EnemyBehaviourTypes enemyBehaviourType = EnemyBehaviourTypes.Follow;
    public bool enemyAggroed = false;

    [Header("Debug")]
    [SerializeField] bool renderSphere = false;

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _enemy = this.gameObject;
        _rigidbody = GetComponent<Rigidbody>();

    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        TargetDistance = Vector3.Distance(_player.transform.position, this.transform.position);

        if (TargetDistance < TargetingRange)
        {
            enemyAggroed = true;
        }

        if (enemyAggroed)
        {
            transform.LookAt(new Vector3(_player.transform.position.x, this.transform.position.y, _player.transform.position.z));
        }
    }

    void FixedUpdate()
    {
        var moveVector = Vector3.forward;

        var magnitude = moveVector.magnitude > 1 ? 1 : moveVector.magnitude;
        var speed = magnitude * EnemySpeed;

        var movement = transform.forward /* Time.deltaTime*/;

        if (enemyAggroed && enemyBehaviourType == EnemyBehaviourTypes.Follow)
        {
            movement = new Vector3(movement.x * speed, _rigidbody.velocity.y, movement.z * speed);

            _rigidbody.velocity = movement;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (renderSphere)
        {
            // Draw a yellow sphere at the transform's position
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, TargetingRange);
        }
    }
}
