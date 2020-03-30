using System;
using UnityEngine;
using EZCameraShake;

public class PunchHitDetection : MonoBehaviour
{
    public event Action<GameObject, Vector3> OnHit;
    [SerializeField] private bool _effectsOn;
    [SerializeField] private Transform _instantiationPointParticles;
    [SerializeField] private GameObject _particle;
    [SerializeField] private float _particleDuration;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent != transform.root && other.CompareTag("hitbox") && !other.transform.root.CompareTag(transform.root.tag))
        { 
            OnHit?.Invoke(other.transform.root.gameObject, transform.root.forward);
            if(_effectsOn)
            {
                GameObject obj = Instantiate(_particle, _instantiationPointParticles);
                obj.transform.position = _instantiationPointParticles.transform.position;
                obj.transform.parent = null;
                Destroy(obj, _particleDuration);
                CameraShaker.Instance.ShakeOnce(2, 10, 0.25f, 0.5f);
            }
        }
    }
}
