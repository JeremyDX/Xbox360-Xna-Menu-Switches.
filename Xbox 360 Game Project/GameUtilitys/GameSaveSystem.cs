using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;

namespace Xbox_360_Game_Project
{
    class GameSaveSystem 
    {

        private String username = null;
        private FileBuilder fileStorage;
        private StorageDevice device;
        private StorageContainer container;
        private bool selectorInUse = false;
        private bool NoDeviceChoosen = false;
        private long device_size = 0;
        private sbyte messageBox = -1;

        public GameSaveSystem()
        {
            StorageDevice.DeviceChanged += new EventHandler<EventArgs>(DeviceChanged);
        }

        private void DeviceChanged(object sender, EventArgs e)
        {

        }

        private void ShowLoadingScreen(bool show)
        {
            GameConstants.WINDOW_OVERLAY_INDEX = (sbyte)(show ? 0 : (GameConstants.WINDOW_OVERLAY_INDEX == 0 ? -1 : GameConstants.WINDOW_OVERLAY_INDEX));
        }

        private void SaveDeviceChoosenScreen(IAsyncResult result)
        {
            int? choice = Guide.EndShowMessageBox(result);
            if (result.IsCompleted && choice.HasValue && choice == 1)
            {
                NoDeviceChoosen = false;
            }
        }

        public void ChooseDevice()
        {
            ShowLoadingScreen(false);
            if (!Guide.IsVisible)
            {
                selectorInUse = true;
                try
                {
                    StorageDevice.BeginShowSelector(GameConstants.CONTROLLER_INDEX, GetDevice, null);
                }
                catch (GuideAlreadyVisibleException)
                {
                    ShowLoadingScreen(true);
                }
            }
        }

        void GetDevice(IAsyncResult result)
        {
            bool screen = Guide.IsVisible;
            bool hasDevice = device != null;
            StorageDevice newDevice = StorageDevice.EndShowSelector(result);
            selectorInUse = false;
            if (newDevice != null && newDevice.IsConnected && result.IsCompleted)
            {
                NoDeviceChoosen = false;
                device = newDevice;
                if (!screen && hasDevice && device_size != 0)
                {
                    messageBox = 1;
                }
                else if (!hasDevice)
                {
                    if (username == null)
                    {
                        LoadPreviousProfile();
                    }
                }
                device_size = device.TotalSpace;
            }
            else
            {
                if (!hasDevice)
                    messageBox = 0;
                NoDeviceChoosen = true;
            }
            ShowLoadingScreen(false);
	    }

        public void Update(GameTime gameTime)
        {
            if (messageBox != -1 && !Guide.IsVisible)
            {
                switch (messageBox)
                {
                    case 0:
                        Guide.BeginShowMessageBox("Device Not Selected", "No device was selected would you like to try again?", new[] { "No", "Yes" }, 0, MessageBoxIcon.Warning, SaveDeviceChoosenScreen, null);
                        break;
                    case 1:
                        Guide.BeginShowMessageBox("Device Selected", "Hard Drive Selected", new[] { "OK" }, 0, MessageBoxIcon.Alert, SaveDeviceChoosenScreen, null);
                        break;
                }
                if (username == null && device != null)
                {
                    LoadPreviousProfile();
                }
                messageBox = -1;
            }
            if (device == null && !selectorInUse && !NoDeviceChoosen)
            {
                ChooseDevice();
            }
        }

        public void OpenGameProfile()
        {
            OpenOrCreateFile("GameSave" + username + ".sav");
        }

        private void OpenOrCreateFile(String filename)
        {
            IAsyncResult result = device.BeginOpenContainer("(DXG)The Survival Game Save", null, null);
            result.AsyncWaitHandle.WaitOne();
            try
            {
                container = device.EndOpenContainer(result);
            }
            catch (System.InvalidOperationException)
            {
                return;
            }
            result.AsyncWaitHandle.Close();
            if (container.FileExists(filename))
            {

            }
            result.AsyncWaitHandle.Close();
            container.Dispose();
        }

        private void LoadPreviousProfile()
        {
            IAsyncResult result = device.BeginOpenContainer("(DXG)The Survival Game Save", null, null);
            if (result == null)
            {
                return;
            }
            result.AsyncWaitHandle.WaitOne();
            try
            {
                container = device.EndOpenContainer(result);
            }
            catch (System.InvalidOperationException)
            {
                return;
            }
            result.AsyncWaitHandle.Close();
            if (container.FileExists("LastGameSettings.sav"))
            {
                BinaryReader reader = new BinaryReader(container.OpenFile("LastGameSettings.sav", FileMode.Open));
                short length = reader.ReadInt16();
                if (length == 0)
                {

                }
                else
                {
                    fileStorage = new FileBuilder(reader.ReadBytes(length));
                    username = fileStorage.ReadString();
                }
                reader.Close();
            } 
            else
            {
                BinaryWriter writer = new BinaryWriter(container.CreateFile("LastGameSettings.sav"));
                writer.Write(new byte[] { 0, 0 }); //New Empty Profile Created!
                writer.Close();
            }
            container.Dispose();
        }

        public bool CreateProfile(string username)
        {
            if (device == null)
            {
                //ProfileCreationProblem.DETAILS = "No Save Device Found.";
                return false;
            }
            IAsyncResult result = device.BeginOpenContainer("(DXG)The Survival Game Save", null, null);
            result.AsyncWaitHandle.WaitOne();
            try
            {
                container = device.EndOpenContainer(result);
            }
            catch (System.InvalidOperationException)
            {
                //ProfileCreationProblem.DETAILS = "Unable To Read Device.";
                return false;
            }
            result.AsyncWaitHandle.Close();
            if (container.FileExists("GameSave" + username + ".sav")) {
                //ProfileCreationProblem.DETAILS = "Profile Already Exists.";
                return false;
            }
            fileStorage = new FileBuilder(username.Length + 3);
            fileStorage.WriteShort(username.Length);
            fileStorage.WriteString(username);
            BinaryWriter writer = new BinaryWriter(container.CreateFile("LastGameSettings.sav"));
            writer.Write(fileStorage.Array()); //New Empty Profile Created!
            writer.Close();
            writer = new BinaryWriter(container.CreateFile("GameSave" + username + ".sav"));
            writer.Write(fileStorage.Array()); //New Empty Profile Created!
            writer.Close();
            container.Dispose();
            return true;
        }

    }

}
