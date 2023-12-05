﻿using NOTEA.Models.ConspectModels;
using NOTEA.Models.UserModels;
using NOTEA.Services.LogServices;
using NOTEA.Models.ExceptionModels;
using Microsoft.EntityFrameworkCore;

namespace NOTEA.Repositories.GenericRepositories
{
    public class GenericRepository<ConspectType> : IGenericRepository<ConspectType> where ConspectType : class, IConspectModel
    {
        private readonly ILogsService _logsService;
        private DbSet<ConspectType> _conspectTypes;
        public GenericRepository(ILogsService logsService)
        {
            _logsService = logsService;
            //_database = database;
            //_conspectTypes = _database.Set<ConspectType>();
        }
        public ConspectType LoadConspect(int id)
        {
            return _conspectTypes.Find(id);
        }
        //public ConspectListModel<ConspectType> LoadConspects(int user_id, Func<IQueryable<ConspectType>, List<ConspectType>> Select = null)
        //{

        //    //var conspects = Select == null ? _database.UserConspects.Where(uc => uc.User_Id == user_id)
        //    //                                                        .Join(_conspectTypes, uc => uc.Conspect_Id, c => c.Id, (uc, c) => c).ToList()
        //    //                               : Select(_database.UserConspects.Where(uc => uc.User_Id == user_id)
        //    //                                                        .Join(_conspectTypes, uc => uc.Conspect_Id, c => c.Id, (uc, c) => c));
        //    //return new ConspectListModel<ConspectType>(conspects);
        //}
        //public void SaveConspect(ConspectType conspect, int id)
        //{
        //    try
        //    {
        //        var temp = _conspectTypes.Find(id);
        //        if (temp == null)
        //           _conspectTypes.Add(conspect);
        //        else
        //        {
        //            _database.Entry(temp).CurrentValues.SetValues(conspect);

        //        }
        //        _database.SaveChanges();
        //    }
        //    catch (Exception ex)
        //    {
        //        _logsService.SaveExceptionInfo(new ExceptionModel(ex));
        //    }
        //}
        //public void AssignToUser(int conspect_id, int user_id, char access_type = 'a')
        //{
        //    try
        //    {
        //        _database.UserConspects.Add(new UserConspectsModel(user_id, conspect_id, access_type));
        //        _database.SaveChanges();
        //    }
        //    catch (Exception ex)
        //    {
        //        _logsService.SaveExceptionInfo(new ExceptionModel(ex));
        //     }
        //}
        //public void DeleteConspect(int id, int user_id)
        //{
        //    if (_database.UserConspects.Where(a => a.Conspect_Id == id).Count() > 1)
        //    {
        //        _database.UserConspects.Remove(_database.UserConspects.Where(a => a.Conspect_Id == id && a.User_Id == user_id).Single());
        //        _database.SaveChanges();
        //        _database.UserConspects.Where(a => a.Conspect_Id == id).First().Access_Type = 'a';
        //        _database.SaveChanges();
        //    }
        //    else 
        //    {
        //        _conspectTypes.Remove(_conspectTypes.Find(id));
        //        _database.UserConspects.Remove(_database.UserConspects.Where(a => a.Conspect_Id == id && a.User_Id == user_id).Single());
        //        _database.SaveChanges();
        //    }
        //}
    }
}