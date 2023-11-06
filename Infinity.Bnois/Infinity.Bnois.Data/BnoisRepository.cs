﻿using Infinity.Bnois.Data.Core;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;

namespace Infinity.Bnois.Data
{
    public class BnoisRepository<T> : Repository<BnoisDbContext, T>, IBnoisRepository<T>
         where T : class
    {
        internal SqlConnection Connection;
        public BnoisRepository(BnoisDbContext context) : base(context)
        {
            Connection = context.Database.Connection as SqlConnection;
        }

        public DataTable ExecWithStoreProcedure(string query, IDictionary<string, object> values)
        {

            using (Connection)

            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandText = query;
                foreach (KeyValuePair<string, object> item in values)
                {
                    cmd.Parameters.AddWithValue("@" + item.Key, item.Value);
                }
                DataTable table = new DataTable();
                using (var reader = cmd.ExecuteReader())
                {
                    table.Load(reader);
                    return table;
                }
            }
        }

        public DataTable ExecWithSqlQuery(string query)
        {
            try
            {
                Connection.Open();
                SqlCommand cmd = new SqlCommand(query, Connection);
                cmd.CommandTimeout = 0;
                DataTable dt = new DataTable();
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);
                dataAdapter.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if
                    (Connection.State == ConnectionState.Open)
                {

                    Connection.Close();
                }
            }


          
        }

        public int ExecNoneQuery(string query)
        {
	        try
	        {
		        Connection.Open();
				using (SqlCommand cmd = new SqlCommand(query, Connection))
				{
                    cmd.CommandTimeout=0;
					return cmd.ExecuteNonQuery();
				}
			}
	        catch(Exception ex)
	        {
		        throw new Exception(ex.Message);
	        }
	        finally
	        {
		        if
			        (Connection.State == ConnectionState.Open)
		        {

			        Connection.Close();
		        }
	        }

			
        }
    }
}
