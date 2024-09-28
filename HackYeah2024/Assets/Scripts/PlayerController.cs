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
    [SerializeField] private MoveData[] _moveData;

    void Update()
    {
        transform.Translate(Vector3.forward * _speed * Time.deltaTime);
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
}
