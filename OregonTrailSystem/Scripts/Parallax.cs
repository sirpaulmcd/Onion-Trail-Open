// using System; // For events
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace EGS
{
/// <summary>
/// This script is used to add a parallax effect to the Oregon Trail inspired
/// scene. Based on the wonderful Code Monkey tutorial: 
/// https://www.youtube.com/watch?v=wBol2xzxCOU
/// </summary>
public class Parallax : MonoBehaviour
{
    //=========================================================================
    #region Instance variables
    //=========================================================================
    /// <summary>
    /// Multiplier used to set the movement speed of the parallax layer. A
    /// value of 0.5 means that the background will move at half the speed of 
    /// the camera. A value of 1 means that the background will move at the
    /// same speed as the camera and appear to be stationary.
    /// </summary>
    [SerializeField] private Vector2 _parallaxSpeedMultiplier = default;
    /// <summary>
    /// Whether or not the parallax scrolls horizontally.
    /// </summary>
    [SerializeField] private bool _horizontalScroll = false;
    /// <summary>
    /// Whether or not the parallax scrolls vertically.
    /// </summary>
    [SerializeField] private bool _verticalScroll = false;
    /// <summary>
    /// The transform of the main camera.
    /// </summary>
    private Transform _cameraTransform;
    /// <summary>
    /// The last known positiion of the camera.
    /// </summary>
    private Vector3 _lastCameraPosition;
    /// <summary>
    /// The width of the texture in Unity units.
    /// </summary>
    private float _textureUnitSizeX;
    /// <summary>
    /// The height of the texture in Unity units.
    /// </summary>
    private float _textureUnitSizeY;
    #endregion

    //=========================================================================
    #region MonoBehavior
    //=========================================================================
    /// <summary>
    /// Called before the first frame update.
    /// </summary>
    private void Start()
    {
        InitOnStart();
    }

    /// <summary>
    /// Called after all Update functions have been called.
    /// </summary>
    private void LateUpdate()
    {
        Vector3 deltaMovement = _cameraTransform.position - _lastCameraPosition;
        transform.position += new Vector3(deltaMovement.x * _parallaxSpeedMultiplier.x, deltaMovement.y * _parallaxSpeedMultiplier.y);
        _lastCameraPosition = _cameraTransform.position;

        if (_horizontalScroll && Mathf.Abs(_cameraTransform.position.x - transform.position.x) >= _textureUnitSizeX)
        {
            float offsetPositionX = (_cameraTransform.position.x - transform.position.x) % _textureUnitSizeX;
            transform.position = new Vector3(_cameraTransform.position.x + offsetPositionX, transform.position.y, transform.position.z);
        }

        if (_verticalScroll && Mathf.Abs(_cameraTransform.position.y - transform.position.y) >= _textureUnitSizeY)
        {
            float offsetPositionY = (_cameraTransform.position.y - transform.position.y) % _textureUnitSizeY;
            transform.position = new Vector3(transform.position.x, _cameraTransform.position.y + offsetPositionY, transform.position.z);
        }
    }
    #endregion

    //=========================================================================
    #region Private methods
    //=========================================================================
    /// <summary>
    /// Initialises the component in Start().
    /// </summary>
    private void InitOnStart()
    {
        InitVars();
        CheckMandatoryComponents();
    }

    /// <summary>
    /// Sources and initializes component variables.
    /// </summary>
    private void InitVars()
    {
        _cameraTransform = Camera.main.transform;
        _lastCameraPosition = _cameraTransform.position;
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        Texture2D texture = sprite.texture;
        _textureUnitSizeX = (texture.width / sprite.pixelsPerUnit) * transform.localScale.x;
        _textureUnitSizeY = (texture.height / sprite.pixelsPerUnit) * transform.localScale.y;
    }

    /// <summary>
    /// Ensures mandatory components are accounted for.
    /// </summary>
    private void CheckMandatoryComponents()
    {
        // Assert.IsNotNull(_myVariable, gameObject.name + " is missing _myVariable");
    }
    #endregion
}
}