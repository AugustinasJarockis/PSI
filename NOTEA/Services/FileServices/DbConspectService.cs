using NOTEA.Models.ConspectModels;
using NOTEA.Services.LogServices;
using NOTEA.Models.ExceptionModels;
using NOTEA.Database;

namespace NOTEA.Services.FileServices
{
    public class DbConspectService : IFileService
    {
        private readonly ILogsService _logsService;
        private readonly DatabaseContext _database;
        public DbConspectService(ILogsService logsService, DatabaseContext database)
        {
            _logsService = logsService;
            _database = database;
        }
        public ConspectModel LoadConspect(int id)
        {
            return _database.Conspects.Where(c => c.Id == id).First();
        }
        public ConspectListModel<ConspectModel> LoadConspects()
        {
            var conspects = _database.Conspects.ToList();
            return new ConspectListModel<ConspectModel>(conspects);
        }
        public /*async*/ void SaveConspect(ConspectModel conspect)
        {
            try
            {
                _database.Conspects.Add(conspect);
                string userN = Environment.UserName;
                //var user = _database.Users.Where(c => c.Username == userN).First();
                //user.Conspects_Id.Add(conspect.Id);
                _database.Users.Where(c => c.Username == userN).First().Conspects_Id.Add(conspect.Id);
                _database.SaveChanges();
            }
            catch (Exception ex)
            {
                ExceptionModel info = new ExceptionModel(ex);
                _logsService.SaveExceptionInfo(info);
            }
            //await _database.SaveChangesAsync();
        }
    }
}