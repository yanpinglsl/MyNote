﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.OleDb;
using MySql.Data.MySqlClient;
using System.Data.Odbc;
using Oracle.ManagedDataAccess.Client;

namespace DBHelper
{
    public class DBManagerFactory
    {
        private DBManagerFactory()
        {
        }

        public static IDbConnection? GetConnection(DataProvider providerType)
        {
            IDbConnection iDbConnection;
            switch (providerType)
            {
                case DataProvider.SqlServer:
                    iDbConnection = new SqlConnection();
                    break;
                case DataProvider.OleDb:
                    iDbConnection = new OleDbConnection();
                    break;
                case DataProvider.Odbc:
                    iDbConnection = new OdbcConnection();
                    break;
                case DataProvider.Oracle:
                    iDbConnection = new OracleConnection();
                    break;
                case DataProvider.MySql:
                    iDbConnection = new MySqlConnection();
                    break;
                default:
                    return null;
            }
            return iDbConnection;
        }

        public static IDbCommand? GetCommand(DataProvider providerType)
        {
            switch (providerType)
            {
                case DataProvider.SqlServer:
                    return new SqlCommand();
                case DataProvider.OleDb:
                    return new OleDbCommand();
                case DataProvider.Odbc:
                    return new OdbcCommand();
                case DataProvider.Oracle:
                    return new OracleCommand();
                case DataProvider.MySql:
                    return new MySqlCommand();
                default:
                    return null;
            }
        }

        public static IDbDataAdapter? GetDataAdapter(DataProvider providerType)
        {
            switch (providerType)
            {
                case DataProvider.SqlServer:
                    return new SqlDataAdapter();
                case DataProvider.OleDb:
                    return new OleDbDataAdapter();
                case DataProvider.Odbc:
                    return new OdbcDataAdapter();
                case DataProvider.Oracle:
                    return new OracleDataAdapter();
                case DataProvider.MySql:
                    return new MySqlDataAdapter();
                default:
                    return null;
            }
        }

        public static IDbTransaction? GetTransaction(DataProvider providerType)
        {
            IDbConnection? iDbConnection = GetConnection(providerType);
            IDbTransaction? iDbTransaction = iDbConnection?.BeginTransaction();
            return iDbTransaction;
        }

        public static IDbDataParameter[]? GetParameters(DataProvider providerType, int paramsCount)
        {
            IDbDataParameter[]? idbParams = new IDbDataParameter[paramsCount];
            switch (providerType)
            {
                case DataProvider.SqlServer:
                    for (int i = 0; i < paramsCount; i++)
                    {
                        idbParams[i] = new SqlParameter();
                    }
                    break;
                case DataProvider.OleDb:
                    for (int i = 0; i < paramsCount; i++)
                    {
                        idbParams[i] = new OleDbParameter();
                    }
                    break;
                case DataProvider.Odbc:
                    for (int i = 0; i < paramsCount; i++)
                    {
                        idbParams[i] = new OdbcParameter();
                    }
                    break;
                case DataProvider.Oracle:
                    for (int i = 0; i < paramsCount; i++)
                    {
                        idbParams[i] = new OracleParameter();
                    }
                    break;
                case DataProvider.MySql:
                    for (int i = 0; i < paramsCount; i++)
                    {
                        idbParams[i] = new MySqlParameter();
                    }
                    break;
                default:
                    idbParams = null;
                    break;
            }
            return idbParams;
        }
    }
}
