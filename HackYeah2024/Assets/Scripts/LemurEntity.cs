using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LemurEntity : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject _teamView;
    [SerializeField] private GameObject _notTeamView;
    [SerializeField] private float _destroyForce;

    public Collider Collider;
    public Vector3 Target;
    public bool IsInTeam;

    private Rigidbody _rigidbody;
    private bool _collided;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        RefreshView();
    }

    public void RefreshView()
    {
        _teamView.SetActive(IsInTeam);
        _notTeamView.SetActive(!IsInTeam);
    }

    public void SetInTeam()
    {
        IsInTeam = true;
        Collider.isTrigger = false;
        RefreshView();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(IsInTeam && other.TryGetComponent(out LemurEntity lemurEntity))
        {
            PlayerController.Instance.SetLemursToFree(true, lemurEntity);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(IsInTeam && other.TryGetComponent(out LemurEntity lemurEntity))
        {
            PlayerController.Instance.SetLemursToFree(false, lemurEntity);
        }
    }

    public void OnObstacleCollision()
    {
        if(_collided)
            return;

        PlayerController.Instance.RemoveLemur(this);

        _animator.enabled = true;
        _animator.SetTrigger("Caught");

        _rigidbody.constraints = RigidbodyConstraints.None;
        _rigidbody.AddForce(Vector3.up * _destroyForce);
        _collided = true;
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!IsInTeam)
            return;

        var dir = (PlayerController.Instance.TargetPos + Target) - transform.position;
        _rigidbody.velocity = dir.normalized  * 10.0f * Mathf.Min(1, dir.magnitude);
    }
}
