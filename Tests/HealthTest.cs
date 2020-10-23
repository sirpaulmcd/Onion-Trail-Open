// using System;
// using System.Collections;
// using System.Collections.Generic;
// using NUnit.Framework;
// using UnityEngine;
// using UnityEngine.TestTools;

// namespace Tests
// {
//     public class HealthTest
//     {
//         // Instance Variables
//         private IHealth health;

//         [SetUp]
//         public void Setup()
//         {
//             GameObject healthObject = new GameObject();
//             healthObject.AddComponent<Health>();

//             health = healthObject.GetComponent<Health>();
//         }

//         [Test]
//         public void HealthTestGetMaximumHealth()
//         {
//             Assert.AreEqual(health.maximumHealth,
//                             Health.DEFAULT_INITIALIZED_HEALTH);
//         }

//         [Test]
//         public void HealthTestGetDefaultCurrentHealth()
//         {
//             Assert.AreEqual(health.currentHealth,
//                             Health.DEFAULT_INITIALIZED_HEALTH);
//         }

//         [Test]
//         public void HealthTestCurrentAndMaximumHealthInitializedAsEqual()
//         {
//             Assert.AreEqual(health.currentHealth, health.maximumHealth);
//         }

//         [Test]
//         public void HealthTestDealOneDamage()
//         {
//             int damage = 1;
//             health.Hurt(damage);
//             Assert.AreEqual(health.currentHealth,
//                             Health.DEFAULT_INITIALIZED_HEALTH - damage);
//         }

//         [Test]
//         public void HealthTestDeathByOneDamageHits()
//         {
//             int damage = 1;
//             int totalDamage = 0;
//             for(int i=0; i < Health.DEFAULT_INITIALIZED_HEALTH; i++)
//             {
//                 health.Hurt(damage);
//                 totalDamage += damage;

//                 Assert.AreEqual(health.currentHealth,
//                                 Health.DEFAULT_INITIALIZED_HEALTH-totalDamage);
//             }
//             Assert.IsTrue(health.IsDead());
//         }

//         [Test]
//         public void HealthTestMaximumDamageDealt()
//         {
//             Assert.AreNotEqual(Health.DEFAULT_INITIALIZED_HEALTH, int.MaxValue);
//             health.Hurt(int.MaxValue);
//             Assert.Zero(health.currentHealth);
//         }

//         [Test]
//         public void HealthTestDamageOneThenHealOne()
//         {
//             int damage = 1;
//             health.Hurt(damage);
//             health.Heal(damage);
//             Assert.AreEqual(health.currentHealth,
//                             Health.DEFAULT_INITIALIZED_HEALTH);
//         }

//         [Test]
//         public void HealthTestHealOneThenDamageOne()
//         {
//             int damage = 1;
//             health.Heal(damage);
//             health.Hurt(damage);
//             Assert.AreEqual(health.currentHealth,
//                             Health.DEFAULT_INITIALIZED_HEALTH - damage);
//         }

//         [Test]
//         public void HealthTestOverheal()
//         {
//             int heal = 1;
//             health.Heal(heal);
//             Assert.AreEqual(health.currentHealth,
//                             Health.DEFAULT_INITIALIZED_HEALTH);
//         }

//         [Test]
//         public void HealthTestHealMaximum()
//         {
//             health.Hurt(health.currentHealth);
//             health.Heal(int.MaxValue);
//             Assert.AreEqual(health.currentHealth,
//                             Health.DEFAULT_INITIALIZED_HEALTH);
//         }

//         [Test]
//         public void HealthTestDeath()
//         {
//             health.Hurt(health.currentHealth);
//             Assert.IsTrue(health.IsDead());
//         }

//         [Test]
//         public void HealthTestHealthUpdatedEvent()
//         {
//             int healthChangeCounter = 0;

//             void HealthUpdatedEventHandler(object sender, EventArgs e)
//             {
//                 HealthUpdatedEventArgs healthArgs = (HealthUpdatedEventArgs) e;
//                 healthChangeCounter += 1;
//                 Assert.AreNotEqual(healthArgs.previousHealth,
//                                    healthArgs.newHealth);
//             }

//             int damage = 1;

//             // Register the event
//             health.HealthUpdated += HealthUpdatedEventHandler;

//             // No health change should occur if overhealing
//             health.Heal(damage);

//             // Two health changes
//             health.Hurt(damage);
//             health.Heal(damage);
//             Assert.AreEqual(healthChangeCounter, 2);

//             // Unregister the event (prevent memory leak)
//             health.HealthUpdated -= HealthUpdatedEventHandler;
//         }

//         [Test]
//         public void HealthTestDeathEvent()
//         {
//             int deathCounter = 0;

//             void DeathEventHandler(object sender, EventArgs e)
//             {
//                 deathCounter += 1;
//             }

//             int damage = health.currentHealth;

//             // Register the event
//             health.Death += DeathEventHandler;

//             // Kill and check that the event happened
//             health.Hurt(damage);
//             Assert.AreEqual(deathCounter, 1);

//             // Unregister the event (prevent memory leak)
//             health.Death -= DeathEventHandler;
//         }

//     }
// }
