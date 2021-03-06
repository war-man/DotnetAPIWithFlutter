﻿using API.ViewModels;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Bcrypt = BCrypt.Net.BCrypt;

namespace API.Repository
{
    public class RoleRepository
    {
        IConfiguration _configuration;
        DynamicParameters parameters = new DynamicParameters();
        public RoleRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IEnumerable<GetRoleVM>> getAll()
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("myConn")))
            {
                var procName = "SP_Role_GetAll";
                var getAll = await connection.QueryAsync<GetRoleVM>(procName, commandType: CommandType.StoredProcedure);
                return getAll;
            }
        }

        public GetRoleVM getID(int id)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("myConn")))
            {
                var procName = "SP_Role_GetID";
                parameters.Add("id", id);
                var getId = connection.Query<GetRoleVM>(procName, parameters, commandType: CommandType.StoredProcedure).SingleOrDefault();
                return getId;
            }
        }

    }

    public class UserRepository
    {
        IConfiguration _configuration;
        DynamicParameters parameters = new DynamicParameters();
        public UserRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IEnumerable<GetUserVM>> getAll()
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("myConn")))
            {
                var procName = "SP_User_GetAll";
                var getAll = await connection.QueryAsync<GetUserVM>(procName, commandType: CommandType.StoredProcedure);
                return getAll;
            }
        }

        public GetUserVM getID(int id)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("myConn")))
            {
                var procName = "SP_User_GetID";
                parameters.Add("id", id);
                var getId = connection.Query<GetUserVM>(procName, parameters, commandType: CommandType.StoredProcedure).SingleOrDefault();
                return getId;
            }
        }


    }
}
