using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LemurEntity : MonoBehaviour
{
    [SerializeField] private float _destroyForce;
    public Collider Collider;
    public PlayerController PlayerController;
    public Vector3 Target;
    public bool IsInTeam;

    private Rigidbody _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    public void SetInTeam()
    {
        IsInTeam = true;
        Collider.isTrigger = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Obstacle"))
        {
            // destroy effect to setup later

            // _rigidbody.isKinematic = false;
            // _rigidbody.AddForce(transform.forward * _destroyForce, ForceMode.Impulse);

            if(PlayerController == null)
                PlayerController = FindObjectOfType<PlayerController>();
            
            PlayerController.RemoveLemur(this);
        }

        if(IsInTeam && other.TryGetComponent(out LemurEntity lemurEntity))
        {
            PlayerController.AddLemur(lemurEntity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsInTeam)
            return;

        var dir = (PlayerController.TargetPos + Target) - transform.position;
        _rigidbody.velocity = dir.normalized  * 10.0f * Mathf.Min(1, dir.magnitude);
    }
}
