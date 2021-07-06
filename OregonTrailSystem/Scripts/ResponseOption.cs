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
    ///
    /// </summary>
    [System.Serializable]
    public class ResponseOption
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        /// <summary>
        /// The button corresponding with the option.
        /// </summary>
        [SerializeField] private Button _button = default;
        /// <summary>
        /// The prompt component associated with this option.
        /// </summary>
        private Response _response = default;
        /// <summary>
        /// The info associated with the response option.
        /// </summary>
        private ResponseOptionInfo _optionInfo = default;
        #endregion

        //=====================================================================
        #region Public methods
        //=====================================================================
        /// <summary>
        /// Initialises the component.
        /// </summary>
        public void Init(ResponseOptionInfo optionInfo)
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
        private void InitVars(ResponseOptionInfo optionInfo)
        {
            _response = GameObject.Find("PromptCanvas").GetComponentInChildren<Response>();
            _optionInfo = optionInfo;
            _button.onClick.AddListener(OnClick);
            _button.GetComponentInChildren<TextMeshProUGUI>().text = _optionInfo.buttonText;

        }

        /// <summary>
        /// Ensures mandatory components are accounted for.
        /// </summary>
        private void CheckMandatoryComponents()
        {
            Assert.IsNotNull(_response, "missing _response variable");
            Assert.IsNotNull(_optionInfo, _response.name + " is missing _optionInfo");
            Assert.IsNotNull(_button, _response.name + " is missing _button");
        }

        /// <summary>
        /// Method to be called when the button is clicked.
        /// </summary>
        private void OnClick()
        {
            if (_optionInfo.newScene != "")
            {
                GameManager.Instance.LoadScene(_optionInfo.newScene);
            }
            else { OTGameSession.Instance.TravelOneTurn(); }
            ExitUI();
        }

        /// <summary>
        /// Reverts player controls and exits the prompt.
        /// </summary>
        private void ExitUI()
        {
            Object.Destroy(_response.transform.parent.gameObject);
        }
        #endregion
    }
}
