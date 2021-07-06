// using System; // For events
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    [CreateAssetMenu(menuName = "EnemyAI/State Data/Player Detected State", fileName = "newPlayerDetectedState")]
    public class PlayerDetectedStateSO : ScriptableObject
    {
        public float minimumAgroDistance = 1f, maximumAgroDistance = 2f;
        public float actionTime = 1.5f;
    }
}
