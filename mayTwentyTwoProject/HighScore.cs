using System;
using Windows.Storage;
namespace DodgeGameAlonKolyakov
{
    [Serializable]
    public class HighScore
    {
        //Storage Folder
        public static StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
        public static async void CreateFile()
        {
            try
            {  // saves to a local folder at the mainPage you can track the debug that tells you the path where the file is
                await storageFolder.CreateFileAsync("AlonDodgeGameUWP.txt", CreationCollisionOption.OpenIfExists);  
            }
            catch { }
        }
        public static async void ReadFile()
        {
            try
            {
                StorageFile DataFile = await storageFolder.GetFileAsync("AlonDodgeGameUWP.txt");
                MainPage.strHighScore = await FileIO.ReadTextAsync(DataFile);
                if (MainPage.strHighScore == "")
                    MainPage.strHighScore = "0";
            }
            catch { }
        }
        public static async void UpdateScore(int myScore)
        {
            try
            {
                StorageFile DataFile = await storageFolder.GetFileAsync("AlonDodgeGameUWP.txt");
                await FileIO.WriteTextAsync(DataFile, myScore.ToString());
                ReadFile();
            }
            catch { }
        }

    }
}