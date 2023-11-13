using NOTEA.Models.ConspectModels;
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
        public ConspectListModel<ConspectModel> LoadConspects(Func<DbSet<ConspectModel>, List<ConspectModel>> Select = null/*Func<ConspectModel, bool> filter = null, Func<ConspectModel, string> order = null, IComparer<string> comparer = null*/)
        {
            //var conspects = _database.Conspects.Where(filter == null ? c => true : filter).OrderBy(order == null ? c => "" : order, comparer).ToList();
            var conspects = Select == null ? _database.Conspects.ToList() : Select(_database.Conspects);
            return new ConspectListModel<ConspectModel>(conspects);
        }
        public /*async*/ void SaveConspect(ConspectModel conspect)
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