﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBHelper
{
    public interface IDBManager
    {
        DataProvider ProviderType
        {
            get;
            set;
        }

        IDbConnection Connection
        {
            get;
            set;
        }

        IDataReader DataReader
        {
            get;
            set;
        }

        IDbCommand Command
        {
            get;
            set;
        }

        IDbTransaction Transaction
        {
            get;
            set;
        }

        IDbDataParameter[] Parameters
        {
            get;
            set;
        }

        string ConnectionString
        {
            get;
            set;
        }

        void Open();
        void Close();
        void Dispose();
        void CreateParameters(int paramsCount);
        void AddParameters(int index, string paramName, object objValue);
        void BeginTransaction();
        void CommitTransaction();
        void CloseReader();
        IDataReader ExecuteReader(CommandType commandType, string commandText);
        int ExecuteNonQuery(CommandType commandType, string commandText);
        object ExecuteScalar(CommandType commandType, string commandText);
        DataSet ExecuteDataSet(CommandType commandType, string commandText);
    }
}
