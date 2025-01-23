﻿using Dapper;
using EcommProject.DataAccess.Data;
using EcommProject.DataAccess.Repository.IRepository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcommProject.DataAccess.Repository
{
    public class SP_CALL : ISP_CALL
    {
        private readonly ApplicationDbContext _context;
        private static string connectionString = "";
        public SP_CALL(ApplicationDbContext context)
        {
            _context= context;
            connectionString = _context.Database.GetDbConnection().ConnectionString; //Code to get connectionString
        }
        public void Dispose() //YE Dispose IDisposable se aya
        {
            _context.Dispose();
        }

        public void Execute(string procedureName, DynamicParameters param = null)
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))  //using for disposable objects
            {
                sqlCon.Open();
                sqlCon.Execute(procedureName, param,commandType:CommandType.StoredProcedure);
            }
        }

        public IEnumerable<T> List<T>(string procedureName, DynamicParameters param = null)
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                return sqlCon.Query<T>(procedureName, param,commandType:CommandType.StoredProcedure);
            }
        }

        public Tuple<IEnumerable<T1>, IEnumerable<T2>> List<T1, T2>(string procedureName, DynamicParameters param = null)
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                var result = sqlCon.QueryMultiple(procedureName,param,commandType:CommandType.StoredProcedure);
                var item1 = result.Read<T1>();
                var item2 = result.Read<T2>();
                if(item1!= null && item2 != null)
                    return new Tuple<IEnumerable<T1>, IEnumerable<T2>>(item1, item2 );
                    //agar null nhi hai to khali object bnakr hii return kr do
                    return new Tuple<IEnumerable<T1>, IEnumerable<T2>>(new List<T1>(),new List<T2>() );
            }  
        }

        public T Single<T>(string procedureName, DynamicParameters param = null)
        {
            using(SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                return sqlCon.ExecuteScalar<T>(procedureName, param,commandType:CommandType.StoredProcedure); //ExecuteScalar returns only single value
            }
        }

        public T OneRecord<T>(string procedureName, DynamicParameters param = null)
        {
            using(SqlConnection sqlCon = new SqlConnection(connectionString) )
            {
                sqlCon.Open();
                var value = sqlCon.Query<T>(procedureName, param, commandType: CommandType.StoredProcedure);
                return value.FirstOrDefault();
            }
        }
    }
}
