﻿using Graphene.Grid;
using Graphene.Rhythm.Game;
using UnityEngine;

namespace Graphene.Rhythm
{
    public class CoinSystem : MonoBehaviour
    {
        public int UpdateStep;
        public float Div = 50;
        public float Step = 1;
        public float Space = 4;
        public GridSystem GridSystem;
        public int CoinPool;
        private Player _player;
        private GameObject[] _coins;
        private int _currentCoin;
        private InfiniteHexGrid _infGrid;

        private void Start()
        {
            _player = FindObjectOfType<Player>();

            _coins = new GameObject[CoinPool * 6];

            var go = Resources.Load<GameObject>("Pool/Coin");

            for (int i = 0; i < CoinPool * 6; i++)
            {
                _coins[i] = Instantiate(go);
                _coins[i].transform.position = Vector3.one * -1000;
            }

            _infGrid = (InfiniteHexGrid) GridSystem.Grid;
        }

        private void Update()
        {
            if (_infGrid == null)
                _infGrid = (InfiniteHexGrid) GridSystem.Grid;

            if (_infGrid == null) return;

            //if ((int) (_player.transform.position.x) % UpdateStep == 0)
            DrawCoins();
        }


        void DrawCoins()
        {
            var pos = new Vector3[]
            {
                new Vector3(Mathf.Floor(_player.transform.position.x / Space) * Space, 0, Mathf.Floor(_player.transform.position.z / Space) * Space),
                new Vector3(Mathf.Floor(_player.transform.position.x / Space) * Space, 0, Mathf.Floor(_player.transform.position.z / Space) * Space + Step),
                new Vector3(Mathf.Floor(_player.transform.position.x / Space) * Space, 0, Mathf.Floor(_player.transform.position.z / Space) * Space - Step),
            };

            for (int i = 0; i < CoinPool * 6; i += 6)
            {
                var outPos = CoinMath(pos[0]);
                var split = Mathf.Abs(outPos[0].z - outPos[1].z) > 0.1f;

                outPos[0].y = _infGrid.YGraph(outPos[0].x);
                _coins[i].transform.position = outPos[0];
                if (split)
                {
                    outPos[1].y = _infGrid.YGraph(outPos[1].x);
                    _coins[i + 3].transform.position = outPos[1];
                }

                outPos = CoinMath(pos[1]);
                outPos[0].y = _infGrid.YGraph(outPos[0].x);
                _coins[i + 1].transform.position = outPos[0];
                if (split)
                {
                    outPos[1].y = _infGrid.YGraph(outPos[1].x);
                    _coins[i + 4].transform.position = outPos[1];
                }

                outPos = CoinMath(pos[2]);
                outPos[0].y = _infGrid.YGraph(outPos[0].x);
                _coins[i + 2].transform.position = outPos[0];
                if (split)
                {
                    outPos[1].y = _infGrid.YGraph(outPos[1].x);
                    _coins[i + 5].transform.position = outPos[1];
                }


                for (int j = 0; j < pos.Length; j++)
                {
                    pos[j] += Vector3.right * Space;
                }
            }

            _currentCoin = (_currentCoin + 1) % CoinPool;
        }


        public Vector3[] CoinMath(Vector3 p)
        {
            var l = Mathf.Pow(Mathf.Sin(p.z / Step), 2);

            //l = Mathf.Floor(l * (1+1/_step));

            var z = Mathf.Floor(p.z / Step) * Step;
            var s = Step / 2;
            var naturalMotion = 0;//Mathf.Pow(Mathf.Sin(p.x * 0.01f), 3)* 50;
            var division = Mathf.Clamp(Mathf.Pow(Mathf.Sin((p.x - Div * 2)/ Div), 3) * s, 0, s);
            
            var res = new Vector3[]
            {
                new Vector3(p.x, 0, z + division + naturalMotion),
                new Vector3(p.x, 0, z - division + naturalMotion),
            };

            //res += p;

            return res;


//            var v = Mathf.Round(Mathf.Pow(Mathf.Sin(x), 2)) * Mathf.Round(Mathf.Pow(Mathf.Cos(z * 0.1f), 2));
//            return v;
        }
    }
}