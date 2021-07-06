using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using TMPro;

namespace EGS
{
    /// <summary>
    /// This class is used to control player UI weapon picture and ammo counter
    /// </summary>
    public class UIWeaponInfo : MonoBehaviour
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        /// <summary>
        /// The ammo counter text.
        /// </summary>
        private TextMeshProUGUI ammoCount;
        /// <summary>
        /// The weapon image displayed on UI.
        /// </summary>
        private Image weaponUI;
        #endregion

        //=====================================================================
        #region Properties
        //=====================================================================
        /// <summary>
        /// The WeaponSelector to get weapon updates.
        /// </summary>
        public WeaponSelector WeaponSelector { get; set; } = default;
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

        private void OnEnable()
        {
            EventManager.Instance.AddListener(EventName.WeaponSwitchEvent, HandleWeaponSwitchEvent);
            EventManager.Instance.AddListener(EventName.WeaponAmmoEvent, HandleWeaponAmmoEvent);
        }

        private void OnDisable()
        {
            EventManager.Instance.RemoveListener(EventName.WeaponSwitchEvent, HandleWeaponSwitchEvent);
            EventManager.Instance.RemoveListener(EventName.WeaponAmmoEvent, HandleWeaponAmmoEvent);
        }
        #endregion

        //=====================================================================
        #region Event handlers
        //=====================================================================
        /// <summary>
        /// Called after a WeaponSwitchEvent is invoked.
        /// </summary>
        /// <param name="invoker">The event invoker.</param>
        /// <param name="e">The event arguments/message.</param>
        private void HandleWeaponSwitchEvent(object invoker, EventArgs e)
        {
            //change weapon picture and add/remove ammo count
            if (invoker == WeaponSelector)
            {
                WeaponSwitchEventArgs args = (WeaponSwitchEventArgs)e;
                AWeapon currentWeapon = WeaponSelector.SelectedWeapon;
                weaponUI.overrideSprite = currentWeapon.UIElement;
                // TODO: Find more elegant way to do below
                IAmmoWeapon ammoWeapon = currentWeapon.GetComponent<IAmmoWeapon>();
                if (ammoWeapon != null)
                {
                    if (ammoWeapon.TotalAmmo != 0)
                    {
                        ammoCount.text = ammoWeapon.MagCapacity.ToString() + " / " + ammoWeapon.TotalAmmo.ToString();
                    }
                    else
                    {
                        ammoCount.text = ammoWeapon.MagCapacity.ToString();
                    }
                }
                else
                {
                    ammoCount.text = "";
                }
            }
        }
        /// <summary>
        /// Called after a WeaponAmmoEvent is invoked.
        /// </summary>
        /// <param name="invoker">The event invoker.</param>
        /// <param name="e">The event arguments/message.</param>
        private void HandleWeaponAmmoEvent(object invoker, EventArgs e)
        {
            //change weapon ammo count
            if (invoker == WeaponSelector.SelectedWeapon)
            {
                WeaponAmmoEventArgs args = (WeaponAmmoEventArgs)e;
                if (args.remainingAmmo != 0)
                {
                    ammoCount.text = args.magAmmo.ToString() + " / " + args.remainingAmmo.ToString();
                }
                else
                {
                    ammoCount.text = args.magAmmo.ToString();
                }
            }
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
            CheckMandatoryComponents();
        }

        /// <summary>
        /// Sources and initializes component variables.
        /// </summary>
        private void InitVars()
        {
            //set weapon picture and ammo count
            ammoCount = this.gameObject.GetComponentInChildren<TextMeshProUGUI>();
            weaponUI = this.gameObject.GetComponentInChildren<Image>();
        }

        /// <summary>
        /// Checks whether mandatory components are accounted for.
        /// </summary>
        private void CheckMandatoryComponents()
        {
            Assert.IsNotNull(WeaponSelector, gameObject.name + " is missing WeaponSelector");
        }
        #endregion
    }
}
