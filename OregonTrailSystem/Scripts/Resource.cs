using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// This class represents resources that can be collected by the players.
    /// Resources can either be collected by walking over them (small items) or
    /// by picking them up and bringing them to the vehicle (large items).
    /// </summary>
    public class Resource : MonoBehaviour
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        [SerializeField] private bool _isLiftable = false;
        [SerializeField] private int _foodValue = 0;
        [SerializeField] private int _fuelValue = 0;
        [SerializeField] private int _medValue = 0;
        #endregion

        //=====================================================================
        #region MonoBehavior
        //=====================================================================
        private void Start()
        {
            InitVars();
        }

        private void OnCollisionEnter(Collision other)
        {
            ProcessCollision(other);
        }
        #endregion

        //=====================================================================
        #region Initialization
        //=====================================================================
        private void InitVars()
        {
            if (!_isLiftable) { Destroy(GetComponent<Liftable>()); }

        }
        #endregion

        //=====================================================================
        #region Resource incrementation
        //=====================================================================
        private void ProcessCollision(Collision other)
        {
            if (_isLiftable)
            {
                if (other.gameObject.name == "Tractor") { IncrementResources(); }
            }
            else
            {
                if (other.gameObject.CompareTag("Player")) { IncrementResources(); }
            }
        }

        private void IncrementResources()
        {
            OTGameSession gs = OTGameSession.Instance;
            if (_foodValue > 0)
            {
                gs.Food += _foodValue;
                CreateIntPopup(_foodValue);
            }
            else if (_fuelValue > 0)
            {
                gs.Fuel += _fuelValue;
                CreateIntPopup(_fuelValue);
            }
            else if (_medValue > 0)
            {
                gs.Med += _medValue;
                CreateIntPopup(_medValue);
            }
            Destroy(this.gameObject);
        }

        private void CreateIntPopup(int value)
        {
            TextPopup.CreateIntPopup(
                transform.position + new Vector3(0, 2, 0),
                value,
                TextAnimName.INCREMENTATION);
        }
        #endregion
    }
}
