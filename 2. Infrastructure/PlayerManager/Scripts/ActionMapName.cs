using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGS
{
    /// <summary>
    /// The ActionMaps (i.e. the controll mappings) available for use.
    /// </summary>
    public static class ActionMapName
    {
        /// <summary>
        /// Action map for leader control changes. Player has no controls. This
        /// allows only 1 person to control a menu at a time.
        /// </summary>
        public const string BRICKED = "Bricked";
        /// <summary>
        /// Action map for dialogue. Player can only pause and continue dialogue.
        /// </summary>
        public const string DIALOGUE = "Dialogue";
        /// <summary>
        /// Action map for incapacitation and death. Players can only pause.
        /// </summary>
        public const string PARALYZED = "Paralyzed";
        /// <summary>
        /// Action map for default player movement.
        /// </summary>
        public const string PLAYER = "Player";
        /// <summary>
        /// Action map used to control UI.
        /// </summary>
        public const string UI = "UI";
    }
}
