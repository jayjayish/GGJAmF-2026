using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameFlowManager : MonoBehaviour
{     
    [SerializeField] public GameObject TextCanvas;
    [SerializeField] public TMP_Text TutorialText;
    [SerializeField] public GameObject PaletteMob;
    [SerializeField] public GameObject Boss;

    private bool _showingText = true;
    private float _minTextDisplayTime = 2f;
    private float _textDisplayTime = 0f;

    private int _textToDisplay = 0;
    private bool _movementInputReceived = false;
    private bool _attackInputReceived = false;
    private bool _mouseButtonInputReceived = false;

    public List<string> tutorialTexts = new List<string>
    {
        "Press WASD to move",
        "Aim with the MOUSE and press SPACEBAR to splash paint",
        "Use left and right MOUSE BUTTONS to change color of paint"
    };

    private void Start()
    {
        _showingText = true;
        _textToDisplay = 0;
        _textDisplayTime = 0f;
        _movementInputReceived = false;
        _attackInputReceived = false;
        _mouseButtonInputReceived = false;

        // Clear initially; Update() will set the first line.
        TutorialText.text = "";
        // Display tutorial text then call BossStart
        
    }

    private void OnEnable()
    {
        InputManager.AddMoveAction(OnMoveInput);
        InputManager.AddAttackDownAction(OnAttackDown);
        InputManager.AddLeftDownAction(OnMouseButtonDown);
        InputManager.AddRightDownAction(OnMouseButtonDown);
    }

    private void OnDisable()
    {
        InputManager.RemoveMoveAction(OnMoveInput);
        InputManager.RemoveAttackDownAction(OnAttackDown);
        InputManager.RemoveLeftDownAction(OnMouseButtonDown);
        InputManager.RemoveRightDownAction(OnMouseButtonDown);
    }

    private void Update()
    {
        if (_showingText)
        {
            // if (TutorialText == null || tutorialTexts == null || tutorialTexts.Count == 0)
            // {
            //     _showingText = false;
            //     BossStart();
            //     return;
            // }

            // Show the current line for at least _minTextDisplayTime seconds.
            if (_textDisplayTime <= 0f)
            {
                _textToDisplay = Mathf.Clamp(_textToDisplay, 0, tutorialTexts.Count - 1);
                TutorialText.text = tutorialTexts[_textToDisplay];

                // Reset per-line input gates so earlier inputs don't skip steps.
                if (_textToDisplay == 0)
                {
                    _movementInputReceived = false;
                }
                else if (_textToDisplay == 1)
                {
                    _attackInputReceived = false;
                }
                else if (_textToDisplay == 2)
                {
                    _mouseButtonInputReceived = false;
                }
            }

            _textDisplayTime += Time.deltaTime;

            // Special case: first text stays until the player moves AND minimum time passes.
            if (_textToDisplay == 0)
            {
                if (_textDisplayTime < _minTextDisplayTime || !_movementInputReceived)
                {
                    return;
                }
            }
            // Special case: second text stays until attack pressed AND minimum time passes.
            else if (_textToDisplay == 1)
            {
                if (_textDisplayTime < _minTextDisplayTime || !_attackInputReceived)
                {
                    return;
                }
            }
            // Special case: third text stays until left or right mouse pressed AND minimum time passes.
            else if (_textToDisplay == 2)
            {
                if (_textDisplayTime < _minTextDisplayTime || !_mouseButtonInputReceived)
                {
                    return;
                }
            }
            else
            {
                if (_textDisplayTime < _minTextDisplayTime)
                {
                    return;
                }
            }

            _textDisplayTime = 0f;
            _textToDisplay++;

            if (_textToDisplay >= tutorialTexts.Count)
            {
                _showingText = false;
                TutorialText.text = "";
                TextCanvas.SetActive(false);
                // Go to next thing
                // BossStart();
            }
        }
    }

    private void OnMoveInput(Vector2 direction)
    {
        if (direction.sqrMagnitude > 0.001f)
        {
            _movementInputReceived = true;
        }
    }

    private void OnAttackDown()
    {
        _attackInputReceived = true;
    }


    private void OnMouseButtonDown()
    {
        _mouseButtonInputReceived = true;
    }


    public void BossStart() {
        
    }
}