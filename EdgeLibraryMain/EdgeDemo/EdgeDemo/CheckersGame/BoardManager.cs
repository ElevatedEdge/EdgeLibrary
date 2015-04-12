﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdgeLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using EdgeDemo.CheckersService;

namespace EdgeDemo.CheckersGame
{
    /// <summary>
    /// This class manages the board and updates it
    /// </summary>
    public class BoardManager : Sprite
    {
        //Game components to update and draw
        public static List<GameComponent> Components { get; private set; }

        //The main board - static so other classes can access it
        public static Board Board { get; private set; }

        //If set to true, will reset the game when created by GameMenu
        public static bool ResetGame = true;

        //Displays which team should move and the move state
        public TextSprite StatusSprite;

        //Displays how many captures each team has
        public static TextSprite CaptureSprite;

        //The sprite which displays when there is an important message
        public static ColorTextSprite MessageSprite;

        //Displays debug info
        public TextSprite DebugSprite;

        //Is it player 1's turn
        public static bool Player1Turn;

        //The players in the game
        public static Player Player1;
        public static Player Player2;

        //Text for the current team
        private string TeamText;
        public int TurnsCount;

        CheckersServiceClient ServiceClient = new CheckersServiceClient();

        public BoardManager()
            : base("", Vector2.Zero)
        {
            //Initializing the board
            Components = new List<GameComponent>();
            Board = new Board(Config.SquareTexture, EdgeGame.WindowSize / 2, Config.BoardSize, Config.SquareSize, Config.SquareDistance, Config.SquareColor1, Config.SquareColor2, Config.BorderSize, Config.BorderColor, Config.PieceTexture, Config.PieceSize, Config.TopColor, Config.BottomColor);
            Components.Add(Board);

            //Initializing the debug sprite
            DebugSprite = new DebugText(Config.DebugFont, new Vector2(0, EdgeGame.WindowSize.Y - 250)) { Color = Color.Goldenrod, CenterAsOrigin = false, FollowsCamera = false, ScaleWithCamera = false, Include3D = false };
            Components.Add(DebugSprite);

            //Initializing the teamtext
            TeamText = Config.Player1Name + ": ";

            if (MenuManager.PreviousMenu is ToGameMenu)
            {
                Player pp1 = ((ToGameMenu)MenuManager.PreviousMenu).Player1;
                Player pp2 = ((ToGameMenu)MenuManager.PreviousMenu).Player2;

                if (pp1 is WebPlayer)
                {
                    Player1 = new WebPlayer(((WebPlayer)pp1).TeamName);
                }
                else
                {
                    Player1 = ((ToGameMenu)MenuManager.PreviousMenu).Player1;
                }

                if (pp2 is WebPlayer)
                {
                    Player2 = new WebPlayer(((WebPlayer)pp2).ThisGameID, ((WebPlayer)pp2).TeamName);
                }
                else
                {
                    Player2 = ((ToGameMenu)MenuManager.PreviousMenu).Player2;
                }
            }
            else
            {
                //Should not be called
                Player1 = new NormalPlayer();
                Player2 = new NormalPlayer();
            }

            //Initializing status sprite
            StatusSprite = new TextSprite(Config.StatusFont, Config.Player1Name + "'s Turn", Vector2.Zero) { CenterAsOrigin = false, FollowsCamera = false, ScaleWithCamera = false };
            Components.Add(StatusSprite);

            //Initializing message sprite
            MessageSprite = new ColorTextSprite(Config.BigStatusFont, EdgeGame.WindowSize / 2, new ColorChangeIndex(1000, Color.Transparent, Color.Red, Color.Transparent));
            Components.Add(MessageSprite);

            //Initializing capture sprite
            CaptureSprite = new TextSprite(Config.StatusFont, "Top Team Captures: 0\nBottom Team Captures: 0", new Vector2(0, 50)) { CenterAsOrigin = false, FollowsCamera = false, ScaleWithCamera = false };
            Components.Add(CaptureSprite);

            //Subscribes to the player events
            Player1.OnRunMove += Player1_OnRunMove;
            Player2.OnRunMove += Player2_OnRunMove;
            TurnsCount = 0;

            //Starts the game off with player 1 moving first
            Player1Turn = false;
            Player2.ReceivePreviousMove(null, MovementManager.GeneratePlayerMoves(Player1Turn));
        }

        //Necessary override to not draw the BoardManager
        public override void Draw(GameTime gameTime)
        {
            if (Visible)
            {
                foreach (DrawableGameComponent component in Components.OfType<DrawableGameComponent>())
                {
                    component.Draw(gameTime);
                }

                Player1.Draw(gameTime);
                Player2.Draw(gameTime);
            }
        }

        //Updates the board
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            foreach (GameComponent component in Components)
            {
                component.Update(gameTime);
            }

            Player1.Update(gameTime);
            Player2.Update(gameTime);

            if (Board.mousedOverSquare != null)
            {
                DebugSprite.Text += "Last Moused Over Square: " + Board.mousedOverSquare.X + ", " + Board.mousedOverSquare.Y;
            }
        }

        void Player1_OnRunMove(Move move)
        {
            RunMove(move);

            StatusSprite.Text = "It is " + Config.Player2Name + "'s Turn";

            Player1Turn = false;
            if (!Player2.ReceivePreviousMove(move, MovementManager.GeneratePlayerMoves(Player1Turn)))
            {
                EndGame();
            }
        }

        void Player2_OnRunMove(Move move)
        {
            RunMove(move);

            StatusSprite.Text = "It is " + Config.Player1Name + "'s Turn";

            Player1Turn = true;
            if (!Player1.ReceivePreviousMove(move, MovementManager.GeneratePlayerMoves(Player1Turn)))
            {
                EndGame();
            }
        }

        public void RunMove(Move move)
        {
            move.RunMove(BoardManager.Board);
        }

        //Ends the game
        public static void EndGame()
        {
            
            MessageSprite.Display((!Player1Turn ? Config.Player1Name : Config.Player2Name) + " Has Won the Game", new ColorChangeIndex(5000, Color.Blue, Color.Transparent));
            Ticker ticker = new Ticker(6000);
            ticker.Enabled = true;
            ticker.OnTick += new Ticker.TickerEventHandler(ticker_OnTick);
            Components.Add(ticker);
        }

        static void ticker_OnTick(GameTime gameTime)
        {
            MenuManager.SwitchMenu("MainMenu");
        }
    }
}
