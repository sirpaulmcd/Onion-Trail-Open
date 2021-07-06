using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGS
{
    /// <summary>
    /// A complete wave survival sequence with a fixed amount of waves.
    /// </summary>
    public class WaveSurvivalSequence : MonoBehaviour
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        /// <summary>
        /// The generic spawn locations mobs use if they do not have their own
        /// defined spawn locations.
        /// </summary>
        [SerializeField] private Transform[] _genericSpawnLocations = default;
        /// <summary>
        /// The waves that comprise a wave survival sequence.
        /// </summary>
        [SerializeField] private Wave[] _waves;
        /// <summary>
        /// The index of the current wave.
        /// </summary>
        private int _currentWaveIndex = -1;
        /// <summary>
        /// Whether a wave is currently in progress.
        /// </summary>
        private bool _waveInProgress = false;
        /// <summary>
        /// Whether the wave survival sequence is complete.
        /// </summary>
        private bool _sequenceComplete = false;
        /// <summary>
        /// The total number of mobs in the current wave.
        /// </summary>
        private int _totalMobCount = 0;
        /// <summary>
        /// The number of living mobs in the current wave.
        /// </summary>
        private int _livingMobCount = 0;
        #endregion

        //=====================================================================
        #region Properties
        //=====================================================================
        /// <summary>
        /// The waves associated with a wave survival event.
        /// </summary>
        public Wave[] Waves
        {
            get => _waves;
            private set => _waves = value;
        }

        /// <summary>
        /// Whether the wave survival sequence is complete.
        /// </summary>
        public bool SequenceComplete
        {
            get => _sequenceComplete;
            private set
            {
                _sequenceComplete = value;
            }
        }

        /// <summary>
        /// The number of the current wave.
        /// </summary>
        private int CurrentWaveIndex
        {
            get { return _currentWaveIndex; }
            set
            {
                _currentWaveIndex = value;
                InvokeWaveNumberUpdatedEvent();
            }
        }

        private int LivingMobCount
        {
            get { return _livingMobCount; }
            set
            {
                _livingMobCount = value;
                InvokeMobsUpdatedEvent();
            }
        }
        #endregion

        //=====================================================================
        #region MonoBehaviour
        //=====================================================================
        private void Start()
        {
            RegisterAsListener();
        }

        private void OnDestroy()
        {
            DeregisterAsListener();
        }
        #endregion

        //=====================================================================
        #region Event Invokers
        //=====================================================================
        private void InvokeMobsUpdatedEvent()
        {
            Debug.Log("WaveSurvivalSequence.InvokeMobsUpdatedEvent: Updating mob numbers.");
            EventManager.Instance.Invoke(EventName.MobsUpdatedEvent,
                new MobsUpdatedEventArgs(_livingMobCount, _totalMobCount),
                this);
        }

        private void InvokeWaveNumberUpdatedEvent()
        {
            Debug.Log("WaveSurvivalSequence.InvokeWaveNumberUpdatedEvent: Wave number updated.");
            EventManager.Instance.Invoke(EventName.WaveNumberUpdatedEvent,
                new WaveNumberUpdatedEventArgs(_currentWaveIndex + 1),
                this);
        }

        private void InvokeWaveCompleteEvent()
        {
            Debug.Log("WaveSurvivalSequence.InvokeWaveCompleteEvent: Wave has been completed.");
            EventManager.Instance.Invoke(EventName.WaveCompleteEvent,
                new WaveCompleteEventArgs(),
                this);
        }
        #endregion

        //=====================================================================
        #region Event Listeners
        //=====================================================================
        private void RegisterAsListener()
        {
            EventManager.Instance.AddListener(EventName.MobDeathEvent, HandleMobDeathEvent);
        }

        private void DeregisterAsListener()
        {
            EventManager.Instance.RemoveListener(EventName.MobDeathEvent, HandleMobDeathEvent);
        }

        private void HandleMobDeathEvent(object invoker, EventArgs e)
        {
            Debug.Log("WaveSurvivalSequence.HandleMobDeathEvent: Updating mob numbers.");
            LivingMobCount -= 1;
            if (LivingMobCount == 0) { EndWave(); }
        }
        #endregion

        //=====================================================================
        #region Wave management
        //=====================================================================
        /// <summary>
        /// Starts the next wave in the wave survival sequence.
        /// </summary>
        public void StartNextWave()
        {
            if (!_waveInProgress && _currentWaveIndex < _waves.Length)
            {
                _waveInProgress = true;
                CurrentWaveIndex += 1;
                InitializeWave();
            }
        }

        private void InitializeWave()
        {
            _totalMobCount = 0;
            foreach (Mob mob in Waves[_currentWaveIndex].Mobs)
            {
                StartCoroutine(SpawnMob(mob));
            }
            _livingMobCount = _totalMobCount;
            InvokeMobsUpdatedEvent();
        }

        private IEnumerator SpawnMob(Mob mob)
        {
            _totalMobCount += mob.Quantity;
            float elapsedSeconds = 0;
            float spawnIntervalSeconds = GetSpawnIntervalSeconds(mob);
            while (elapsedSeconds < mob.SpawnWindowSeconds)
            {
                SpawnBatch(mob);
                elapsedSeconds += spawnIntervalSeconds;
                yield return new WaitForSeconds(spawnIntervalSeconds);
            }
        }

        private float GetSpawnIntervalSeconds(Mob mob)
        {
            float intervals = (float)mob.Quantity / mob.BatchSize;
            return mob.SpawnWindowSeconds / intervals;
        }

        private void SpawnBatch(Mob mob)
        {
            for (int i = 0; i < mob.BatchSize; i++)
            {
                Instantiate(mob.Prefab, GetRandomSpawnPoint(mob), Quaternion.identity);
            }
        }

        private Vector3 GetRandomSpawnPoint(Mob mob)
        {
            if (mob.SpawnLocations.Length > 0)
            {
                return mob.SpawnLocations[UnityEngine.Random.Range(0, mob.SpawnLocations.Length - 1)].position;
            }
            else
            {
                return _genericSpawnLocations[UnityEngine.Random.Range(0, _genericSpawnLocations.Length - 1)].position;
            }
        }

        private void EndWave()
        {
            _waveInProgress = false;
            if (_currentWaveIndex == _waves.Length - 1) { SequenceComplete = true; }
            InvokeWaveCompleteEvent();
        }
        #endregion
    }
}
