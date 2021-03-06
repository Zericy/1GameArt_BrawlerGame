﻿using System;
using System.Collections;
using UnityEngine;
using EZCameraShake;

[RequireComponent(typeof(CharacterController))]
public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField] private CharacterStats _characterStats;
    public CharacterStats PlayerStats { get => _characterStats; }

    [SerializeField] private Animator _animController;
    public Animator AnimController { get => _animController; set { _animController = value; } }

    [SerializeField] private GameObject[] _attackTriggers;
    public GameObject[] AttackTriggers { get => _attackTriggers; set { _attackTriggers = value; } }

    [SerializeField] private PunchHitDetection[] _hitDetection;
    [SerializeField] private GameObject _floatingPoints;
    [SerializeField] private Transform _instantiationPointFloatingPoints;

    [SerializeField] private Transform _instantiationPointParticles;
    [SerializeField] private ParticleSystem _hitParticleEffect;

    private int _playerNumber = 1;
    public int PlayerNumber { get => _playerNumber; set { _playerNumber = value; } }    
    private float _currentHP = 0;
    public float CurrentHP => _currentHP;
    private bool _isDamageDone = false;
    public bool IsDamageDone { get => _isDamageDone; set { _isDamageDone = value; } }
    private bool _isAttacking = false;
    public bool IsAttacking { get => _isAttacking; set { _isAttacking = value; } }
    private bool _isBlocking = false;
    public bool IsBlocking { get => _isBlocking; set { _isBlocking = value; } }

    public event EventHandler OnChangeCurrentHealth;

    private CharacterController _characterController;
    private Vector3 _direction;

    private string _horizontalAxis, _normalAttackButton, _heavyAttackButton, _blockButton;
    private bool _canBlock = true;
    private bool _hasInitialized = false;
    private float _doDamageValue = 0;
    private Vector2 _screenBounds;
    private float _objectWidth;

    private Vector3 impact;

    [SerializeField] private GameObject _shootingParticle;
    [SerializeField] private GameObject _shootingParticlePosition;

    [SerializeField] private float _timeProjectileInstantiation;

    // Start is called before the first frame update
    public void Initialize()
    {
        transform.parent = null;
        this.tag = "Player" + _playerNumber;
        _characterStats.Initialize();
        _currentHP = _characterStats.Health;
        
        _horizontalAxis = "Player" + _playerNumber + "_HorizontalAxis";
        _normalAttackButton = "Player" + _playerNumber + "_AButton";
        _heavyAttackButton = "Player" + _playerNumber + "_BButton";
        _blockButton = "Player" + _playerNumber + "_XButton";

        foreach (var trigger in _attackTriggers)
        {
            trigger.SetActive(false);
        }

        _characterController = GetComponent<CharacterController>();

        _screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        _objectWidth = GetComponent<Collider>().bounds.size.x;

        _hasInitialized = true;
    }

    void Update()
    {
        if (_hasInitialized && GameController.IsGamePlaying)
        {
            if (Input.GetButtonDown(_normalAttackButton) && !_isAttacking)
            {
                _animController.SetTrigger("FastAttack");
                _doDamageValue = _characterStats.NormalAttackDamage;
                _isAttacking = true;
            }
            if (Input.GetButtonDown(_heavyAttackButton) && !_isAttacking)
            {
                if (_shootingParticle != null)
                {
                    StartCoroutine(FireProjectile(_timeProjectileInstantiation));
                }

                _animController.SetTrigger("HeavyAttack");
                _doDamageValue = _characterStats.HeavyAttackDamage;
                _isAttacking = true;
            }
            if(Input.GetButtonDown(_blockButton) && !_isAttacking && _canBlock)
            {
                _animController.SetTrigger("IsBlocking");
                StartCoroutine(TimeTillBlock());
            }
            _direction = new Vector3(Input.GetAxis(_horizontalAxis), 0, 0);

            foreach (var hit in _hitDetection)
            {
                hit.OnHit += DoDamage;
            }

            if (impact.magnitude > 0.2) _characterController.Move(impact * Time.deltaTime);
            impact = Vector3.Lerp(impact, Vector3.zero, 2 * Time.deltaTime);

        }
    }
    IEnumerator FireProjectile(float time)
    {
        yield return new WaitForSeconds(time);
        Vector3 position = this.transform.position + new Vector3(2, 4, 0);
        GameObject particleObj = Instantiate(_shootingParticle, position, Quaternion.Euler(0, 90, 0));
        particleObj.transform.SetParent(this.transform);
    }

    private void TriggerFightEnd()
    {
        _animController.SetTrigger("HasFainted");
        GameController.ChangeGameState(false);
        StartCoroutine(GameController.GoToCharacterSelectScreen());
    }

    private IEnumerator TimeTillBlock()
    {
        _canBlock = false;
        yield return new WaitForSeconds(_characterStats.TimeUntilNextBlock);
        _canBlock = true;
    }

    public void TakeDamage(float damage, float force, Vector3 direction)
    {
        if (!_isBlocking)
        {
            _currentHP -= _characterStats.TakeDamage(damage);

            if (_currentHP > 0)
                _animController.SetTrigger("Hit");
            else
                TriggerFightEnd();

            InstantiateDamage();

            if (_currentHP < 0) _currentHP = 0;
            OnChangeCurrentHealth?.Invoke(this, EventArgs.Empty);

            impact += direction * force / _characterStats.Weight;
         
            //Debug.Log($"Player{_playerNumber} impact value = {impact}");
        }
        if (_instantiationPointParticles != null)
        {
            GameObject obj = Instantiate(_hitParticleEffect.gameObject, _instantiationPointParticles);
            obj.transform.position = _instantiationPointParticles.transform.position;
            obj.transform.SetParent(null);
            Destroy(obj, _hitParticleEffect.main.duration);
        }
    }

    private void InstantiateDamage()
    {
        GameObject go = Instantiate(_floatingPoints, transform.position, Quaternion.identity);
        go.transform.position = _instantiationPointFloatingPoints.position;
        go.transform.parent = null;
        FloatingDamageBehaviour fdb = go.GetComponentInChildren<FloatingDamageBehaviour>();
        fdb.DamageAmount = _characterStats.ActualDamageTaken;
        fdb.Initialize();
    }

    private void DoDamage(GameObject go, Vector3 direction)
    {
        if (!_isDamageDone)
        {
            go?.GetComponent<PlayerBehaviour>()?.TakeDamage(_doDamageValue, _characterStats.ActualDamageTaken, direction);
            CameraShaker.Instance.ShakeOnce(_doDamageValue / 5, (_doDamageValue/5)*2, 0.25f, 0.5f);
            
            _isDamageDone = true;
        }
    }

    private void FixedUpdate()
    {
        if (_hasInitialized && GameController.IsGamePlaying)
        {
            if (!_isAttacking&&!_isBlocking)
            {
                WalkAndIdle();
            }
        }
        if(_hasInitialized && !GameController.IsGamePlaying) _animController.SetFloat("WalkSpeed", 0);
    }

    private void WalkAndIdle()
    {
        Vector3 speed = _direction * _characterStats.CharacterSpeed * Time.deltaTime;
        if (!_characterController.isGrounded)
            speed -= transform.up * 9.81f * Time.deltaTime;
                
        _characterController.Move(speed);

        if (_playerNumber % 2 == 0)
            _animController.SetFloat("WalkSpeed", -_direction.x);
        else
            _animController.SetFloat("WalkSpeed", _direction.x);
        
        Clamp();
    }

    private void Clamp()
    {
        Vector3 viewPos = transform.position;
        viewPos = new Vector3
        (
            FloatClamp(viewPos.x, _screenBounds.x + _objectWidth, -(_screenBounds.x + _objectWidth)),
            viewPos.y,
            viewPos.z
        );

        transform.position = viewPos;
    }

    private float FloatClamp(float value, float min, float max)
    {
        return (value <= min) ? min : (value >= max) ? max : value;
    }
}