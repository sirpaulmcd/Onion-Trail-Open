// using System;
// using System.Collections;
// using System.Collections.Generic;
// using NUnit.Framework;
// using UnityEngine;
// using UnityEngine.TestTools;

// namespace Tests
// {
//     class StubHealthUpdatedEventArgs : AHealthUpdatedEventArgs
//     {
//         public override int previousHealth { get; }
//         public override int newHealth { get; }
//         public override int maxHealth { get; }

//         public StubHealthUpdatedEventArgs(int previousHealth, int newHealth)
//         {
//             this.previousHealth = previousHealth;
//             this.newHealth = newHealth;
//         }
//     }

//     class StubDeathEventArgs : ADeathEventArgs
//     {
//         // Empty
//     }

//     class StubHealth: MonoBehaviour, IHealth
//     {
//         #pragma warning disable 67  // Disable unused variable warning
//         public event EventHandler<AHealthUpdatedEventArgs> HealthUpdated;
//         public event EventHandler<ADeathEventArgs> Death;
//         #pragma warning restore 67

//         private int _currentHealth = 0;
//         public int maximumHealth { get; set; }
//         public int currentHealth {
//             get => _currentHealth;
//             set
//             {
//                 // We don't check that the 0 <= currentHealth <= maximumHealth
//                 // invariant holds. This was done to keep the class short and
//                 // readable, and should be sufficient for testing.
//                 var args = new StubHealthUpdatedEventArgs(_currentHealth, value);
//                 _currentHealth = value;
//                 OnHealthUpdated(args);
//             }
//         }

//         private void OnHealthUpdated(StubHealthUpdatedEventArgs e)
//         {
//             EventHandler<AHealthUpdatedEventArgs> handler = HealthUpdated;
//             handler?.Invoke(this, e);
//         }

//         // This stub will not use these convenience functions, and instead use
//         // currentHealth and maximumHealth directly
//         public void Hurt(int HP){}
//         public void Heal(int HP){}
//         public bool IsDead(){return false;}
//     }

//     public class HealthBarTest
//     {
//         private GameObject _healthObject;
//         private StubHealth _health;
//         private GameObject _healthBarPrefab;
//         private HealthBar _healthBar;

//         [SetUp]
//         public void Setup()
//         {
//             // Initialize a Health component, and a HealthBar
//             _healthObject = new GameObject();
//             _healthObject.AddComponent<StubHealth>();
//             _healthBarPrefab = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/HealthBar"));

//             // Get component references
//             _health = _healthObject.GetComponent<StubHealth>();
//             _healthBar = _healthBarPrefab.GetComponent<HealthBar>();

//             // Initialize health values
//             _health.currentHealth = 50;
//             _health.maximumHealth = 100;

//             // Link the healthBar to the object with a Health component
//             // then call Start() to make sure the events attach properly
//             _healthBar.healthSource = _healthObject;
//             _healthBar.Init();
//         }

//         [Test]
//         public void HealthBarTestIncreaseSize()
//         {
//             Vector3 origSize = _healthBar.transform.Find("HealthScaler").localScale;
//             _health.currentHealth += 5;
//             Vector3 newSize = _healthBar.transform.Find("HealthScaler").localScale;
//             Assert.Greater(newSize.x, origSize.x);
//         }

//         [Test]
//         public void HealthBarTestDecreasesSize()
//         {
//             Vector3 origSize = _healthBar.transform.Find("HealthScaler").localScale;
//             _health.currentHealth -= 5;
//             Vector3 newSize = _healthBar.transform.Find("HealthScaler").localScale;
//             Assert.Less(newSize.x, origSize.x);
//         }
//     }
// }
