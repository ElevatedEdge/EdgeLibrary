﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Xml;

namespace EdgeLibrary
{
    /// <summary>
    /// An emmiter of particles.
    /// </summary>

    public class ParticleEmitter : Sprite
    {
        public ColorChangeIndex MinColorIndex;
        public ColorChangeIndex MaxColorIndex;
        public Vector2 MinVelocity;
        public Vector2 MaxVelocity;
        public Vector2 MinSize;
        public Vector2 MaxSize;
        public float GrowSpeed;
        public float MaxStartRotation;
        public float MinStartRotation;
        public float MaxRotationSpeed;
        public float MinRotationSpeed;
        public float MaxLife;
        public float MinLife;
        public float EmitWait;
        public int MaxParticles;

        protected List<Particle> particles;
        protected TimeSpan timeSinceLastEmit;

        //To stop garbage collection every single time
        protected List<Particle> particlesToRemove;
        protected const int maxParticlesToRemove = 30;

        public delegate void ParticleEventHandler(ParticleEmitter sender);
        public event ParticleEventHandler OnEmit;

        public ParticleEmitter(string eTextureName, Vector2 ePosition) : this(MathTools.RandomID(), eTextureName, ePosition) { }

        public ParticleEmitter(string id, string eTextureName, Vector2 ePosition) : base(eTextureName, ePosition)
        {
            particles = new List<Particle>();

            MinColorIndex = new ColorChangeIndex(Color.White);
            MaxColorIndex = MinColorIndex;
            MinVelocity = -Vector2.One;
            MaxVelocity = Vector2.One*2;
            if (Texture != null)
            {
                MinSize = new Vector2(Texture.Width, Texture.Height);
            }
            MaxSize = MinSize;
            GrowSpeed = 0;
            MinStartRotation = 0;
            MaxStartRotation = MinStartRotation;
            MinRotationSpeed = 0;
            MaxRotationSpeed = MinRotationSpeed;
            MinLife = 1000;
            MaxLife = MinLife;
            EmitWait = 0;
            MaxParticles = 10000;

            particlesToRemove = new List<Particle>();
            timeSinceLastEmit = TimeSpan.Zero;
        }

        public void SetColor(Color color)
        {
            MinColorIndex = new ColorChangeIndex(color);
            MaxColorIndex = MinColorIndex;
        }
        public void SetVelocity(Vector2 v)
        {
            MinVelocity = v;
            MaxVelocity = v;
        }
        public void SetSize(Vector2 s)
        {
            MinSize = s;
            MaxSize = s;
        }
        public void SetRotation(float r)
        {
            MinStartRotation = r;
            MaxStartRotation = r;
        }
        public void SetRotationSpeed(float r)
        {
            MinRotationSpeed = r;
            MaxRotationSpeed = r;
        }
        public void SetLife(float l)
        {
            MinLife = l;
            MaxLife = l;
        }
        public void SetEmitArea(int width, int height)
        {
           _width = width;
           _height = height;
        }

        public void EmitSingleParticle()
        {
            Particle particle = new Particle(MathTools.RandomID("particle"), "", RandomTools.RandomFloat(MinLife, MaxLife), RandomTools.RandomFloat(MinRotationSpeed, MaxRotationSpeed), GrowSpeed);
            particle.REMOVE();

            particle.velocity = new Vector2(RandomTools.RandomFloat(MinVelocity.X, MaxVelocity.X), RandomTools.RandomFloat(MinVelocity.Y, MaxVelocity.Y));

            particle.Texture = Texture;
            particle.CollisionBody = null;
            particle.Position = new Vector2(RandomTools.RandomFloat(Position.X - Width / 2, Position.X + Width / 2), RandomTools.RandomFloat(Position.Y - Height / 2, Position.Y + Height / 2));
            particle.Style.Rotation = RandomTools.RandomFloat(MinStartRotation, MaxStartRotation);
            particle.Height = RandomTools.RandomFloat(MinSize.Y, MaxSize.Y);
            particle.Width = RandomTools.RandomFloat(MinSize.X, MaxSize.X);
            particle.ColorIndex = ColorChangeIndex.Lerp(MinColorIndex, MaxColorIndex, RandomTools.RandomFloat());

            particles.Add(particle);

            if (OnEmit != null)
            {
                OnEmit(this);
            }
        }

        protected override void drawElement(GameTime gameTime)
        {
            foreach (Particle particle in particles)
            {
                if (!particlesToRemove.Contains(particle))
                {
                    particle.Draw(gameTime);
                }
            }
        }

        protected override void updateElement(GameTime gameTime)
        {
            timeSinceLastEmit += gameTime.ElapsedGameTime;

            if (timeSinceLastEmit.TotalMilliseconds >= EmitWait)
            {
                timeSinceLastEmit = new TimeSpan(0);
                EmitSingleParticle();
            }

            foreach (Particle particle in particles)
            {
                if (!particlesToRemove.Contains(particle))
                {
                    particle.Update(gameTime);
                    if (particle.shouldRemove)
                    {
                        particlesToRemove.Add(particle);
                    }
                }
            }

            if (particlesToRemove.Count >= maxParticlesToRemove)
            {
                foreach (Particle particle in particlesToRemove)
                {
                    particles.Remove(particle);
                }
                particlesToRemove.Clear();
            }

            while (particles.Count > MaxParticles)
            {
                particles.RemoveAt(0);
            }
        }

        public override void DebugDraw(Color color)
        {
            TextureTools.DrawHollowRectangleAt(GetBoundingBox(), color, 1);
        }
    }
}
