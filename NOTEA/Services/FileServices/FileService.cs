//using Microsoft.EntityFrameworkCore;
//using Newtonsoft.Json;
//using NOTEA.Models.ConspectModels;
//using NOTEA.Models.ExceptionModels;
//using NOTEA.Models.UserModels;
//using NOTEA.Services.LogServices;
//using System.Collections;

//namespace NOTEA.Services.FileServices
//{
//    public class FileService : IGenericRepository
//    {
//        private readonly ILogsService _logsService;
//        public FileService(ILogsService logsService)
//        {
//            _logsService = logsService;
//        }
//        public ConspectType LoadConspect<ConspectType>(string filePath)
//        {
//            string text = File.ReadAllText(filePath);
//            ConspectType conspectModel = JsonConvert.DeserializeObject<ConspectType>(text);
//            return conspectModel;
//        }
//        public ConspectModel LoadConspect(int id)
//        {
//            ConspectListModel<ConspectModel> conspectListModel = LoadConspects();
//            ConspectModel conspectModel = conspectListModel.Conspects.Where(c => c.Id == id).First();
//            return conspectModel;
//        }
//        public ConspectListModel<ConspectType> LoadConspects<ConspectType>(Func<ConspectType, string> filter = null, Func<ConspectType, bool> order = null)
//        {
//            ConspectListModel<ConspectType> conspectListModel = new ConspectListModel<ConspectType>();
//            string fullDirectoryPath = Directory.GetCurrentDirectory() + "\\" + "Conspects";
//            ArrayList filenameList = new ArrayList(Directory.GetFiles(fullDirectoryPath));
//            foreach (string fileName in filenameList)
//            {
//                conspectListModel.Conspects.Add(LoadConspect<ConspectType>(fileName));
//            }
//            return conspectListModel;
//        }
//        //Non of the optional parameters work here at all
//        public ConspectListModel<ConspectModel> LoadConspects(Func<DbSet<ConspectModel>, List<ConspectModel>> Select = null/*Func<ConspectModel, bool> filter = null, Func<ConspectModel, string> order = null, IComparer<string> comparer = null*/)
//        {
//            ConspectListModel<ConspectModel> conspectListModel = new ConspectListModel<ConspectModel>();
//            string fullDirectoryPath = Directory.GetCurrentDirectory() + "\\" + "Conspects";
//            ArrayList filenameList = new ArrayList(Directory.GetFiles(fullDirectoryPath));
//            foreach (string fileName in filenameList)
//            {
//                conspectListModel.Conspects.Add(LoadConspect<ConspectModel>(fileName));
//            }
//            return conspectListModel;
//        }
//        public void SaveConspect<ConspectType>(ConspectType conspect)
//            where ConspectType : IConspectModel
//        {
//            using (StreamWriter writer = new StreamWriter("Conspects//" + conspect.Name + ".txt"))
//                try
//                {
//                    string serializedJSON = JsonConvert.SerializeObject(conspect);
//                    writer.Write(serializedJSON);
//                }
//                catch (Exception ex)
//                {
//                    ExceptionModel info = new ExceptionModel(ex);
//                    _logsService.SaveExceptionInfo(info);
//                }
//        }

//        public void SaveConspect(ConspectModel conspect)
//        {
//            using (StreamWriter writer = new StreamWriter("Conspects//" + conspect.Name + ".txt"))
//                try
//                {
//                    string serializedJSON = JsonConvert.SerializeObject(conspect);
//                    writer.Write(serializedJSON);
//                }
//                catch (Exception ex)
//                {
//                    ExceptionModel info = new ExceptionModel(ex);
//                    _logsService.SaveExceptionInfo(info);
//                }
//        }
//        public void AssignToUser(int conspect_id, int user_id, char access_type = 'a')
//        {
//        }
//    }
//}