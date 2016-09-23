using SqlServerDataAdapter;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace StatisticalAnalysis.Infrastruture.Configuration
{
    public static class ConnectionStringFactory
    {
        public static string NXJCConnectionString { get { return ConfigurationManager.ConnectionStrings["ConnNXJC"].ToString(); } }



        // 临时取数据库名称

        private static IDictionary<string, string> ammeterDatabases = new Dictionary<string, string>();

        public static string GetAmmeterDatabaseName(string organizationId)
        {
            if (ammeterDatabases.ContainsKey(organizationId))
            {
                return ammeterDatabases[organizationId];
            }

            string connectionString = NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);

            string queryString = @"SELECT [system_Database].[MeterDatabase]
                                     FROM [system_Organization] INNER JOIN
                                          [system_Database] ON [system_Organization].[DatabaseID] = [system_Database].[DatabaseID]
                                    WHERE [system_Organization].[OrganizationID] = @organizationId
                                ";


            SqlParameter[] parameters = new SqlParameter[]{
                new SqlParameter("organizationId", organizationId)
            };

            DataTable dt = dataFactory.Query(queryString, parameters);
            if (dt.Rows.Count == 0)
            {
                throw new ArgumentException("无该组织机构ID对应的数据");
            }

            string ammeterDatabaseName = dt.Rows[0]["MeterDatabase"].ToString().Trim();
            ammeterDatabases.Add(organizationId, ammeterDatabaseName);
            return ammeterDatabaseName;
        }
    }
}
