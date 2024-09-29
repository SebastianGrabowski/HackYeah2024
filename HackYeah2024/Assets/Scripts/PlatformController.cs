using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
   [SerializeField] private float _maxDistance;
   [SerializeField] private float _minDistance;
   [SerializeField] private float _offsetY;
   [SerializeField] private Transform _player;
   [SerializeField] private Platform[] _platforms;
   
   private void Update()
   {
        int i = 0;
        foreach (var platform in _platforms)
        {
            i++;
            var distance = Vector3.Distance(_player.position, platform.transform.position);
            Debug.Log("distance: "+distance+" to platform: "+platform.gameObject.name);
            if(distance > _minDistance && distance < _maxDistance && _player.position.z > platform.transform.localPosition.y)
            {
               platform.transform.localPosition = new Vector3(0f, platform.transform.localPosition.y + _offsetY, 0f);
               platform.Setup();
            }
        }
   }
}
