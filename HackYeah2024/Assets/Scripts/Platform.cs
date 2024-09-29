using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Platform : MonoBehaviour
{
    [SerializeField] private bool _waitIfFirst;
    [SerializeField] private float _lemurChance = 0.65f;

    [SerializeField] private int _minEnemyAmount = 1;
    [SerializeField] private int _maxEnemyAmount = 2;

    [SerializeField] private int _minLemurAmount = 1;
    [SerializeField] private int _maxLemurAmount = 3;
    
    [SerializeField] private LemurEntity _lemurEntity;
    [SerializeField] private Obstacle _obstacle;

    [SerializeField] public Vector3[] _minSpawnOffsets;
    [SerializeField] public Vector3[] _maxSpawnOffsets;

    [SerializeField] private Transform[] _entityPoints;
   
    private List<LemurEntity> _lemurEntities = new List<LemurEntity>();
    private List<Obstacle> _obstacles = new List<Obstacle>();

    void Start()
    {
        Setup();
    }

    public void Setup()
    {
        if(_waitIfFirst) {
            _waitIfFirst = false;
            return;
        }

        for (int i = 0; i < _lemurEntities.Count; i++)
        {
            if(!_lemurEntities[i].IsInTeam)
                Destroy(_lemurEntities[i].gameObject);
        }

        for (int i = 0; i < _obstacles.Count; i++)
        {
            Destroy(_obstacles[i].gameObject);
        }

        _lemurEntities.Clear();
        _obstacles.Clear();

        var entityAmount = Random.Range(2, _entityPoints.Length);
        for (int i = 0; i < entityAmount; i++)
        {
            float randomChance = Random.value;
            if(randomChance <= _lemurChance)
            {
                var lemurAmount = Random.Range(_minLemurAmount, _maxLemurAmount);
                for (int j = 0; j < lemurAmount; j++)
                {
                    var lemur = Instantiate(_lemurEntity, _entityPoints[i].position, Quaternion.identity);
                    lemur.transform.SetParent(_entityPoints[i]);
                    _lemurEntities.Add(lemur);
                    lemur.transform.localPosition = new Vector3(
                        Random.Range(_minSpawnOffsets[j].x, _maxSpawnOffsets[j].x),
                        Random.Range(_minSpawnOffsets[j].y, _maxSpawnOffsets[j].y),
                        Random.Range(_minSpawnOffsets[j].z, _maxSpawnOffsets[j].z)
                    );
                }
                
            }
            else
            {
                var enemyAmount = Random.Range(_minEnemyAmount, _maxEnemyAmount);
                for (int j = 0; j < enemyAmount; j++)
                {
                    var obstacle = Instantiate(_obstacle, _entityPoints[i].position, Quaternion.identity);
                    obstacle.transform.SetParent(_entityPoints[i]);
                    _obstacles.Add(obstacle);
                    obstacle.transform.localPosition = new Vector3(
                        Random.Range(_minSpawnOffsets[j].x, _maxSpawnOffsets[j].x),
                        Random.Range(_minSpawnOffsets[j].y, _maxSpawnOffsets[j].y),
                        Random.Range(_minSpawnOffsets[j].z, _maxSpawnOffsets[j].z)
                    );
                }
            }
        }
        
    }
}
