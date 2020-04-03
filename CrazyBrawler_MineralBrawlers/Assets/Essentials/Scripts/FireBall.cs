using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    [SerializeField] private float _speed = 5;
    private PlayerBehaviour _pb;
    private float _time;
    private Vector3 _direction;
    private void Start()
    {
        if (this.transform.root.CompareTag("Player1"))
        {
            _direction = Vector3.right;
        }
        else
        {
            _direction = Vector3.left;
            this.transform.rotation = Quaternion.Euler(0, -90, 0);
        }
    }
    void Update()
    {
        this.transform.position += _direction * _speed * Time.deltaTime;
        _time += Time.deltaTime;
        if (_time > 10)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_time > 0.1f)
        {
            if (other.CompareTag("projectile"))
            {
                Destroy(other.gameObject);
                Destroy(this.gameObject);
            }
            if (this.transform.root.CompareTag("Player1"))
            {
                if (other.CompareTag("Player2"))
                {
                    //PlayerBehaviour _pb = this.transform.root.GetComponent<PlayerBehaviour>();
                    PlayerBehaviour pb = other.transform.root.GetComponent<PlayerBehaviour>();
                    pb.TakeDamage(50, 10, Vector3.right);
                    Destroy(this.gameObject);
                }
            }
            if (this.transform.root.CompareTag("Player2"))
            {
                if (other.CompareTag("Player1"))
                {
                    //PlayerBehaviour _pb = this.transform.root.GetComponent<PlayerBehaviour>();
                    PlayerBehaviour pb = other.transform.root.GetComponent<PlayerBehaviour>();
                    pb.TakeDamage(50, 10, Vector3.right);
                    Destroy(this.gameObject);
                }
            }

        }
    }
}
