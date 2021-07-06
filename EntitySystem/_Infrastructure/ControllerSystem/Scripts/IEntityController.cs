using System;

namespace EGS
{
    /// <summary>
    /// Interface that dictates fundamental entity properties.
    /// </summary>
    public interface IEntityController
    {
        //=====================================================================
        #region Properties
        //=====================================================================
        // [Header("Health point manipulation")]
        /// <summary>
        /// Whether or not the controller allows movement.
        /// </summary>
        bool FreezeMovement { get; set; }
        /// <summary>
        /// The movement speed of the entity.
        /// </summary>
        float MoveSpeed { get; set; }
        /// <summary>
        /// The EntityStats component of the entity.
        /// </summary>
        EntityStats EntityStats { get; }
        #endregion
    }
}
