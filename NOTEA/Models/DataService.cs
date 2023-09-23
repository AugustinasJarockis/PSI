using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Newtonsoft.Json;
using System.Diagnostics.Contracts;

namespace NOTEA.Models
{
    public class DataService : IDataService
    {
        private void WriteConspectToFile(string fileName, ConspectListModel conspects)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(fileName))
                {
                    string serializedJSON = JsonConvert.SerializeObject(conspects);
                    writer.Write(serializedJSON);
                }
            }
            catch (Exception exp)
            {
                Console.Write(exp.Message);
            }
        }
        public ConspectListModel LoadConspects(string fileName)
        {
            string text = File.ReadAllText(fileName);
            ConspectListModel conspects = JsonConvert.DeserializeObject<ConspectListModel>(text);
            return conspects;
        }
        public void SaveConspects(ConspectModel conspect)
        {
            ConspectListModel conspectsList = new ConspectListModel();
            conspectsList.conspects.Add(conspect);
            string fileName = "Test.txt";
            WriteConspectToFile(fileName, conspectsList);
        }


    }
}
