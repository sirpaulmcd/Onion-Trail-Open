using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
    /// <summary>
    /// Temporary script used to ignite player when it touches this GameObject.
    /// </summary>
    public class Igniter : MonoBehaviour
    {
        private void OnCollisionEnter(Collision other)
        {
            EntityStats es = other.gameObject.GetComponent<EntityStats>();
            if (es != null) { es.IsCombusting = true; }
        }
    }
}
