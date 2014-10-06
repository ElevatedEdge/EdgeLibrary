﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdgeLibrary;
using Microsoft.Xna.Framework;

namespace EdgeDemo.CheckersGame
{
    public class BoardManager : Sprite
    {
        public Board Board;
        public TextSprite StatusSprite;

        //Used for each move
        public bool TopTeamTurn;
        public bool SelectedFirstSquare;
        int StartX;
        int StartY;
        int FinishX;
        int FinishY;

        public Color TopColor = Color.Gray;
        public Color BottomColor = Color.DarkGray;

        public BoardManager()
            : base("", Vector2.Zero)
        {
            Board = new Board("Pixel", EdgeGame.WindowSize/2, 8, 64, 0, Color.SaddleBrown, Color.Tan, 5, Color.DarkGoldenrod, "Checkers", 54, TopColor, BottomColor);
            Board.AddToGame();

            DebugText debug = new DebugText("Impact-20", new Vector2(0, EdgeGame.WindowSize.Y - 75)) { Color = Color.Goldenrod, CenterAsOrigin = false };
            debug.AddToGame();

            StatusSprite = new TextSprite("ComicSans-20", "Top Team: Select a square to move from", Vector2.Zero);
            StatusSprite.CenterAsOrigin = false;
            StatusSprite.AddToGame();
        }

        public string Move(int startX, int startY, int finishX, int finishY)
        {
            Piece movePiece = Board.GetPieceAt(startX, startY);

            if (movePiece == null)
            {
                return "Please click on a square where there is a piece";
            }

            if (Board.GetPieceAt(finishX, finishY) != null)
            {
                return "That is an invalid move";
            }

            if (Board.GetSquareAt(finishX, finishY).FakeSquare)
            {
                return "That is an invalid square";
            }

            //Checks if the player can jump but isn't
            if (Board.TeamCanJump(movePiece.TopTeam) &&
                //Checks if the player jumped by checking the distance it moved
                ((finishX == startX + 1 && finishY == startY + 1) || (finishX == startX - 1 && finishY == startY + 1) ||
                (finishX == startX + 1 && finishY == startY - 1) || (finishX == startX - 1 && finishY == startY - 1)))
            {
                return "You must jump";
            }

            //Checks if a piece isn't moving in a correct direction
            if (!movePiece.King && ((movePiece.TopTeam && finishY >= startY) || (!movePiece.TopTeam && finishY <= startY)))
            {
                return "Please click on a valid square";
            }

            //Checks if a piece trying to move off of the board
            if (finishX > Board.Size || finishY > Board.Size)
            {
                return "Please click on a square which is on the board";
            }

            return "☺";

            //Eventually this will call the web service's move function
            //Something like this:
            //CheckersService.move(short pieceId, short destX, short destY)
        }

        public override void Draw(GameTime gameTime)
        {
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Input.JustLeftClicked())
            {
                Square square = Board.GetSquareClicked();
                StatusSprite.Text = "X: " + square.X + ", Y: " + square.Y;

                if (square != null)
                {
                    if (!SelectedFirstSquare)
                    {
                        StartX = square.X;
                        StartY = square.Y;
                        square.Color = Color.DarkGreen;
                        SelectedFirstSquare = true;
                    }
                    else
                    {
                        FinishX = square.X;
                        FinishY = square.Y;
                        square.Color = Color.DarkRed;
                        StatusSprite.Text = Move(StartX, StartY, FinishX, FinishY);
                        SelectedFirstSquare = false;
                    }
                }
            }
        }
    }
}
