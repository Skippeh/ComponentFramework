﻿using System;
using ComponentSystem;
using Microsoft.Xna.Framework;
using GameComponent = ComponentSystem.GameComponent;

namespace MiscComponents.Components
{
    public class Shake : GameComponent
    {
        public Vector2 Intensity { get; set; } = Vector2.Zero;
        public float Frequency { get; set; } = 0f;

        private Vector2 shakeOffset;
        private Vector2 preDrawPosition;
        private float timeSinceFrequencyUpdate = 0;

        public override void OnCreate()
        {
            timeSinceFrequencyUpdate = Frequency;
        }

        public override void OnUpdate()
        {
            timeSinceFrequencyUpdate += Time.DT;

            if (timeSinceFrequencyUpdate >= Frequency)
            {
                shakeOffset.X = Intensity.X * (Mathf.Random() * 2f - 1f);
                shakeOffset.Y = Intensity.Y * (Mathf.Random() * 2f - 1f);
                timeSinceFrequencyUpdate = 0;
            }
        }

        public override void OnPreDraw()
        {
            float lerpAmount = timeSinceFrequencyUpdate / Frequency;
            
            preDrawPosition = GameObject.Transform.Position;
            GameObject.Transform.Position += Mathf.SmoothLerp010(Vector2.Zero, shakeOffset, lerpAmount);
        }

        public override void OnPostDraw()
        {
            GameObject.Transform.Position = preDrawPosition;
        }
    }
}