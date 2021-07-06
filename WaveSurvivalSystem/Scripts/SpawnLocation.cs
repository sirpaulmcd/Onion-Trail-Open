using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// Draws a small spherical gizmo for spawn location transforms.
    /// </summary>
    public class SpawnLocation : MonoBehaviour
    {
        //=====================================================================
        #region MonoBehaviour
        //=====================================================================
        private void OnDrawGizmos()
        {
            DrawGizmos();
        }
        #endregion

        //=====================================================================
        #region Gizmos
        //=====================================================================
        /// <summary>
        /// Draws gizmos for the patrol points and various detection ranges.
        /// </summary>
        private void DrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
        #endregion

    }
}
