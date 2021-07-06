// using System; // For events
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    [CreateAssetMenu(menuName = "EnemyAI/State Data/Chase State", fileName = "newChaseStateData")]
    public class ChaseStateSO : ScriptableObject
    {
        public float chaseSpeed = 6f;
    }
}
