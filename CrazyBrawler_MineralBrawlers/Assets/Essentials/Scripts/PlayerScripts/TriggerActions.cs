using UnityEngine;

public class TriggerActions : MonoBehaviour
{
    [SerializeField] private PlayerBehaviour _playerBeh;
    [SerializeField] private Transform _instantiationPointParticles;
    [SerializeField] private ParticleSystem _attackParticleEffect;
    [SerializeField] private AudioSource _audioSource;

    [Header("Audioclips")]
    [SerializeField] private AudioClip _hitSound;
    [SerializeField] private AudioClip _quickAttackSound;
    [SerializeField] private AudioClip _heavyAttackSound;
    [SerializeField] private AudioClip _heavyAttackWindUpSound;
    [SerializeField] private AudioClip _deathSound;
    [SerializeField] private AudioClip _selectSound;
    [SerializeField] private AudioClip _idleSound;
    [SerializeField] private AudioClip _blockSound;
    [SerializeField] private AudioClip[] _footStepSounds;
    [SerializeField] private AudioClip[] _optionalExtras;




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
    public void PlayHitSound()
    {
        _audioSource.PlayOneShot(_hitSound);
    }
    public void PlayHeavySound()
    {
        _audioSource.PlayOneShot(_heavyAttackSound);
    }
    public void PlayQuickSound()
    {
        _audioSource.PlayOneShot(_quickAttackSound);
    }
    public void PlayDeathSound()
    {
        _audioSource.PlayOneShot(_deathSound);
    }
    public void PlayPickSound()
    {
        _audioSource.PlayOneShot(_selectSound);
    }
    public void PlayBlockSound()
    {
        _audioSource.PlayOneShot(_blockSound);
    }
    public void PlayIdleSound()
    {
        _audioSource.PlayOneShot(_idleSound);
    }

    public void PlayWindUpSound()
    {
        _audioSource.PlayOneShot(_heavyAttackWindUpSound);
    }

    private void PlayStepSound()
    {
        AudioClip clip = GetRandomClip(_footStepSounds);
        _audioSource.PlayOneShot(clip);
    }

    private AudioClip GetRandomClip(AudioClip[] audioClips)
    {
        return audioClips[UnityEngine.Random.Range(0, audioClips.Length)];
    }

    public void PlayExtraSound(int i)
    {
        _audioSource.PlayOneShot(_optionalExtras[i]);
    }
}
