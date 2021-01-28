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
/// The response UI for an OT event. Upon enable, prints response text to the 
/// screen and waits for a PromptOption to be selected.
/// </summary>
public class Response : MonoBehaviour
{
    //=========================================================================
    #region Instance variables
    //=========================================================================
    /// <summary>
    /// The scriptable object associated with the response.
    /// </summary>
    [SerializeField] private ResponseSO _scriptableObject = default;
    /// <summary>
    /// The TextMeshPro component that displays the text.
    /// </summary>
    [SerializeField] private TextMeshProUGUI _textDisplayer = default;
    /// <summary>
    /// The options available for the prompt.
    /// </summary>
    [SerializeField] private ResponseOption[] _options = default;
    #endregion

    //=========================================================================
    #region MonoBehavior
    //=========================================================================
    /// <summary>
    /// Called when the object is Enabled.
    /// </summary>
    private void OnEnable()
    {
        InitOnEnable();
    }
    #endregion

    //=========================================================================
    #region Private methods
    //=========================================================================
    /// <summary>
    /// Initialises the component in Start().
    /// </summary>
    private void InitOnEnable()
    {
        InitVars();
        StartCoroutine(WriteText(_scriptableObject.text));
    }

    /// <summary>
    /// Sources and initializes component variables.
    /// </summary>
    private void InitVars()
    {
        if (_options.Length == _scriptableObject.optionInfo.Length)
        {
            for (int i = 0; i < _options.Length; i++)
            {
                _options[i].Init(_scriptableObject.optionInfo[i]);
            }
        }
        else { Debug.LogWarning("Response not wired correctly.", this.gameObject); }
    }

    /// <summary>
    /// Prints input text to the text displayer.
    /// </summary>
    /// <param name="text">
    /// The text to be printed on the text displayer.
    /// </param>
    private IEnumerator WriteText(string text)
    {
        _textDisplayer.text = "";
        for (int i = 0; i < text.Length; i++)
        {
            _textDisplayer.text += text[i];
            yield return new WaitForSeconds(_scriptableObject.typeDelaySeconds);
        }
    }
    #endregion
}
}