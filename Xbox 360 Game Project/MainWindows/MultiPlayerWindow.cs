using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.GamerServices;

namespace Xbox_360_Game_Project
{
    class MultiPlayerWindow : MainInterface
    {
        private SpriteBatch d3ddev;
        private Texture2D background;
        private LobbyNetworkSession lobby_network;
        private SideBySideMenuSystem menuSystem;
        private Vector2 location_start;
        private byte lobbyMembers = 8;
        private const byte MAX_LOBBY = 8;
        private byte session_stage = 0;
        private Matrix World;
        private AvatarAnimation Animation;
        private AvatarRenderer Renderer;

        private string[] menu_string = {"Public Match", "Private Match", "Invite Players", "Lobby Statistics", "Options", "Main Menu"};

        public string[] message_string = {
            "Search for an online match\nWith other players.",
            "Enter a private lobby and\nCustomize the game modes!",
            "Invite players to your\nGame session for more fun!",
            "View statistics of current\nLobby members.",
            "View Game options to optimize\nYour in-game experience.",
            "Return to the Main Menu."
        };

        public MultiPlayerWindow(SpriteBatch d3ddev, LobbyNetworkSession lobby_network)
        {
            this.d3ddev = d3ddev;
            background = GameConstants.LOADER.Load<Texture2D>("BACKGROUND");
            menuSystem = new SideBySideMenuSystem(true, (byte)menu_string.Length, lobbyMembers, 0 | (2 << 8));
            menuSystem.reset(0, 2);
            this.lobby_network = lobby_network;
            Animation = new AvatarAnimation((AvatarAnimationPreset)4);
            Renderer = new AvatarRenderer(AvatarDescription.CreateRandom(), false);
            World = Matrix.CreateRotationY(2.0f * MathHelper.TwoPi) * Matrix.CreateTranslation(new Vector3(0, 0, 0));
        }

        public void BeginSession()
        {
            GameConstants.WINDOW_OVERLAY_INDEX = 1;
            session_stage = 1;
        }

        public void Draw() {
            byte index = menuSystem.PrimaryIndex();
            location_start.X = 0;
            location_start.Y = 0;
            d3ddev.Draw(background, location_start, null, Color.Gray * 0.75f, 0, Vector2.Zero, (float)(GameConstants.SAFE_ZONE.Center.Y * 2) / 720.0f, SpriteEffects.None, 0);
            int width = (int)GameConstants.XBOX_FONT.MeasureString(menu_string[3]).X;
            Point center = GameConstants.SAFE_ZONE.Center;
            int text_middle = GameConstants.SAFE_ZONE.X + (int)((center.X - GameConstants.SAFE_ZONE.X) * 0.5) - 48;
            location_start.X = center.X - 48;
            d3ddev.Draw(GameConstants.LOBBY_BKG, location_start, new Rectangle(0, 40, 1280, 4), Color.White, MathHelper.ToRadians(90), Vector2.Zero, 1.0f, SpriteEffects.None, 0);
            if (menuSystem.PrimaryLocationSelected())
            {
                location_start.Y = GameConstants.SAFE_ZONE.Y + 206 + (52 * index);
                location_start.X = text_middle - 48 - (int)(width * 0.5);
                d3ddev.Draw(GameConstants.EMPTY_SQUARE, location_start, new Rectangle(0, 0, width + 148, 52), Color.Black);
                d3ddev.Draw(GameConstants.EMPTY_SQUARE, location_start, new Rectangle(0, 0, width + 96, 52), Color.Red * 0.20f);
                location_start.X += width + 96;
                d3ddev.Draw(GameConstants.EMPTY_SQUARE, location_start, new Rectangle(0, 0, 52, 52), Color.Gray * 0.10f);
                location_start.X += 4;
                location_start.Y += 8;
                d3ddev.DrawString(GameConstants.XBOX_FONT, "@", location_start, Color.White);
                location_start.X = text_middle - 48 - (int)(width * 0.5);
                location_start.Y -= 8;
                d3ddev.Draw(GameConstants.MESSAGE_BOX, location_start, new Rectangle(0, 0, 52, 4), Color.DarkGray, MathHelper.ToRadians(90), Vector2.Zero, 1, SpriteEffects.None, 0);
                location_start.X += width + 92;
                d3ddev.Draw(GameConstants.MESSAGE_BOX, location_start, new Rectangle(0, 0, 52, 4), Color.DarkGray, MathHelper.ToRadians(90), Vector2.Zero, 1, SpriteEffects.FlipVertically, 0);
                location_start.X += 4;
                d3ddev.Draw(GameConstants.MESSAGE_BOX, location_start, new Rectangle(0, 0, 52, 4), Color.DarkGray, MathHelper.ToRadians(90), Vector2.Zero, 1, SpriteEffects.None, 0);
                location_start.X += 52;
                d3ddev.Draw(GameConstants.MESSAGE_BOX, location_start, new Rectangle(0, 0, 52, 4), Color.DarkGray, MathHelper.ToRadians(90), Vector2.Zero, 1, SpriteEffects.FlipVertically, 0);
                location_start.X = text_middle - 52 - (int)(width * 0.5);
                d3ddev.Draw(GameConstants.MESSAGE_BOX, location_start, new Rectangle(0, 0, width + 152, 4), Color.White, 0, Vector2.Zero, 1, SpriteEffects.FlipVertically, 0);
                location_start.Y += 48;
                d3ddev.Draw(GameConstants.MESSAGE_BOX, location_start, new Rectangle(0, 0, width + 152, 4), Color.White);
            }
            NetworkSession session = lobby_network.Channel().GetSession();
            location_start.Y = GameConstants.SAFE_ZONE.Y + 216;
            for (int i = 0; i < menu_string.Length; ++i)
            {
                bool blocked = false;
                if (session != null)
                {
                    if (i <= 2 && !session.IsHost)
                        blocked = true;
                    else if (i == 2 && lobby_network.Channel().GetPlayerHandler().Size() > 7)
                        blocked = true;
                }
                else if (i <= 2)
                {
                    blocked = true;
                }
                int oldY = (int)location_start.Y;
                int len = (int)GameConstants.XBOX_FONT.MeasureString(menu_string[i]).X;
                location_start.X = text_middle - (int)(len * 0.5);
                if (i == index && menuSystem.PrimaryLocationSelected())
                {
                    d3ddev.DrawString(GameConstants.XBOX_FONT, menu_string[i], location_start, blocked ? Color.Gray * 0.5f : Color.White * 0.95f);
                    location_start.Y = (center.Y * 2) - GameConstants.SAFE_ZONE.Y - 104;
                    if (blocked)
                    {
                        location_start.X = (center.X * 0.5f) - (GameConstants.SMALL_LABEL_FONT.MeasureString("This Feature is currently").X * 0.5f);
                        d3ddev.DrawString(GameConstants.SMALL_LABEL_FONT, "This Feature is currently", location_start, Color.DarkRed);
                        location_start.X = (center.X * 0.5f) - (GameConstants.SMALL_LABEL_FONT.MeasureString((blocked ? "DISABLED." : "LOCKED.")).X * 0.5f);
                        d3ddev.DrawString(GameConstants.SMALL_LABEL_FONT, (blocked ? "\nDISABLED." : "\nLOCKED."), location_start, Color.DarkRed);
                    }
                    else
                    {
                        string[] split = message_string[i].Split('\n');
                        for (int n = 1; n < split.Length; ++n)
                            split[n] = "\n" + split[n];
                        foreach (string s in split)
                        {
                            location_start.X = (center.X * 0.5f) - (GameConstants.SMALL_LABEL_FONT.MeasureString(s).X * 0.5f);
                            d3ddev.DrawString(GameConstants.SMALL_LABEL_FONT, s, location_start, Color.DarkRed);
                        }
                    }
                }
                else
                {
                    d3ddev.DrawString(GameConstants.XBOX_FONT, menu_string[i], location_start, blocked ? Color.Gray * 0.5f : Color.Gray);
                }
                location_start.Y = oldY + 52;
            }
            if (session_stage == 3) 
            {
                location_start.X = (center.X * 1.5f) - 240;
                string selected_username = null;
                PlayerHandler handler = lobby_network.Channel().GetPlayerHandler();
                for (int line = 0; line < handler.Size(); ++line)
                {
                    Player player = handler.PlayerList(line);
                    if (player == null)
                        break;
                    location_start.Y = GameConstants.SAFE_ZONE.Y + 116 + (line * 50);
                    if (line != menuSystem.SecondaryIndex() || menuSystem.PrimaryLocationSelected())
                    {
                        d3ddev.Draw(GameConstants.LOBBY_BKG, location_start, new Rectangle(0, 0, 476, 44), Color.DarkGray * 0.25f);
                        location_start.X += 476;
                        d3ddev.Draw(GameConstants.LOBBY_BKG, location_start, new Rectangle(1276, 0, 4, 44), Color.DarkGray * 0.25f);
                        location_start.X -= 464;
                        location_start.Y += 4;
                        d3ddev.DrawString(GameConstants.XBOX_FONT, player.SecondaryUsername(), location_start, player.Channel().IsLocal ? Color.MediumSlateBlue * 0.85f : Color.Orange * 0.85f);
                        location_start.X -= 12;
                        location_start.Y -= 4;
                    }
                    else
                    {
                        selected_username = player.SecondaryUsername();
                    }
                    if (GameConstants.ALLOWED_MULTIPLAYER && player.Channel().IsMutedByLocalUser)
                    {
                        //Muted.
                        location_start.X -= 36;
                        location_start.Y += 11;
                        d3ddev.Draw(GameConstants.GAME_ICONS, location_start, new Rectangle(0, 0, 32, 26), Color.Red * 0.50f);
                        d3ddev.Draw(GameConstants.GAME_ICONS, location_start, new Rectangle(0, 0, 14, 26), Color.LightSalmon);
                        location_start.X += 36;
                        location_start.Y -= 11;
                    }
                    else if (player.HAS_XBOX_PARTY)
                    {
                        location_start.X -= 36;
                        location_start.Y += 11;
                        d3ddev.Draw(GameConstants.GAME_ICONS, location_start, new Rectangle(0, 0, 32, 26), Color.DarkRed);
                        d3ddev.Draw(GameConstants.GAME_ICONS, location_start, new Rectangle(0, 0, 32, 26), Color.White * 0.25f);
                        location_start.X += 15;
                        d3ddev.Draw(GameConstants.GAME_ICONS, location_start, new Rectangle(15, 0, 18, 26), Color.White * 0.50f);
                        location_start.X += 21;
                        location_start.Y -= 11;
                    }
                    else if (player.Channel().IsTalking)
                    {
                        //Talking.
                        location_start.X -= 36;
                        location_start.Y += 11;
                        d3ddev.Draw(GameConstants.GAME_ICONS, location_start, new Rectangle(33, 0, 29, 26), Color.LightYellow);
                        location_start.X += 36;
                        location_start.Y -= 11;
                    }
                    else if (player.Channel().HasVoice)
                    {
                        //Idle.
                        location_start.X -= 36;
                        location_start.Y += 11;
                        d3ddev.Draw(GameConstants.GAME_ICONS, location_start, new Rectangle(33, 0, 14, 26), Color.LightGray);
                        location_start.X += 36;
                        location_start.Y -= 11;
                    }
                }
                if (selected_username != null)
                {
                    location_start.Y = GameConstants.SAFE_ZONE.Y + 94 + (menuSystem.SecondaryIndex() * 50);
                    d3ddev.Draw(GameConstants.LOBBY_BKG, location_start, new Rectangle(0, 0, 120, 44), Color.Gray, 0, Vector2.Zero, 2.00f, SpriteEffects.None, 0);
                    location_start.X += 240;
                    d3ddev.Draw(GameConstants.LOBBY_BKG, location_start, new Rectangle(0, 0, 120, 44), Color.Gray, 0, Vector2.Zero, 2.00f, SpriteEffects.FlipHorizontally, 0);
                    location_start.X -= 228;
                    location_start.Y += 12;
                    Player player = handler.PlayerList(menuSystem.SecondaryIndex());
                    d3ddev.Draw(player.GetGamerPicture(), location_start, Color.White);
                    location_start.Y += 14;
                    location_start.X += 72;
                    int len = (int)((88 - GameConstants.MEDIUM_LABEL_FONT.MeasureString(selected_username).Y) * 0.5);
                    location_start.Y += len - 24;
                    location_start.X += 2;
                    d3ddev.DrawString(GameConstants.MEDIUM_LABEL_FONT, selected_username, location_start, Color.Black);
                    location_start.Y -= 2;
                    location_start.X -= 2;
                    d3ddev.DrawString(GameConstants.MEDIUM_LABEL_FONT, selected_username, location_start, Color.BlanchedAlmond * 0.95f);
                }
            }
            else
            {
                if (++session_stage == 3)
                {
                    lobby_network.Channel().NetworkStatusStage = 0;
                    lobby_network.BeginSession(8);
                    lobby_network.lobbySizeChanged = true;
                }
            }
            location_start.Y = GameConstants.SAFE_ZONE.Y + 48;
            width = (int)GameConstants.LARGE_LABEL_FONT.MeasureString("Multiplayer").X;
            location_start.X = (text_middle + 18 - (int)(width * 0.75));
            d3ddev.DrawString(GameConstants.LARGE_LABEL_FONT, "Multiplayer", location_start, Color.Red, 0.0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0);
            location_start.Y = (center.Y * 2) - GameConstants.SAFE_ZONE.Y - 64;
            location_start.X = (center.X * 1.5f) - GameConstants.SAFE_ZONE.X - (GameConstants.XBOX_FONT.MeasureString("@ select   ^ Friends   ; Back").X * 0.5f);
            d3ddev.DrawString(GameConstants.XBOX_FONT, "@ select   ^ Friends   ; Back", location_start, Color.White);
            if (lobby_network.Channel().NetworkStatusStage == 2)
            {
                lobby_network.Channel().NetworkStatusStage = 3;
                session_stage = 1;
            }
        }

        private void DrawAvatar()
        {
            Vector3 cameraPos = new Vector3((float)Math.Sin(3.0f), .5f, (float)Math.Cos(3.0f)) * 8f;
            Matrix view = Matrix.CreateLookAt(cameraPos, new Vector3(0f, 1f, 0f), Vector3.Up);
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, d3ddev.GraphicsDevice.Viewport.AspectRatio, 1f, 1000f);
            Renderer.World = World;
            Renderer.View = view;
            Renderer.Projection = projection;
            Renderer.Draw(Animation);
        }

        public void resizeList()
        {
            int size = lobby_network.Channel().GetPlayerHandler().Size();
            menuSystem.resizeList(false, size);
        }

        public void Update(GameTime gameTime)
        {
            if (lobby_network.lobbySizeChanged)
            {
                resizeList();
                lobby_network.lobbySizeChanged = false;
            }
            menuSystem.Update(gameTime);
            GamePadState current = GamePad.GetState(GameConstants.CONTROLLER_INDEX);
            GamePadState last = GameConstants.lastGamePadState[(int)GameConstants.CONTROLLER_INDEX];
            bool available = true;
            if (menuSystem.PrimaryLocationSelected())
            {
                if (current.Buttons.A == ButtonState.Pressed)
                {
                    if (last.Buttons.A == ButtonState.Released)
                    {
                        switch (menuSystem.PrimaryIndex())
                        {
                            case 2:
                                Guide.ShowGameInvite(0, null);
                                break;
                            case 5:
                                GameConstants.GAME_SCREEN_INDEX = 0;
                                menuSystem.reset(0, 2);
                                lobby_network.CloseSession();
                                GameConstants.transition = 40;
                                break;
                        }
                        available = false;
                    }
                }
            }
            else
            {
                if (current.Buttons.A == ButtonState.Pressed)
                {
                    if (last.Buttons.A == ButtonState.Released)
                    {
                        GameConstants.SelectionID = menuSystem.SecondaryIndex();
                        GameConstants.WINDOW_OVERLAY_INDEX = 4;
                    }
                }
            }
            if (available && current.Buttons.B == ButtonState.Pressed)
            {
                if (last.Buttons.B == ButtonState.Released)
                {
                    GameConstants.GAME_SCREEN_INDEX = 0;
                    menuSystem.reset(0, 2);
                    lobby_network.CloseSession();
                    GameConstants.transition = 40;
                    available = false;
                }
            }
            if (available && current.Buttons.Back == ButtonState.Pressed)
            {
                if (last.Buttons.Back == ButtonState.Released)
                {
                    GameConstants.GAME_SCREEN_INDEX = 0;
                    menuSystem.reset(0, 2);
                    lobby_network.CloseSession();
                    GameConstants.transition = 40;
                    available = false;
                }
            } 
            if (available && current.Buttons.Y == ButtonState.Pressed)
            {
                if (last.Buttons.Y == ButtonState.Released)
                {
                    GameConstants.WINDOW_OVERLAY_INDEX = 2;
                    available = false;
                }
            }
            Animation.Update(gameTime.ElapsedGameTime, false);
            GameConstants.lastGamePadState[(int)GameConstants.CONTROLLER_INDEX] = current;
        }

    }
}
