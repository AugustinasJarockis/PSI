using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Newtonsoft.Json;
using System.Diagnostics.Contracts;

namespace NOTEA.Models
{
    public class DataService : IDataService
    {
        ConspectListModel conspectsList = new ConspectListModel();
        FileNameListModel fileNameList = new FileNameListModel();
        ConspectModel conspectModel = new ConspectModel();
        public ConspectModel LoadConspects(string fileName)
        {
            string text = File.ReadAllText("Conspects//" + fileName + ".txt");
            Console.WriteLine(text);
            ConspectModel conspectModel = JsonConvert.DeserializeObject<ConspectModel>(text);
            Console.WriteLine("konspio" + conspectModel.Name + " " + conspectModel.ConspectText);
            return conspectModel;
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
                Console.WriteLine("konspis the original  " + conspect.Name);
               string serializedJSON = JsonConvert.SerializeObject(conspect);
               writer.Write(serializedJSON);
            }
        }
        public void SaveFileName (FileNameModel fileNames, string fileName) 
        {
            fileNameList = LoadFileNames();
            if (!(fileNameList.fileNameList.Any(x => x.Name == new FileNameModel(fileName).Name)))
            {
                fileNameList.fileNameList.Add(new FileNameModel (fileName));
                using (StreamWriter nameWriter = new StreamWriter("FileNames.txt"))
                {
                    string serializedJSON = JsonConvert.SerializeObject(fileNameList);
                    nameWriter.Write(serializedJSON);
                }
            }
        }
        public FileNameListModel LoadFileNames()
        {
            string text = File.ReadAllText("FileNames.txt");
            FileNameListModel fileNameList = JsonConvert.DeserializeObject<FileNameListModel>(text);
            return fileNameList;
        }
    }
}