using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace Xbox_360_Game_Project
{
    class SafeZoneWindow : MainInterface
    {
        private SpriteBatch d3ddev;
        private SpriteFont boxFont;
        double x = 0.0;
        double y = 0.0;
        private Rectangle original_position;

        public SafeZoneWindow(SpriteBatch d3ddev)
        {
            this.d3ddev = d3ddev;
            original_position = GameConstants.SAFE_ZONE;
            boxFont = GameConstants.LOADER.Load<SpriteFont>("XBOX_FONTv2");
        }

        public void BeginSession()
        {

        }

        public void Draw()
        {
            int sizeX = (int)boxFont.MeasureString("@").X;
            int sizeY = (int)boxFont.MeasureString("@").Y;
            Vector2 position = new Vector2(GameConstants.SAFE_ZONE.X, GameConstants.SAFE_ZONE.Y);
            d3ddev.DrawString(boxFont, "@", position, Color.White);
            position.Y = GameConstants.SAFE_ZONE.Bottom - sizeY;
            d3ddev.DrawString(boxFont, "@", position, Color.White);
            position.X = GameConstants.SAFE_ZONE.Right - sizeX;
            d3ddev.DrawString(boxFont, "@", position, Color.White);
            position.Y = GameConstants.SAFE_ZONE.Y;
            d3ddev.DrawString(boxFont, "@", position, Color.White);
            position.X = 240;
            position.Y = 240;
            d3ddev.DrawString(boxFont, "BaseX: " + GameConstants.SAFE_ZONE.X + " , BaseY: " + GameConstants.SAFE_ZONE.Y, position, Color.White);
            position.Y += 50;
            d3ddev.DrawString(boxFont, "Width: " + GameConstants.SAFE_ZONE.Width + " , Height: " + GameConstants.SAFE_ZONE.Height, position, Color.White);
            position.Y += 50;
            d3ddev.DrawString(boxFont, "FullX: " + (GameConstants.SAFE_ZONE.Right + GameConstants.SAFE_ZONE.X) + " , FullY: " + (GameConstants.SAFE_ZONE.Bottom + GameConstants.SAFE_ZONE.Y), position, Color.White);
            int displayX = (int)Math.Ceiling(1.25 * GameConstants.d3dpp.GraphicsDevice.Viewport.TitleSafeArea.Width);
            int displayY = (int)Math.Ceiling(1.25 * GameConstants.d3dpp.GraphicsDevice.Viewport.TitleSafeArea.Height);
            position.Y += 50;
            d3ddev.DrawString(boxFont, "DispX: " + displayX + " , DispY: " + displayY, position, Color.White);
            position.Y += 50;
            d3ddev.DrawString(boxFont, "GD ASP: " + GameConstants.d3dpp.GraphicsDevice.DisplayMode.AspectRatio + " , Wide?: " + GameConstants.d3dpp.GraphicsDevice.Adapter.IsWideScreen, position, Color.White);
            position.Y += 50;
            d3ddev.DrawString(boxFont, "Title Safe: " + GameConstants.d3dpp.GraphicsDevice.Viewport.TitleSafeArea, position, Color.White);
            position.Y += 50;
            d3ddev.DrawString(boxFont, "VP ASP: " + GameConstants.d3dpp.GraphicsDevice.Viewport.AspectRatio + " , SC ASP: " + (GameConstants.SAFE_ZONE.Width / (float)GameConstants.SAFE_ZONE.Height), position, Color.White);
            position.Y = GameConstants.SAFE_ZONE.Bottom - 174;
            position.X = GameConstants.SAFE_ZONE.X + 64;
            d3ddev.DrawString(GameConstants.XBOX_FONT, "^ Reset", position, Color.White);
            position.Y += 42;
            d3ddev.DrawString(GameConstants.XBOX_FONT, "; Back", position, Color.White);
            position.Y += 42;
            d3ddev.DrawString(GameConstants.XBOX_FONT, "(L) or (R) Move", position, Color.White);
        }

        public void Update(GameTime gameTime)
        {
            GamePadState current = GamePad.GetState(GameConstants.CONTROLLER_INDEX);
            GamePadState last = GameConstants.lastGamePadState[(int)GameConstants.CONTROLLER_INDEX];
            if (current.ThumbSticks.Left.Y <= -0.20f || current.ThumbSticks.Left.Y >= 0.20f)
            {
                y += (gameTime.ElapsedGameTime.Milliseconds * 0.01 * current.ThumbSticks.Left.Y);
                GameConstants.SAFE_ZONE.Y = (int)(original_position.Y - y);
                GameConstants.SAFE_ZONE.Height = (original_position.Bottom + original_position.Y) - GameConstants.SAFE_ZONE.Y * 2;
            }
            else if (current.ThumbSticks.Left.X <= -0.20f || current.ThumbSticks.Left.X >= 0.20f)
            {
                x += (gameTime.ElapsedGameTime.Milliseconds * 0.01 * current.ThumbSticks.Left.X);
                GameConstants.SAFE_ZONE.X = (int)(original_position.X + x);
                GameConstants.SAFE_ZONE.Width = (original_position.Right + original_position.X) - GameConstants.SAFE_ZONE.X * 2;
            }
            if (current.ThumbSticks.Right.Y <= -0.20f || current.ThumbSticks.Right.Y >= 0.20f)
            {
                y += (gameTime.ElapsedGameTime.Milliseconds * 0.01 * current.ThumbSticks.Right.Y);
                GameConstants.SAFE_ZONE.Y = (int)(original_position.Y - y);
                GameConstants.SAFE_ZONE.Height = (original_position.Bottom + original_position.Y) - GameConstants.SAFE_ZONE.Y * 2;
            }
            else if (current.ThumbSticks.Right.X <= -0.20f || current.ThumbSticks.Right.X >= 0.20f)
            {
                x += (gameTime.ElapsedGameTime.Milliseconds * 0.01 * current.ThumbSticks.Right.X);
                GameConstants.SAFE_ZONE.X = (int)(original_position.X + x);
                GameConstants.SAFE_ZONE.Width = (original_position.Right + original_position.X) - GameConstants.SAFE_ZONE.X * 2;
            }
            if (current.Buttons.Y == ButtonState.Pressed)
            {
                if (last.Buttons.Y == ButtonState.Released)
                {
                    GameConstants.SAFE_ZONE = original_position;
                    x = 0;
                    y = 0;
                }
            } 
            else if (current.Buttons.B == ButtonState.Pressed)
            {
                if (last.Buttons.B == ButtonState.Released)
                {
                    GameConstants.GAME_SCREEN_INDEX = 3;
                }
            }
            else if (current.Buttons.Back == ButtonState.Pressed)
            {
                if (last.Buttons.Back == ButtonState.Released)
                {
                    GameConstants.GAME_SCREEN_INDEX = 3;
                }
            }
            GameConstants.lastGamePadState[(int)GameConstants.CONTROLLER_INDEX] = current;
        }

    }
}
