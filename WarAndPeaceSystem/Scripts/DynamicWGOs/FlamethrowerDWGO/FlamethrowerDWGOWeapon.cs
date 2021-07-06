using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// Instance of BulletDWGOWeapon that launches flame projectiles.
    /// </summary>
    public class FlamethrowerDWGOWeapon : BulletDWGOWeapon
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        /// <summary>
        /// The maximum spread the DWGO will have.
        /// </summary>
        [SerializeField] protected float spread = 0;
        #endregion

        //=====================================================================
        #region Properties
        //=====================================================================
        /// <summary>
        /// The maximum spread the DWGO will have.
        /// </summary>
        public float Spread
        {
            get => spread;
            set => spread = value;
        }
        #endregion

        //=====================================================================
        #region Attack action
        //=====================================================================
        /// <summary>
        /// Instantiates and initilaizes a self-regulating FlamethrowerDWGO prefab in
        /// front of the attacker in the attack direction plus a certain deviation.
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
            if (OutOfAmmo)
            {
                Reload();
                return;
            }
            if (this.CooldownSeconds > 0) { StartCoroutine(base.Cooldown()); }
            float offset = Spread * Mathf.Sin(2.0f * Mathf.PI * Time.time) / 180.0f;
            Vector3 newDirection = RotateVector(direction, offset);
            GameObject flamethrowerDWGO = Instantiate(bulletPrefab,
                this.Attacker.transform.position + newDirection * this.SpawnOffsetDistance,
                Quaternion.identity);
            flamethrowerDWGO.GetComponent<FlamethrowerDWGO>().Init(this, this.Attacker, newDirection);
            MagCapacity--;
            EventManager.Instance.Invoke(EventName.WeaponAmmoEvent,
                new WeaponAmmoEventArgs(this.MagCapacity, this.TotalAmmo),
                this);
        }

        /// <summary>
        /// Calculates a triangle wave.
        /// </summary>
        /// <param name=x>
        /// The value to calculate the wave at.
        /// </param>
        private float TriangleWave(float x)
        {
            return (Mathf.Asin(Mathf.Sin(x)));
        }

        /// <summary>
        /// Rotates a direction vector by a number of degrees about the y axis.
        /// </summary>
        /// <param name=vector>
        /// The vector to rotate.
        /// </param>
        /// <param name=x>
        /// The value in degrees to rotate the vector.
        /// </param>
        private Vector3 RotateVector(Vector3 vector, float x)
        {
            Vector3 rotated = new Vector3();
            rotated.x = Mathf.Cos(x) * vector.x - Mathf.Sin(x) * vector.z;
            rotated.y = vector.y;
            rotated.z = Mathf.Sin(x) * vector.x + Mathf.Cos(x) * vector.z;
            return Vector3.Normalize(rotated);
        }
        #endregion
    }
}
