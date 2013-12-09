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

namespace EdgeLibrary.Edge
{
    //Base for all "game elements" - stuff that needs to be updated and drawn
    public class EElement : EObject
    {
        public virtual bool ClampedToMouse { get; set; }
        public virtual Vector2 Position { get; set; }
        public virtual string Data { get; set; }
        public virtual bool IsVisible { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual int DrawLayer { get; set; }

        public Texture2D Texture { get; set; }
        public SpriteFont Font { get; set; }

        public EElement()
        {
            IsActive = true;
            IsVisible = true;
            DrawLayer = 0;
        }

        public void Update(EUpdateArgs updateArgs)
        {
            if (IsActive) { updateElement(updateArgs); }
        }
        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (IsVisible) { drawElement(spriteBatch, gameTime); }
        }

        public virtual void updateElement(EUpdateArgs updateArgs) { }
        public virtual void drawElement(SpriteBatch spriteBatch, GameTime gameTime) { }

        protected void DrawStringToSpriteBatch(SpriteBatch spriteBatch, SpriteFont font, string text, Color color, float Rotation, Vector2 scale)
        {
            spriteBatch.DrawString(font, text, Position, color, (float)EdgeGame.RadiansToDegrees(Rotation), Vector2.Zero, scale, SpriteEffects.None, 0);
        }

        protected void DrawToSpriteBatch(SpriteBatch spriteBatch, Texture2D texture, Rectangle bounds, Color color, float Rotation, Vector2 scale)
        {
            spriteBatch.Draw(texture, bounds, null, color, (float)EdgeGame.RadiansToDegrees(Rotation), Vector2.Zero, SpriteEffects.None, 0);
        }
    }
}
