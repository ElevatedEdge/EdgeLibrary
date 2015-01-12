﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdgeDemo.CheckersGame
{
    //The base for all other 'players'
    public class Player
    {
        public delegate void PlayerMoveEvent(Move move);
        public event PlayerMoveEvent OnRunMove;

        public bool CanMove = false;

        public virtual void Update(GameTime gameTime) { }

        public virtual void Draw(GameTime gameTime) { }

        //The player gets send the previous move and possible moves
        public virtual void ReceivePreviousMove(Move previousMove, Dictionary<Piece, List<Move>> possibleMoves) { CanMove = true; }
        protected virtual void SendMove(Move move)
        {
            if (OnRunMove != null)
            {
                OnRunMove(move);
            }

            CanMove = false;
        }
    }
}
