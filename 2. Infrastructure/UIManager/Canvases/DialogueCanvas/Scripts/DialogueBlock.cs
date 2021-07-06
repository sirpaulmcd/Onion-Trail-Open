using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace EGS
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    /// <summary>
    /// A class that holds all the characteristics of one block of dialogue.
    /// Responsible for writing its block of dialogue to TextMeshProUGUI
    /// component. Requires a child gameObject of type image.
    /// </summary>
    public class DialogueBlock : MonoBehaviour
    {
        //=====================================================================
        #region Instance variables
        //=====================================================================
        /// <summary>
        /// The component which holds the text to be written to.
        /// </summary>
        private TextMeshProUGUI _textholder;
        /// <summary>
        /// The gameObject which holds the image being displayed on the canvas.
        /// </summary>
        private Image _imageHolder;
        /// <summary>
        /// IEnumerator reponsible for saving the write text function and is
        /// used in a coroutine.
        /// </summary>
        private IEnumerator _blockAppear;
        /// <summary>
        /// The time at which a dialogue block has started printing.
        /// </summary>
        private float _startedTime;
        /// <summary>
        /// The input string to display.
        /// </summary>
        [SerializeField] private string _input = "Default Text";
        /// <summary>
        /// The color of the dialogue text.
        /// </summary>
        [SerializeField] private Color _textColor = Color.black;
        /// <summary>
        /// The font of the text to be displayed.
        /// </summary>
        [SerializeField] private TMPro.TMP_FontAsset _textFont = default;
        /// <summary>
        /// The size of the text to be displayed.
        /// </summary>
        [SerializeField] private float _textSize = 14.0f;
        /// <summary>
        /// The delay in seconds between displaying each letter in the dialogue
        /// block (type-writer effect).
        /// </summary>
        [SerializeField] private float _delayBetweenLetters = 0.1f;
        /// <summary>
        /// The delay in seconds to wait after the block is displayed before
        /// moving to the next block. Requires waitfortime to be true.
        /// </summary>
        [SerializeField] private float _delayTillNextBlock = 0;
        /// <summary>
        /// The character sprite we want to display beside the text.
        /// </summary>
        [SerializeField] private Sprite _characterSprite = default;
        /// <summary>
        /// The sound of the volume
        /// </summary>
        [SerializeField] [Range(0, 1)] private float _typingSoundVolume = 0.5f;
        /// <summary>
        /// Inidcates whether the next dialogue is reached due to a timer
        /// rather than from receiving player input.
        /// </summary>
        [SerializeField] private bool _waitForTime = false;
        /// <summary>
        /// Inidcates whether the writing animation should be skipped.
        /// </summary>
        private bool _skipWriting = false;
        #endregion

        //=====================================================================
        #region Inspector Linked Components
        //=====================================================================
        /// <summary>
        /// The sound to be played with each typing animation.
        /// </summary>
        [SerializeField] private SFXClipName _typingSound = default;
        #endregion

        //=====================================================================
        #region Properties
        //=====================================================================
        /// <summary>
        /// Indicates whether the dialogue has finished writing text to
        /// textholder.
        /// </summary>
        public bool Finished { get; protected set; }
        /// <summary>
        /// Indicates whether or not the player has opted to skip the writing
        /// animation of the current block of dialogue. If _waitForTime is
        /// true, skipWriting cannot be set to true.
        /// </summary>
        public bool SkipWriting
        {
            get { return _skipWriting; }
            set
            {
                if (_waitForTime) { _skipWriting = false; }
                else { _skipWriting = value; }
            }
        }
        #endregion

        //=====================================================================
        #region MonoBehaviour
        //=====================================================================
        /// <summary>
        /// Called when an object is first activated before Start.
        /// </summary>
        private void Awake()
        {
            InitOnAwake();
        }

        /// <summary>
        /// Called when the object is Enabled.
        /// </summary>
        private void OnEnable()
        {
            StartWritingText();
        }
        #endregion

        //=====================================================================
        #region Public methods
        //=====================================================================
        /// <summary>
        /// Initialises the component in Awake().
        /// </summary>
        private void InitOnAwake()
        {
            ResetBlock();
            InitVars();
            CheckMandatoryComponents();
        }

        /// <summary>
        /// Sources and initializes component variables.
        /// </summary>
        private void InitVars()
        {
            _imageHolder = GetComponentInChildren<Image>();
            _imageHolder.sprite = _characterSprite;
            _imageHolder.preserveAspect = true;
        }

        /// <summary>
        /// Ensures mandatory components are accounted for.
        /// </summary>
        private void CheckMandatoryComponents()
        {
            Assert.IsNotNull(_textholder, gameObject.name + " is missing _textholder");
            Assert.IsNotNull(_imageHolder, gameObject.name + " is missing _imageholder");
        }

        /// <summary>
        /// Writes text from the input parameter to the textholder parameter
        /// with characteristics based on the parameters. Has a typewriter '
        /// effect.
        /// </summary>
        /// <param name="inputText">
        /// The input string to display.
        /// </param>
        /// <param name="textHolder">
        /// The TextMeshProUGUI component to write to.
        /// </param>
        /// <param name="textColour">
        /// The text color to be displayed</param>
        /// <param name="textFont">
        /// The text font to be displayed
        /// </param>
        /// <param name="charDelay">
        /// The time in seconds between the display of each character in the
        /// input string (typewriter delay)
        /// </param>
        /// <param name="delayBeforeNextBlock">
        /// The time in seconds before the next dialogue item is displayed
        /// </param>
        /// <param name="waitForTime">
        /// If true, the dialogue will wait for the delay before displaying the
        /// next dialogue. If false, the dialogue will wait for the next key
        /// pressed.
        /// </param>
        /// <param name="textSize">
        /// The size of the text to display.
        /// </param>
        /// <returns>
        /// IEnumerator that can be placed in a Quoratine to write the text.
        /// </returns>
        protected IEnumerator WriteText(string inputText, TextMeshProUGUI
            textHolder, Color textColour, TMPro.TMP_FontAsset textFont,
            float charDelay, float delayBeforeNextBlock,
            bool waitForTime, float textSize)
        {
            textHolder.font = textFont;
            textHolder.color = textColour;
            textHolder.fontSize = textSize;
            for (int i = 0; i < inputText.Length; i++)
            {
                if (SkipWriting && !waitForTime) { break; }
                textHolder.text += inputText[i];
                PlayTypingSound();
                yield return new WaitForSeconds(charDelay);
            }
            if (SkipWriting) { textHolder.text = inputText; }
            if (waitForTime) { yield return new WaitForSeconds(delayBeforeNextBlock); }
            Finished = true;
        }

        /// <summary>
        /// Starts writing text from the serialized fields in Unity to the
        /// textholder.
        /// </summary>
        private void StartWritingText()
        {
            ResetBlock();
            _startedTime = Time.time;
            StartCoroutine(WriteText(
                _input, _textholder, _textColor, _textFont,
                _delayBetweenLetters, _delayTillNextBlock,
                _waitForTime, _textSize
            ));
        }

        /// <summary>
        /// Resets the dialogue block to initial state. Unfinished and empty.
        /// </summary>
        private void ResetBlock()
        {
            SkipWriting = false;
            Finished = false;
            _textholder = GetComponent<TextMeshProUGUI>();
            _textholder.text = "";
        }

        /// <summary>
        /// Plays the typing sound at the position of the main camera.
        /// </summary>
        private void PlayTypingSound()
        {
            AudioManager.Instance.PlaySFX(_typingSound, _typingSoundVolume);
        }
        #endregion
    }
}
