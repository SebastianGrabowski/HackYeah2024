using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out LemurEntity lemurEntity))
        {
            lemurEntity.OnObstacleCollision();
        }
    }
}
