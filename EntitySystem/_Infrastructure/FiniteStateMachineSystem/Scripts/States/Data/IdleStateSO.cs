// using System; // For events
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    [CreateAssetMenu(menuName = "EnemyAI/State Data/Idle State", fileName = "newIdleStateData")]
    public class IdleStateSO : ScriptableObject
    {
        public float minIdleTime = 1f, maxIdleTime = 2f;
    }
}
