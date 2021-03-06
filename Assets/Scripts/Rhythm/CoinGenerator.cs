using System.Collections.Generic;
using System.Linq;
using CSharpSynth.Midi;
using Graphene.Grid;
using Graphene.Rhythm.Presentation;
using UnityEngine;

namespace Graphene.Rhythm
{
    public class CoinGenerator : MonoBehaviour
    {
        public int CoinPool;
        public float Offset = 0;

        private Transform _target;
        private GameObject[] _coins;
        private TrailSystem _trail;
        private InfiniteHexGrid _infGrid;
        private GridSystem _gridSystem;
        private Metronome _metronome;

        private int _lastPos;
        private int _currentCoin;
        private bool _baseLineGenerated;

        private readonly int _mul = 6;
        private MenuManager _menuManager;
        private int _lastSide;
        private MidiFile _midi;
        private List<float> _events;

        private void Awake()
        {
            _coins = new GameObject[CoinPool * _mul];

            var go = Resources.Load<GameObject>("Pool/Coin");

            _menuManager = FindObjectOfType<MenuManager>();
            _menuManager.OnRestartGame += RestartGame;

            for (int i = 0; i < CoinPool * _mul; i++)
            {
                _coins[i] = Instantiate(go);
                _coins[i].transform.position = Vector3.one * -1000;
            }

            _trail = FindObjectOfType<TrailSystem>();

            if (_gridSystem == null)
                _gridSystem = GetComponent<GridSystem>();

            _infGrid = (InfiniteHexGrid) _gridSystem.Grid;
            _metronome = FindObjectOfType<Metronome>();
            _metronome.Beat += PopCoin;

            _infGrid = (InfiniteHexGrid) _gridSystem.Grid;
        }

        private void PopCoin(int index)
        {
            DrawCoins(new Vector3((_metronome.ElapsedTime + 22) * _gridSystem.Widith * _metronome.Bpm / 60f, 0, _target.position.z));
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
        }

        private void RestartGame()
        {
            _lastPos = 0;
        }

        void DrawCoins(Vector3 p)
        {
            if (_infGrid == null) return;
            
            p.x += Offset;
            var pos = new Vector3[]
            {
                new Vector3(p.x, 0, p.z),
                new Vector3(p.x, 0, p.z + _trail.Step),
                new Vector3(p.x, 0, p.z - _trail.Step),
            };

            DistributeOnPath(pos);
        }

        private void DistributeOnPath(Vector3[] pos)
        {
            var side = Random.Range(-1, 1);

            if (Mathf.Abs(_lastSide + side) > 3)
                side *= -1;
            
            for (int i = 0; i < pos.Length; i++)
            {
                var outPos = _trail.TrailMath(pos[i]);
                var split = Mathf.Abs(outPos[0].z - outPos[1].z) > 3f;

                outPos[0].z = Mathf.Floor(outPos[0].z / _gridSystem.Widith) * _gridSystem.Widith;
                outPos[0].z += (_lastSide + side) * _gridSystem.Widith;
                outPos[0].y = _infGrid.YGraph(outPos[0]);

                _coins[_currentCoin + i * 2].transform.position = outPos[0];
                _coins[_currentCoin + i * 2].gameObject.SetActive(true);
                
                if (split)
                {
                    outPos[1].z = Mathf.Floor(outPos[1].z / _gridSystem.Widith) * _gridSystem.Widith;
                    outPos[1].z += (_lastSide + side) * _gridSystem.Widith;
                    outPos[1].y = _infGrid.YGraph(outPos[1]);

                    _coins[_currentCoin + i * 2 + 1].transform.position = outPos[1];
                    _coins[_currentCoin + i * 2 + 1].gameObject.SetActive(true);
                }
            }

            _lastSide = _lastSide + side;
            _currentCoin = (_currentCoin + _mul) % CoinPool * _mul;
        }
    }
}