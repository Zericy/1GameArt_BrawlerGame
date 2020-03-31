using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonBehaviour : MonoBehaviour
{
    private GameObject _characterObject;
    public GameObject CharacterObject { get => _characterObject; set { _characterObject = value; } }

    [SerializeField] private CharacterButtonSetup _characterSetup;
    public CharacterButtonSetup CharacterSetup { get => _characterSetup; set { _characterSetup = value; } }

    private int _characterNumber;
    public int CharacterNumber 
    { 
        get => _characterNumber; 
        set 
        { _characterNumber = value;
            _text.text = "Character " + _characterNumber;
        } 
    }

    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private bool _isCharacterSelectButton, _isSceneChangingButton;
    [SerializeField] private string _sceneNameToLoad;
    [SerializeField] private Image _image;
    [SerializeField] private Sprite[] _sprites;

    public ButtonBehaviour(GameObject characterObject, CharacterButtonSetup characterSetup, int characterNumber)
    {
        _characterObject = characterObject;
        _characterSetup = characterSetup;
        _characterNumber = characterNumber;
    }

    public void ChangeCharacter(int playerNumber)
    {
        _characterSetup.ChangePlayerCharacter(playerNumber, _characterObject);

        if (playerNumber == 1) _characterSetup.IsPlayer1LockedIn = true;
        else if (playerNumber == 2) _characterSetup.IsPlayer2LockedIn = true;

        if (_characterSetup.IsPlayer1LockedIn && _characterSetup.IsPlayer2LockedIn) _characterSetup.IsLockedIn = true;
    }

    public void TaskOnClick(int playerNumber)
    {
        if (_isSceneChangingButton) SetSceneInSelectScreen(_sceneNameToLoad);
        else if (_isCharacterSelectButton) ChangeCharacter(playerNumber);
    }

    public void TaskOnHover()
    {
        if(_sprites.Length>=1)
            _image.sprite = _sprites[1];
    }

    public void ResetSprite()
    {
        if (_sprites.Length >= 1)
            _image.sprite = _sprites[0];
    }

    public void SetSceneInSelectScreen(string sceneName)
    {
        if (_characterSetup.IsLockedIn)
        {
            GameController.ChangeGameState(true);
        }
    }
}
