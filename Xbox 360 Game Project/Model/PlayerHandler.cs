using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.GamerServices;

namespace Xbox_360_Game_Project
{
    class PlayerHandler
    {
        private Player[] gamePlayers = new Player[8];
        private sbyte[] indexes = new sbyte[8];
        private sbyte curIdx = 0;
        private byte MAX_SIZE = 0;
        public bool Simulation = false;

        public PlayerHandler()
        {
            for (int i = 0; i < indexes.Length; ++i)
            {
                indexes[i] = -1;
            }
        }

        public sbyte Size()
        {
            return curIdx;
        }

        public bool IsFull()
        {
            return curIdx == MAX_SIZE;
        }

        public void MaxSize(byte max)
        {
            MAX_SIZE = max;
        }

        public void EmptyList()
        {
            int size = curIdx;
            for (int i = 0; i < size; ++i)
            {
                gamePlayers[i] = null;
                indexes[i] = -1;
            }
            Simulation = false;
            curIdx = 0;
        }

        public Player PlayerList(int get)
        {
            if (get >= 0 && get < curIdx)
                return gamePlayers[get];
            return null;
        }

        /*
         * @code 3 - List Full
         * @code 2 - Gamer Already Exists.
         * @code 1 - Successfully added player.
         */
        public byte AddPlayer(NetworkGamer gamer)
        {
            if (curIdx >= MAX_SIZE)
                return 3;
            if (!Simulation && GamerExists(gamer))
                return 2;
            byte index = 0;
            for (; index < indexes.Length; ++index)
                if (indexes[index] == -1)
                    break;
            gamer.Tag = index;
            indexes[index] = curIdx;
            gamePlayers[curIdx++] = new Player(gamer);
            return 1;
        }

        public bool RemovePlayer(NetworkGamer gamer)
        {
            --curIdx;
            if (GetPlayerByCheckingGamer(gamer) == null)
                return false;
            for (sbyte idx = indexes[(byte)gamer.Tag]; idx < curIdx; idx++)
            {
                gamePlayers[idx] = gamePlayers[idx + 1];
                indexes[(byte)gamePlayers[idx].Channel().Tag] = idx;
            }
            indexes[(byte)gamer.Tag] = -1;
            return true;
        }

        public bool GamerExists(NetworkGamer gamer)
        {
            return GetPlayerByCheckingGamer(gamer) != null;
        }

        public Player GetPlayerByCheckingGamer(NetworkGamer gamer)
        {
            if (gamer.Tag == null || (byte)gamer.Tag >= MAX_SIZE || indexes[(byte)gamer.Tag] == -1)
                return null;
            return gamePlayers[indexes[(byte)gamer.Tag]];
        }

        public Player GetPlayerByGamer(NetworkGamer gamer)
        {
            return gamePlayers[indexes[(byte)gamer.Tag]];
        }

        public Player GetPlayerByIndex(int index)
        {
            return gamePlayers[indexes[index]];
        }
    }
}
