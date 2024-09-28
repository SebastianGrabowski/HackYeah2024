using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Formation : MonoBehaviour
{
    [SerializeField]private FormationData[] _Data;
    [SerializeField]private FormationUnit _Unit;

    private List<FormationUnit> _SpawnedUnits = new();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)){
            foreach(var unit in _SpawnedUnits)
                Destroy(unit.gameObject);
            _SpawnedUnits.Clear();
            var r = Random.Range(1, 40);
            for(var i = 0; i < r; i++)
            {
                var newUnit = Instantiate(_Unit);
                newUnit.transform.position = Vector3.zero + new Vector3(Random.Range(-5.0f, 5.0f), 0.0f, Random.Range(-5.0f, 5.0f));
                newUnit.gameObject.SetActive(true);
                _SpawnedUnits.Add(newUnit);
            }
        }

        if(Input.GetKeyDown(KeyCode.Alpha1))
            RunFormation(_Data[0]);
        if(Input.GetKeyDown(KeyCode.Alpha2))
            RunFormation(_Data[1]);
        if(Input.GetKeyDown(KeyCode.Alpha3))
            RunFormation(_Data[2]);
        if(Input.GetKeyDown(KeyCode.Alpha4))
            RunFormation(_Data[3]);
        if(Input.GetKeyDown(KeyCode.Alpha5))
            RunFormation(_Data[4]);
        if(Input.GetKeyDown(KeyCode.Alpha6))
            RunFormation(_Data[5]);
    }

    private void RunFormation(FormationData data)
    {
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

        for(var j = 0; j < 10000; j++)
        {
            var tempPoints = new List<Vector3>();
            for(var i = 0; i < _SpawnedUnits.Count; i++)
            {
                var p = points[Random.Range(0, points.Count)];
                var offset = Random.insideUnitSphere * 0.3f;
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


        for(var i = 0; i < _SpawnedUnits.Count; i++)
        {
            var pp = bestPoints[i];
            _SpawnedUnits[i].Target = pp;
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
