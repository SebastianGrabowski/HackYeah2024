using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LemurEntity : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject _teamView;
    [SerializeField] private GameObject _notTeamView;
    [SerializeField] private Renderer _teamR;
    [SerializeField] private float _destroyForce;
    public Collider Collider;
    public Collider ColliderTeam;
    public Vector3 Target;
    public bool IsInTeam;

    private Rigidbody _rigidbody;
    private bool _collided;

    private float _animTime;
    private float _animMinMax;

    private void Awake()
    {
        _animTime = Random.Range(35.0f, 45.0f);
        _animMinMax = Random.Range(35.0f, 45.0f);
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

    public void Kill(Vector3 point)
    {
        StartCoroutine(KillUpdate(point));
    }

    private IEnumerator KillUpdate(Vector3 point)
    {
        var t = 0.0f;
        var maxt = 0.4f;
        var startPos = transform.position;
        while(t < maxt)
        {
            t += Time.unscaledDeltaTime;
            _teamR.material.color = Color.Lerp(Color.white, Color.black, t/maxt);
            transform.position = Vector3.Lerp(startPos, point, t/maxt);
            yield return null;
        }
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
            PlayerController.Instance.SetLemursToFree(true, lemurEntity);
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
            PlayerController.Instance.SetLemursToFree(false, lemurEntity);
    }

    public void OnObstacleCollision(Vector3 point)
    {
        if(_collided)
            return;

        PlayerController.Instance.RemoveLemur(this);

        _animator.enabled = true;
        _animator.SetTrigger("Caught");
        Kill(point);

        _rigidbody.constraints = RigidbodyConstraints.None;
        _rigidbody.AddForce(Vector3.up * _destroyForce);
        _collided = true;
        
    }

    // Update is called once per frame
    void Update()
    {
        var dir = (PlayerController.Instance.TargetPos + Target) - transform.position;

        if (!IsInTeam)
        {
            _rigidbody.velocity = dir.normalized  * 0.2f * Mathf.Min(1, dir.magnitude);
            return;
        }

        _rigidbody.velocity = dir.normalized  * 10.0f * Mathf.Min(1, dir.magnitude);

        _teamView.transform.rotation = Quaternion.Euler(
            _teamView.transform.rotation.eulerAngles.x, 
            _teamView.transform.rotation.eulerAngles.y, 
            Mathf.PingPong(Time.unscaledTime*_animTime, _animMinMax)-(_animMinMax/2.0f)// transform.rotation.z
            );
        _teamView.transform.localPosition += Time.deltaTime * 0.8f * (Vector3.up * Mathf.Sin((Time.unscaledTime+transform.position.x) * 15.0f));
    }
}
