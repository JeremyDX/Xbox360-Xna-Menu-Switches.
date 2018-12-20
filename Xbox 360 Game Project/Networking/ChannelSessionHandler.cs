using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.GamerServices;

namespace Xbox_360_Game_Project
{
    class ChannelSessionHandler
    {
        private NetworkSession session;
        private LocalNetworkGamer local_gamer;
        private PacketDispatcher dispatcher;
        private PacketReader reader;
        private PacketBuilder builder;
        private PlayerHandler pHandler;
        public byte NetworkStatusStage = 0;

        private byte[] C_PACKET_LENGTHS = {
            1,
        };

        private byte[] S_PACKET_LENGTHS = {

        };

        private GamePacket[] clientPackets = {
                new PartyChatChanged()
        };


        private GamePacket[] serverPackets = {
                
        };

        NetworkSessionProperties sessionProperties = new NetworkSessionProperties();

        public ChannelSessionHandler()
        {
            builder = new PacketBuilder();
            reader = new PacketReader();
            dispatcher = new PacketDispatcher(builder);
            pHandler = new PlayerHandler();
            NetworkSession.InviteAccepted += InviteAcceptedEventHandler;
        }

        public void Update(GameTime gameTime)
        {
            UpdateMyPlayer();
            if (session != null && !session.IsDisposed)
            {
                if (session.IsHost)
                {
                    DispatchServerPackets();
                }
                else
                {
                    DispatchClientPackets();
                }
                session.Update();
            }
            if (session != null && !session.IsDisposed) 
            {
                if (session.IsHost)
                {
                    ReceiveClientPackets();
                }
                else
                {
                    ReceiveServerPackets();
                }
            }
        }

        public void UpdateMyPlayer()
        {
            if (pHandler.Size() == 0)
                return;
            Player player = pHandler.GetPlayerByCheckingGamer(local_gamer);
            if (player != null)
            {
                if (player.HAS_XBOX_PARTY == (local_gamer.SignedInGamer.PartySize == 0))
                {
                    player.SwapPartyChat();
                    dispatcher.SendPacket_PartyChat(player.HAS_XBOX_PARTY);
                }
            }
        }

        public void SimulatePlayersJoining(int accounts)
        {
            pHandler.Simulation = true;
            for (int i = 0; i < accounts; ++i)
            {
                pHandler.AddPlayer(local_gamer);
            }
            
            pHandler.Simulation = false;
        }

        public void BeginSession(int size)
        {
            SignedInGamer signedInGamer = GetSignedInGamer();
            if ((signedInGamer == null || NetworkStatusStage != 0) && GameConstants.ALLOWED_MULTIPLAYER)
                return;
            CloseSession();
            try
            {
                if (GameConstants.ALLOWED_MULTIPLAYER)
                {
                    sessionProperties = new NetworkSessionProperties();
                    GamerSessionProperties(signedInGamer.Gamertag);
                    pHandler.MaxSize((byte)size);
                    session = NetworkSession.Create(NetworkSessionType.PlayerMatch, new[] { signedInGamer }, size, size - 1, sessionProperties);
                    session.AllowJoinInProgress = true;
                    session.AllowHostMigration = true;
                }
                else
                {
                    pHandler.MaxSize((byte)1);
                    session = NetworkSession.Create(NetworkSessionType.SystemLink, 1, 2);
                }
                foreach (LocalNetworkGamer lng in session.LocalGamers)
                {
                    if (lng.IsLocal && lng.SignedInGamer.PlayerIndex == GameConstants.CONTROLLER_INDEX)
                    {
                        local_gamer = lng;
                        lng.SignedInGamer.Presence.PresenceMode = GamerPresenceMode.WaitingInLobby;
                        break;
                    }
                }
            }
            catch (InvalidOperationException)
            {

            }
        }

        private void BeginJoinSession()
        {
            CloseSession();
            foreach (LocalNetworkGamer lng in session.LocalGamers)
            {
                if (lng.IsLocal && lng.SignedInGamer.PlayerIndex == GameConstants.CONTROLLER_INDEX) 
                {
                    local_gamer = lng;
                    break;
                }
            }
            session = NetworkSession.JoinInvited(1);
        }

        public void JoinPlayerSession(FriendGamer gamer)
        {
            if (NetworkStatusStage != 0 || !GameConstants.ALLOWED_MULTIPLAYER)
                return;
            NetworkStatusStage = 1;
            SignedInGamer signedInGamer = GetSignedInGamer();
            if (signedInGamer == null)
                return;
            sessionProperties = new NetworkSessionProperties();
            GamerSessionProperties(gamer.Gamertag);
            CloseSession();
            NetworkSession.BeginFind(NetworkSessionType.PlayerMatch, new[] { signedInGamer }, sessionProperties, JoinPrompt, null);
        }

        private void JoinPrompt(IAsyncResult result)
        {
            result.AsyncWaitHandle.WaitOne();
            CloseSession();
            try
            {
                AvailableNetworkSessionCollection collection = NetworkSession.EndFind(result);
                result.AsyncWaitHandle.Close();
                if (collection.Count == 0)
                {
                    NetworkStatusStage = 2;
                }
            }
            catch (InvalidOperationException)
            {
                NetworkStatusStage = 2;
                //Just in case something goes wrong...
            }
        }

        public SignedInGamer GetSignedInGamer()
        {
            if (local_gamer == null)
            {
                foreach (SignedInGamer signedInGamer in Gamer.SignedInGamers)
                    if (signedInGamer.PlayerIndex == GameConstants.CONTROLLER_INDEX)
                        return signedInGamer;
            }
            else
                return local_gamer.SignedInGamer;
            return null;
        }

        public bool CheckAllowedToPlay()
        {
            return local_gamer != null && GameConstants.ALLOWED_MULTIPLAYER;
        }

        public void GamerSessionProperties(String gamertag)
        {
            for (int i = 0; i < sessionProperties.Count; ++i)
		        sessionProperties[i] = 0;
		    char[] c = gamertag.ToCharArray();
		    int begin = 0;
		    int session_value = 0;
		    int start = 1;
		    int val = 0;
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 32)
                    val = 1;
                else if (c[i] >= 48 || c[i] <= 57)
                    val = c[i] - 46;
                else if (c[i] >= 65 && c[i] <= 90)
                    val = c[i] - 53;
                else if (c[i] >= 97 && c[i] <= 122)
                    val = c[i] - 59;
                else
                    val = 0;
                session_value += val * start;
                start *= 64;
                sessionProperties[begin] = session_value;
                if ((i + 1) % 5 == 0)
                {
                    ++begin;
                    start = 1;
                    session_value = 0;
                }
            }
        }

        public bool CloseSession()
        {
            pHandler.EmptyList();
            if (session != null)
            {
                session.Dispose();
                session = null;
                return true;
            }
            return false;
        }

        public NetworkSession GetSession() 
        {
            return session;
        }

        public PlayerHandler GetPlayerHandler()
        {
            return pHandler;
        }

        public LocalNetworkGamer LocalGamer()
        {
            return local_gamer;
        }

        private void ReceiveClientPackets()
        {
            while (local_gamer.IsDataAvailable)
            {
                NetworkGamer sender;
                local_gamer.ReceiveData(reader, out sender);
                if (!sender.IsLocal)
                {
                    HandleServerPackets(sender);
                    DispatchServerPackets();
                }
            }
        }

        private void ReceiveServerPackets()
        {
            NetworkGamer sender;
            local_gamer.ReceiveData(reader, out sender);
            HandleClientPackets(sender);
        }

        private void DispatchServerPackets()
        {
            if (builder.WriterIndex() > 1)
            {
                local_gamer.SendData(builder.Array(), 0, builder.WriterIndex(), SendDataOptions.None);
                builder.Reset();
            }
        }

        private void DispatchClientPackets()
        {
            if (builder.WriterIndex() > 1 && !session.IsHost)
            {
                local_gamer.SendData(builder.Array(), 0, builder.WriterIndex(), SendDataOptions.None, session.Host);
                builder.Reset();
            }
        }

        private void HandleClientPackets(NetworkGamer gamer)
        {
            byte index = reader.ReadByte();
            while (reader.Length - reader.Position > 0)
            {
                byte opcode = reader.ReadByte();
                if (opcode >= C_PACKET_LENGTHS.Length)
                {
                    reader.Position = reader.Length;
                    continue;
                }
                ushort length = C_PACKET_LENGTHS[opcode];
                if (length == 255)
                    length = reader.ReadUInt16();
                else if (length == 254)
                    length = reader.ReadByte();
                Packet packet = new Packet(opcode, reader.ReadBytes(length));
                clientPackets[packet.ID].HandlePacket(pHandler, gamer, packet);
            }
        }

        //Server Handles this individual Player.
        //Server Writes the Updates to a PacketBuilder.
        //Server Completes the Packet with the players Tag value.
        //Server Sends This Packet to all Players.
        private void HandleServerPackets(NetworkGamer gamer)
        {
            byte index = reader.ReadByte();
            int writerIndex = builder.SkipBytes(1);
            while (reader.Length - reader.Position > 0)
            {
                byte opcode = reader.ReadByte();
                if (opcode >= S_PACKET_LENGTHS.Length)
                {
                    reader.Position = reader.Length;
                    continue;
                }
                ushort length = S_PACKET_LENGTHS[opcode];
                if (length == 255)
                    length = reader.ReadUInt16();
                else if (length == 254)
                    length = reader.ReadByte();
                Packet packet = new Packet(opcode, reader.ReadBytes(length));
                serverPackets[packet.ID].HandlePacket(pHandler, gamer, packet);
            }
            if (builder.WriterIndex() != writerIndex)
            {
                writerIndex = builder.SetIndex(writerIndex);
                builder.AddByte((byte)gamer.Tag);
                builder.SetIndex(writerIndex);
            }
        }

        void InviteAcceptedEventHandler(object sender, InviteAcceptedEventArgs e)
        {
            if (!GameConstants.ALLOWED_MULTIPLAYER)
                return;
            if (e.IsCurrentSession)
            {
                if (session != null)
                {
                    session.AddLocalGamer(e.Gamer);
                }
                else
                {
                    //Draw Failed To Join Menu.
                }
            }
            else
            {
                //Draw Joining Session Menu.
                BeginJoinSession();
            }
        }
    }
}
