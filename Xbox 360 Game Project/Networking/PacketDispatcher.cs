namespace Xbox_360_Game_Project
{
    class PacketDispatcher
    {
        private PacketBuilder builder;

        public PacketDispatcher(PacketBuilder builder)
        {
            this.builder = builder;
        }

        //Send this when local user has Entered or Left a Party Chat Session.
        public void SendPacket_PartyChat(bool enter)
        {
            builder.CreatePacket(1).AddBoolean(enter);
        }
    }
}
