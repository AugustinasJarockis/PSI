﻿using NOTEA.Models.ConspectModels;
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