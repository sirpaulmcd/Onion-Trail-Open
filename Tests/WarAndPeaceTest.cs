using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using EGS;

namespace Tests
{
    class FakeHealth: MonoBehaviour, IHealth
    {
        public int MaximumHealth { get; set; }
        public int CurrentHealth { get; set; }
        public bool IsDead 
        { 
            get { return CurrentHealth <= 0; }
        }
        public bool IsInvulnerable { get; set; }
        public void Hurt(int HP){ CurrentHealth -= HP; }
        public void Heal(int HP){ CurrentHealth += HP; }
    }

    public class WarAndPeaceTest
    {
        private GameObject _target;
        private FakeHealth _targetHealth;
        private GameObject _projectile;
        private WarAndPeace _projectileWap;
        private TimeSpan _collisionTime;
        private int _defaultHP = 100;


        [SetUp]
        public void Setup()
        {
            // Setup a target to attack
            // This is just a unit-cube at the origin.
            _target = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _target.transform.localScale = Vector3.one;
            _target.transform.position = Vector3.zero;
            _target.tag = "Player"; // This MUST be different from the projectile's tag
            
            Rigidbody targetRigidbody = _target.AddComponent<Rigidbody>();
            targetRigidbody.useGravity = false;

            _targetHealth = _target.AddComponent<FakeHealth>();
            _targetHealth.MaximumHealth = _defaultHP;
            _targetHealth.CurrentHealth = _defaultHP;

            // Setup a projectile object
            // It is a unit-sphere which is two units to the right of the target
            // and is flying towards the target.
            _projectile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _projectile.transform.localScale = Vector3.one; 
            _projectile.transform.position = 2 * Vector3.right;
            _projectile.tag = "Untagged"; 

            Rigidbody projectileRigidbody = _projectile.AddComponent<Rigidbody>();
            projectileRigidbody.useGravity = false;
            projectileRigidbody.velocity = Vector3.left; 
            
            _projectileWap = _projectile.AddComponent<WarAndPeace>();
            _projectileWap.attacker = _projectile;  // Note: This would usually be an enemy.
            _projectileWap.criticalHitMultiplier = 1;
            _projectileWap.criticalHitChance = 0;
            _projectileWap.heal = false;
            _projectileWap.knockback = 1.0f;
            _projectileWap.knockbackOrigin = _projectile;
            _projectileWap.minimumDamage = 10;
            _projectileWap.maximumDamage = 50;
            _projectileWap.name = "projectile";

            // The projectile's center is 2m away from the object's center.
            // The object's exterior is 0.5m away from the center.
            // The project's exterior is 0.5m away from the center.
            // The projectile is flying at 1m/s.
            // Therefore, most of our tests need at least 1 sec of wait-time. 
            // We add an additional 50ms in case Unity is running slow.
            _collisionTime = new TimeSpan(0, 0, 0, 1, 50);
        }

        [TearDown]
        public void Teardown()
        {
            UnityEngine.Object.Destroy(_target);
            UnityEngine.Object.Destroy(_projectile);
        }

        [UnityTest]
        public IEnumerator Attack()
        {                            
            yield return new WaitForSeconds((float)_collisionTime.TotalSeconds);
            Assert.LessOrEqual(_targetHealth.CurrentHealth,
                               _targetHealth.MaximumHealth - _projectileWap.minimumDamage);
            Assert.GreaterOrEqual(_targetHealth.CurrentHealth,
                                  _targetHealth.MaximumHealth - _projectileWap.maximumDamage);
            // Check that the attack is now gone.
            // We use ... == null, rather is IsNull() since destroyed Unity 
            // objects aren't really null, but rather hidden with an overloaded
            // == operator. 
            Assert.IsTrue(_projectile == null, "Projectile should have been destroyed");
        }

        [UnityTest]
        public IEnumerator CriticalHitAttack()
        {                            
            // Turn on critical hits.
            _projectileWap.criticalHitChance = 100;
            _projectileWap.criticalHitMultiplier = 2;
            // Turn off damage randomness
            _projectileWap.minimumDamage = 45;
            _projectileWap.maximumDamage = 45;

            yield return new WaitForSeconds((float)_collisionTime.TotalSeconds);
            Assert.AreEqual(_targetHealth.MaximumHealth - 90,
                            _targetHealth.CurrentHealth);
        }   

        [UnityTest]
        public IEnumerator CriticalHitHeal()
        {                            
            // Turn on critical hits.
            _projectileWap.criticalHitChance = 100;
            _projectileWap.criticalHitMultiplier = 2;
            // Turn off damage randomness
            _projectileWap.minimumDamage = 45;
            _projectileWap.maximumDamage = 45;
            // Turn on healing
            _projectileWap.heal = true;
            _projectile.tag = "Player"; // Can only heal friendly units.

            yield return new WaitForSeconds((float)_collisionTime.TotalSeconds);
            Assert.AreEqual(_targetHealth.MaximumHealth + 90,
                            _targetHealth.CurrentHealth);
        }

        [UnityTest]
        public IEnumerator Heal()
        {                            
            // Turn on healing
            _projectileWap.heal = true;
            _projectile.tag = "Player"; // Can only heal friendly units.

            yield return new WaitForSeconds((float)_collisionTime.TotalSeconds);
            Assert.GreaterOrEqual(_targetHealth.CurrentHealth,
                                  _targetHealth.MaximumHealth + _projectileWap.minimumDamage);
            Assert.LessOrEqual(_targetHealth.CurrentHealth,
                               _targetHealth.MaximumHealth + _projectileWap.maximumDamage);
        }

        [UnityTest]
        public IEnumerator Knockback()
        {                            
            yield return new WaitForSeconds((float)_collisionTime.TotalSeconds);
            // The projectile is moving towards the left & hits the _target with
            // a knockback of 1.0. This should move the _target (at the origin)
            // to Vector3.zero + Vector3.left == Vector3.left.
            AssertVector3Equality(Vector3.left, _target.transform.position);
        }

        [UnityTest]
        public IEnumerator HealsDontKnockback()
        {
            // Turn on healing
            _projectileWap.heal = true;
            _projectile.tag = "Player"; // Can only heal friendly units.

            yield return new WaitForSeconds((float)_collisionTime.TotalSeconds);
            AssertVector3Equality(Vector3.zero, _target.transform.position);
        }

        [UnityTest]
        public IEnumerator AttacksDontHitAllies()
        {
            // Make the attack have a friendly tag.
            _projectile.tag = "Player";

            yield return new WaitForSeconds((float)_collisionTime.TotalSeconds);
            // There should be no knockback, damage done, and the projectile 
            // should still exist.
            AssertVector3Equality(Vector3.zero, _target.transform.position);
            Assert.AreEqual(_defaultHP, _targetHealth.CurrentHealth);
            Assert.IsTrue(_projectile != null, "Projectile shouldn't be destroyed.");
        }

        [UnityTest]
        public IEnumerator AdjustableKnockback()
        {
            _projectileWap.knockback = 2;

            yield return new WaitForSeconds((float)_collisionTime.TotalSeconds);

            AssertVector3Equality(2*Vector3.left, _target.transform.position);
        }

        //======================================================================
        // Helper Methods
        //======================================================================
        ///<summary>
        /// Asserts approximate equality between two Vector3s.
        ///</summary>
        ///<remarks>
        /// The precision of this assertion is pretty forgiving. Unity is going 
        /// to move our GameObjects around slightly to deal with collisions, so
        /// we aren't gunning for ultra-precision here. 
        ///</remarks>
        private void AssertVector3Equality(Vector3 expected, Vector3 actual)
        {
            float precision = 0.1f;
            Assert.IsTrue(Mathf.Abs(expected.x - actual.x) < precision, 
                          String.Format("Expected x: {0}. Actual x: {1}",
                                        expected.x, actual.x));
            Assert.IsTrue(Mathf.Abs(expected.y - actual.y) < precision, 
                          String.Format("Expected y: {0}. Actual y: {1}",
                                        expected.y, actual.y));
            Assert.IsTrue(Mathf.Abs(expected.z - actual.z) < precision, 
                          String.Format("Expected z: {0}. Actual z: {1}",
                                        expected.z, actual.z));
        }
    }
}

