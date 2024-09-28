using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private float _speed;
    [SerializeField] private Transform _lemursParent;
    [SerializeField] private Vector3[] _formationsTemp;
    [SerializeField] private MoveData[] _moveData;

    private List<LemurEntity> _lemurs = new List<LemurEntity>();

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out LemurEntity lemurEntity))
        {
            AddLemur(lemurEntity);
        }
    }

    void Update()
    {
        transform.Translate(Vector3.forward * _speed * Time.deltaTime);

        //Debug Movement
        if(Input.GetKeyDown(KeyCode.A)) Move(_moveData[0].Keyword);
        if(Input.GetKeyDown(KeyCode.D)) Move(_moveData[1].Keyword);
    }

    public void Move(string keyword)
    {
        var moveData = GetMoveData(keyword, out bool notFound);
        if(notFound)
            return;

        transform.position += moveData.MoveOffset;
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

    public void AddLemur(LemurEntity lemurEntity)
    {
        if(!_lemurs.Contains(lemurEntity) && _formationsTemp.Length > _lemurs.Count)
        {
            _lemurs.Add(lemurEntity);
            lemurEntity.transform.SetParent(_lemursParent);
            lemurEntity.transform.localPosition = _formationsTemp[_lemurs.Count - 1];
        }
    }
}
