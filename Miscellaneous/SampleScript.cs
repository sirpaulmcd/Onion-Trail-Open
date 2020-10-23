/*
Important notes
  - When coding:
    - Remember that, in Unity, scripts are treated as components of parent 
      GameObjects. This is just like other common Unity components such as 
      Transform, Rigidbody, and SpriteRenderer. However, scripts are GameObject
      components that are defined by you!
    - If your code is not deemed sufficiently readable, optimized, or
      formatted by the project team, you will be unable to merge your branch 
      to master until requested changes have been made. To keep pull request 
      reviews short and timely, try your best to follow these guidelines.
      Keep in mind, it's okay and expected to make mistakes.
    - Give variables and methods descriptive names that are intuitive to people
      with little to no background knowledge of your code. Fill the rest of the
      gaps with detailed documentation.
  - When documenting code:
    - Keep documentation lines from extending past 80 characters. This 
      includes headers and section separators. This is important for comments
      but is only a rule of thumb for actual code.
    - Use full sentences with proper punctuation. This helps documentation 
      not feel like it has been written by several different people. 
  - Code formatting for the project follows these guidelines:
    https://docs.godotengine.org/en/3.1/getting_started/scripting/c_sharp/c_sharp_style_guide.html
    When in doubt, look at the PlayerController.cs script for code formatting
    examples. 
  - If you are having trouble with a concept, just ask a member of the team
    to fill you in! We're all friends here. 
*/

//=============================================================================
#region Headers
//=============================================================================
/*
    Major sections of code should be separated into regions with header lines. 
    They provide a foundation for code organization. They should be included 
    even if the script is small. Small scripts can quickly grow into large 
    scripts and it is good practice to establish proper organization 
    immediately. The typical header sections that you will commonly see in 
    Unity scripts are included in the below example script.

    When making a header or separator, place it in the working indent and then
    extend it until it reaches the 80 char limit.
    
Section separators
    If a section is so large that it can be split into logical components,
    use  section separators like the one below:
//=============================================================================
*/
#endregion

/*
Imports section
    Used to import external scripts and their associated functions. For
    example, this section is why the script knows what a "GameObject" is
    even though it isn't explicitly defined here.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EGS
{
/* 
Required component section
    Used to indicate whether the script depends on a particular component to 
    function. If a component is required by the script and is not detected by
    Unity, a default component of that variety is added to the parent 
    GameObject. RequireComponent can be a pain when used inappropriately.

    Rule of thumb:
    - Only include a "RequireComponent" when you need a DEFAULT component that
      does not require any additional changes via the inspector. Avoid using
      RequireComponent otherwise.
*/ 
[RequireComponent(typeof(Rigidbody))]

/*
Summary tags
    Summary tags are the standard notation for documenting classes, variables,
    and methods in C#. If you are using a C# compatible editor, summary tags 
    allow for mouse-hover pop-up descriptions. As such, these are essential 
    for constructing readable code.
*/ 
/// <summary>
/// Example script to showcase Unity scripting basics as well as the team's 
/// documentation standards. For this example, we will construct a scripted
/// component such that it grants its parent object with the ability to
/// majestically fly away.
/// </summary>
public class SampleScript : MonoBehaviour
{
    /*
    Classes
        Classes represent the component that you are creating for a GameObject.
        If you opened up the script for Unity's Transform component, you would
        see that the script's class is named "Transform". Since classes are
        components of GameObjects, this component and it's associated public 
        methods and properties can be accessed by other scripts using
        GetComponent<SampleScript>().
        
        Note that the name of your class must be EXACTLY THE SAME as the script 
        seen in the Unity Assets folder. If the name is different, or has
        different capitalization, your script will break.
    */
    //=========================================================================
    #region Instance Variables
    //=========================================================================
    /*
    Instance variables
        These are variables that belong to an "instance" of a component. Two
        GameObjects can hold the same type of component but each instance
        of that component are unique to the parent GameObject. As such,
        instance variables describe fundamental attributes of the parent 
        object. For example, an instance variable for a Circle.cs component 
        would be "radius".
        Types:
        - Private
            - Named with leading underscore and camelCase format (e.g. _myName)
            - Optionally linked to inspector through [SerializeField]
            - Used to:
                - Store data
                - Store components sourced through GetComponent<>() method
        - Public
            - Named with camelCase format (e.g. myName)
            - Automatically linked to inspector
            - Uses:
                - Store components that cannot be sourced through
                  GetComponent() method.

        Note that public variables should only be used in place of the
        GameObject.Find() method. The Find() method grabs a GameObject from the
        Scene by name. As a result, it makes the script dependent on that 
        GameObject yet doesn't make that dependency clear outside of the 
        script. This interferes with unit testing. Instead, if objects need to
        be grabbed from the scene, use a public variable and link the object 
        through the inspector.
    */

    /// <summary>
    /// The following is an example of a private instance variable. It is used
    /// to store a reference to the Rigidbody component of the parent 
    /// GameObject and does not need to be visible from the inspector. 
    /// </summary>
    private Rigidbody _rigidbody;

    /// <summary>
    /// Indicator used to track if gravity is currently applied to the parent
    /// GameObject object. It is initialized to true because, by default, the 
    /// parent GameObject is under the influence of gravity.
    /// </summary>
    private bool _gravityOn = true;

    /// <summary>
    /// The following is an example of a private instance variable linked to
    /// the inspector through the use of the [SerializeField] parameter. It 
    /// represents the speed at which the parent GameObject will jump. The 
    /// initialized value of 30.0f can be overridden if changed in the 
    /// inspector.
    /// </summary>
    [SerializeField] private float _jumpSpeed = 30.0f;

    /// <summary>
    /// The following is an example of a public instance variable. Public
    /// variables are automatically visible from the inspector. Therefore, 
    /// the [SerializeField] parameter is not necessary. Public variables
    /// should only be used when the GetComponent<>() method is not an
    /// option. In this case, it is used because the PlayerPositionSystem
    /// is requred and cannot be found:
    /// - On the parent GameObject
    /// - Through interactions between GameObjects such as collision, 
    ///   trigger, or raycasting.
    /// 
    /// Let's assume that SampleScript needs to determine the location of 
    /// living players. The PlayerPositionSystem isn't actually used anywhere 
    /// else in this script. It has just been included for example purposes.
    /// </summary>
    public PlayerPositionSystem _playerPositionSystem;
    #endregion

    //=========================================================================
    #region MonoBehavior
    //=========================================================================
    /*
    MonoBehavior
        Functions/methods that have been defined by Unity should go in this
        section. Some common examples have been included. Do not include raw
        code in MonoBehavior methods. Instead, refactor raw code into private 
        helper methods that are called within your MonoBehavior methods. See
        "Helper method" section for more information. 
    */

    /// <summary>
    /// Called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        // Call a function here if it's important that it is initialized 
        // before everything else. 
    }
    
    /// <summary>
    /// Called before the first frame update.
    /// </summary>
    void Start()
    {
        // Call a function here if it's important that it is initialized
        // before the game starts. This is a good place for Init methods.
        // See "Helper methods section".
        Init(); 
    }

    /// <summary>
    /// Called once per frame.
    /// </summary>
    void Update()
    {
        // Call a function here if it needs to be ran every frame. For example,
        // a method that checks for user input. 
    }

    /// <summary>
    /// Called every physics update.
    /// </summary>
    private void FixedUpdate()
    {
        // Call a function here if it needs to be ran every physics update.
        // For example, a method that continually modifies player physics. 
    }
    #endregion

    //=========================================================================
    #region Instance methods
    //=========================================================================
    /*
    Instance methods
        Methods that belong to an instance of a GameObject component AND are to
        be called by an external script. Therefore, all instance methods are 
        public. Following Unity convention, instance methods should be named 
        using PascalCase (i.e. MyMethod). An instance methods are called 
        through dot notation. For example, an external script could call the 
        below method with "SampleScript.InfiniteJump()"
    */

    /// <summary>
    /// Toggles gravity and sends object flying vertically into the air.
    /// </summary>
    public void InfiniteJump()
    {
        ToggleGravity();
        Jump();
    }
    #endregion

    //=========================================================================
    #region Helper methods
    //=========================================================================
    /*
    Helper methods
        Methods that belong to an instance of a GameObject component AND are 
        not to be called by an external script. Therefore, all instance methods
        are private. Helper methods are used to refactor public methods such 
        that they are easier to read. As you can see from above, refactoring 
        InfiniteJump() made it more readable.
    */

    /// <summary>
    /// Init methods are used to populate instance variables with meaningful 
    /// data upon game start. In this case, we will use it to link the 
    /// "_rigidbody" varaible with the Ridigbody component of the parent 
    /// GameObject. Remember that, since GetComponent<>() was used in this 
    /// method, a corresponding [RequireComponent(typeof(Rigidbody))] entry is 
    /// required at the top of the script.
    /// </summary>
    private void Init()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Toggles object gravity on/off.
    /// </summary>
    private void ToggleGravity()
    {
        // Toggle gravity boolean
        _gravityOn = !_gravityOn;
        // Set "useGravity" to match current bool
        _rigidbody.useGravity = _gravityOn;
    }

    /// <summary>
    /// Jumps the object by applying a vertical force to the rigidbody.
    /// </summary>
    private void Jump()
    {
        // Create force vector in y direction
        Vector3 force = new Vector3(0, _jumpSpeed, 0);
        // Apply force to rigidbody as an instant impulse
        _rigidbody.AddForce(force, ForceMode.Impulse);
    }

    /// <summary>
    /// Doubles the input number. 
    /// </summary>
    /// <remarks>
    /// Remarks tags are useful for including additional information that is
    /// not necesssary for the summary description. For example, I'm using
    /// this remarks tag to indicate that the below method is just for
    /// example purposes and does not have anything to do with InfiniteJump().
    /// The following "param" and "returns" tags are required when a method 
    /// accepts an input parameter or returns a value respectively.
    /// <remarks>
    /// <param name="number"> 
    /// The number to be doubled.
    /// </param> 
    /// <returns>
    /// Returns twice the input number.
    /// </returns>
    private float DoubleInputFloat(float number)
    {
        return number * 2;
    }
    #endregion

    //=========================================================================
    #region Properties
    //=========================================================================
    /*
    Properties
        If you're familiar with object oriented programming, properties are 
        C#'s implementation of getters and setters. If you're not familiar, 
        properties are used in order to recieve/overwrite (i.e. get/set) 
        instance variable data. Perhaps a power-up is applied to the player 
        that increases their jump speed. In this case, the power-up would need 
        to set the _jumpSpeed variable of the player. This is made possible 
        through properties.

        Note that, in order to be accessed by external scripts, properties are
        public and, as such, are not named with a leading underscore. 
    */

    /// <summary>
    /// Property for the _jumpSpeed variable that indicates how hard the player
    /// jumps.
    /// </summary>
    public float JumpSpeed 
    {
        // External scripts can now check the value of the _jumpSpeed variable 
        // of this script. For example: "SampleScript.jumpSpeed"
        get { return _jumpSpeed; }
        // Note that, when defining the setter of a property, the setting value
        // is referred to as "value" as seen in the example below.
        // External scripts can now set the _jumpSpeed variable of this script.
        // For example: "SampleScript.jumpSpeed = 9999f"
        set { _jumpSpeed = value; }

        // Additionally, get and set can be made private by placing the 
        // "private" keyword in front of the definition. (i.e. private set)
    }
    #endregion
}
}
