using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Shapes;

namespace DodgeGameAlonKolyakov
{
   
    public static class SaveLoad
    {
       //var enemyRect = new List<Rectangle>();

        //public void LoadJson()
        //{
        //    readTextJson.Visibility = Visibility.Visible;
        //    try
        //    {
        //        // read File path
        //        string jsonFromFile = null;
        //        using (var reader = new StreamReader(_path))
        //        {
        //            jsonFromFile = reader.ReadToEnd();
        //        }
        //        readTextJson.Text = jsonFromFile;

        //        var DodgeGmaeFromJson = JsonConvert.DeserializeObject<dynamic /*Can be GameBoard*/>(jsonFromFile, jsonSettings);
        //    }
        //    catch (Exception ex)
        //    {
        //        readTextJson.Text = $"Couldn't find any saved tedails";
        //    }
        //}
        //public void SaveJson()
        //{
        //    #region
        //    //string S = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        //    //FileStream fs = new FileStream(S, FileMode.OpenOrCreate);
        //    //using (StreamWriter s = new StreamWriter(fs))
        //    //{
        //    //    s.WriteLine("lolgflgl");
        //    //}

        //    //if (File.Exists(S))
        //    //{
        //    #endregion
        //    try
        //    {
        //        var jsonSettings = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All };
        //        // Convert object to string
        //        //var objectToSerialize = GetGameLastSaving();
        //        var jsonToWrite = JsonConvert.SerializeObject(gb/*gb.PrintDeatialsToSave()*//* can be string and also an object*/, Formatting.Indented, jsonSettings);

        //        using (StreamWriter file = File.CreateText(_path))
        //        {
        //            var serializer = new JsonSerializer();
        //            serializer.Serialize(file, jsonToWrite);
        //        }
        //    }
        //    catch { }

        //}
    }
}
