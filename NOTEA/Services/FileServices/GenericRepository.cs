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
    public class GenericRepository<ConspectType> : IGenericRepository<ConspectType> where ConspectType : class, IConspectModel
    {
        private readonly ILogsService _logsService;
        private readonly DatabaseContext _database;
        //private DSGeneralEntities _context = null;
        private DbSet<ConspectType> _conspectTypes;
        public GenericRepository(ILogsService logsService, DatabaseContext database)
        {
            _logsService = logsService;
            _database = database;
            _conspectTypes = _database.Set<ConspectType>();
        }
        public ConspectType LoadConspect(int id)
        {
            return _conspectTypes.Find(id);
            //return _database.Conspects.Find(id);
        }
        public ConspectListModel<ConspectType> LoadConspects(int user_id, Func<IQueryable<ConspectType>, List<ConspectType>> Select = null)
        {
            //var x = _conspectTypes.Where(a => a.Conspect_Id == _s);

            var conspects = Select == null ? _database.UserConspects.Where(uc => uc.User_Id == user_id)
                                                                    .Join(_conspectTypes, uc => uc.Conspect_Id, c => c.Id, (uc, c) => c).ToList()
                                           : Select(_database.UserConspects.Where(uc => uc.User_Id == user_id)
                                                                    .Join(_conspectTypes, uc => uc.Conspect_Id, c => c.Id, (uc, c) => c).ToList().AsQueryable());
            return new ConspectListModel<ConspectType>(conspects);
        }
        public void SaveConspect(ConspectType conspect, int id)
        {
            try
            {
                var temp = _conspectTypes.Find(id);
                if (temp == null)
                   // _database.Conspects.Add(conspect);
                   _conspectTypes.Add(conspect);
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
        private Task AddAsync(int conspect_id, int user_id, char access_type = 'a')
        {
            return Task.Factory.StartNew(() =>
            { _database.UserConspects.Add(new UserConspectsModel(user_id, conspect_id, access_type)); });
        }
        //private Task SaveAsync()
        //{
        //    return Task.Factory.StartNew(() =>
        //    { _database.SaveChanges(); });
        //}
        public void DeleteConspect(int id)
        {
            _conspectTypes.Remove(_conspectTypes.Find(id));
            _database.UserConspects.Remove(_database.UserConspects.Find(id));
            _database.SaveChanges();
        }
    }
}