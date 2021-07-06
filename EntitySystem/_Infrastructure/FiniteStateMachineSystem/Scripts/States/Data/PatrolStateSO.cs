// using System; // For events
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    [CreateAssetMenu(menuName = "EnemyAI/State Data/Patrol State", fileName = "newPatrolState")]
    public class PatrolStateSO : ScriptableObject
    {
        public float patrolSpeed = 3f;
        public int currentPatrolPointIndex = 0;
        public Transform[] patrolPoints;
    }
}
