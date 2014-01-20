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
using EdgeLibrary.Basic;

// NOTE: REQUIRES EdgeLibraryBasic!

namespace EdgeLibrary.Menu
{
    public class ButtonEventArgs : EventArgs
    {
        public EButton button;
        public Vector2 clickPosition;
    }

    public class EButton : ESprite
    {
        //Not yet implemented
        public ELabel label;
        public Color offColor;
        public Color onColor;
        public Texture2D onTexture;
        public Texture2D offTexture;
        private string onData;
        private string offData;

        protected bool launchedMouseOver;
        protected bool launchedMouseOff;
        
        public delegate void ButtonEventHandler(ButtonEventArgs e);
        public virtual event ButtonEventHandler Click;
        public virtual event ButtonEventHandler MouseOver;
        public virtual event ButtonEventHandler MouseOff;

        public EButton(string eTextureName, Vector2 ePosition, int eWidth, int eHeight, Color eClickColor) : base(eTextureName, ePosition, eWidth, eHeight)
        {
            onColor = eClickColor;
            offData = eTextureName;
            init();
        }

        public EButton(string eTextureName, Vector2 ePosition, int eWidth, int eHeight, Color eClickColor, Color eColor, float eRotation, Vector2 eScale) : base(eTextureName, ePosition, eWidth, eHeight, eColor, eRotation, eScale)
        {
            onColor = eClickColor;
            offData = eTextureName;
            init();
        }

        public void setClickTexture(string textureName)
        {
            onData = textureName;
        }

        protected void init()
        {
            IsActive = true;
            offColor = Color.White;
            launchedMouseOver = false;
            launchedMouseOff = false;
        }

        public override void FillTexture()
        {
            try
            {
                onTexture = EData.getTexture(onData);
            }
            catch { }
            offTexture = EData.getTexture(offData);
        }

        protected void UpdateSpritePortion(EUpdateArgs updateArgs)
        {
            base.updateElement(updateArgs);
        }

        public override void updateElement(EUpdateArgs updateArgs)
        {
            base.updateElement(updateArgs);

            Vector2 mousePosition = new Vector2(updateArgs.mouseState.X, updateArgs.mouseState.Y);

            if (updateArgs.mouseState.LeftButton == ButtonState.Released)
            {
                Color = Color.White;
            }

            if (BoundingBox.Contains(new Rectangle((int)mousePosition.X, (int)mousePosition.Y, 1, 1)))
            {
                Color = onColor;
                Texture = onTexture;

                ButtonEventArgs clickArgs = new ButtonEventArgs();
                clickArgs.button = this;
                clickArgs.clickPosition = mousePosition;

                if (!launchedMouseOver)
                {
                    if (MouseOver != null)
                    {
                        MouseOver(clickArgs);
                    }
                    launchedMouseOver = true;
                    launchedMouseOff = false;
                }

                if (updateArgs.mouseState.LeftButton == ButtonState.Pressed)
                {
                    if (Click != null)
                    {
                        Click(clickArgs);
                    }
                }
            }
            else
            {
                Color = offColor;
                Texture = offTexture;

                if (!launchedMouseOff)
                {
                    ButtonEventArgs clickArgs = new ButtonEventArgs();
                    clickArgs.button = this;
                    clickArgs.clickPosition = Vector2.Zero;
                    MouseOff(clickArgs);
                    launchedMouseOver = false;
                    launchedMouseOff = true;
                }
            }
        }
    }
}
