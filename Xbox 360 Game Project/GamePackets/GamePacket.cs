using Microsoft.Xna.Framework.Net;

namespace Xbox_360_Game_Project
{
    interface GamePacket
    {
        void HandlePacket(PlayerHandler pList, NetworkGamer gamer, Packet packet);
    }
}
