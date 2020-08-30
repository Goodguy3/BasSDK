﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ThunderRoad
{
    public class UIWaveSelector : MonoBehaviour
    {
        public string id;
        public SpawnLocation spawnLocation;

        void OnValidate()
        {
            if (id == null || id == "") id = this.gameObject.scene.name;
        }


        protected void OnDrawGizmos()
        {
            Gizmos.matrix = this.transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, new Vector3(0.402f, 0.29f, 0));
        }
    }
}
