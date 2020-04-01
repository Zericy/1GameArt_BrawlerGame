using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GUIController : MonoBehaviour
{
    [SerializeField] private Slider _healthbarPlayer1;
    [SerializeField] private Slider _healthbarPlayer2;

    [SerializeField] private Image _imagePlayer1;
    [SerializeField] private Image _imagePlayer2;

    [SerializeField] private TextMeshProUGUI _textPlayer1;
    [SerializeField] private TextMeshProUGUI _textPlayer2;

    private PlayerBehaviour _player1PB;
    private PlayerBehaviour _player2PB;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartRoutine());
    }

    private void Initialize()
    {
        _player1PB = GameObject.FindGameObjectWithTag("Player1").GetComponentInChildren<PlayerBehaviour>();
        _player2PB = GameObject.FindGameObjectWithTag("Player2").GetComponentInChildren<PlayerBehaviour>();

        _healthbarPlayer1.maxValue = _player1PB.CurrentHP;
        _healthbarPlayer2.maxValue = _player2PB.CurrentHP;

        _healthbarPlayer1.value = _healthbarPlayer1.maxValue;
        _healthbarPlayer2.value = _healthbarPlayer2.maxValue;

        _player1PB.OnChangeCurrentHealth += ChangePlayerHealthbar;
        _player2PB.OnChangeCurrentHealth += ChangePlayerHealthbar;

        _imagePlayer1.sprite = _player1PB.PlayerStats.CharacterImage;
        _imagePlayer2.sprite = _player2PB.PlayerStats.CharacterImage;

        _textPlayer1.text = _player1PB.PlayerStats.CharacterName;
        _textPlayer2.text = _player2PB.PlayerStats.CharacterName;
    }

    private void ChangePlayerHealthbar(object sender, EventArgs e)
    {
        _healthbarPlayer2.value = _player2PB.CurrentHP;
        _healthbarPlayer1.value = _player1PB.CurrentHP;
    }
    IEnumerator StartRoutine()
    {
        yield return new WaitForEndOfFrame();
        Initialize();
    }
}
