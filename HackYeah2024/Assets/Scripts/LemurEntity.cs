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
    public Collider ColliderTeam;
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

    private void OnEnable()
    {
        PlayerController.OnRight += RightHandler;
        PlayerController.OnLeft += LeftHandler;
    }

    private void OnDisable()
    {
        PlayerController.OnRight -= RightHandler;
        PlayerController.OnLeft -= LeftHandler;
    }

    private void LeftHandler()
    {
        if(!IsInTeam) return;
        _teamView.transform.localScale = Vector3.one * 0.7f;
        _notTeamView.transform.localScale = Vector3.one * 0.7f;
    }

    private void RightHandler()
    {
        if(!IsInTeam) return;
        var inv = new Vector3(-1.0f, 1.0f, 1.0f) * 0.7f;
        _teamView.transform.localScale = inv;
        _notTeamView.transform.localScale = inv;
    }


    public void RefreshView()
    {
        _teamView.SetActive(IsInTeam);
        _notTeamView.SetActive(!IsInTeam);
    }

    public void SetInTeam()
    {
        IsInTeam = true;
        ColliderTeam.enabled = true;
        Collider.enabled = false;
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
            StartCoroutine(SetLemursToFreeNull(lemurEntity));
        }
    }

    private IEnumerator SetLemursToFreeNull(LemurEntity lemurEntity)
    {
        yield return new WaitForSeconds(0.5f);
            PlayerController.SetLemursToFree(false, lemurEntity);
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
        var dir = (PlayerController.TargetPos + Target) - transform.position;

        if (!IsInTeam)
        {
            _rigidbody.velocity = dir.normalized  * 0.1f * Mathf.Min(1, dir.magnitude);
            return;
        }

        _rigidbody.velocity = dir.normalized  * 10.0f * Mathf.Min(1, dir.magnitude);
    }
}
