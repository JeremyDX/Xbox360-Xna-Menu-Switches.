using System;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Xbox_360_Game_Project
{
    class MainMenuWindow : MainInterface
    {

        private SpriteBatch d3ddev;
        private Texture2D background;
        private Texture2D SELECT, UNSELECT;
        private MenuSystem menuSystem;
        private Vector2 location_start;
        private SignedInGamer gamer;

        private static string[] button_text = {
            "Undead Survival", "Multiplayer", "Settings"
        };

        private static string[] button_messages = {
            "Complete waves of the Undead solo or 2-4 player co-op.\nChallenging and strategic game play with highscores awaits you!",
            "Online multiplayer supporting 8 Game Modes. Team up and defeat\nthe appossing team to complete tasks and rank up on the highescores!",
            "Manage your current game settings and defaults with additional settings\nChange settings to enhance your gameplay and menu loading."
        };

        public MainMenuWindow(SpriteBatch d3ddev)
        {
            this.d3ddev = d3ddev;
            background = GameConstants.LOADER.Load<Texture2D>("BACKGROUND");
            SELECT = GameConstants.LOADER.Load<Texture2D>("SELECTED_BUTTON_BKG");
            UNSELECT = GameConstants.LOADER.Load<Texture2D>("UNSELECTED_BUTTON_BKG");
            menuSystem = new MenuSystem(false, 3);
            foreach (SignedInGamer sig in Gamer.SignedInGamers)
            {
                if (sig.PlayerIndex == GameConstants.CONTROLLER_INDEX)
                {
                    gamer = sig;
                }
            }
        }

        public void BeginSession()
        {

        }

        public void Draw()
        {
            if (GameConstants.transition < 200)
                ++GameConstants.transition;
            byte button = menuSystem.SelectedItemIndex();
            bool locked = button == 1 && !GameConstants.ALLOWED_MULTIPLAYER;
            location_start.X = 0;
            location_start.Y = 0;
            d3ddev.Draw(background, location_start, null, Color.Gray * 0.75f, 0, Vector2.Zero, (float)(GameConstants.SAFE_ZONE.Center.Y * 2) / 720.0f, SpriteEffects.None, 0);
            location_start.X = (int)(0.5 * ((GameConstants.SAFE_ZONE.Center.X * 2) - GameConstants.TITLE_FONT.MeasureString("The Survival Game").X));
            location_start.Y = 105;

            d3ddev.DrawString(GameConstants.TITLE_FONT, "The Survival Game", location_start, Color.White * (GameConstants.transition * 0.005f));
            int area = (int)((GameConstants.SAFE_ZONE.Center.X * 2) - (SELECT.Width * 3)); // - (GameConstants.SAFE_ZONE.X * 2);
            location_start.X = area * 0.4f;
            for (int i = 0; i < 3; ++i)
            {
                location_start.X = (area * 0.4f) + (area * 0.1f * i) + (i * SELECT.Width); //(int)(GameConstants.SAFE_ZONE.X + (area * 0.25f * (i + 1)) + (i * SELECT.Width));
                location_start.Y = 384;
                if (i == button)
                {
                    if (locked)
                    {
                        d3ddev.Draw(SELECT, location_start, null, Color.Black * 0.20f);
                        location_start.Y += 312;
                        int oldX = (int)location_start.X;
                        location_start.X += 180 - (GameConstants.LARGE_LABEL_FONT.MeasureString("Mode Locked").X * 0.5f);
                        d3ddev.DrawString(GameConstants.LARGE_LABEL_FONT, "Mode Locked", location_start, Color.Red * 0.65f);
                        location_start.X = oldX;
                    }
                    else
                    {
                        d3ddev.Draw(SELECT, location_start, null, Color.White);
                    }
                }
                else 
                {
                    d3ddev.Draw(UNSELECT, location_start, null, Color.White * 0.50f);
                }
                Vector2 dim = GameConstants.MEDIUM_LABEL_FONT.MeasureString(button_text[i]);
                location_start.X += ((SELECT.Width - dim.X) / 2);
                location_start.Y = location_start.Y + SELECT.Height - dim.Y;
                if (!(i == button && locked))
                    d3ddev.DrawString(GameConstants.MEDIUM_LABEL_FONT, button_text[i], location_start, button == i ? Color.White * 0.85f : Color.LightGray * 0.50f);
            }
            location_start.Y += 112;
            int val_location = (int)(GameConstants.SAFE_ZONE.Center.X + ((button - 1) * area * 0.1f) + ((button - 1) * SELECT.Width));
            if (locked)
            {
                string message1 = "We have detected that you're playing in Trial Mode.";
                string message2 = "This means that the Multiplayer feature is currently locked.";
                if (!Guide.IsTrialMode)
                    message1 = "We have detected that your account is not allowed to play Multiplayer.";
                location_start.X = GameConstants.SAFE_ZONE.Center.X - (GameConstants.SMALL_LABEL_FONT.MeasureString(message1).X * 0.5f);
                d3ddev.DrawString(GameConstants.SMALL_LABEL_FONT, message1, location_start, Color.White);
                location_start.X = GameConstants.SAFE_ZONE.Center.X - (GameConstants.SMALL_LABEL_FONT.MeasureString(message2).X * 0.5f);
                d3ddev.DrawString(GameConstants.SMALL_LABEL_FONT, "\n" + message2, location_start, Color.White);
                location_start.Y = 752;
                location_start.X = val_location - (GameConstants.XBOX_FONT.MeasureString("@ Unlock").X * 0.5f);
                d3ddev.DrawString(GameConstants.XBOX_FONT, "@ Unlock", location_start, Color.White);
            }
            else
            {
                string[] messages = button_messages[button].Split('\n');
                location_start.X = GameConstants.SAFE_ZONE.Center.X - (GameConstants.SMALL_LABEL_FONT.MeasureString(messages[0]).X * 0.5f);
                d3ddev.DrawString(GameConstants.SMALL_LABEL_FONT, messages[0], location_start, Color.White);
                location_start.X = GameConstants.SAFE_ZONE.Center.X - (GameConstants.SMALL_LABEL_FONT.MeasureString(messages[1]).X * 0.5f);
                d3ddev.DrawString(GameConstants.SMALL_LABEL_FONT, "\n" + messages[1], location_start, Color.White);
                location_start.Y = 752;
                location_start.X = val_location - (GameConstants.XBOX_FONT.MeasureString("@ Select").X * 0.5f);
                d3ddev.DrawString(GameConstants.XBOX_FONT, "@ Select", location_start, Color.White);
            }
        }
        public void Update(GameTime gameTime)
        {
            gamer.Presence.PresenceMode = GamerPresenceMode.AtMenu;
            menuSystem.Update(gameTime);
            GamePadState current = GamePad.GetState(GameConstants.CONTROLLER_INDEX);
            GamePadState last = GameConstants.lastGamePadState[(int)GameConstants.CONTROLLER_INDEX];
            bool available = true;
            if (current.Buttons.A == ButtonState.Pressed)
            {
                if (last.Buttons.A == ButtonState.Released)
                {
                    if (!(menuSystem.SelectedItemIndex() == 1 && !GameConstants.ALLOWED_MULTIPLAYER))
                    {
                        GameConstants.GAME_SCREEN_INDEX = (byte)(menuSystem.SelectedItemIndex() + 1);
                        available = false;
                    }
                    else
                    {
                        try {
                            Guide.ShowMarketplace(GameConstants.CONTROLLER_INDEX);
                        } catch (GuideAlreadyVisibleException) { }
                             
                    }
                }
            }
            if (current.Buttons.Start == ButtonState.Pressed)
            {
                if (last.Buttons.Start == ButtonState.Released)
                {
                    if (!(menuSystem.SelectedItemIndex() == 1 && !GameConstants.ALLOWED_MULTIPLAYER))
                    {
                        GameConstants.GAME_SCREEN_INDEX = (byte)(menuSystem.SelectedItemIndex() + 1);
                        available = false;
                    }
                }
            }
            if (available && current.Buttons.B == ButtonState.Pressed)
            {
                if (last.Buttons.B == ButtonState.Released)
                {
                    GameConstants.SPLASH_SCREEN_INDEX = 2;
                    GameConstants.transition = 40;
                    available = false;
                }
            }
            if (available && current.Buttons.Back == ButtonState.Pressed)
            {
                if (last.Buttons.Back == ButtonState.Released)
                {
                    GameConstants.SPLASH_SCREEN_INDEX = 2;
                    GameConstants.transition = 40;
                }
            }
            GameConstants.lastGamePadState[(int)GameConstants.CONTROLLER_INDEX] = current;
        }

    }

}