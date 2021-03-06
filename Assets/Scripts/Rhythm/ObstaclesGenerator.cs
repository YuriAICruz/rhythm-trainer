using System.Linq;
using Graphene.Grid;
using Graphene.Rhythm.Presentation;
using UnityEngine;

namespace Graphene.Rhythm
{
    public class ObstaclesGenerator : MonoBehaviour
    {
        public int PoolSize;
        public float Space = 4;
        public float Offset;

        private Transform _target;
        private GameObject[] _pool;
        private TrailSystem _trail;
        private InfiniteHexGrid _infGrid;
        private GridSystem _gridSystem;
        private Metronome _metronome;

        private int _lastPos;
        private int _currentCoin;
        private bool _baseLineGenerated;

        private readonly int _mul = 6;
        private GameObject[] _obstacles;
        private MenuManager _menuManager;

        private void Awake()
        {
            _pool = new GameObject[PoolSize * _mul];

            _obstacles = Resources.LoadAll<GameObject>("Pool/Obstacles");
            
            _menuManager = FindObjectOfType<MenuManager>();
            _menuManager.OnRestartGame += RestartGame;

            for (int i = 0; i < PoolSize * _mul; i++)
            {
                _pool[i] = Instantiate(_obstacles[i % _obstacles.Length]);
                _pool[i].transform.position = Vector3.one * -1000;
            }

            _trail = FindObjectOfType<TrailSystem>();

            if (_gridSystem == null)
                _gridSystem = GetComponent<GridSystem>();

            _infGrid = (InfiniteHexGrid) _gridSystem.Grid;
            _metronome = FindObjectOfType<Metronome>();

            _infGrid = (InfiniteHexGrid) _gridSystem.Grid;
        }

        private void RestartGame()
        {
            _lastPos = 0;
        }

        public void SetTarget(Transform target)
        {
            _target = target;
        }

        private void Update()
        {
            if (_target == null || _gridSystem == null || _gridSystem.Grid == null) return;

            if (_infGrid == null)
                _infGrid = (InfiniteHexGrid) _gridSystem.Grid;

            if (_infGrid == null) return;
            
            var space = GetSpace();

            var p = (int) ((_target.position.x + space * (PoolSize / 4f)) / space);

            if (p <= _lastPos)
                return;

            _lastPos = p;

            Draw(new Vector3(_lastPos * space, 0, _target.position.z));
        }

        private float GetSpace()
        {
            return Space;
        }


        void Draw(Vector3 p)
        {
            if(p.x < 500) return;

            p.x += Offset;
            var space = GetSpace();
            
            var pos = new Vector3[]
            {
                new Vector3(p.x, 0, Mathf.Floor(p.z / space) * space),
                new Vector3(p.x, 0, Mathf.Floor(p.z / space) * space + _trail.Step),
                new Vector3(p.x, 0, Mathf.Floor(p.z / space) * space - _trail.Step),
            };

            var offset = Random.Range(-1, 1);

            var rot = Random.Range(0, 360);

            for (int i = 0; i < pos.Length; i++)
            {
                var outPos = _trail.TrailMath(pos[i]);
                var split = Mathf.Abs(outPos[0].z - outPos[1].z) > 1f;

                outPos[0].z = Mathf.Floor(outPos[0].z / _gridSystem.Widith) * _gridSystem.Widith;
                outPos[0].z += offset *  _gridSystem.Widith;
                outPos[0].y = _infGrid.YGraph(outPos[0]);

                _pool[_currentCoin + i * 2].transform.eulerAngles = new Vector3(0, rot, 0);
                _pool[_currentCoin + i * 2].transform.position = outPos[0];
                _pool[_currentCoin + i * 2].gameObject.SetActive(true);

                if (split)
                {
                    outPos[1].z = Mathf.Floor(outPos[0].z / _gridSystem.Widith) * _gridSystem.Widith;
                    outPos[1].z += offset *  _gridSystem.Widith;
                    outPos[1].y = _infGrid.YGraph(outPos[1]);
                    _pool[_currentCoin + i * 2 + 1].transform.eulerAngles = new Vector3(0, rot, 0);
                    _pool[_currentCoin + i * 2 + 1].transform.position = outPos[1];
                    _pool[_currentCoin + i * 2 + 1].gameObject.SetActive(true);
                }
            }
            _currentCoin = (_currentCoin + _mul) % PoolSize * _mul;
        }
    }
}