using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;

namespace Xbox_360_Game_Project
{
    class Player
    {
        public bool HAS_XBOX_PARTY;
        private NetworkGamer session;
        private string USERNAME_AS_STRING;
        private Texture2D picture;

        public Player(NetworkGamer gamer)
        {
            session = gamer;
            USERNAME_AS_STRING = session.Gamertag;
            picture = Texture2D.FromStream(GameConstants.d3dpp.GraphicsDevice, gamer.GetProfile().GetGamerPicture());
        }

        public NetworkGamer Channel() {
            return session;
        }

        public string SecondaryUsername()
        {
            return USERNAME_AS_STRING;
        }

        public Texture2D GetGamerPicture()
        {
            return picture;
        }

        public void SwapPartyChat()
        {
            HAS_XBOX_PARTY = !HAS_XBOX_PARTY;
        }
    }
}
