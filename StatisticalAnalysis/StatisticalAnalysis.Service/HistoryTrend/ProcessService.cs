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

            string queryString = @"SELECT A.Name + B.Name as Name,
                                        A.OrganizationID as OrganizationID,
                                        B.VariableId as VariableId,
                                        B.LevelCode as LevelCode,
                                        B.LevelType as LevelType
                                        from tz_Formula A, formula_FormulaDetail B 
                                        where A.KeyID = B.KeyID
                                        and A.Type = 2
                                        and A.ENABLE = 1
                                        and A.State = 0
                                        and A.OrganizationID = @organizationId";

            SqlParameter[] parameters = new SqlParameter[]{
                new SqlParameter("organizationId", organizationId)
            };

            return dataFactory.Query(queryString, parameters);
        }
    }
}
