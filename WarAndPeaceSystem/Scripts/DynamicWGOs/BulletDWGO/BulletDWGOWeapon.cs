using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// Instance of Piercing DWGO Weapon that spawns BulletDWGOs.
    /// </summary>
    public class BulletDWGOWeapon : APiercingDWGOWeapon, IBulletDWGO, IAmmoWeapon
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        // [Header("IBulletDWGO variables")]
        /// <summary>
        /// The LayerMask that the raycast can detect. To set the LayerMask to
        /// detect everything, initialize as -1. In order for DWGO to work, the
        /// 'Ignore Raycast' layermask must be deselected through the inspector.
        /// </summary>
        [SerializeField] protected LayerMask layerMask = -1;
        /// <summary>
        /// The maximum distance that the DWGO raycast will travel.
        /// </summary>
        [SerializeField] protected float maxRaycastDistance = 100f;
        /// <summary>
        /// The seconds between raycasts used to collect reflection information.
        /// </summary>
        [SerializeField] protected float raycastRefreshSeconds = 0.1f;
        /// <summary>
        /// The number of times the DWGO can ricochet off of objects in the
        /// environment.
        /// </summary>
        [SerializeField] protected int ricochetCount = 0;

        //=====================================================================
        // [Header("IAmmoWeapon variables")]
        /// <summary>
        /// Boolean indicating whether weapon is currently reloading.
        /// </summary>
        public bool isReloading = false;
        /// <summary>
        /// The amount ammo in a magazine.
        /// </summary>
        [SerializeField] protected uint magCapacity = 0;
        /// <summary>
        /// The amount of ammo in your reserve.
        /// </summary>
        [SerializeField] protected uint totalAmmo = 0;
        /// <summary>
        /// The amount of times you can fire before a reload.
        /// 0 means bottomless mag
        /// </summary>
        [SerializeField] protected uint maxMagCapacity = 0;
        /// <summary>
        /// The maximum amount of ammo you can carry.
        /// 0 means infinite ammo
        /// </summary>
        [SerializeField] protected uint maxTotalAmmo = 0;
        /// <summary>
        /// The time it takes to reload your weapon.
        /// </summary>
        [SerializeField] protected float reloadSeconds = 0;

        //=====================================================================
        // [Header("BulletDWGOWeapon variables")]
        /// <summary>
        /// The bullet DWGO prefab instantiated by this weapon.
        /// </summary>
        [SerializeField] protected GameObject bulletPrefab = default;
        #endregion

        //=====================================================================
        #region Properties
        //=====================================================================
        // [Header("IBulletDWGO properties")]
        /// <summary>
        /// The LayerMask that the raycast can detect. To set the LayerMask to
        /// detect everything, initialize as -1. In order for DWGO to work, the
        /// 'Ignore Raycast' layermask must be deselected through the inspector.
        /// </summary>
        public LayerMask LayerMask
        {
            get => layerMask;
            set => layerMask = value;
        }
        /// <summary>
        /// The maximum distance that the DWGO raycast will travel.
        /// </summary>
        public float MaxRaycastDistance
        {
            get => maxRaycastDistance;
            set => maxRaycastDistance = value;
        }
        /// <summary>
        /// The seconds between raycasts used to collect reflection information.
        /// </summary>
        public float RaycastRefreshSeconds
        {
            get => raycastRefreshSeconds;
            set => raycastRefreshSeconds = value;
        }
        /// <summary>
        /// The number of times the DWGO can ricochet off of objects in the
        /// environment.
        /// </summary>
        public int RicochetCount
        {
            get => ricochetCount;
            set => ricochetCount = value;
        }
        //=====================================================================
        // [Header("IAmmoWeapon properties")]
        /// <summary>
        /// Boolean indicating whether weapon is empty.
        /// </summary>
        public bool OutOfAmmo
        {
            get => (MaxMagCapacity != 0 && MagCapacity == 0);
        }
        public uint MagCapacity
        {
            get => magCapacity;
            set => magCapacity = value;
        }
        /// <summary>
        /// The amount of ammo in your reserve.
        /// </summary>
        public uint TotalAmmo
        {
            get => totalAmmo;
            set => totalAmmo = value;
        }
        /// <summary>
        /// The amount of times you can fire before a reload.
        /// 0 means bottomless mag
        /// </summary>
        public uint MaxMagCapacity
        {
            get => maxMagCapacity;
            set => maxMagCapacity = value;
        }
        /// <summary>
        /// The maximum amount of ammo you can carry.
        /// 0 means infinite ammo
        /// </summary>
        public uint MaxTotalAmmo
        {
            get => maxTotalAmmo;
            set => maxTotalAmmo = value;
        }
        /// <summary>
        /// The amount of time reloading takes in seconds.
        /// </summary>
        public float ReloadSeconds
        {
            get => reloadSeconds;
            set => ReloadSeconds = value;
        }
        #endregion

        //=====================================================================
        #region Public methods
        //=====================================================================
        /// <summary>
        /// Instantiates and initilaizes a self-regulating BulletDWGO prefab in
        /// front of the attacker in the attack direction.
        /// </summary>
        /// <param name="direction">
        /// The direction that the DWGO will fly.
        /// </param>
        /// <remarks>
        /// Currently players can shoot through walls by instantiating projectiles
        /// inside of the wall. This will need to be fixed if it ends up being a
        /// problem. See Physics.CheckSphere.
        /// </remarks>
        public override void Attack(Vector3 direction)
        {
            if (onCooldown || isReloading) { return; }
            // TODO: Find more elegant way to set infinite ammo
            if (OutOfAmmo)
            {
                Reload();
                return;
            }
            if (this.CooldownSeconds > 0) { StartCoroutine(base.Cooldown()); }
            GameObject bulletDWGO = Instantiate(bulletPrefab,
                this.Attacker.transform.position + direction * this.SpawnOffsetDistance,
                Quaternion.identity);
            bulletDWGO.GetComponent<BulletDWGO>().Init(this, this.Attacker, direction);
            MagCapacity--;
            EventManager.Instance.Invoke(EventName.WeaponAmmoEvent,
                new WeaponAmmoEventArgs(this.MagCapacity, this.TotalAmmo),
                this);
        }

        /// <summary>
        /// Starts the coroutine to process a reload.
        /// </summary>
        public virtual void Reload()
        {
            // check if reload can be performed
            if (MaxMagCapacity != 0 && (TotalAmmo != 0 || MaxTotalAmmo == 0) && !this.isReloading)
            {
                this.isReloading = true;
                StartCoroutine(StartReload());
            }
        }

        public virtual IEnumerator StartReload()
        {
            yield return new WaitForSeconds(reloadSeconds);

            if (MaxTotalAmmo != 0)
            {
                uint difference = MaxMagCapacity - MagCapacity;
                uint reloading = Math.Min(difference, TotalAmmo);
                MagCapacity += reloading;
                TotalAmmo -= reloading;
            }
            else
            {
                MagCapacity = MaxMagCapacity;
            }
            EventManager.Instance.Invoke(EventName.WeaponAmmoEvent,
                new WeaponAmmoEventArgs(this.MagCapacity, this.TotalAmmo),
                this);
            this.isReloading = false;
        }
        #endregion

        //=====================================================================
        #region Protected methods
        //=====================================================================
        /// <summary>
        /// Initialises the component in Start().
        /// </summary>
        protected override void InitOnStart()
        {
            base.InitOnStart();
        }

        /// <summary>
        /// Ensures mandatory components are accounted for.
        /// </summary>
        protected override void CheckMandatoryComponents()
        {
            base.CheckMandatoryComponents();
            Assert.IsNotNull(bulletPrefab, gameObject.name + " is missing bulletPrefab");
        }
        #endregion
    }
}
