using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Xbox_360_Game_Project
{
    class SettingsWindow : MainInterface
    {
        private SpriteBatch d3ddev;
        private GameSaveSystem save_device;
        private Texture2D background;
        private MenuSystem menuSystem;
        private Vector2 location_start;

        private static string[] menu_string = { "Load Profile", "Create New Profile", "Profile Settings", "Change Save Device", "System Information", "Change Safe Zone", "Main Menu" };

        public SettingsWindow(SpriteBatch d3ddev, GameSaveSystem save_device)
        {
            this.d3ddev = d3ddev;
            this.save_device = save_device;
            background = GameConstants.LOADER.Load<Texture2D>("BACKGROUND");
            menuSystem = new MenuSystem(true, (byte)menu_string.Length);
        }

        public void BeginSession()
        {

        }

        public void Draw()
        {
            byte index = menuSystem.SelectedItemIndex();
            location_start.X = 0;
            location_start.Y = 0;
            d3ddev.Draw(background, location_start, null, Color.Gray * 0.75f, 0, Vector2.Zero, (float)(GameConstants.SAFE_ZONE.Center.Y * 2) / 720.0f, SpriteEffects.None, 0);
            int centerX = (int)(0.5 * (GameConstants.SAFE_ZONE.Width + (GameConstants.SAFE_ZONE.X * 2)));
            int width = (int)GameConstants.XBOX_FONT.MeasureString(menu_string[3]).X;
            location_start.Y = 264 + (52 * index);
            location_start.X = (int)(centerX - 48 - (width * 0.50));
            d3ddev.Draw(GameConstants.EMPTY_SQUARE, location_start, new Rectangle(0, 0, width + 148, 52), Color.Black);
            d3ddev.Draw(GameConstants.EMPTY_SQUARE, location_start, new Rectangle(0, 0, width + 96, 52), Color.Red * 0.20f);
            location_start.X += width + 96;
            d3ddev.Draw(GameConstants.EMPTY_SQUARE, location_start, new Rectangle(0, 0, 52, 52), Color.Gray * 0.10f);
            location_start.X += 6;
            location_start.Y += 8;
            d3ddev.DrawString(GameConstants.XBOX_FONT, "@", location_start, Color.White);
            location_start.X -= (width + 102);
            location_start.Y -= 8;
            d3ddev.Draw(GameConstants.MESSAGE_BOX, location_start, new Rectangle(0, 0, width + 148, 4), Color.White);
            location_start.Y += 48;
            d3ddev.Draw(GameConstants.MESSAGE_BOX, location_start, new Rectangle(0, 0, width + 148, 4), Color.White);
            location_start.Y -= 44;
            location_start.X += 4;
            d3ddev.Draw(GameConstants.MESSAGE_BOX, location_start, new Rectangle(0, 0, 44, 4), Color.DarkGray, MathHelper.ToRadians(90), Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 0);
            location_start.X += width + 92;
            d3ddev.Draw(GameConstants.MESSAGE_BOX, location_start, new Rectangle(0, 0, 44, 4), Color.DarkGray, MathHelper.ToRadians(90), Vector2.Zero, 1, SpriteEffects.None, 0);
            location_start.X += 4;
            d3ddev.Draw(GameConstants.MESSAGE_BOX, location_start, new Rectangle(0, 0, 44, 4), Color.DarkGray, MathHelper.ToRadians(90), Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 0);
            location_start.X += 48;
            d3ddev.Draw(GameConstants.MESSAGE_BOX, location_start, new Rectangle(0, 0, 44, 4), Color.DarkGray, MathHelper.ToRadians(90), Vector2.Zero, 1, SpriteEffects.None, 0);
            location_start.Y = 270;
            for (int i = 0; i < menu_string.Length; ++i)
            {
                int posX = (int)GameConstants.XBOX_FONT.MeasureString(menu_string[i]).X;
                location_start.X = (int)(centerX - (posX * 0.50));
                d3ddev.DrawString(GameConstants.XBOX_FONT, menu_string[i], location_start, i == index ? Color.White * 0.95f : Color.Gray);
                location_start.Y += 52;
            }
            location_start.Y = GameConstants.SAFE_ZONE.Y + 48;
            width = (int)GameConstants.LARGE_LABEL_FONT.MeasureString("Game Settings").X;
            location_start.X = (int)(centerX - (width * 0.75));
            d3ddev.DrawString(GameConstants.LARGE_LABEL_FONT, "Game Settings", location_start, Color.Red, 0.0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0);
        }

        public void Update(GameTime gameTime)
        {
            menuSystem.Update(gameTime);
            GamePadState current = GamePad.GetState(GameConstants.CONTROLLER_INDEX);
            GamePadState last = GameConstants.lastGamePadState[(int)GameConstants.CONTROLLER_INDEX];
            bool available = true;
            if (current.Buttons.A == ButtonState.Pressed)
            {
                if (last.Buttons.A == ButtonState.Released)
                {
                    switch (menuSystem.SelectedItemIndex())
                    {
                        case 3:
                            save_device.ChooseDevice();
                            break;
                        case 5:
                            GameConstants.GAME_SCREEN_INDEX = 4;
                            break;
                        case 6:
                            GameConstants.GAME_SCREEN_INDEX = 0;
                            GameConstants.transition = 40;
                            break;
                    }
                    available = false;
                }
            }
            if (available && current.Buttons.B == ButtonState.Pressed)
            {
                if (last.Buttons.B == ButtonState.Released)
                {
                    GameConstants.GAME_SCREEN_INDEX = 0;
                    GameConstants.transition = 40;
                    available = false;
                }
            }
            if (available && current.Buttons.Back == ButtonState.Pressed)
            {
                if (last.Buttons.Back == ButtonState.Released)
                {
                    GameConstants.GAME_SCREEN_INDEX = 0;
                    GameConstants.transition = 40;
                    available = false;
                }
            }
            GameConstants.lastGamePadState[(int)GameConstants.CONTROLLER_INDEX] = current;
        }

    }
}
