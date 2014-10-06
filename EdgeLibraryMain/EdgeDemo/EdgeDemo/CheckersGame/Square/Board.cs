﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdgeLibrary;
using Microsoft.Xna.Framework;

namespace EdgeDemo.CheckersGame
{
    public class Board : Sprite
    {
        public Square[,] Squares;
        public int Size;
        public Border Border;

        public float CompleteSize;
        public float SquareSize;

        public Board(string squareTexture, Vector2 position, int size, float squareSize, float squareDistance, Color color1, Color color2, float borderSize, Color borderColor, string pieceTexture, float pieceSize, Color pieceColor1, Color pieceColor2)
            : base(squareTexture, position)
        {
            if (size < 4)
            {
                throw new ArgumentException("Board size must be greater than three");
            }

            float totalSquareDistance = squareDistance * (size - 1);

            Border = new Border(squareTexture, position, borderSize, squareSize * size + totalSquareDistance, borderColor);

            Vector2 topLeft = new Vector2(position.X - (squareSize * size - squareSize + totalSquareDistance) / 2, position.Y - (squareSize * size - squareSize + totalSquareDistance) / 2);
            CompleteSize = (Position.X - topLeft.X) * 2 + squareSize + totalSquareDistance;

            SquareSize = squareSize;

            Size = size;
            Squares = new Square[size, size];
            bool hasPiece = true;
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    hasPiece = !hasPiece;
                    Squares[x, y] = new Square(squareTexture, new Vector2(topLeft.X + (squareSize + squareDistance) * x, topLeft.Y + (squareSize + squareDistance) * y), squareSize, hasPiece ? color1 : color2) { X = x, Y = y, TopLeft = new Vector2(topLeft.X + x * squareSize - (squareSize / 2), topLeft.Y + y * squareSize - (squareSize / 2)) };

                    if (hasPiece && (y < ((size - 3) / 2 + 1) || y > ((size - 3) / 2 + 2)))
                    {
                        bool topTeam = y < ((size - 3) / 2 + 1);
                        Squares[x, y].SetPiece(new Piece(pieceTexture, Squares[x, y].Position, topTeam ? pieceColor1 : pieceColor2, pieceSize, topTeam));
                    }
                }

                if (size % 2 == 0)
                {
                    hasPiece = !hasPiece;
                }
            }
        }

        public void MoveBoard(Vector2 position)
        {
            Vector2 topLeft = new Vector2(Position.X - CompleteSize/2, Position.Y - CompleteSize/2);

            Position = position;
            for (int x = 0; x < Size; x++)
            {
                for (int y = 0; y < Size; y++)
                {
                    Squares[x, y].TopLeft = topLeft + new Vector2(x * SquareSize, y * SquareSize);
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (Square piece in Squares)
            {
                piece.Draw(gameTime);
            }

            Border.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Border.Update(gameTime);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public bool CheckForClick()
        {
            return Input.MousePosition.X > Position.X - CompleteSize / 2 && Input.MousePosition.X < Position.X + CompleteSize / 2
                && Input.MousePosition.Y > Position.Y - CompleteSize / 2 && Input.MousePosition.Y < Position.Y + CompleteSize / 2
                && Input.JustLeftClicked();
        }

        public Square GetSquareClicked()
        {
            if (CheckForClick())
            {
                Vector2 topLeft = new Vector2(Position.X - CompleteSize / 2, Position.Y - CompleteSize / 2);
                float modifiedX = Input.MousePosition.X - topLeft.X;
                float modifiedY = Input.MousePosition.Y - topLeft.Y;

                Vector2 modifiedPosition = new Vector2(modifiedX - (modifiedX % SquareSize), modifiedY - (modifiedY % SquareSize));
                modifiedPosition += topLeft;

                foreach (Square square in Squares)
                {
                    if (square.TopLeft == modifiedPosition)
                    {
                        return square;
                    }
                }
            }
            return null;
        }

        public bool TeamCanJump(bool team)
        {
            foreach (Square square in Squares)
            {
                Piece piece = square.OccupyingPiece;
                if (piece != null)
                {
                    #region BottomTeam
                    if (!piece.TopTeam || piece.King)
                    {
                        if (piece.X > 1 && piece.Y > 1)
                        {
                            Square topLeft = Squares[square.X - 1, square.Y - 1];
                            Square topLeftTopLeft = Squares[square.X - 1, square.Y - 1];

                            if (topLeft.OccupyingPiece != null && topLeft.OccupyingPiece.TopTeam != piece.TopTeam)
                            {
                                if (Squares[topLeftTopLeft.X, topLeftTopLeft.Y].OccupyingPiece == null)
                                {
                                    return true;
                                }
                            }
                        }
                        if(piece.X < Size - 2 && piece.Y > 1)
                        {
                            Square topRight = Squares[square.X + 1, square.Y - 1];
                            Square topRightTopRight = Squares[square.X + 2, square.Y - 2];

                            if (topRight.OccupyingPiece != null && topRight.OccupyingPiece.TopTeam != piece.TopTeam)
                            {
                                if (Squares[topRightTopRight.X, topRightTopRight.Y].OccupyingPiece == null)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                    #endregion BottomTeam
                    #region TopTeam
                    if (piece.TopTeam || piece.King)
                    {
                        if (piece.X > 1 && piece.Y < Size - 2)
                        {
                            Square bottomLeft = Squares[square.X - 1, square.Y + 1];
                            Square bottomLeftBottomLeft = Squares[square.X - 2, square.Y + 2];
                            if (bottomLeft.OccupyingPiece != null && bottomLeft.OccupyingPiece.TopTeam != piece.TopTeam)
                            {
                                if (Squares[bottomLeftBottomLeft.X, bottomLeftBottomLeft.Y].OccupyingPiece == null)
                                {
                                    return true;
                                }
                            }
                        }
                        if (piece.X < Size - 2 && piece.Y < Size - 2)
                        {
                            Square bottomRight = Squares[square.X + 1, square.Y + 1];
                            Square bottomRightBottomRight = Squares[square.X + 2, square.Y + 2];
                            if (bottomRight.OccupyingPiece != null && bottomRight.OccupyingPiece.TopTeam != piece.TopTeam)
                            {
                                if (Squares[bottomRightBottomRight.X, bottomRightBottomRight.Y].OccupyingPiece == null)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                    #endregion TopTeam
                }
            }
            return false;
        }

        public Piece GetPieceAt(int x, int y)
        {
            return Squares[x, y].OccupyingPiece;
        }

        public Square GetSquareAt(int x, int y)
        {
            return Squares[x, y];
        }
    }

    public enum Direction
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }
}
