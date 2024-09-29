using System;
using System.Collections;
using System.Collections.Generic;
using Game.UI;
using Game.UI.Fade;
using UnityEngine;

// public enum MoveType
// {
//     Left,
//     Right,
//     Up,
//     Down
// }

[System.Serializable]
public struct MoveData
{
    public string Keyword;
    public Vector3 MoveOffset;
}

public class PlayerController : MonoBehaviour
{
    public event Action OnTeamChanged;
    public event Action OnFormationChanged;
    public static event Action OnLeft;
    public static event Action OnRight;

    [SerializeField] private Transform _cam;
    [SerializeField] private Transform _camS;
    [SerializeField] private string _freeKeyword;
    [SerializeField] private string _resetKeyword;
    [SerializeField] private float _removeTime = 0.75f;
    [SerializeField] private float _speed;
    [SerializeField] private float _minLeftOffset = -5;
    [SerializeField] private float _maxRightOffset = 5;
    [SerializeField] private LemurEntity _lemurPrefab;
    [SerializeField] private Transform _lemursParent;
    [SerializeField] private MoveData[] _moveData;
    [SerializeField] private Transform _target;
    [SerializeField] private PanelsController _panelsController;
    [SerializeField] private FadeController _fadeController;
    [SerializeField] private FormationData[] _formationData;

    private List<LemurEntity> _lemurs = new List<LemurEntity>();
    private List<LemurEntity> _lemursToFree = new List<LemurEntity>();
    public int LastFormation;
    private bool _isGameOver;

    public int TeamCount => _lemurs.Count;
    public Vector3 TargetPos => _target.position;

    public static PlayerController Instance { get; private set; }

    private float _startGameTime;

    public void ShakeCam(int c)
    {
        StartCoroutine(ShakeCamUpdate(c));
    }

    private IEnumerator ShakeCamUpdate(int c)
    {
        var t = 0.0f;
        var force = Mathf.Lerp(0.5f, 0.1f, (Mathf.Max(c, 10))/10.0f);
        var maxt = UnityEngine.Random.Range(0.05f, 0.2f);
        while(t < maxt)
        {
            t += Time.deltaTime;
            _camS.localPosition += (t/maxt) * UnityEngine.Random.insideUnitSphere * force;
            yield return null;
        }
        
        t = 0.0f;
        maxt = 0.1f;
        var startS = _camS.transform.localPosition;
        while(t < maxt)
        {
            t += Time.deltaTime;
            _camS.localPosition = Vector3.Lerp(startS, Vector3.zero, t/maxt);
            yield return null;
        }
        _camS.localPosition = Vector3.zero;
    }

    private void Start()
    {
        _startGameTime = Time.unscaledTime;
        Instance = this;
        var initialLemur = Instantiate(_lemurPrefab, _lemursParent);
        initialLemur.transform.localPosition = Vector3.zero;
        AddLemur(initialLemur);
        SetFormation(0);
    }
    
    public static float ProgressSpeed;


    void Update()
    {
        if(_lemurs.Count <= 0 && !_isGameOver) {
            StartCoroutine(_panelsController.SetGameOver());
            _isGameOver = true;
            return;
        }

        if(_isGameOver)
            return;
            
        ProgressSpeed = Mathf.Lerp(1.0f, 11.0f, Mathf.Min(120.0f, Time.unscaledTime-_startGameTime)/120.0f);
        _target.Translate(Vector3.forward * _speed * ProgressSpeed * Time.deltaTime);
        _cam.transform.position = new Vector3(_cam.transform.position.x, _cam.transform.position.y, _target.position.z);

        //Debug input movement
        if(Input.GetKeyDown(KeyCode.A)) Move(_moveData[0].Keyword);
        if(Input.GetKeyDown(KeyCode.D)) Move(_moveData[1].Keyword);
        if(Input.GetKeyDown(KeyCode.Space)) FreeLemurs();

        if(Input.GetKeyDown(KeyCode.Alpha1))
            SetFormation(0);
        if(Input.GetKeyDown(KeyCode.Alpha2))
            SetFormation(1);
        if(Input.GetKeyDown(KeyCode.Alpha3))
            SetFormation(2);
        if(Input.GetKeyDown(KeyCode.Alpha4))
            SetFormation(3);
        if(Input.GetKeyDown(KeyCode.Alpha5))
            SetFormation(4);
        if(Input.GetKeyDown(KeyCode.Alpha6))
            SetFormation(5);
    }

    public void Move(string keyword)
    {
        if(keyword == "left") OnLeft?.Invoke();
        if(keyword == "right") OnRight?.Invoke();

        var moveData = GetMoveData(keyword, out bool notFound);
        if (notFound)
        {
            for(var i = 0; i < _formationData.Length; i++)
                if(_formationData[i].Keyword == keyword)
                {
                    SetFormation(i);
                    return;
                }
                
            if(keyword == _freeKeyword)
            {
                FreeLemurs();
                return;
            }

            if(keyword == _resetKeyword)
            {
                _fadeController.FadeIn(()=>{ 
                    UnityEngine.SceneManagement.SceneManager.LoadScene(0);
                });
                
                return;
            }

            return;
        }


        if(moveData.MoveOffset.x < 0 && _target.position.x < _minLeftOffset)
            return;

        if(moveData.MoveOffset.x > 0 && _target.position.x > _maxRightOffset)
            return;

        _target.position += moveData.MoveOffset;
    }

    private MoveData GetMoveData(string keyword, out bool notFound)
    {
        notFound = false;

        foreach(var moveData in _moveData)
        {
            if(moveData.Keyword == keyword) 
                return moveData;
        }

        notFound = true;

        return default;
    }

    public void SetLemursToFree(bool canBeUnlocked, LemurEntity lemurEntity)
    {
        if(canBeUnlocked && !_lemursToFree.Contains(lemurEntity))
        {
            _lemursToFree.Add(lemurEntity);
        }
        else if(!canBeUnlocked && _lemursToFree.Contains(lemurEntity))
        {
            _lemursToFree.Remove(lemurEntity);
        }

        _panelsController.SetFreeView(_lemursToFree.Count > 0);
    }

    private void FreeLemurs()
    {
        var cpy = new List<LemurEntity>();
        cpy.AddRange(_lemursToFree);
        for (int i = 0; i < cpy.Count; i++)
        {
            StartCoroutine(TakeLemur((i*0.1f) + 0.1f, cpy[i]));
        }
    }

    private IEnumerator TakeLemur(float offset, LemurEntity l)
    {
        yield return new WaitForSeconds(offset);
            AddLemur(l);

    }
    public void AddLemur(LemurEntity lemurEntity)
    {
        if(!_lemurs.Contains(lemurEntity))
        {
            _lemurs.Add(lemurEntity);
            OnTeamChanged?.Invoke();
            lemurEntity.transform.SetParent(_lemursParent);
            lemurEntity.SetInTeam();
            RefreshFormation();
        }
    }

    public void RemoveLemur(LemurEntity lemurEntity)
    {
        if(_lemurs.Contains(lemurEntity))
        {
            _lemurs.Remove(lemurEntity);
            OnTeamChanged?.Invoke();
            RefreshFormation();
            ShakeCam(_lemurs.Count);
            Destroy(lemurEntity.gameObject, _removeTime);
        }
    }

    public void SetFormation(int formationID)
    {
        LastFormation = formationID;
        RefreshFormation();
        OnFormationChanged?.Invoke();
    }

    private void RefreshFormation()
    {
        Debug.Log("RefreshFormation");
        var data = _formationData[LastFormation];
        var points = new List<Vector3>();

        for(var i = 0; i < 100; i++)
        {
            var v = data.MyGrid.Values[i];
            if (v)
            {
                var y = i/10;
                var x = (i-(y*10));
                var p = ((new Vector3(x, 0, y) - new Vector3(5.0f, 0.0f, 5.0f))*.3f);
                points.Add(p);
            }
        }
        
        var bestPoints = new List<Vector3>();
        var maxMinDistance = float.MinValue;

        for(var j = 0; j < 2000; j++)
        {
            var tempPoints = new List<Vector3>();
            for(var i = 0; i < _lemurs.Count; i++)
            {
                var p = points[UnityEngine.Random.Range(0, points.Count)];
                var offset = UnityEngine.Random.insideUnitSphere * 0.3f;
                offset = new Vector3(offset.x, 0.0f, offset.z);
                tempPoints.Add(p + offset);
            }
            var minMax = GetMinMaxDistance(tempPoints);
            if(minMax > maxMinDistance)
            {
                maxMinDistance = minMax;
                bestPoints.Clear();
                bestPoints.AddRange(tempPoints.ToArray());
            }
        }


        for(var i = 0; i < _lemurs.Count; i++)
        {
            var pp = bestPoints[i];
            _lemurs[i].Target = pp;
        }
    }

    public float GetMinMaxDistance(List<Vector3> points)
    {
        var min = float.MaxValue;
        for(var i = 0; i < points.Count; i++)
        {
            for(var j = i+1; j < points.Count; j++)
            {
                var d = Vector3.Distance(points[i], points[j]);
                if(d < min)
                    min = d;
            }
        }
        return min;
    }
}
