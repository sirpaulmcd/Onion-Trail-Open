// using System; // For events
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using TMPro;

namespace EGS
{
    /// <summary>
    /// One option corresponding with a prompt. Button displays text. Upon being
    /// clicked, edits resources and activates the corresponding Response UI.
    /// </summary>
    [System.Serializable]
    public class PromptOption
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        /// <summary>
        /// The button corresponding with the option.
        /// </summary>
        [SerializeField] private Button _button = default;
        /// <summary>
        /// The GameObject holding the response UI.
        /// </summary>
        [SerializeField] private GameObject _responseUI = default;
        /// <summary>
        /// The prompt component associated with this option.
        /// </summary>
        private Prompt _prompt = default;
        /// <summary>
        /// The info related to the option.
        /// </summary>
        private PromptOptionInfo _optionInfo = default;
        #endregion

        //=====================================================================
        #region Public methods
        //=====================================================================
        /// <summary>
        /// Initialises the component.
        /// </summary>
        public void Init(PromptOptionInfo optionInfo)
        {
            InitVars(optionInfo);
            CheckMandatoryComponents();
        }
        #endregion

        //=====================================================================
        #region Private methods
        //=====================================================================
        /// <summary>
        /// Sources and initializes component variables.
        /// </summary>
        private void InitVars(PromptOptionInfo optionInfo)
        {
            _prompt = GameObject.Find("PromptCanvas").GetComponentInChildren<Prompt>();
            _optionInfo = optionInfo;
            _button.onClick.AddListener(OnClick);
            _button.GetComponentInChildren<TextMeshProUGUI>().text = _optionInfo.buttonText;
        }

        /// <summary>
        /// Ensures mandatory components are accounted for.
        /// </summary>
        private void CheckMandatoryComponents()
        {
            Assert.IsNotNull(_prompt, "missing _prompt variable");
            Assert.IsNotNull(_optionInfo, _prompt.name + " is missing _optionInfo");
            Assert.IsNotNull(_button, _prompt.name + " is missing _button");
            Assert.IsNotNull(_responseUI, _prompt.name + " is missing _responseUI");
        }

        /// <summary>
        /// Method to be called when the button is clicked.
        /// </summary>
        private void OnClick()
        {
            OTGameSession.Instance.Food += _optionInfo.foodDelta;
            OTGameSession.Instance.Fuel += _optionInfo.fuelDelta;
            OTGameSession.Instance.Med += _optionInfo.medDelta;
            _prompt.ActivateResponse(_responseUI);
        }
        #endregion
    }
}
