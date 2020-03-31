using UnityEngine;

public class TriggerActions : MonoBehaviour
{
    [SerializeField] private PlayerBehaviour _playerBeh;
    [SerializeField] private Transform _instantiationPointParticles;
    [SerializeField] private ParticleSystem _attackParticleEffect;
    public void ResetAnimTrigger()
    {
        _playerBeh.AnimController.ResetTrigger("FastAttack");
        _playerBeh.AnimController.ResetTrigger("HeavyAttack");
        _playerBeh.AnimController.ResetTrigger("Hit");
        _playerBeh.AnimController.ResetTrigger("IsBlocking");
        _playerBeh.IsDamageDone = false;
        _playerBeh.IsAttacking = false;
        _playerBeh.IsBlocking = false;
    }

    public void SetBlocking()
    {
        _playerBeh.IsBlocking = true;
    }

    public void ResetBlocking()
    {
        _playerBeh.IsBlocking = false;
    }

    public void ResetTriggers()
    {
        foreach (var trigger in _playerBeh.AttackTriggers)
        {
            trigger.SetActive(false);
        }
    }

    public void SetTriggers()
    {
        foreach (var trigger in _playerBeh.AttackTriggers)
        {
            trigger.SetActive(true);
        }
    }

    public void TriggerParticles()
    {
        GameObject obj = Instantiate(_attackParticleEffect.gameObject, _instantiationPointParticles);
        obj.transform.position = _instantiationPointParticles.transform.position;
        obj.transform.SetParent(null);
        Destroy(obj, _attackParticleEffect.main.duration);
    }
}
