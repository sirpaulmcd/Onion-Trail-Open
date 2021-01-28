using UnityEngine;

namespace EGS
{
/// <summary>
/// The singleton design pattern restricts the instantiation of a class to a 
/// single instance. This is done in order to provide coordinated access to a 
/// certain resource throughout an entire software system. The singleton class 
/// ensures that it’s only instantiated once, and can provide easy access to 
/// the single instance.
/// </summary>
public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    //=========================================================================
    #region Instance variables
    //=========================================================================
    private static T _instance;
    #endregion

    //=========================================================================
    #region Properties
    //=========================================================================
    /// <summary>
    /// The current instance of the singleton class.
    /// </summary>
    public static T Instance
    {
        get
        {
            return _instance;
        }
    }

    /// <summary>
    /// Whether or not the singleton is already initialized.
    /// </summary>
    public static bool IsInitialized
    {
        get
        {
            return _instance != null;
        }
    }
    #endregion

    //=========================================================================
    #region Monobehaviour
    //=========================================================================
    protected virtual void Awake()
    {
        SetUpSingleton();
    }

    protected virtual void OnDestroy()
    {
        DestroySingleton();
    }
    #endregion

    //=========================================================================
    #region Singleton methods
    //=========================================================================
    private void SetUpSingleton()
    {
        if (_instance != null)
        {
            Debug.LogErrorFormat("[Singleton] Trying to instantiate a " +
            "second instance of singleton class {0}", GetType().Name);
            Destroy(gameObject);
        }
        else
        {
            _instance = (T)this;
        }
    }

    private void DestroySingleton()
    {
        if (_instance == this)
        {
            _instance = null;
        }   
    }
    #endregion
}
}