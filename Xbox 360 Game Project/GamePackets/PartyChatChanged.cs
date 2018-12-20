using Microsoft.Xna.Framework.Net;

namespace Xbox_360_Game_Project
{
    class PartyChatChanged : GamePacket
    {
        public void HandlePacket(PlayerHandler pList, NetworkGamer gamer, Packet packet)
        {
            Player player = pList.GetPlayerByCheckingGamer(gamer);
            bool hasParty = packet.ReadBoolean();
            if (player != null)
            {
                player.HAS_XBOX_PARTY = hasParty;
            }
        }
    }
}
