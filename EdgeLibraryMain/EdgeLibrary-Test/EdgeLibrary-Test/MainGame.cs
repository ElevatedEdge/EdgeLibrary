using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using EdgeLibrary.Basic;
using EdgeLibrary.Menu;
using EdgeLibrary.Effects;

namespace EdgeLibrary_Test
{
    /// <summary>
    /// TODO:
    /// -General
    ///     -Add "Animation" function
    ///     -Fix "Remove Element/Object" function
    ///     -Add basic collision functions
    ///         -Fix collision between a circle and a rectangle
    /// -Actions
    ///     -Fix "EActionSequence"
    ///     -Fix "EActionRotate"?
    /// -Menu
    ///     -More Menu Items
    ///         -Label button
    /// </summary>
    /// 

    /// <summary>
    /// MUSIC AND TEXTURES:
    /// - cynicmusic.com/pixelsphere.org
    /// - MoikMellah at OpenGameArt.org
    /// </summary>
    /// 

    public class MainGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        EdgeGame edgeGame;

        #region NOT-USED
        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        protected override void Initialize()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            edgeGame = new EdgeGame(Content, spriteBatch, graphics, GraphicsDevice);
            edgeGame.Init();

            initializeGame();

            base.Initialize();
        }
        protected override void LoadContent()
        {
            edgeGame.LoadContent();
        }
        protected override void UnloadContent() { edgeGame.UnloadContent(); }
        protected override void Update(GameTime gameTime)
        {
            edgeGame.Update(new EUpdateArgs(gameTime, Keyboard.GetState(), Mouse.GetState()));
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            edgeGame.Draw(gameTime);
            base.Draw(gameTime);
        }
        #endregion

        private void initializeGame()
        {
            loadResources();
            initializeGameWindow();
            initializeMenuScene();
            initializeGameScene();
        }

        private void loadResources()
        {
            edgeGame.LoadSong("battleThemeA", "battleSong");

            edgeGame.LoadTexture("Particle Textures/fire", "fire");
            edgeGame.LoadTexture("Particle Textures/stars", "star");
            edgeGame.LoadTexture("Particle Textures/smoke", "smoke");
            edgeGame.LoadTexture("Particle Textures/snow", "snow");

            edgeGame.LoadTexture("Players/Ninja/ninja1", "ninja");
            edgeGame.LoadTexture("Players/Normal/player1", "player");
            edgeGame.LoadTexture("Statues/sprite1", "statues");
        }

        private void initializeGameWindow()
        {
            edgeGame.playSong("battleSong");
            edgeGame.setWindowHeight(1000);
            edgeGame.setWindowWidth(1000);
            IsMouseVisible = true;
        }

        private void initializeMenuScene()
        {
            EScene menuScene = new EScene("menuScene");
            edgeGame.addScene(menuScene);

            ESprite sprite = new ESprite("player", new Vector2(450, 450), 50, 100);
            menuScene.addElement(sprite);

            #region PARTICLES
            EParticleEmitter dotsEmitter = new EParticleEmitter("snow", new Vector2(500, 0));
            dotsEmitter.ShouldEmit = true;
            dotsEmitter.DrawLayer = 1;
            dotsEmitter.EmitPositionVariance = new ERangeArray(ERange.RangeWithDiffer(0, 900), ERange.RangeWithDiffer(0, 0));
            dotsEmitter.ColorVariance = new ERangeArray(new ERange(100), new ERange(100), new ERange(100), new ERange(255));
            dotsEmitter.VelocityVariance = new ERangeArray(ERange.RangeWithDiffer(0, 0.1f), ERange.RangeWithDiffer(8, 2.5f));
            dotsEmitter.SizeVariance = new ERangeArray(new ERange(15), new ERange(15));
            dotsEmitter.GrowSpeed = 0f;
            dotsEmitter.StartRotationVariance = ERange.RangeWithDiffer(0, 0);
            dotsEmitter.RotationSpeedVariance = ERange.RangeWithDiffer(0, 0);
            dotsEmitter.LifeVariance = new ERange(5000);
            dotsEmitter.EmitWait = 0;
            menuScene.addElement(dotsEmitter);

            EParticleEmitter mouseEmitter = new EParticleEmitter("fire", new Vector2(400, 400));
            mouseEmitter.ShouldEmit = true;
            mouseEmitter.DrawLayer = 3;
            mouseEmitter.EmitPositionVariance = new ERangeArray(new ERange(0), new ERange(0));
            mouseEmitter.ColorVariance = new ERangeArray(new ERange(0), new ERange(40, 80), new ERange(40, 80), new ERange(255));
            mouseEmitter.VelocityVariance = new ERangeArray(ERange.RangeWithDiffer(0, 4), ERange.RangeWithDiffer(0, 4));
            mouseEmitter.SizeVariance = new ERangeArray(ERange.RangeWithDiffer(100, 25), ERange.RangeWithDiffer(100, 25));
            mouseEmitter.GrowSpeed = 1f;
            mouseEmitter.StartRotationVariance = ERange.RangeWithDiffer(0, 0);
            mouseEmitter.RotationSpeedVariance = ERange.RangeWithDiffer(0, 0);
            mouseEmitter.LifeVariance = new ERange(1000);
            mouseEmitter.EmitWait = 0;
            mouseEmitter.ActionToRunOnParticles = new EActionFollow(sprite, 4);
            mouseEmitter.ClampToMouse();
            menuScene.addElement(mouseEmitter);
            #endregion

            ESprite s1 = new ESprite("player", new Vector2(100, 100), 50, 50);
            s1.AddCollision(new ECollisionBody(new EShapeCircle(Vector2.Zero, 1), "something"));
            s1.runAction(new EActionMove(new Vector2(500, 100), 1));
            menuScene.addElement(s1);
            ESprite s2 = new ESprite("player", new Vector2(500, 100), 50, 50);
            s2.AddCollision(new ECollisionBody(new EShapeCircle(Vector2.Zero, 1), "something"));
            s2.runAction(new EActionMove(new Vector2(100, 100), 1));
            menuScene.addElement(s2);
        }

        private void initializeGameScene()
        {
            EScene gameScene = new EScene("gameScene");

            edgeGame.addScene(gameScene);
        }

        private void SpriteCollisionStart(ESpriteCollisionArgs e)
        {
        }
    }
}
