using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Opsive.UltimateCharacterController.Demo
{
    public class BattleRegistry : MonoBehaviour
    {
        [Header("Battle Registry")]
        public List<BattleSystem> Battles;
        private Dictionary<BattleSystem, string> BattlesMap = new Dictionary<BattleSystem, string>();

        public void ResetBattles()
        {
            foreach (BattleSystem battle in Battles)
            {
                battle.ResetBattle();
            }
        }
    }
}
