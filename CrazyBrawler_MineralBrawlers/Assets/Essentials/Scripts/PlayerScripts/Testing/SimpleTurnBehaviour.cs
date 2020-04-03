using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTurnBehaviour : MonoBehaviour
{
    public float RotationSpeed = 25f;
    public Vector3 TurnAxis = Vector3.forward;
    private void Update()
    {
        this.transform.Rotate(TurnAxis * RotationSpeed * Time.deltaTime);
    }
}
