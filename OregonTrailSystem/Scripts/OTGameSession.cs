using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;  // For InitOnSceneLoad

namespace EGS
{
/// <summary>
/// This class is responsible for tracking active Game Session information
/// related to oregon trail gameplay.
/// </summary>
public class OTGameSession : Singleton<OTGameSession>
{
    //=========================================================================
    #region Instance variables
    //=========================================================================
    /// <summary>
    /// The amount of food that the team has.
    /// </summary>
    [SerializeField] private int _food = 0;
    /// <summary>
    /// The amount of fuel that the team has.
    /// </summary>
    [SerializeField] private int _fuel = 0;
    /// <summary>
    /// The amount of medical supply that the team has.
    /// </summary>
    [SerializeField] private int _med = 0;
    /// <summary>
    /// Whether or not the squad is currently on the road.
    /// </summary>
    private bool _isTraveling = false;
    /// <summary>
    /// The distance travelled over the trip.
    /// </summary>
    private float _distanceTraveled = 0;
    /// <summary>
    /// Whether or not the OTGameSession is initialized.
    /// </summary>
    private bool isInitialized = false; 
    /// <summary>
    /// List of UI prefabs corresponding with on road events.
    /// </summary>
    [SerializeField] private List<GameObject> _onRoadEvents = default;
    /// <summary>
    /// List of UI prefabs corresponding with off road events.
    /// </summary>
    [SerializeField] private List<GameObject> _offRoadEvents = default;
    /// <summary>
    /// The main camera in the Oregon Trail Roadtrip scene.
    /// </summary>
    private OTCameraMovement _camera = default;
    /// <summary>
    /// The amount of seconds the camera moves for while travelling.
    /// </summary>
    [SerializeField] private float _travelSeconds = 3f;
    #endregion

    //=========================================================================
    #region Properties
    //=========================================================================
    /// <summary>
    /// The amount of food that the team has.
    /// </summary>
    public int Food
    {
        get { return _food; }
        set 
        {
            if (value != _food)
            {
                if (value < 0) { value = 0; }
                if (value > 99) { value = 99; }
                _food = value;
                InvokeFoodUpdatedEvent();
            }
        }
    }

    /// <summary>
    /// The amount of fuel that the team has.
    /// </summary>
    public int Fuel
    {
        get { return _fuel; }
        set 
        {
            if (value != _fuel)
            {
                if (value < 0) { value = 0; }
                if (value > 99) { value = 99; }
                _fuel = value;
                InvokeFuelUpdatedEvent();
            }
        }
    }

    /// <summary>
    /// The amount of medical supply that the team has.
    /// </summary>
    public int Med
    {
        get { return _med; }
        set 
        {
            if (value != _med)
            {
                if (value < 0) { value = 0; }
                if (value > 99) { value = 99; }
                _med = value;
                InvokeMedUpdatedEvent();
            }
        }
    }

    /// <summary>
    /// Whether or not the squad is currently on the road.
    /// </summary>
    public bool IsTraveling
    {
        get { return _isTraveling; }
        set 
        {
            if (value != _isTraveling) { _isTraveling = value; }
        }
    }

    /// <summary>
    /// Whether or not the squad is currently on the road.
    /// </summary>
    public float DistanceTraveled
    {
        get { return _distanceTraveled; }
        set 
        {
            if (value != _distanceTraveled) 
            {  
                _distanceTraveled = value;
                TravelToPosition(new Vector3(_distanceTraveled, transform.position.y, transform.position.z));
            }
        }
    }
    #endregion

    //=========================================================================
    #region Event invokers
    //=========================================================================
    /// <summary>
    /// Invokes a FoodUpdatedEvent.
    /// </summary>
    private void InvokeFoodUpdatedEvent()
    {
        Debug.Log("OTGameSession.InvokeFoodUpdatedEvent: Food value has been updated.");
        EventManager.Instance.Invoke(EventName.FoodUpdatedEvent, new FoodUpdatedEventArgs(_food), this);
    }

    /// <summary>
    /// Invokes a FuelUpdatedEvent.
    /// </summary>
    private void InvokeFuelUpdatedEvent()
    {
        Debug.Log("OTGameSession.InvokeFuelUpdatedEvent: Fuel value has been updated.");
        EventManager.Instance.Invoke(EventName.FuelUpdatedEvent, new FuelUpdatedEventArgs(_fuel), this);
    }

    /// <summary>
    /// Invokes a MedUpdatedEvent.
    /// </summary>
    private void InvokeMedUpdatedEvent()
    {
        Debug.Log("OTGameSession.InvokeMedUpdatedEvent: Med value has been updated.");
        EventManager.Instance.Invoke(EventName.MedUpdatedEvent, new MedUpdatedEventArgs(_med), this);
    }
    #endregion

    //=========================================================================
    #region MonoBehavior
    //=========================================================================
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        // Call InitOnSceneLoad() whenever a scene is loaded.
        SceneManager.sceneLoaded += InitOnSceneLoad;
    }
    #endregion

    //=========================================================================
    #region Initialization
    //=========================================================================
    /// <summary>
    /// To be called when a scene is loaded. Occurs before Start().
    /// </summary>
    private void InitOnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        if (!isInitialized) 
        {
            isInitialized = true;
        }
        if (scene.name == SceneName.ROADTRIP) 
        { 
            _camera = GameObject.Find("MainCamera").GetComponent<OTCameraMovement>();
            StartCoroutine(StartTrip()); 
        }
    }

    /// <summary>
    /// Waits until the end of the first frame and starts travel.
    /// WaitForEndOfFrame ensures that all Awake() and Start() methods have
    /// executed before additional methods are called.
    /// </summary>
    private IEnumerator StartTrip()
    {
        yield return new WaitForEndOfFrame();
        TravelOneTurn();
    }
    #endregion

    //=========================================================================
    #region Travelling
    //=========================================================================
    /// <summary>
    /// Plays one turn.
    /// </summary>
    public void TravelOneTurn()
    {
        StartCoroutine(PlayTurn());
    }

    /// <summary>
    /// Plays one Oregon Trail style turn.
    /// </summary>
    private IEnumerator PlayTurn()
    {
        // TODO stop Play turn coroutine when scene changes
        DistanceTraveled += CalculateTravelDistance();
        yield return new WaitForSeconds(1f);
        Food -= CalculateRations();
        yield return new WaitForSeconds(1f);
        Med -= CalculateMedicalExpenses();
        yield return new WaitForSeconds(1f);
        SelectEvent();
    }

    /// <summary>
    /// Calculates the distance travelled in a turn.
    /// </summary>
    /// <returns>
    /// The distance travelled in a turn.
    /// </returns>
    private float CalculateTravelDistance()
    {
        //  return _vehicleSpeed * _conditionMultiplier;
        return 10f;
    }

    /// <summary>
    /// Moves the camera to the input position.
    /// </summary>
    /// <param name="travelDelta">
    /// The distance travelled since the previous turn.
    /// </param>
    private void TravelToPosition(Vector3 position)
    {
        _camera.TravelToPosition(position, _travelSeconds);
    }

    /// <summary>
    /// Calculates the food used up in one turn.
    /// </summary>
    /// <returns>
    /// The food used up in one turn.
    /// </returns>
    private int CalculateRations()
    {
        return 3;
    }

    /// <summary>
    /// Calculates the med used in one turn.
    /// </summary>
    /// <returns>
    /// The med used in one turn.
    /// </returns>
    private int CalculateMedicalExpenses()
    {
        return 1;
    }

    /// <summary>
    /// Selects an event to prompt the players.
    /// </summary>
    private void SelectEvent()
    {
        System.Random rng = new System.Random();
        if (rng.Next(0, 5) == 4)
        {
            int i = rng.Next(0, _offRoadEvents.Count);
            GameObject offRoadEventPrefab = _offRoadEvents[i];
            // _offRoadEvents.RemoveAt(i);
            UIManager.Instance.InstantiateCanvas(offRoadEventPrefab);
        }
        else
        {
            int i = rng.Next(0, _onRoadEvents.Count);
            GameObject onRoadEventPrefab = _onRoadEvents[i];
            // _onRoadEvents.RemoveAt(i);
            UIManager.Instance.InstantiateCanvas(onRoadEventPrefab);
        }
    }
    #endregion
}
}