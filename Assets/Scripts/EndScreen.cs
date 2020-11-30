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

        [SerializeField] protected TextMeshProUGUI GameTimeText;
        [SerializeField] protected TextMeshProUGUI EnemyKillsText;
        [SerializeField] protected TextMeshProUGUI WeaponsFoundText;
        [SerializeField] protected TextMeshProUGUI SecretsFoundText;

        Animator _animator;

        // Start is called before the first frame update
        void Start()
        {
            SetEndScreenValues();
        }

        void Update()
        {

        }

        public void SetEndScreenValues()
        {
            GameTimeText.text = GameManager.Instance.GameTimeFormatted;
            EnemyKillsText.text = GameManager.Instance.EnemyKills.ToString() + " / " + GameManager.Instance.TotalEnemies;
            WeaponsFoundText.text = GameManager.Instance.WeaponsFound.ToString() + " / " + GameManager.Instance.TotalWeapons;
            SecretsFoundText.text = GameManager.Instance.SecretsFound.ToString() + " / " + GameManager.Instance.TotalSecrets;
        }
    }

    [Serializable]
    public class EndScreenItem
    {
        public string Name;
        public GameObject itemContainer;
        public TextMeshProUGUI itemText;
        public AudioClip clipToPlay;
        public float nextItemDelay;
    }
}
