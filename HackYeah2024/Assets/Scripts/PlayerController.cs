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

    [SerializeField] private Transform _cam;
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
    private int _lastFormation;
    private bool _isGameOver;

    public int TeamCount => _lemurs.Count;
    public Vector3 TargetPos => _target.position;

    public static PlayerController Instance { get; private set; }

    private void Start()
    {
        Instance = this;
        var initialLemur = Instantiate(_lemurPrefab, _lemursParent);
        initialLemur.transform.localPosition = Vector3.zero;
        AddLemur(initialLemur);
        SetFormation(0);
    }

    void Update()
    {
        if(_lemurs.Count <= 0 && !_isGameOver) {
            StartCoroutine(_panelsController.SetGameOver());
            _isGameOver = true;
            return;
        }

        if(_isGameOver)
            return;
            
        _target.Translate(Vector3.forward * _speed * Time.deltaTime);
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
        foreach(var lemur in _lemursToFree)
        {
            AddLemur(lemur);
        }
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

            Destroy(lemurEntity.gameObject, _removeTime);
        }
    }

    public void SetFormation(int formationID)
    {
        _lastFormation = formationID;
        RefreshFormation();
    }

    private void RefreshFormation()
    {
        Debug.Log("RefreshFormation");
        var data = _formationData[_lastFormation];
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
