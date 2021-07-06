using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// This class toggles building exterior visibility. When a player enters the
    /// trigger, exterior visibility is toggled off so that the player can see
    /// inside. When no more players are in the trigger, exterior visibility is
    /// toggled back on.
    public class IndoorTrigger : MonoBehaviour
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        /// <summary>
        /// Whether or not the building exterior meshes are enabled.
        /// </summary>
        private bool _exteriorEnabled = true;
        /// <summary>
        /// List of all the players contained by the building.
        /// </summary>
        private List<GameObject> _containedPlayers = new List<GameObject>();
        /// <summary>
        /// The GameObjects that make up the exterior of the building.
        /// </summary>
        [SerializeField] private List<GameObject> _exteriorObjects = new List<GameObject>();
        #endregion

        //=====================================================================
        #region MonoBehavior
        //=====================================================================
        private void OnTriggerEnter(Collider collider)
        {
            ProcessPlayerEntrance(collider);
        }

        private void OnTriggerExit(Collider collider)
        {
            ProcessPlayerExit(collider);
        }

        private void Start()
        {
            CheckMandatoryComponents();
        }
        #endregion

        //=====================================================================
        #region Initialization
        //=====================================================================
        private void CheckMandatoryComponents()
        {
            Assert.IsNotNull(_exteriorObjects, gameObject.name + " is missing _exteriorObjects");
        }
        #endregion

        //=====================================================================
        #region Toggling exterior visibility
        //=====================================================================
        private void ProcessPlayerEntrance(Collider collider)
        {
            if (collider.gameObject.CompareTag("Player") && !_containedPlayers.Contains(collider.gameObject))
            {
                _containedPlayers.Add(collider.gameObject);
                CheckToggleExteriorConditions();
            }
        }

        private void ProcessPlayerExit(Collider collider)
        {
            if (collider.gameObject.CompareTag("Player") && _containedPlayers.Contains(collider.gameObject))
            {
                _containedPlayers.Remove(collider.gameObject);
                CheckToggleExteriorConditions();
            }
        }

        private void EnableExteriorObjects(bool enableBool)
        {
            foreach (GameObject obj in _exteriorObjects)
            {
                MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
                if (meshRenderer != null)
                {
                    meshRenderer.enabled = enableBool;
                }
                _exteriorEnabled = enableBool;
            }
        }

        private void CheckToggleExteriorConditions()
        {
            if (_containedPlayers.Count == 0 && !_exteriorEnabled)
            {
                EnableExteriorObjects(true);
            }
            else if (_containedPlayers.Count > 0 && _exteriorEnabled)
            {
                EnableExteriorObjects(false);
            }
        }
        #endregion
    }
}
