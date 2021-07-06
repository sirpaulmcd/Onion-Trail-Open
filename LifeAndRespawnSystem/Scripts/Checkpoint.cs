using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// This class is a checkpoint. When the checkpoint collider is touched, it
    /// updates the checkpoint respawn position in the LARSGameSession and respawns
    /// dead players.
    /// </summary>
    public class Checkpoint : MonoBehaviour
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        /// <summary>
        /// Height above checkpoint transform players are to spawn. This is used
        /// to prevent players from spawning inside the checkpoint object.
        /// </summary>
        [SerializeField] private float _heightOffset = 1f;
        /// <summary>
        /// Boolean that indicates whether this checkpoint is the initial spawn
        /// location for a scene. Set true to allow players to spawn here when the
        /// scene is loaded.
        /// </summary>
        [SerializeField] private bool _startingCheckpoint = false;
        #endregion

        //=====================================================================
        #region MonoBehaviour
        //=====================================================================
        /// <summary>
        /// Called before the first frame update.
        /// </summary>
        void Start()
        {
            InitOnStart();
        }

        /// <summary>
        /// Called when this collider/rigidbody has begun touching another
        /// rigidbody/collider.
        /// </summary>
        /// <param name="collision">
        /// Collision holding information about colliding object.
        /// </param>
        void OnCollisionEnter(Collision collision)
        {
            UpdateCheckpointAndRespawn(collision);
        }
        #endregion

        //=====================================================================
        #region Private methods
        //=====================================================================
        /// <summary>
        /// Initialises the component in Start().
        /// </summary>
        private void InitOnStart()
        {
            InitVars();
        }

        /// <summary>
        /// Sources and initializes component variables.
        /// </summary>
        private void InitVars()
        {
            // If starting checkpoint, teleport players to self
            if (_startingCheckpoint) { UpdateCheckpointAndTeleport(); }
        }

        /// <summary>
        /// Updates the checkpoint position in LARSGameSession to match this
        /// checkpoint. Heigh offset is used to ensure players do not spawn inside
        /// checkpoint GameObject.
        /// </summary>
        private void UpdateCheckpointPosition()
        {
            LARSGameSession.Instance.CheckpointPosition = this.transform.position +
                new Vector3(0, _heightOffset, 0);
        }

        /// <summary>
        /// If a player touches the checkpoint, updates checkpoint position and
        /// respawns dead players.
        /// </summary>
        /// <param name="other">
        /// Collision associated with the contacting GameObject.
        /// </param>
        private void UpdateCheckpointAndRespawn(Collision other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                UpdateCheckpointPosition();
                LARSGameSession.Instance.RespawnDeadPlayers();
            }
        }

        /// <summary>
        /// Updates the LARSGameSession checkpoint to match this checkpoint and
        /// teleports all living players to that location.
        /// </summary>
        private void UpdateCheckpointAndTeleport()
        {
            UpdateCheckpointPosition();
            LARSGameSession.Instance.TeleportLivingPlayersToCheckpoint();
        }
        #endregion
    }
}
