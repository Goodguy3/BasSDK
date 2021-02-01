﻿using UnityEngine;
using System;

#if PrivateSDK
using RainyReignGames.RevealMask;
#endif

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#else
using EasyButtons;
#endif

namespace ThunderRoad
{
    [AddComponentMenu("ThunderRoad/Reveal Decal")]
    public class RevealDecal : MonoBehaviour
    {
        [Tooltip("These materials are what will be switched to on the renderer once the reveal masks are activated. Corresponds with shared materials index.")]
        public Material[] materials;
        [Tooltip("Resolution of the reveal mask")]
        public RevealMaskResolution maskWidth = RevealMaskResolution.Size_512;
        [Tooltip("Resolution of the reveal mask")]
        public RevealMaskResolution maskHeight = RevealMaskResolution.Size_512;
        [Tooltip("Reveal type")]
        public Type type = Type.Default;

        public enum Type
        {
            Default,
            Body,
            Outfit,
        }

        public enum RevealMaskResolution
        {
            Size_32 = 32,
            Size_64 = 64,
            Size_128 = 128,
            Size_256 = 256,
            Size_512 = 512,
            Size_1024 = 1024,
            Size_2048 = 2048,
            Size_4096 = 4096,
        }

        [Button()]
        public void SetMaskResolutionFull()
        {
            SetMaskResolution(1f);
        }

        [Button()]
        public void SetMaskResolutionHalf()
        {
            SetMaskResolution(0.5f);
        }

        [Button()]
        public void SetMaskResolutionQuarter()
        {
            SetMaskResolution(0.25f);
        }

        [Button()]
        public void SetMaskResolutionEighth()
        {
            SetMaskResolution(0.125f);
        }

        public void SetMaskResolution(float scale = 1)
        {
            Renderer renderer = this.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material mat = renderer.sharedMaterial;
                if (mat != null)
                {
                    Texture baseMap = mat.GetTexture("_BaseMap");
                    if (baseMap != null)
                    {
                        maskWidth = (RevealMaskResolution)Mathf.ClosestPowerOfTwo((int)(baseMap.width * scale));
                        maskHeight = (RevealMaskResolution)Mathf.ClosestPowerOfTwo((int)(baseMap.height * scale));
                        if (maskWidth != maskHeight) Debug.Log(this.gameObject.name);
                    }
                }
            }
        }

#if PrivateSDK
        [NonSerialized]
        public RevealMaterialController revealMaterialController;

        void Awake()
        {
            revealMaterialController = this.gameObject.AddComponent<RevealMaterialController>();
            revealMaterialController.revealMaterials = materials;
            revealMaterialController.width = (int)maskWidth;
            revealMaterialController.height = (int)maskHeight;
            revealMaterialController.maskPropertyName = "_RevealMask";
            revealMaterialController.preserveRenderQueue = true;
            revealMaterialController.renderTextureFormat = RenderTextureFormat.ARGB64;

            foreach (Material material in revealMaterialController.revealMaterials)
            {
                if (material.HasProperty("_Bitmask"))
                {
                    revealMaterialController.propertiesToPreserve = new RevealMaterialController.ShaderProperty[1];
                    revealMaterialController.propertiesToPreserve[0].name = "_Bitmask";
                    revealMaterialController.propertiesToPreserve[0].type = RevealMaterialController.ShaderProperty.ShaderPropertyType.Int;
                    break;
                }
            }
        }

        public void Reset()
        {
            revealMaterialController.Reset();
        }
#endif
    }
}