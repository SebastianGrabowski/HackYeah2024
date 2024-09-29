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
    public PlayerController PlayerController;
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
            PlayerController.SetLemursToFree(true, lemurEntity);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(IsInTeam && other.TryGetComponent(out LemurEntity lemurEntity))
        {
            PlayerController.SetLemursToFree(false, lemurEntity);
        }
    }

    public void OnObstacleCollision()
    {
        if(_collided)
            return;

        if(PlayerController == null)
            PlayerController = FindObjectOfType<PlayerController>();

        PlayerController.RemoveLemur(this);

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

        var dir = (PlayerController.TargetPos + Target) - transform.position;
        _rigidbody.velocity = dir.normalized  * 10.0f * Mathf.Min(1, dir.magnitude);
    }
}
