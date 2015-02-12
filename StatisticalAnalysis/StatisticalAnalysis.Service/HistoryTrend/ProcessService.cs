using SqlServerDataAdapter;
using StatisticalAnalysis.Infrastruture.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace StatisticalAnalysis.Service.HistoryTrend
{
    public static class ProcessService
    {
        /// <summary>
        /// 根据组织机构ID（生产线）获取工序信息
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public static DataTable GetSystemProcessByOrganizationID(string organizationId)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);

            string queryString = @"SELECT *
                                     FROM [dbo].[formula_FormulaDetail]
                                    WHERE [KeyID] = (
                                        SELECT [KeyID]
                                          FROM [tz_Formula]
                                         WHERE [Type] = 2 AND
                                        	   [ENABLE] = 1 AND
                                        	   [OrganizationID] = @organizationId
                                        )
                                ";

            SqlParameter[] parameters = new SqlParameter[]{
                new SqlParameter("organizationId", organizationId)
            };

            return dataFactory.Query(queryString, parameters);
        }
    }
}
