using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using TMPro;

namespace EGS
{
    /// <summary>
    /// This class generates popup text for when damage is taken or resources are
    /// collected.
    /// </summary>
    public class TextPopup : MonoBehaviour
    {
        //=====================================================================
        #region Class variables
        //=====================================================================
        private static Color32 _critColor = new Color32(255, 255, 0, 255);
        private static Color32 _damageColor = new Color32(255, 0, 0, 255);
        private static Color32 _healColor = new Color32(0, 255, 0, 255);
        #endregion

        //=====================================================================
        #region Class methods
        //=====================================================================
        /// <summary>
        /// Instantiates a text popup matching the input values.
        /// </summary>
        /// <param name="position">The instantiation position.</param>
        /// <param name="value">The integer value of the popup.</param>
        /// <param name="animation">The animation of the popup.</param>
        /// <param name="crit">Whether the value is a crit.</param>
        /// <returns>Initialized TextPopup.</returns>
        public static TextPopup CreateIntPopup(
            Vector3 position, int value, TextAnimName animation = TextAnimName.NONE, bool crit = false)
        {
            Transform textPopupTransform = InstantiateTransform(position, animation);
            SetupInt(textPopupTransform, value, crit);
            return textPopupTransform.GetComponent<TextPopup>();
        }

        /// <summary>
        /// Instantiates a text popup matching the input values.
        /// </summary>
        /// <param name="position">The instantiation position.</param>
        /// <param name="value">The string value of the popup.</param>
        /// <param name="animation">The animation of the popup.</param>
        /// <param name="color">The color of the popup.</param>
        /// <returns>Initialized TextPopup.</returns>
        public static TextPopup CreateTextPopup(
            Vector3 position, string value, TextAnimName animation = TextAnimName.NONE, Color32? color = null)
        {
            Transform textPopupTransform = InstantiateTransform(position, animation);
            SetupStr(textPopupTransform, value, color);
            return textPopupTransform.GetComponent<TextPopup>();
        }
        #endregion

        //=====================================================================
        #region Instance variables
        //=====================================================================
        [SerializeField] private float _selfDestructSeconds = 1f;
        #endregion

        //=====================================================================
        #region MonoBehavior
        //=====================================================================
        private void Awake()
        {
            Destroy(gameObject, _selfDestructSeconds);
        }
        #endregion

        //=====================================================================
        #region Initialization
        //=====================================================================
        private static Transform InstantiateTransform(Vector3 position, TextAnimName animation)
        {
            switch (animation)
            {
                case TextAnimName.DAMAGE:
                    return Instantiate(
                        GameAssets.Instance.DamageTextPopup,
                        position,
                        Quaternion.Euler(45, 0, 0));
                case TextAnimName.INCREMENTATION:
                    return Instantiate(
                        GameAssets.Instance.IncrementationTextPopup,
                        position,
                        Quaternion.Euler(45, 0, 0));
                default:
                    if (animation != TextAnimName.NONE) { Debug.LogWarning("Invalid animation name detected."); }
                    return Instantiate(
                        GameAssets.Instance.TextPopup,
                        position,
                        Quaternion.Euler(45, 0, 0));
            }
        }

        private static void SetupInt(Transform textPopupTransform, int value, bool crit)
        {
            TextMeshPro tmp = textPopupTransform.GetComponent<TextMeshPro>();
            string s = value.ToString();
            if (value > 0)
            {
                s = "+" + s;
                tmp.color = _healColor;
            }
            else if (value < 0)
            {
                if (crit) { tmp.color = _critColor; }
                else { tmp.color = _damageColor; }
            }
            tmp.SetText(s);
        }

        private static void SetupStr(Transform textPopupTransform, string value, Color32? color)
        {
            TextMeshPro tmp = textPopupTransform.GetComponent<TextMeshPro>();
            if (color.Equals(null)) { tmp.color = new Color32(255, 255, 255, 255); }
            else { tmp.color = (Color32)color; }
            tmp.SetText(value);
        }
        #endregion
    }
}
