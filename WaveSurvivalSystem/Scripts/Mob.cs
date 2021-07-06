using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGS
{
    /// <summary>
    /// A specific monster spawned/managed by the WaveSystemGameSession.
    /// </summary>
    [Serializable]
    public class Mob
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        [SerializeField] private GameObject _prefab = default;
        [SerializeField] private Transform[] _spawnLocations = default;
        [SerializeField] private int _quantity = 0;
        [SerializeField] private int _batchSize = 0;
        [SerializeField] private float _spawnWindowSeconds = 0;
        [SerializeField] private int _delaySeconds = 0;
        [SerializeField] private int _sendWhenXMobsRemain = 0;
        #endregion

        //=====================================================================
        #region Properties
        //=====================================================================
        /// <summary>
        /// The prefab of the mob.
        /// </summary>
        public GameObject Prefab
        {
            get => _prefab;
            private set => _prefab = value;
        }
        /// <summary>
        /// The spawn locations of the mob.
        /// </summary>
        public Transform[] SpawnLocations
        {
            get => _spawnLocations;
            private set => _spawnLocations = value;
        }
        /// <summary>
        /// The quantity of the mob to be spawned in the wave.
        /// </summary>
        public int Quantity
        {
            get => _quantity;
            private set => _quantity = value;
        }
        /// <summary>
        /// The batch size of mobs to be spawned at a given time. If you want to
        /// spawn all instances at once, _batchSize == _quantity.
        /// </summary>
        public int BatchSize
        {
            get => _batchSize;
            private set => _batchSize = value;
        }
        /// <summary>
        /// The window of time that the quantity of mobs will spawn over.
        /// </summary>
        public float SpawnWindowSeconds
        {
            get => _spawnWindowSeconds;
            private set => _spawnWindowSeconds = value;
        }
        /// <summary>
        /// Starts spawning after a certain number of seconds.
        /// </summary>
        public int DelaySeconds
        {
            get => _delaySeconds;
            private set => _delaySeconds = value;
        }
        /// <summary>
        /// Starts spawning after a certain number of enemies remain.
        /// </summary>
        public int SendWhenXMobsRemain
        {
            get => _sendWhenXMobsRemain;
            private set => _sendWhenXMobsRemain = value;
        }
        #endregion
    }
}
