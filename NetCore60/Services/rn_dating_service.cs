using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MySqlConnector;
using NetCoreSpace.DTO;
using NetCoreSpace.Models;
using NetCoreSpace.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Threading.Tasks.Dataflow;
using System.Xml.Linq;

namespace NetCoreSpace.Services
{
    public class RNDatingService
    {
        private readonly ApplicationDbContext _dbContext;

        public RNDatingService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public List<RequestLog> GetRequestLogs()
        {
                var userEntity = _dbContext.RequestLogs.ToList();
                return userEntity;
            
        }

        public List<VTagGroupDetail> GetTagGroupDetails()
        {

                var userEntity = _dbContext.VTagGroupDetails.ToList();
                return userEntity;
            
        }

        public string testConnectionDatabase()
        {

                try
                {
                    _dbContext.Database.OpenConnection();
                    bool isConnected = _dbContext.Database.CanConnect();
                    if (isConnected)
                    {
                        return "成功連線到資料庫！";
                    }
                    else
                    {
                        return "無法連線到資料庫。";
                    }
                }
                catch (Exception ex)
                {
                    return "連線時發生錯誤：" + ex.Message;
                }
            }
        

        public bool CheckDatabaseConnection()
        {
            try
            {
                _dbContext.Database.OpenConnection();
                _dbContext.Database.CloseConnection();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }

}
