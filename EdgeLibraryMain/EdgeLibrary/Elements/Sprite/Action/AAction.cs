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

namespace EdgeLibrary
{
    //Provides the base for all Actions - sprite changers
    public abstract class AAction
    {
        public bool toRemove = false;

        //Used to update the action
        public abstract void UpdateAction(GameTime gameTime, Sprite sprite);

        //Returns a copy of the action so that multiple sprites don't share the same action
        public abstract AAction Copy();
        
        //Marks the action for removal from the sprite's action list
        public void Stop() { toRemove = true; }
    }
}
