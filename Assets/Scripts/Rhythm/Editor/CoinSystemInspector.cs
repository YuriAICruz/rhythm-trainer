﻿using Graphene.Grid;
using UnityEditor;
using UnityEngine;

namespace Graphene.Rhythm
{
    [CustomEditor(typeof(CoinSystem))]
    public class CoinSystemInspector : Editor
    {
        private CoinSystem _self;

        private void Awake()
        {
            _self = (CoinSystem) target;

            if (_self.GridSystem == null)
                _self.GridSystem = _self.GetComponent<GridSystem>();
        }

        private void OnSceneGUI()
        {
            if (_self.GridSystem == null || _self.GridSystem.Grid == null)
                return;

            if (_self.GridSystem.GridType != GridType.InfHex)
                return;

            DebugCoins();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }

        private void DebugCoins()
        {
            var cam = Camera.current;

            var gr = (InfiniteHexGrid) _self.GridSystem.Grid;
            if (gr == null) return;

            var view = cam.transform.TransformPoint(Vector3.forward * 5);
            var w = _self.Space;
            var x = ((int) (view.x - w * 30) / w) * w;
            var z = ((int) (view.z - w * 5) / w) * w;
            var y = gr.YGraph(x);

            var color = Handles.color;
            Handles.color = Color.green;

            for (int ix = 0; ix < 60; ix++)
            {
                var oz = z;
                var nx = (int) ((x + w) / w) * w;
                for (int iy = 0; iy < 10; iy++)
                {
                    var px = new Vector3(x, gr.YGraph(x), z);
                    var nz = (int) ((z + w) / w) * w;
                    var coinPos = _self.CoinMath(px);

                    for (int i = 0; i < coinPos.Length; i++)
                    {
                        Handles.color = new Color(i % 2, (i + 1) % 2, 0.5f, 1);
                        coinPos[i].y = 0;
                        Handles.DrawLine(coinPos[i], coinPos[i] + Vector3.forward);
                        Handles.DrawLine(coinPos[i], coinPos[i] + Vector3.right);
                        Handles.DrawLine(coinPos[i], coinPos[i] - Vector3.forward);
                        Handles.DrawLine(coinPos[i], coinPos[i] - Vector3.right);
                    }

                    z = nz;
                }
                z = oz;
                x = nx;
            }

            Handles.color = color;
        }
    }
}