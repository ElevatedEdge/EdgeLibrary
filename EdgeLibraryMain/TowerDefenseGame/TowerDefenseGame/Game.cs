﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using EdgeLibrary;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;

namespace TowerDefenseGame
{
    public class Game
    {
        public void OnInit()
        {
            EdgeGame.InitializeWorld(new Vector2(0, 0));

            EdgeGame.GameSpeed = 1;

            EdgeGame.ClearColor = new Color(10, 10, 10);

            EdgeGame.IsShuffled = true;

            OptionsMenu.FullscreenOn = false;
            OptionsMenu.MusicOn = true;
            OptionsMenu.ParticlesOn = true;
            OptionsMenu.SoundEffectsOn = true;

            EdgeGame.OnUpdate += OnUpdate;
            EdgeGame.OnDraw += OnDraw;
            EdgeGame.OnResized += EdgeGame_OnResized;

            MenuManager.Init();
            MenuManager.AddMenu(new MainMenu());
            MenuManager.AddMenu(new CreditsMenu());
            MenuManager.AddMenu(new GameMenu());
            MenuManager.AddMenu(new GameSelectMenu());
            MenuManager.AddMenu(new OptionsMenu());
            MenuManager.SwitchMenu("MainMenu");
        }

        void EdgeGame_OnResized()
        {
            throw new NotImplementedException();
        }

        public void OnUpdate(GameTime gameTime)
        {
            MenuManager.Update(gameTime);
        }

        public void OnDraw(GameTime gameTime)
        {
            MenuManager.Draw(gameTime);
        }

        public void OnLoadContent()
        {
            //Window size must be set here for the credits render target
            EdgeGame.WindowSize = new Vector2(1000, 800);

            EdgeGame.LoadFont("Fonts/Comic Sans/ComicSans-10");
            EdgeGame.LoadFont("Fonts/Comic Sans/ComicSans-20");
            EdgeGame.LoadFont("Fonts/Comic Sans/ComicSans-30");
            EdgeGame.LoadFont("Fonts/Comic Sans/ComicSans-40");
            EdgeGame.LoadFont("Fonts/Comic Sans/ComicSans-50");
            EdgeGame.LoadFont("Fonts/Comic Sans/ComicSans-60");
            EdgeGame.LoadFont("Fonts/Courier New/CourierNew-10");
            EdgeGame.LoadFont("Fonts/Courier New/CourierNew-20");
            EdgeGame.LoadFont("Fonts/Courier New/CourierNew-30");
            EdgeGame.LoadFont("Fonts/Courier New/CourierNew-40");
            EdgeGame.LoadFont("Fonts/Courier New/CourierNew-50");
            EdgeGame.LoadFont("Fonts/Courier New/CourierNew-60");
            EdgeGame.LoadFont("Fonts/Georgia/Georgia-10");
            EdgeGame.LoadFont("Fonts/Georgia/Georgia-20");
            EdgeGame.LoadFont("Fonts/Georgia/Georgia-30");
            EdgeGame.LoadFont("Fonts/Georgia/Georgia-40");
            EdgeGame.LoadFont("Fonts/Georgia/Georgia-50");
            EdgeGame.LoadFont("Fonts/Georgia/Georgia-60");
            EdgeGame.LoadFont("Fonts/Georgia/Georgia-SemiSmall");
            EdgeGame.LoadFont("Fonts/Impact/Impact-10");
            EdgeGame.LoadFont("Fonts/Impact/Impact-20");
            EdgeGame.LoadFont("Fonts/Impact/Impact-30");
            EdgeGame.LoadFont("Fonts/Impact/Impact-40");
            EdgeGame.LoadFont("Fonts/Impact/Impact-50");
            EdgeGame.LoadFont("Fonts/Impact/Impact-60");
            EdgeGame.LoadBitmapFont("Fonts/KenVector/kenvector_future_regular_32", "Fonts/KenVector/kenvector_future_regular_32");
            EdgeGame.LoadBitmapFont("Fonts/windsong_regular_65", "Fonts/windsong_regular_65");
            EdgeGame.LoadTexturesInSpritesheet("ParticleSheet", "ParticleSheet");
            EdgeGame.LoadTexturesInSpritesheet("GUI/ButtonSheet", "GUI/ButtonSheet");
            EdgeGame.LoadTexturesInSpritesheet("GUI/GreyGUI", "GUI/GreyGUI");
            EdgeGame.LoadTexturesInSpritesheet("GUI/WhiteIcons2x", "GUI/WhiteIcons2x");
            EdgeGame.LoadTexturesInSpritesheet("GUI/UIRPG", "GUI/UIRPG");
            EdgeGame.LoadTexturesInSpritesheet("KenneyImages", "KenneyImages");
            EdgeGame.LoadTexture("Levels/Grassy Plains");
            EdgeGame.LoadTexture("Levels/Islands");
            EdgeGame.LoadTexture("Levels/Rocky Bridges");
            EdgeGame.LoadTexture("Levels/Village Loop");
            EdgeGame.LoadTexture("Levels/Around the Lake");
            EdgeGame.LoadTexture("Levels/Return to the Village");
            EdgeGame.LoadTexture("Levels/Grassy Plains Preview");
            EdgeGame.LoadTexture("Levels/Islands Preview");
            EdgeGame.LoadTexture("Levels/Rocky Bridges Preview");
            EdgeGame.LoadTexture("Levels/Village Loop Preview");
            EdgeGame.LoadTexture("Levels/Around the Lake Preview");
            EdgeGame.LoadTexture("Levels/Return to the Village Preview");
            EdgeGame.LoadTexture("Levels/Color Quadrants");
            EdgeGame.LoadTexture("Levels/Color Quadrants Preview");
            EdgeGame.LoadTexture("Circle");
            EdgeGame.LoadTexture("CircleOutline");
            EdgeGame.LoadTexture("flameBlue");
            EdgeGame.LoadTexture("Health Bar/health10");
            EdgeGame.LoadTexture("Health Bar/health9");
            EdgeGame.LoadTexture("Health Bar/health8");
            EdgeGame.LoadTexture("Health Bar/health7");
            EdgeGame.LoadTexture("Health Bar/health6");
            EdgeGame.LoadTexture("Health Bar/health5");
            EdgeGame.LoadTexture("Health Bar/health4");
            EdgeGame.LoadTexture("Health Bar/health3");
            EdgeGame.LoadTexture("Health Bar/health2");
            EdgeGame.LoadTexture("Health Bar/health1");
            EdgeGame.LoadTexture("Health Bar/health0");
            EdgeGame.LoadTexture("target");

            EdgeGame.LoadSong("Music/Chipper Doodle v2");
            EdgeGame.LoadSong("Music/Funky Chunk");
            EdgeGame.LoadSong("Music/Firebrand");
            EdgeGame.AddPlaylist("Music", "Chipper Doodle v2", "Funky Chunk");
            EdgeGame.AddPlaylist("TitleMusic", "Firebrand");

            //Creating the Credits 'particle' text
            RenderTargetImager imager = new RenderTargetImager();
            imager.Components.Add(new TextSprite(Config.MenuTitleFont, "Credits", new Vector2(EdgeGame.WindowSize.X * 0.5f, EdgeGame.WindowSize.Y * 0.05f)) { Color = Color.White });
            imager.Components.Add(new TextSprite(Config.MenuTitleFont, "Jaren", new Vector2(EdgeGame.WindowSize.X * 0.25f, EdgeGame.WindowSize.Y * 0.25f)) { Color = Color.White });
            imager.Components.Add(new TextSprite(Config.MenuTitleFont, "Aaron", new Vector2(EdgeGame.WindowSize.X * 0.75f, EdgeGame.WindowSize.Y * 0.25f)) { Color = Color.White });
            imager.Components.Add(new TextSprite("Georgia-40", "Incompetech", new Vector2(EdgeGame.WindowSize.X * 0.25f, EdgeGame.WindowSize.Y * 0.75f)) { Color = Color.White });
            imager.Components.Add(new TextSprite(Config.MenuTitleFont, "GMR", new Vector2(EdgeGame.WindowSize.X * 0.75f, EdgeGame.WindowSize.Y * 0.75f)) { Color = Color.White });
            Texture2D creditsTexture = imager.RenderToTarget(Color.Black);
            TextureEditor editor = new TextureEditor();
            editor.OnEditPixel += editor_OnEditPixel;
            editor.ApplyTo(creditsTexture);
            EdgeGame.AddTexture("CreditsTexture", creditsTexture.Clone());
        }

        private void editor_OnEditPixel(ref Color color, int x, int y)
        {
            if (color != Color.Black)
            {
                color = Color.Transparent;
            }
        }
    }
}
