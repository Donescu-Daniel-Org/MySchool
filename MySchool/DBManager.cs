using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace MySchool
{
    class DBManager
    {
        string connectionString;
        SqlConnection connection;

        public DBManager(string connStringName)
        {
            connectionString = ConfigurationManager.ConnectionStrings[connStringName].ConnectionString;
            connection = new SqlConnection(connectionString);
        }

        public DataTable ExecuteProcedure(string procedureName, params Tuple<string, object, SqlDbType>[] procParams)
        {
            connection.Open();

            using (SqlCommand sqlCommand = new SqlCommand(procedureName, connection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;

                for (int index = 0; index < procParams.Length; index++)
                {
                    Tuple<string, object, SqlDbType> t = procParams[index];
                    sqlCommand.Parameters.AddWithValue(t.Item1, t.Item3).Value = t.Item2;
                }

                SqlDataReader dataReader = sqlCommand.ExecuteReader();

                DataTable dataTable = new DataTable();
                dataTable.Load(dataReader);

                connection.Close();

                return dataTable;
            }
        }

        public void ExecuteNonProcedure(string procedureName, params Tuple<string, object, SqlDbType>[] procParams)
        {
            connection.Open();

            using (SqlCommand sqlCommand = new SqlCommand(procedureName, connection))
            {
                sqlCommand.CommandType = CommandType.StoredProcedure;

                for (int index = 0; index < procParams.Length; index++)
                {
                    Tuple<string, object, SqlDbType> t = procParams[index];
                    sqlCommand.Parameters.AddWithValue(t.Item1, t.Item3).Value = t.Item2;
                }

                sqlCommand.ExecuteNonQuery();

                connection.Close();
            }
        }
    }
}
