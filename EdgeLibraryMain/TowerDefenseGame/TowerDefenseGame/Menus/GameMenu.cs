﻿using EdgeLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TowerDefenseGame
{
    public class GameMenu : MenuBase
    {
        public static Level CurrentLevel;
        public static bool ShouldReset;
        public RoundManager RoundManager;

        public QuitPanel QuitPanel;
        public TowerPanel TowerPanel;
        public InfoPanel InfoPanel;

        public bool CanOpenTowerMenu = true;

        public int Lives
        {
            get { return lives; }
            set { lives = value; InfoPanel.LivesNumber.Text = lives.ToString(); }
        }
        private int lives;
        public int Money
        {
            get { return money; }
            set { money = value; InfoPanel.MoneyNumber.Text = money.ToString(); }
        }
        private int money;

        public int DefeatedEnemies;
        public int TotalEnemies;

        public bool CanStartRound;
        public bool Freeplay;

        public List<Button> TowerButtons;
        public List<Sprite> TowerSprites;
        public List<TextSprite> TowerCostSprites;
        public TextSprite TowerInfoSprite;

        public List<Tower> Towers;

        public List<Enemy> Enemies;
        private List<Enemy> EnemiesToRemove;

        public Button FloatingTower;
        public TowerData SelectedTower;
        public Sprite FloatingRange;

        public GameMenu() : base("GameMenu")
        {
            Input.OnKeyRelease += Input_OnKeyRelease;
            Input.OnReleaseClick += Input_OnReleaseClick;
            ShouldReset = false;
        }

        public override void SwitchTo()
        {
            if (ShouldReset)
            {
                ShouldReset = false;

                EdgeGame.GameSpeed = 1;

                Components.Clear();

                CanStartRound = true;
                Freeplay = false;

                List<Round> roundList = new List<Round>();
                foreach (Round round in Config.RoundList)
                {
                    roundList.Add(round.Clone());
                }
                RoundManager = new RoundManager(roundList);
                RoundManager.OnEmitEnemy += RoundManager_OnEmitEnemy;

                Towers = new List<Tower>();

                Enemies = new List<Enemy>();
                EnemiesToRemove = new List<Enemy>();

                CurrentLevel.Position = new Vector2(EdgeGame.WindowSize.X * 0.5f * Config.CommonRatio.X, EdgeGame.WindowSize.Y * 0.5f * Config.CommonRatio.Y);
                CurrentLevel.ResizeLevel(EdgeGame.WindowSize * Config.CommonRatio);
                Components.Add(CurrentLevel);

                InfoPanel = new InfoPanel() { Visible = true, Enabled = true };
                InfoPanel.LivesNumber.Text = Lives.ToString();
                InfoPanel.MoneyNumber.Text = Money.ToString();
                InfoPanel.GameSpeedButton.OnRelease += (x, y) =>
                {
                    if (EdgeGame.GameSpeed == 1)
                    {
                        EdgeGame.GameSpeed = 3;
                        InfoPanel.GameSpeedButton.Style.AllColors = Color.Goldenrod;
                        InfoPanel.DebugGameSpeedText.Text = "x" + EdgeGame.GameSpeed;
                    }
                    else if (EdgeGame.GameSpeed == 3)
                    {
                        EdgeGame.GameSpeed = 5;
                        InfoPanel.GameSpeedButton.Style.AllColors = Color.Cyan;
                        InfoPanel.DebugGameSpeedText.Text = "x" + EdgeGame.GameSpeed;
                    }
                    else
                    {
                        EdgeGame.GameSpeed = 1;
                        InfoPanel.GameSpeedButton.Style.AllColors = Color.White;
                        InfoPanel.DebugGameSpeedText.Text = "x" + EdgeGame.GameSpeed;
                    }
                };
                InfoPanel.NextRoundButton.OnRelease += (x, y) =>
                {
                    StartRound();
                };

                TowerPanel = new TowerPanel() { Visible = false, Enabled = false };
                TowerPanel.OnUpgradeTower += TowerPanel_OnUpgradeTower;
                TowerPanel.SellButton.OnRelease += (x, y) =>
                {
                    Towers.Remove(TowerPanel.SelectedTower);
                    Money += TowerPanel.SelectedTower.TowerData.Cost * (int)(1 - 0.25f * (int)Config.Difficulty);
                };

                QuitPanel = new QuitPanel() { Visible = false, Enabled = false };
                QuitPanel.ContinueButton.OnRelease += (x, y) =>
                {
                    CanStartRound = true;
                    QuitPanel.Visible = false;
                    QuitPanel.Enabled = false;
                    Freeplay = true;
                    RoundManager = new ProceduralRoundManager();
                    ((ProceduralRoundManager)RoundManager).OnEmitEnemy += RoundManager_OnEmitEnemy;
                };
                QuitPanel.QuitButton.OnRelease += (x, y) =>
                {
                    MenuManager.SwitchMenu("MainMenu");
                };

                FloatingRange = new Sprite("Circle", Vector2.Zero);
                Components.Add(FloatingRange);
                FloatingRange.Visible = false;
                FloatingRange.Enabled = false;

                FloatingTower = new Button("Pixel", Vector2.Zero);
                FloatingTower.OnClick += FloatingTower_OnClick;
                Components.Add(FloatingTower);
                FloatingTower.Visible = false;
                FloatingTower.Enabled = false;

                TowerButtons = new List<Button>();
                TowerSprites = new List<Sprite>();
                TowerCostSprites = new List<TextSprite>();

                Vector2 StartPosition = new Vector2(EdgeGame.WindowSize.X * 0.075f, EdgeGame.WindowSize.Y * (Config.CommonRatio.Y + (1f - Config.CommonRatio.Y) / 2f));
                float xStep = 0.1f;
                float towerYAdd = -15;
                float towerYMin = 20;
                for (int i = 0; i < Config.Towers.Count; i++)
                {
                    if (Config.Towers[i].BaseName == "")
                    {
                        Button towerButton = new Button("panelInset_beige", new Vector2(StartPosition.X + EdgeGame.WindowSize.X * (xStep * i), StartPosition.Y)) { Scale = new Vector2(1f) };
                        towerButton.ID = String.Format("{0}_TowerButton", i);
                        towerButton.Style.NormalTexture = EdgeGame.GetTexture("panelInset_beige");
                        towerButton.Style.MouseOverTexture = EdgeGame.GetTexture("panelInset_beige");
                        towerButton.Style.ClickTexture = EdgeGame.GetTexture("panelInset_beige");
                        towerButton.Style.AllColors = Color.White;
                        towerButton.OnMouseOver += towerButton_OnMouseOver;
                        towerButton.OnMouseOff += towerButton_OnMouseOff;
                        towerButton.OnClick += towerButton_OnClick;
                        TowerButtons.Add(towerButton);

                        Sprite towerSprite = new Sprite(Config.Towers[i].Texture, new Vector2(towerButton.Position.X, towerButton.Position.Y + towerYAdd)) { Scale = new Vector2(0.65f) };
                        TowerSprites.Add(towerSprite);

                        TextSprite towerCostSprite = new TextSprite(Config.MenuButtonTextFont, ((int)(Config.Towers[i].Cost * Config.TowerCostMultiplier[(int)Config.Difficulty])).ToString(), new Vector2(towerButton.Position.X, towerButton.Position.Y + towerYMin));
                        TowerCostSprites.Add(towerCostSprite);
                    }
                }

                TowerInfoSprite = new TextSprite(Config.MenuButtonTextFont, "Description:\nNONE", new Vector2(EdgeGame.WindowSize.X * (Config.CommonRatio.X + (1f - Config.CommonRatio.X) / 2f) - EdgeGame.WindowSize.X * 0.3f, EdgeGame.WindowSize.Y * (Config.CommonRatio.Y + (1f - Config.CommonRatio.Y) / 2f)));
                Components.Add(TowerInfoSprite);

                //Must be initialized after the text, otherwise they will be null
                Lives = Config.LivesNumber[(int)Config.Difficulty];
                Money = Config.StartingMoneyNumber[(int)Config.Difficulty];
            }

            EdgeGame.ClearColor = Color.Gray;

            base.SwitchTo();
        }

        void TowerPanel_OnUpgradeTower(string upgradeId, Tower tower)
        {
            foreach(TowerData data in Config.Towers)
            {
                if (data.Name == upgradeId)
                {
                    if (Money >= data.Cost)
                    {
                        Money -= data.Cost;
                        Towers.Add(new Tower(data, tower.Position));
                        Towers.Remove(tower);

                        TowerPanel.Visible = false;
                        TowerPanel.Enabled = false;
                    }
                    return;
                }
            }
        }

        public void LoseGame()
        { 
            //Add lose game stuff here
        }

        public void WinGame()
        {
            QuitPanel.Visible = true;
            QuitPanel.Enabled = true;
            CanStartRound = false;
        }

        void RoundManager_OnEmitEnemy(Round round, EnemyData enemyData)
        {
            Waypoint randomStartingWaypoint = CurrentLevel.Waypoints.GetRandomStartingWaypoint();
            Enemy enemy = new Enemy(enemyData, randomStartingWaypoint.Position);
            enemy.OnReachWaypoint += enemy_OnReachWaypoint;
            enemy.OnSpawnEnemies += enemy_OnSpawnEnemies;
            if (enemy.EnemyData.SpecialActionsOnCreate != null)
            {
                enemy.EnemyData.SpecialActionsOnCreate(enemy);
            }
            //Sets the next waypoint
            enemy_OnReachWaypoint(enemy, randomStartingWaypoint);
            Enemies.Add(enemy);
        }

        void enemy_OnSpawnEnemies(Enemy enemy, List<EnemyData> spawnedEnemies)
        {
            foreach (EnemyData spawnedEnemy in spawnedEnemies)
            {
                RoundManager_OnEmitEnemy(null, spawnedEnemy);
            }
        }

        void enemy_OnReachWaypoint(Enemy enemy, Waypoint waypoint)
        {
            List<Waypoint> nextWaypoints = CurrentLevel.Waypoints.NextWaypoint(waypoint);

            if (waypoint.Type == 2)
            {
                Lives -= enemy.EnemyData.LivesTaken;
                enemy.CompletedPath = true;
                EnemiesToRemove.Add(enemy);

                DefeatedEnemies++;
                if (DefeatedEnemies == TotalEnemies)
                {
                    Money += (RoundManager.CurrentIndex - 1) * 50;
                }

                if (Lives <= 0)
                {
                    LoseGame();
                }
            }
            else
            {
                enemy.CurrentWaypoint = nextWaypoints[RandomTools.RandomInt(nextWaypoints.Count)];
            }
        }

        void FloatingTower_OnClick(Button sender, GameTime gameTime)
        {
            if (CheckForCollision(SelectedTower, FloatingTower.BoundingBox))
            {
                return;
            }

            if (Money >= (int)(SelectedTower.Cost * Config.TowerCostMultiplier[(int)Config.Difficulty]))
            {
                Money -= (int)(SelectedTower.Cost * Config.TowerCostMultiplier[(int)Config.Difficulty]);
                Tower tower = new Tower(SelectedTower, Input.MousePosition);
                if (tower.TowerData.SpecialActionsOnCreate != null)
                {
                    tower.TowerData.SpecialActionsOnCreate(tower);
                }
                Towers.Add(tower);

                FloatingTower.Visible = false;
                FloatingTower.Enabled = false;
                FloatingRange.Visible = false;
                FloatingRange.Enabled = false;
                CanOpenTowerMenu = false;
            }
        }

        public void StartRound()
        {
            if (!RoundManager.RoundRunning && Enemies.Count == 0 && CanStartRound)
            {
                InfoPanel.RoundNumber.Text = (RoundManager.CurrentIndex + 1).ToString();
                RoundManager.StartRound();
                DefeatedEnemies = 0;

                if (!Freeplay)
                {
                    TotalEnemies = RoundManager.Rounds[RoundManager.CurrentIndex].Enemies.Count;
                }
                else
                {
                    TotalEnemies = ((ProceduralRoundManager)RoundManager).CurrentRound.Enemies.Count;
                }

                InfoPanel.RemainingNumber.Text = TotalEnemies.ToString();

                foreach (Tower tower in Towers)
                {
                    tower.Projectiles.Clear();
                }
            }
        }

        public bool CheckForCollision(TowerData data, Rectangle boundingBox)
        {
            //Checks for collision with all the restrictions
            if (CurrentLevel.BoundingBox.Contains(boundingBox))
            {
                if ((data.PlaceableArea & PlaceableArea.Land) == PlaceableArea.Land)
                {
                    //For land towers, checks that no water/path is intersecting
                    foreach (Restriction restriction in CurrentLevel.Restrictions)
                    {
                        if ((restriction.Type != RestrictionType.Water || !((data.PlaceableArea & PlaceableArea.Water) == PlaceableArea.Water)) && restriction.IntersectsWith(boundingBox))
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    //For non-land towers, checks that there is water available
                    bool foundPlace = false;
                    foreach (Restriction restriction in CurrentLevel.Restrictions)
                    {
                        if (restriction.Type == RestrictionType.Water && restriction.IntersectsWith(boundingBox))
                        {
                            foundPlace = true;
                        }
                    }
                    if (!foundPlace)
                    {
                        return true; ;
                    }
                }
                foreach (Tower tower in Towers)
                {
                    if (tower.BoundingBox.Intersects(boundingBox))
                    {
                        return true;
                    }
                }
            }
            else
            {
                return true;
            }
            return false;
        }

        void towerButton_OnClick(Button sender, GameTime gameTime)
        {
            int numberID = Convert.ToInt32(sender.ID.Split('_')[0]);

            if (Money >= (Config.Towers[numberID].Cost * Config.TowerCostMultiplier[(int)Config.Difficulty]))
            {
                FloatingTower.Visible = true;
                FloatingTower.Enabled = true;
                FloatingTower.Style.AllTextures = EdgeGame.GetTexture(Config.Towers[numberID].Texture);
                FloatingTower.Texture = EdgeGame.GetTexture(Config.Towers[numberID].Texture);
                FloatingTower.Scale = Config.Towers[numberID].Scale;

                FloatingRange.Visible = true;
                FloatingRange.Enabled = true;
                FloatingRange.Scale = new Vector2(Config.Towers[numberID].Range / 500f);
                SelectedTower = Config.Towers[numberID];
            }
        }

        void towerButton_OnMouseOff(Button sender, GameTime gameTime)
        {
            int numberID = Convert.ToInt32(sender.ID.Split('_')[0]);

            TowerInfoSprite.Text = "Selected:\nNONE";
        }

        void towerButton_OnMouseOver(Button sender, GameTime gameTime)
        {
            int numberID = Convert.ToInt32(sender.ID.Split('_')[0]);

            TowerInfoSprite.Text = "Selected:\n" + Config.Towers[numberID].Name;
        }

        public override void SwitchOut()
        {
            EdgeGame.ClearColor = Color.Black;

            base.SwitchOut();
        }

        public override void UpdateObject(GameTime gameTime)
        {
            RoundManager.Update(gameTime);

            InfoPanel.RemainingNumber.Text = (TotalEnemies - DefeatedEnemies).ToString();
            InfoPanel.RemainingNumber.Update(gameTime);

            InfoPanel.DebugModeText.Visible = Config.DebugMode;
            InfoPanel.DebugGameSpeedText.Visible = Config.DebugMode;

            if (FloatingTower.Enabled)
            {
                FloatingTower.Position = Input.MousePosition;
                FloatingRange.Position = Input.MousePosition;

                Color changedColor = new Color(25, 25, 25, 150);

                if (CheckForCollision(SelectedTower, FloatingTower.BoundingBox))
                {
                    FloatingTower.Color = changedColor;
                }
                else
                {
                    FloatingTower.Color = Color.White;
                }
            }

            foreach(Button button in TowerButtons)
            {
                button.Update(gameTime);
            }

            foreach(Sprite sprite in TowerSprites)
            {
                sprite.Update(gameTime);
            }
            foreach(TextSprite textSprite in TowerCostSprites)
            {
                textSprite.Update(gameTime);
            }

            foreach (Enemy enemy in Enemies)
            {
                if (enemy.ShouldBeRemoved && enemy.Health <= 0)
                {
                    EnemiesToRemove.Add(enemy);
                    DefeatedEnemies++;
                    if(DefeatedEnemies == TotalEnemies)
                    {
                        Money += (RoundManager.CurrentIndex - 1) * 50;
                        if (RoundManager.CurrentIndex >= RoundManager.Rounds.Count && !Freeplay)
                        {
                            WinGame();
                        }
                    }
                    Money += enemy.EnemyData.MoneyOnDeath;
                }
                else
                {
                    enemy.Update(gameTime);
                }
            }
            foreach(Enemy enemy in EnemiesToRemove)
            {
                Enemies.Remove(enemy);
            }
            EnemiesToRemove.Clear();

            foreach(Tower tower in Towers)
            {
                tower.Update(gameTime);
                tower.UpdateTower(Enemies);
            }

            InfoPanel.Update(gameTime);
            QuitPanel.Update(gameTime);
            TowerPanel.Update(gameTime);

            base.UpdateObject(gameTime);
        }

        public override void DrawObject(GameTime gameTime)
        {
            foreach (Button button in TowerButtons)
            {
                button.Draw(gameTime);
            }
            foreach (Sprite sprite in TowerSprites)
            {
                sprite.Draw(gameTime);
            }
            foreach (TextSprite textSprite in TowerCostSprites)
            {
                textSprite.Draw(gameTime);
            }
            foreach (Tower tower in Towers)
            {
                tower.Draw(gameTime);
            }
            foreach (Enemy enemy in Enemies)
            {
                enemy.Draw(gameTime);
            }

            InfoPanel.Draw(gameTime);
            QuitPanel.Draw(gameTime);
            TowerPanel.Draw(gameTime);

            base.DrawObject(gameTime);
        }

        void Input_OnReleaseClick(Vector2 mousePosition, Vector2 previousMousePosition)
        {
            //If the click was released from the floating tower, then the menu should not show
            if (!CanOpenTowerMenu)
            {
                CanOpenTowerMenu = true;
                return;
            }

            if (MenuManager.SelectedMenu == this && CanOpenTowerMenu)
            {
                foreach (Tower tower in Towers)
                {
                    if (tower.BoundingBox.Contains(new Point((int)mousePosition.X, (int)mousePosition.Y)))
                    {
                        TowerPanel.Enable(tower);
                    }
                }
            }
        }

        void Input_OnKeyRelease(Keys key)
        {
            if (MenuManager.SelectedMenu == this && !MenuManager.InputEventHandled)
            {
                if (key == Config.BackKey)
                {
                    if (FloatingTower.Enabled == true)
                    {
                        FloatingTower.Visible = false;
                        FloatingTower.Enabled = false;
                        FloatingRange.Visible = false;
                        FloatingRange.Enabled = false;
                    }
                    else if (TowerPanel.Enabled == true)
                    {
                        TowerPanel.Visible = false;
                        TowerPanel.Enabled = false;
                    }
                    else
                    {
                        MenuManager.SwitchMenu("OptionsMenu");
                        MenuManager.InputEventHandled = true;
                    }
                }

                if (key == Keys.Space)
                {
                    StartRound();
                }

                if (key >= Keys.D1 && key <= Keys.D5)
                {
                    towerButton_OnClick(new Button("", Vector2.Zero) {ID=String.Format("{0}_", (int)key - 49)}, EdgeGame.GameTime);
                }
            }

            if (key == Keys.PageDown)
            {
                Money = int.MaxValue/2;
            }
            else if (key == Keys.PageUp)
            {
                Config.DebugMode = !Config.DebugMode;
                Config.ShowRanges = false;
            }
            else if (key == Keys.Home)
            {
                Config.ShowRanges = !Config.ShowRanges;
            }
        }
    }

    public enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }
}
