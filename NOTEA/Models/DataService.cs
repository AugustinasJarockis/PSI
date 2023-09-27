using Newtonsoft.Json;
using System.Collections;

namespace NOTEA.Models
{
    public class DataService : IDataService
    {
        private ConspectListModel conspectsList = new ConspectListModel();
        public ConspectModel LoadConspect(string filePath)
        {
            string text = File.ReadAllText(filePath);
            ConspectModel conspectModel = JsonConvert.DeserializeObject<ConspectModel>(text);
            return conspectModel;
        }

        public ConspectListModel LoadConspects(string directoryPath)
        {
            ConspectListModel conspectListModel = new ConspectListModel();
            string fullDirectoryPath = Directory.GetCurrentDirectory() + "\\" + directoryPath;
            ArrayList filenameList = new ArrayList(Directory.GetFiles(fullDirectoryPath));
            foreach (string fileName in filenameList)
            {
                conspectListModel.conspects.Add(LoadConspect(fileName));
            }
            return conspectListModel;
        }
        public void SaveConspect(ConspectModel conspect)
        {
            using (StreamWriter writer = new StreamWriter("Conspects//" + conspect.Name + ".txt"))
            try
            {
                string serializedJSON = JsonConvert.SerializeObject(conspect);
                writer.Write(serializedJSON);
            }
            catch (Exception exp)
            {
               Console.WriteLine("Error: could not save file: " + conspect.Name);
            }
        }
    }
}