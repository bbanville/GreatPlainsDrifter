using Assets.Scripts.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using BrendonBanville.Tools;
using Assets.Scripts.Environment.Interactable;

namespace Assets.Scripts
{
    public class EndScreen : MonoBehaviour
    {
        [SerializeField] List<EndScreenItem> endScreenItems;

        KeyboardMovement _playerMovement;
        MouseRotation _playerMouseRotation;
        LockMouse _playerMouseLock;
        Animator _animator;
        float Timer;

        public float BestTime;
        public float NextBestTime;

        // Start is called before the first frame update
        void Start()
        {
            var _player = GameObject.FindGameObjectWithTag("Player");

            _playerMovement = _player.GetComponent<KeyboardMovement>();
            _playerMouseRotation = _player.GetComponent<MouseRotation>();
            _playerMouseLock = _player.GetComponent<LockMouse>();
            _animator = GetComponent<Animator>();
        }

        void Update()
        {
            Timer += Time.deltaTime;
        }

        public void LevelComplete()
        {
            _playerMovement.enabled = false;
            _playerMouseRotation.enabled = false;
            _playerMouseLock.enabled = false;

            NextBestTime = Timer;
            if (NextBestTime > BestTime)
            {
                BestTime = NextBestTime;
            }

            _animator.Play("EndScreen");
        }
    }

    [Serializable]
    public class EndScreenItem
    {
        public string Name;
        public List<GameObject> itemObject;
        public AudioClip clipToPlay;
        public float nextItemDelay;
    }
}
