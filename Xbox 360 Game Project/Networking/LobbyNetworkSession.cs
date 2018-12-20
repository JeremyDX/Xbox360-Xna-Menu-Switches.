using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace Xbox_360_Game_Project
{
    class LobbyNetworkSession
    {
        private ChannelSessionHandler session_handler;
        public bool lobbySizeChanged;

        public LobbyNetworkSession(ChannelSessionHandler session_handler)
        {
            this.session_handler = session_handler;
        }

        public void BeginSession(int maximum)
        {
            session_handler.BeginSession(maximum);
            if (GetSession() != null)
                HookSessionEvents();
            if (GameConstants.WINDOW_OVERLAY_INDEX == 1)
                GameConstants.WINDOW_OVERLAY_INDEX = -1;
        }

        public void CloseSession()
        {
            session_handler.CloseSession();
        }

        public NetworkSession GetSession()
        {   
            return session_handler.GetSession();
        }

        public ChannelSessionHandler Channel()
        {
            return session_handler;
        }

        void HookSessionEvents()
        {
            GetSession().GamerJoined += GamerJoinedEventHandler;
            GetSession().GamerLeft += GamerLeftEventHandler;
            GetSession().GameEnded += GameEndedEventHandler;
        }

        void GamerJoinedEventHandler(object sender, GamerJoinedEventArgs e)
        {
            if (session_handler.GetPlayerHandler().AddPlayer(e.Gamer) == 1)
            {
                lobbySizeChanged = true;
            }
        }

        void GamerLeftEventHandler(object sender, GamerLeftEventArgs e)
        {
            if (e.Gamer.IsLocal)
                CloseSession();
            else
            {
                session_handler.GetPlayerHandler().RemovePlayer(e.Gamer);
                lobbySizeChanged = true;
            }
        }

        void GameEndedEventHandler(object sender, GameEndedEventArgs e)
        {
            CloseSession();
        }

    }
}
