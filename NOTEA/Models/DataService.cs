using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Newtonsoft.Json;
using System.Diagnostics.Contracts;

namespace NOTEA.Models
{
    public class DataService
    {
        public void SaveConspects(string fileName, ConspectListModel conspects)
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
        public void UsingSave()
        {
            DataService dataService = new DataService();
            ConspectListModel conspectsList = new ConspectListModel();
            ConspectModel conspect = new ConspectModel();   
            conspect.Name = "Test1";
            conspect.Date = "Test1";
            conspect.ConspectText = "Test1"; 
            conspectsList.conspects.Add(conspect);
            string fileName = "Test.txt";
            dataService.SaveConspects(fileName, conspectsList);
        }


    }
}
