// using System; // For events
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    [CreateAssetMenu(menuName = "EnemyAI/State Data/Attack State", fileName = "newAttackStateData")]
    public class AttackStateSO : ScriptableObject
    {
        public int damage = 1;
        public float range = 0.5f;
        public float radiusOfAttack = 0.25f;
    }
}
