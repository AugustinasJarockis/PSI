using Newtonsoft.Json;
using NOTEA.Models.ConspectModels;
using NOTEA.Models.ExceptionModels;
using NOTEA.Services.LogServices;
using System.Collections;

namespace NOTEA.Services.FileServices
{
    public class FileService : IFileService
    {
        private readonly ILogsService _logsService;
        public FileService(ILogsService logsService)
        {
            _logsService = logsService;
        }
        public ConspectType LoadConspect<ConspectType>(string filePath)
        {
            string text = File.ReadAllText(filePath);
            ConspectType conspectModel = JsonConvert.DeserializeObject<ConspectType>(text);
            return conspectModel;
        }

        public ConspectListModel<ConspectType> LoadConspects<ConspectType>(string directoryPath)
        {
            ConspectListModel<ConspectType> conspectListModel = new ConspectListModel<ConspectType>();
            string fullDirectoryPath = Directory.GetCurrentDirectory() + "\\" + directoryPath;
            ArrayList filenameList = new ArrayList(Directory.GetFiles(fullDirectoryPath));
            foreach (string fileName in filenameList)
            {
                conspectListModel.Conspects.Add(LoadConspect<ConspectType>(fileName));
            }
            return conspectListModel;
        }
        public void SaveConspect<ConspectType>(ConspectType conspect)
            where ConspectType : IConspectModel
        {
            using (StreamWriter writer = new StreamWriter("Conspects//" + conspect.Name + ".txt"))
                try
                {
                    string serializedJSON = JsonConvert.SerializeObject(conspect);
                    writer.Write(serializedJSON);
                }
                catch (Exception ex)
                {
                    ExceptionModel info = new ExceptionModel(ex);
                    _logsService.SaveExceptionInfo(info);
                }
        }
    }
}