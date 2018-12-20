using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Xbox_360_Game_Project
{
    class GameConstants
    {
        public static GraphicsDeviceManager d3dpp;
        public static ContentManager LOADER;

        public static SpriteFont SMALL_LABEL_FONT;
        public static SpriteFont MEDIUM_LABEL_FONT;
        public static SpriteFont LARGE_LABEL_FONT;
        public static SpriteFont TITLE_FONT;

        /*
            & = A BUTTON
            ; = B BUTTON
            * = X BUTTON
            ^ = Y BUTTON
            > = START BUTTON
            < = BACK BUTTON
        */
        public static SpriteFont XBOX_FONT;
        public static Texture2D LOBBY_BKG;
        public static Texture2D MESSAGE_BOX;
        public static Texture2D EMPTY_SQUARE;

        //Rectangle(0, 0, 32, 26) == Muted.
        //Rectangle(33, 0, 29, 26) == Talking.
        //Rectangle(33, 1, 14, 24) == HasVoice.
        //Rectangle(62, 0, 29, 30) == HasXboxParty.
        //Rectangle(0, 33, 52, 48) == Locked Item.
        public static Texture2D GAME_ICONS;

        public static PlayerIndex CONTROLLER_INDEX;
        public static bool ALLOWED_MULTIPLAYER = true;
        public static byte GAME_SCREEN_INDEX = 0;
        public static sbyte SPLASH_SCREEN_INDEX = 2;
        public static sbyte WINDOW_OVERLAY_INDEX = -1;
        public static byte transition = 40;
        public static byte SelectionID = 0;
        public static Rectangle SAFE_ZONE = new Rectangle(0, 0, 0, 0);

        public static GamePadState[] lastGamePadState = new GamePadState[4];
    }

}