using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public Transform Point;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Obs A");
        if(other.TryGetComponent(out LemurEntity lemurEntity))
        {
        Debug.Log("Obs B    ");
            lemurEntity.OnObstacleCollision(Point.position);
        }else if(other.transform.parent != null && other.transform.parent.TryGetComponent(out LemurEntity lemurEntity2))
        {
            
            lemurEntity2.OnObstacleCollision(Point.position);
        }else if(other.transform.parent.parent != null && other.transform.parent.parent.TryGetComponent(out LemurEntity lemurEntity3))
        {
            
            lemurEntity3.OnObstacleCollision(Point.position);
        }
    }
}
