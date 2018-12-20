using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;

namespace Xbox_360_Game_Project
{
    class UndeadSoloLobby : MainInterface 
    {
        private SpriteBatch d3ddev;

        public UndeadSoloLobby(SpriteBatch d3ddev)
        {
            this.d3ddev = d3ddev;
        }

        public void BeginSession()
        {

        }

        public void Draw()
        {

        }

        public void Update(GameTime gameTime)
        {
            //menuSystem.Update(gameTime);
            GamePadState current = GamePad.GetState(GameConstants.CONTROLLER_INDEX);
            GamePadState last = GameConstants.lastGamePadState[(int)GameConstants.CONTROLLER_INDEX];
            bool available = true;
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
