using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Audio;

namespace EGS
{
    /// <summary>
    /// Responsible for managing audio sources and playing audio.
    /// </summary>
    public class AudioManager : Singleton<AudioManager> //TODO: Should extend singleton
    {
        //TODO: Implement the gamestate handling
        //=========================================================================
        #region Instance variables
        //=========================================================================
        private AudioClipManager _audioClipManager;
        #endregion

        //=========================================================================
        #region MonoBehaviour
        //=========================================================================
        private void OnEnable()
        {
            RegisterAsListener();
        }

        private void OnDisable()
        {
            DeregisterAsListener();
        }
        #endregion
        //=========================================================================
        #region Debug and Testing         //TODO: Remove this. This is only for testing purposes.
        //=========================================================================
        public bool MusicAuxIn, MusicIn, MusicOut, AmbienceAuxIn, AmbienceIn, AmbienceOut, Event, SFX, ChangeTrack, LoadAmbience, LoadTrack;
        public void HandleDebug()
        {
            if (MusicAuxIn)
            {
                TransitionMusicToAuxIn(0.5f);
                MusicAuxIn = false;
            }
            if (MusicIn)
            {
                TransitionMusicToIn(0.5f);
                MusicIn = false;
            }
            if (MusicOut)
            {
                TransitionMusicToOut(0.5f);
                MusicOut = false;
            }

            if (AmbienceIn)
            {
                TransitionAmbientToIn(0.5f);
                AmbienceIn = false;
            }
            if (AmbienceAuxIn)
            {
                TransitionAmbientToAuxIn(0.5f);
                AmbienceAuxIn = false;
            }
            if (AmbienceOut)
            {
                TransitionAmbientToOut(0.5f);
                AmbienceOut = false;
            }
            if (Event)
            {
                PlayEvent(EventClipName.Victory);
                Event = false;
            }
            if (SFX)
            {
                PlaySFX(SFXClipName.Bomb, 0.5f);
                SFX = false;
            }
            if (LoadTrack)
            {
                PlayMusic(MusicClipName.SwordShovelsCave);
                LoadTrack = false;
            }
            if (LoadAmbience)
            {
                PlayAmbient(AmbientClipName.Rainforest);
                LoadAmbience = false;
            }
            if (ChangeTrack)
            {
                PlayMusic(MusicClipName.Scifi);
                ChangeTrack = false;
            }
        }
        #endregion
        //=========================================================================
        #region Event listeners
        //=========================================================================
        private void RegisterAsListener()
        {
            EventManager.Instance.AddListener(EventName.GameStateUpdatedEvent, HandleGameStateUpdatedEvent);
        }

        private void DeregisterAsListener()
        {
            EventManager.Instance.RemoveListener(EventName.GameStateUpdatedEvent, HandleGameStateUpdatedEvent);
        }

        private void HandleGameStateUpdatedEvent(object invoker, System.EventArgs e)
        {
            GameStateUpdatedEventArgs args = (GameStateUpdatedEventArgs) e;
            if (args.NewState == GameManager.GameState.PAUSED)
            {
                // TODO update audio when game paused
            }
        }
        #endregion

        //=========================================================================
        #region Music and Event audio
        //=========================================================================
        [SerializeField] private AudioSource _musicAudioSource = default, _musicAuxAudioSource = default, _eventAudioSource = default;
        [SerializeField] private AudioMixerSnapshot _musicOutSnapshot = default, _musicInSnapshot = default, _musicAuxInSnapshot = default, _eventSnapshot = default;
        private bool _eventRunning = false, _musicAuxIn = false;
        /// <summary>
        /// Transitions the main music mixer to the AuxIn Snapshot.
        /// </summary>
        /// <param name="transitionTime"> The time in seconds to take to transition.</param>
        public void TransitionMusicToAuxIn(float transitionTime)
        {
            _musicAuxIn = true;
            if (!_eventRunning)
            { _musicAuxInSnapshot.TransitionTo(transitionTime); }
        }
        /// <summary>
        /// Transitions the main music mixer to the In (Only main and no aux) Snapshot.
        /// </summary>
        /// <param name="transitionTime">The time in seconds to take to transition.</param>
        public void TransitionMusicToIn(float transitionTime)
        {
            _musicAuxIn = false;
            if (!_eventRunning)
            { _musicInSnapshot.TransitionTo(transitionTime); }
        }
        /// <summary>
        /// Transitions the main music mixer to the Out (No audio) Snapshot.
        /// </summary>
        /// <param name="transitionTime">The time in seconds to take to transition.</param>
        public void TransitionMusicToOut(float transitionTime)
        {
            _musicAuxIn = false;
            if (!_eventRunning)
            { _musicOutSnapshot.TransitionTo(transitionTime); }
        }
        /// <summary>
        /// Changes the music audio to the relevant clips and plays.
        /// </summary>
        /// <param name="clipName"> The musicclipname that has the audio to be played.</param>
        public void PlayMusic(MusicClipName clipName)
        {
            ChangeMusic(clipName);
            _musicAudioSource.Play();
            if (_musicAuxAudioSource.clip)
            { _musicAuxAudioSource.Play(); }
        }

        private void ChangeMusic(MusicClipName clipName)
        {
            AudioClip[] musicPair = _audioClipManager.GetMusicClipPair(clipName);
            _musicAudioSource.clip = musicPair[0];
            if (musicPair.Length > 1)
            { _musicAuxAudioSource.clip = musicPair[1]; }
            else
            { _musicAuxAudioSource.clip = null; }
        }
        /// <summary>
        /// Changes the event audio to the relevant clip and plays it.
        /// </summary>
        /// <param name="clipName">The eventclipname that has the audio to be played.</param>
        public void PlayEvent(EventClipName clipName)
        {
            ChangeEvent(clipName);
            StartCoroutine(PlayEventMusic());
        }

        private IEnumerator PlayEventMusic()
        {
            _eventRunning = true;
            TransitionMusicToEvent(0.5f);
            yield return new WaitForSeconds(0.3f);
            _eventAudioSource.Play();
            while (_eventAudioSource.isPlaying)
            {
                yield return null;
            }
            yield return new WaitForSeconds(0.5f);
            _eventRunning = false;
            if (_musicAuxIn)
            {
                TransitionMusicToAuxIn(0.5f);
            }
            else
            {
                TransitionMusicToIn(0.5f);
            }
            yield break;
        }
        private void TransitionMusicToEvent(float transitionTime)
        {
            _eventSnapshot.TransitionTo(transitionTime);
        }

        private void ChangeEvent(EventClipName clipName)
        {
            AudioClip eventClip = _audioClipManager.GetEventClip(clipName);
            _eventAudioSource.clip = eventClip;
        }
        #endregion
        //=========================================================================
        #region Ambient audio
        //=========================================================================
        [SerializeField] private AudioSource _ambientAudioSource = default, _ambientAuxAudioSource = default;
        [SerializeField] private AudioMixerSnapshot _ambientOutSnapshot = default, _ambientInSnapshot = default, _ambientAuxInSnapshot = default;
        /// <summary>
        /// Transitions the ambient music mixer to the In (Only ambient no aux) Snapshot.
        /// </summary>
        /// <param name="transitionTime">The time in seconds to take to transition.</param>
        public void TransitionAmbientToIn(float transitionTime)
        {
            _ambientInSnapshot.TransitionTo(transitionTime);
        }
        /// <summary>
        /// Transitions the ambient music mixer to the AuxIn (Ambient with Aux) Snapshot.
        /// </summary>
        /// <param name="transitionTime">The time in seconds to take to transition.</param>
        public void TransitionAmbientToAuxIn(float transitionTime)
        {
            _ambientAuxInSnapshot.TransitionTo(transitionTime);
        }
        /// <summary>
        /// Transitions the ambient music mixer to the Out (No Audio) Snapshot.
        /// </summary>
        /// <param name="transitionTime">The time in seconds to take to transition.</param>
        public void TransitionAmbientToOut(float transitionTime)
        {
            _ambientOutSnapshot.TransitionTo(transitionTime);
        }
        /// <summary>
        /// Changes the ambient audio to the relevant clips and plays.
        /// </summary>
        /// <param name="clipName"> The ambientclipname that has the audio to be played.</param>
        public void PlayAmbient(AmbientClipName clipName)
        {
            ChangeAmbient(clipName);
            _ambientAudioSource.Play();
            if (_ambientAuxAudioSource.clip)
            { _ambientAuxAudioSource.Play(); }
        }

        private void ChangeAmbient(AmbientClipName clipName)
        {
            AudioClip[] ambientPair = _audioClipManager.GetAmbientClipPair(clipName);
            _ambientAudioSource.clip = ambientPair[0];
            if (ambientPair.Length > 1)
            { _ambientAuxAudioSource.clip = ambientPair[1]; }
            else
            {
                _ambientAuxAudioSource.clip = null;
            }
        }
        #endregion
        //=========================================================================
        #region SFX audio
        //=========================================================================
        [SerializeField] private AudioSource _sfxAudioSource = default;
        /// <summary>
        /// Plays a single shot of the SFX at a specific volume.
        /// </summary>
        /// <param name="clipName"> The sfxclipname that has the audio to be played.</param>
        public void PlaySFX(SFXClipName clipName, float volumeScale)
        {
            _sfxAudioSource.PlayOneShot(_audioClipManager.GetSFXClip(clipName), volumeScale);
        }
        #endregion
        //=========================================================================
        #region MonoBehavior
        //=========================================================================
        private void Start()
        {
            Init();
        }

        private void Update()
        {
            //TODO: Remove this. This is only for testing purposes.
            HandleDebug();
        }
        #endregion
        //=========================================================================
        #region Initialization
        //=========================================================================
        private void Init()
        {
            _audioClipManager = new AudioClipManager();
            CheckMandatoryComponents();
        }

        private void CheckMandatoryComponents()
        {
            Assert.IsNotNull(_musicAudioSource, gameObject.name + " is missing _musicAudioSource");
            Assert.IsNotNull(_musicAuxAudioSource, gameObject.name + " is missing _musicAuxAudioSource");
            Assert.IsNotNull(_eventAudioSource, gameObject.name + " is missing _eventAudioSource");
            Assert.IsNotNull(_ambientAudioSource, gameObject.name + " is missing _ambientAudioSource");
            Assert.IsNotNull(_ambientAuxAudioSource, gameObject.name + " is missing _ambientAuxAudioSource");
            Assert.IsNotNull(_sfxAudioSource, gameObject.name + " is missing _sfxAudioSource");
        }
        #endregion
    }
}