﻿using UnityEngine;
using System;

namespace ThunderRoad
{
    public class EffectShader : Effect
    {
        public EffectTarget linkBaseColor = EffectTarget.None;
        public EffectTarget linkEmissionColor = EffectTarget.None;

        public float lifeTime = 0;
        public float refreshSpeed = 0.1f;
        public bool useSecondaryRenderer;

        [NonSerialized]
        public float playTime;

        protected new Renderer renderer;

        protected static int colorPropertyID;
        protected static int emissionPropertyID;

        protected MaterialPropertyBlock materialPropertyBlock;

        protected float currentValue;
        protected Gradient currentMainGradient;
        protected Gradient currentSecondaryGradient;

        private void OnValidate()
        {
            Awake();
        }

        private void Awake()
        {
            materialPropertyBlock = new MaterialPropertyBlock();
            if (colorPropertyID == 0) colorPropertyID = Shader.PropertyToID("_Color");
            if (emissionPropertyID == 0) emissionPropertyID = Shader.PropertyToID("_EmissionColor");
        }

        public override void Play()
        {
            CancelInvoke();
            if ((step == Step.Start || step == Step.End) && lifeTime > 0)
            {
                InvokeRepeating("UpdateLifeTime", 0, refreshSpeed);
            }
            SetIntensity(currentValue);
            playTime = Time.time;
        }

        public override void Stop()
        {
            SetIntensity(0);
        }

        public override void End(bool loopOnly = false)
        {
            Despawn();
        }

        protected void UpdateLifeTime()
        {
            float value = Mathf.Clamp01(1 - ((Time.time - playTime) / lifeTime));
            SetIntensity(value);
            if (value == 0) Despawn();
        }

        public override void SetRenderer(Renderer renderer, bool secondary)
        {
            if ((useSecondaryRenderer && secondary) || (!useSecondaryRenderer && !secondary))
            {
                this.renderer = renderer;
            }
        }

        public override void SetIntensity(float value, bool loopOnly = false)
        {
            if (!loopOnly || (loopOnly && step == Step.Loop))
            {
                currentValue = value;
                if (renderer && renderer.isVisible)
                {
                    if (linkBaseColor == EffectTarget.Main && currentMainGradient != null)
                    {
                        materialPropertyBlock.SetColor(colorPropertyID, currentMainGradient.Evaluate(value));
                    }
                    else if (linkBaseColor == EffectTarget.Secondary && currentSecondaryGradient != null)
                    {
                        materialPropertyBlock.SetColor(colorPropertyID, currentSecondaryGradient.Evaluate(value));
                    }
                    if (linkEmissionColor == EffectTarget.Main && currentMainGradient != null)
                    {
                        materialPropertyBlock.SetColor(emissionPropertyID, currentMainGradient.Evaluate(value));
                    }
                    else if (linkEmissionColor == EffectTarget.Secondary && currentSecondaryGradient != null)
                    {
                        materialPropertyBlock.SetColor(emissionPropertyID, currentSecondaryGradient.Evaluate(value));
                    }
                    renderer.SetPropertyBlock(materialPropertyBlock);
                }
            }
        }

        public override void SetMainGradient(Gradient gradient)
        {
            currentMainGradient = gradient;
            SetIntensity(currentValue);
        }

        public override void SetSecondaryGradient(Gradient gradient)
        {
            currentSecondaryGradient = gradient;
            SetIntensity(currentValue);
        }

        public override void Despawn()
        {
            CancelInvoke();
            SetIntensity(0);
            InvokeDespawnCallback();
            if (Application.isPlaying)
            {
                Destroy(this.gameObject);
            }
            else
            {
                DestroyImmediate(this.gameObject);
            }
        }
    }
}
