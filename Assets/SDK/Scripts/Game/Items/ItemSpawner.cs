﻿using UnityEngine;
using System.Collections.Generic;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace ThunderRoad
{
    public class ItemSpawner : MonoBehaviour
    {
        public string itemId;
        public bool pooled;
        public bool spawnOnStart = true;

    }
}
