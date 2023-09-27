using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Newtonsoft.Json;
using System.Diagnostics.Contracts;

namespace NOTEA.Models
{
    public class DataService : IDataService
    {
        ConspectListModel conspectsList = new ConspectListModel();
        private void WriteConspectToFile(ConspectListModel conspects)
        {
            try
            {
                foreach (ConspectModel conspect in conspects.conspects)
                {
                    using (StreamWriter writer = new StreamWriter("Conspects//" + conspect.Name + ".txt"))
                    {
                        string serializedJSON = JsonConvert.SerializeObject(conspects);
                        writer.Write(serializedJSON);
                    }
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
            conspectsList.conspects.Add(conspect);
            WriteConspectToFile(conspectsList);
        }
    }
}
