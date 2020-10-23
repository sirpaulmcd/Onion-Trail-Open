// using System;
// using System.Collections;
// using System.Collections.Generic;
// using NUnit.Framework;
// using UnityEngine;
// using UnityEngine.TestTools;

// namespace Tests
// {
//     public class PlayerPositionSystemTest
//     {
//         private PlayerPositionSystem _pps;
//         private GameObject _player1;
//         private GameObject _player2;
//         private GameObject _player3;
//         private GameObject _player4;

//         private readonly Vector3 Player1Position = new Vector3(0, 1, 0);
//         private readonly Vector3 Player2Position = new Vector3(1, 0, 0);
//         private readonly Vector3 Player3Position = new Vector3(0, -1, 0);
//         private readonly Vector3 Player4Position = new Vector3(-1, 0, 0);

            
//         [SetUp]
//         public void Setup()
//         {
//             // Setup the player position system
//             GameObject obj = new GameObject();
//             obj.AddComponent<PlayerPositionSystem>();

//             // Assign the instance variables
//             _pps = obj.GetComponent<PlayerPositionSystem>();
//             _player1 = new GameObject();
//             _player2 = new GameObject();
//             _player3 = new GameObject();
//             _player4 = new GameObject();

//             // Setup the dummy players, which need Health and positions
//             _player1.AddComponent<Health>();
//             _player2.AddComponent<Health>();
//             _player3.AddComponent<Health>();
//             _player4.AddComponent<Health>();

//             _player1.transform.position = Player1Position; 
//             _player2.transform.position = Player2Position; 
//             _player3.transform.position = Player3Position; 
//             _player4.transform.position = Player4Position;

//             // Add the players to the PlayerPositionSystem
//             _pps.players.Add(_player1);
//             _pps.players.Add(_player2);
//             _pps.players.Add(_player3);
//             _pps.players.Add(_player4);
//         }

//         //============================================================================
//         // FindClosest() Tests
//         //============================================================================
        
//         [Test]
//         public void FindClosestToExactPositionWhenAllAlive()
//         {                                    
//             Assert.AreEqual(Player1Position, _pps.FindClosest(Player1Position));
//             Assert.AreEqual(Player2Position, _pps.FindClosest(Player2Position));
//             Assert.AreEqual(Player3Position, _pps.FindClosest(Player3Position));
//             Assert.AreEqual(Player4Position, _pps.FindClosest(Player4Position));
//         }

//         [Test]
//         public void FindClosestToExactPositionWhenSomeAlive()
//         {
//             _player3.GetComponent<IHealth>().Hurt(int.MaxValue);
//             _player4.GetComponent<IHealth>().Hurt(int.MaxValue);

//             // Check that we find the living players ...
//             Assert.AreEqual(Player1Position, _pps.FindClosest(Player1Position));
//             Assert.AreEqual(Player2Position, _pps.FindClosest(Player2Position));

//             // ... and that we don't find the dead ones.
//             Assert.AreNotEqual(Player3Position, _pps.FindClosest(Player3Position));
//             Assert.AreNotEqual(Player4Position, _pps.FindClosest(Player4Position));
//         }

//         [Test]
//         public void FindClosestToApproximatePositionWhenAllAlive()
//         {
//             Vector3 offset = new Vector3(0.1f, 0.1f, 0.1f);
            
//             Assert.AreEqual(Player1Position, _pps.FindClosest(Player1Position + offset));
//             Assert.AreEqual(Player2Position, _pps.FindClosest(Player2Position + offset));
//             Assert.AreEqual(Player3Position, _pps.FindClosest(Player3Position + offset));
//             Assert.AreEqual(Player4Position, _pps.FindClosest(Player4Position + offset));
//         }

//         [Test]
//         public void FindClosestToApproximatePositionWhenSomeAlive()
//         {
//             Vector3 offset = new Vector3(0.1f, 0.1f, 0.1f);
            
//             _player3.GetComponent<IHealth>().Hurt(int.MaxValue);
//             _player4.GetComponent<IHealth>().Hurt(int.MaxValue);

//             // Check that we find the living players ...
//             Assert.AreEqual(Player1Position, _pps.FindClosest(Player1Position + offset));
//             Assert.AreEqual(Player2Position, _pps.FindClosest(Player2Position + offset));

//             // ... and that we don't find the dead ones.
//             Assert.AreNotEqual(Player3Position, _pps.FindClosest(Player3Position + offset));
//             Assert.AreNotEqual(Player4Position, _pps.FindClosest(Player4Position + offset));
//         }

//         [Test]
//         public void FindClosestWhenAllDead()
//         {
//             _player1.GetComponent<IHealth>().Hurt(int.MaxValue);
//             _player2.GetComponent<IHealth>().Hurt(int.MaxValue);
//             _player3.GetComponent<IHealth>().Hurt(int.MaxValue);
//             _player4.GetComponent<IHealth>().Hurt(int.MaxValue);

//             #pragma warning disable 168 // Disable unused variable warning for 'e'
//             try
//             {
//                 _pps.FindClosest(Player1Position);
//                 Assert.Fail("Failed. We didn't throw an exception!");
//             }
//             catch (NoLivingPlayersException e)
//             {
//                 // Passed: We avoided the Assert.Fail()
//             }
//             #pragma warning restore 168
//         }

//         //============================================================================
//         // GetCentroid() Tests
//         //============================================================================

//         [Test]
//         public void GetCentroidWhenAllAlive()
//         {
//             Assert.AreEqual(Vector3.zero, _pps.GetCentroid());
//         }

//         [Test]
//         public void GetCentroidWhenSomeAlive()
//         {
//             Vector3 averagePositionP1P2 = new Vector3(0.5f, 0.5f, 0);

//             _player3.GetComponent<IHealth>().Hurt(int.MaxValue);
//             _player4.GetComponent<IHealth>().Hurt(int.MaxValue);

//             Assert.AreEqual(averagePositionP1P2, _pps.GetCentroid());
//         }
        
//         [Test]
//         public void GetCentroidWhenAllDead()
//         {
//             _player1.GetComponent<IHealth>().Hurt(int.MaxValue);
//             _player2.GetComponent<IHealth>().Hurt(int.MaxValue);
//             _player3.GetComponent<IHealth>().Hurt(int.MaxValue);
//             _player4.GetComponent<IHealth>().Hurt(int.MaxValue);

//             #pragma warning disable 168 // Disable unused variable warning for 'e'
//             try
//             {
//                 _pps.GetCentroid();
//                 Assert.Fail("Failed. We didn't throw an exception!");
//             }
//             catch (NoLivingPlayersException e)
//             {
//                 // Passed: We avoided the Assert.Fail()
//             }
//             #pragma warning restore 168
//         }

//         //============================================================================
//         // GetPlayerPositions() Tests
//         //============================================================================
        
//         [Test]
//         public void GetPlayerPositionsWhenAllAlive()
//         {
//             List<Vector3> actual = _pps.GetPlayerPositions();
//             List<Vector3> expected = new List<Vector3> { Player1Position,
//                                                          Player2Position,
//                                                          Player3Position,
//                                                          Player4Position };
//             // We provide Vector3DComparer since Unity doesn't implement IComparable<T>
//             // with its Vector3 class.
//             actual.Sort(new Vector3DComparer());
//             expected.Sort(new Vector3DComparer());

//             Assert.AreEqual(actual, expected);
//         }

//         [Test]
//         public void GetPlayerPositionsWhenSomeAlive()
//         {
//             _player3.GetComponent<IHealth>().Hurt(int.MaxValue);
//             _player4.GetComponent<IHealth>().Hurt(int.MaxValue);

//             List<Vector3> actual = _pps.GetPlayerPositions();
//             List<Vector3> expected = new List<Vector3> { Player1Position,
//                                                          Player2Position };            
//             // We provide Vector3DComparer since Unity doesn't implement IComparable<T>
//             // with its Vector3 class.
//             actual.Sort(new Vector3DComparer());
//             expected.Sort(new Vector3DComparer());

//             Assert.AreEqual(actual, expected);            
//         }
        
//         [Test]
//         public void GetPlayerPositionsWhenAllDead()
//         {
//             _player1.GetComponent<IHealth>().Hurt(int.MaxValue);
//             _player2.GetComponent<IHealth>().Hurt(int.MaxValue);
//             _player3.GetComponent<IHealth>().Hurt(int.MaxValue);
//             _player4.GetComponent<IHealth>().Hurt(int.MaxValue);

//             #pragma warning disable 168 // Disable unused variable warning for 'e'
//             try
//             {
//                 _pps.GetPlayerPositions();
//                 Assert.Fail("Failed. We didn't throw an exception!");
//             }
//             catch (NoLivingPlayersException e)
//             {
//                 // Passed: We avoided the Assert.Fail()
//             }
//             #pragma warning restore 168
//         }

//         //============================================================================
//         // GetBoundingBox() Tests
//         //============================================================================

//         [Test]
//         public void GetBoundingBoxWhenAllAlive()
//         {
//             Vector3 center = new Vector3(0, 0, 0);
//             Vector3 size = new Vector3(2, 2, 0);
//             Bounds expected = new Bounds(center, size);
                        
//             Assert.AreEqual(expected, _pps.GetBoundingBox());
//         }

//         [Test]
//         public void GetBoundingBoxWhenSomeAlive()
//         {
//             Vector3 center = new Vector3(0.5f, 0.5f, 0);
//             Vector3 size = new Vector3(1, 1, 0);
//             Bounds expected = new Bounds(center, size);

//             _player3.GetComponent<IHealth>().Hurt(int.MaxValue);
//             _player4.GetComponent<IHealth>().Hurt(int.MaxValue);            

//             Assert.AreEqual(expected, _pps.GetBoundingBox());
//         }
        
//         [Test]
//         public void GetBoundingBoxWhenAllDead()
//         {
//             _player1.GetComponent<IHealth>().Hurt(int.MaxValue);
//             _player2.GetComponent<IHealth>().Hurt(int.MaxValue);
//             _player3.GetComponent<IHealth>().Hurt(int.MaxValue);
//             _player4.GetComponent<IHealth>().Hurt(int.MaxValue);

//             #pragma warning disable 168 // Disable unused variable warning for 'e'
//             try
//             {
//                 _pps.GetBoundingBox();
//                 Assert.Fail("Failed. We didn't throw an exception!");
//             }
//             catch (NoLivingPlayersException e)
//             {
//                 // Passed: We avoided the Assert.Fail()
//             }
//             #pragma warning restore 168
//         }

//         //============================================================================
//         // Helper Classes
//         //============================================================================

//         /// <summary>
//         ///   Helper class to compare two Vector3s for approximate equality.
//         /// </summary>
//         /// <remarks>
//         ///   Unity doesn't implement an IComparable<T> for Vectors, which means
//         ///   List<Vector3> blah.Sort() fails. In order to get around that, we
//         //    use this comparison class as a parameter in Sort().
//         /// </remarks>
//         private class Vector3DComparer : IComparer<Vector3>
//         {
//             /// <summary>
//             ///   An implementation of a comparison function for Vector3s
//             /// </summary>
//             /// <returns>
//             ///   0 if equal
//             ///  -1 if v1 < v2
//             ///   1 if v1 > v2
//             /// </returns>
//             public int Compare(Vector3 v1, Vector3 v2)
//             {
//                 // This equality check accounts for floating point inaccuracies
//                 // https://docs.unity3d.com/ScriptReference/Vector3-operator_eq.html
//                 if(v1 == v2) return 0;
//                 else         return v1.x > v2.x ? 1 : -1;
//             }
//         }
//     }
// }
