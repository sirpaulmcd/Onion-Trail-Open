using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace EGS
{
    /// <summary>
    /// Instance of Health used for the mobs. Invokes MobDeathEvents.
    /// </summary>
    public class MobHealth : Health
    {
        //=====================================================================
        #region Event Invokers
        //=====================================================================
        private void InvokeMobDeathEvent()
        {
            Debug.Log("MobHealth.InvokeMobDeathEvent: Mob has died.");
            EventManager.Instance.Invoke(EventName.MobDeathEvent,
                new MobDeathEventArgs(),
                this);
        }
        #endregion

        //=====================================================================
        #region Death
        //=====================================================================
        /// <summary>
        /// To be called when the GameObject has died. Plays death effects, invokes
        /// death event, and toggles the death status effect for the player.
        /// </summary>
        protected override void ReactToDeath()
        {
            PlayDeathEffects();
            InvokeMobDeathEvent();
            Destroy(transform.gameObject);
        }
        #endregion
    }
}
