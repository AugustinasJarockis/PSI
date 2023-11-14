using NOTEA.Models.ConspectModels;
using NOTEA.Models.UserModels;
using NOTEA.Services.LogServices;
using NOTEA.Models.ExceptionModels;
using NOTEA.Database;
using System.Configuration;
using System.Collections;
using Microsoft.EntityFrameworkCore;

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
            return _database.Conspects.Find(id);
        }
        public ConspectListModel<ConspectModel> LoadConspects(Func<DbSet<ConspectModel>, List<ConspectModel>> Select = null)
        {
            var conspects = Select == null ? _database.Conspects.ToList() : Select(_database.Conspects);
            return new ConspectListModel<ConspectModel>(conspects);
        }
        public void SaveConspect(ConspectModel conspect)
        {
            try
            {
                var temp = _database.Conspects.Find(conspect.Id);
                if (temp == null)
                    _database.Conspects.Add(conspect);
                else
                {
                    _database.Entry(temp).CurrentValues.SetValues(conspect);

                }
                _database.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ExceptionModel info = new ExceptionModel(ex);
                _logsService.SaveExceptionInfo(info);
            }
            //await _database.SaveChangesAsync();
        }
        public void AssignToUser(int conspect_id, int user_id, char access_type = 'a')
        {
            try
            {
                _database.UserConspects.Add(new UserConspectsModel(user_id, conspect_id, access_type));
                _database.SaveChanges();
            }
            catch (Exception ex)
            {
                ExceptionModel info = new ExceptionModel(ex);
                _logsService.SaveExceptionInfo(info);
             }
        }
    }
}