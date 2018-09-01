using SqlServerDataAdapter;
using StatisticalAnalysis.Infrastruture.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace StatisticalAnalysis.Service.BasicHistoryTrend
{
    public static class ProcessService
    {
        /// <summary>
        /// 根据组织机构ID（生产线）获取工序信息
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public static DataTable GetSystemProcessByOrganizationID(string organizationId, string mEndTime)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);

            string queryString = @"SELECT A.Name + B.Name as Name,
                                        A.OrganizationID as OrganizationID,
                                        B.VariableId as VariableId,
                                        B.LevelCode as LevelCode,
                                        B.LevelType as LevelType
                                        from NXJC_DEV.dbo.tz_Formula A, NXJC_DEV.dbo.formula_FormulaDetail B                                     
                                        where A.KeyID = (SELECT TOP 1 KeyID
                                                            FROM tz_Formula
                                                            WHERE OrganizationID= @organizationId AND CreatedDate <= @endTime AND ENABLE = 1 AND State = 0 
                                                            ORDER BY CreatedDate DESC)--根据结束时间选择最近版本
                                        and A.KeyID = B.KeyID
                                        and A.Type = 2
                                        and A.ENABLE = 1
                                        and A.State = 0";

            SqlParameter[] parameters = new SqlParameter[]{ new SqlParameter("@organizationId", organizationId),
                                                            new SqlParameter("@endTime", mEndTime)};
            DataTable table = dataFactory.Query(queryString, parameters);
            return table;
        }
    }
}
