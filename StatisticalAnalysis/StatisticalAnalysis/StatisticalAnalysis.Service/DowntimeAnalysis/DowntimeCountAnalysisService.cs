using SqlServerDataAdapter;
using StatisticalAnalysis.Infrastruture.Configuration;
using StatisticalAnalysis.Infrastruture.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace StatisticalAnalysis.Service.DowntimeAnalysis
{
    public static class DowntimeCountAnalysisService
    {
        /// <summary>
        /// 获取停机原因列表
        /// </summary>
        /// <returns></returns>
        public static DataTable GetMachineHaltReasons()
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;

            ISqlServerDataFactory factory = new SqlServerDataFactory(connectionString);
            Query query = new Query("system_MachineHaltReason");

            DataTable dt = factory.Query(query);
            foreach (DataRow dr in dt.Rows)
            {
                dr["MachineHaltReasonID"] = dr["MachineHaltReasonID"].ToString().Trim();
            }

            return dt;
        }

        /// <summary>
        /// 获取停机统计信息（自定义，按日统计）
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="reasonText">报警类型</param>
        /// <returns></returns>
        public static DataTable GetDowntimeLogCustom(string organizationId, DateTime startTime, DateTime endTime, string reasonLevelCode = "")
        {
            DataTable downtimeLog = GetDowntimeLogGroupByDay(organizationId, startTime, endTime, reasonLevelCode);

            return CountAnalysisTableHelper.VerticalToHorizontal(downtimeLog, CountAnalysisTableHelper.SourceType.Daily, startTime, endTime);
        }

        /// <summary>
        /// 获取停机统计信息（月查询，按日统计）
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="reasonText">报警类型</param>
        /// <returns></returns>
        public static DataTable GetDowntimeLogMonthly(string organizationId, int year, int month, string reasonLevelCode = "")
        {
            DateTime startTime = DateTime.Parse(year + "-" + month + "-01 00:00:00");
            DateTime endTime = new DateTime(year, month, startTime.AddMonths(1).AddDays(-1).Day, 23, 59, 59);

            DataTable downtimeLog = GetDowntimeLogGroupByDay(organizationId, startTime, endTime, reasonLevelCode);

            return CountAnalysisTableHelper.VerticalToHorizontal(downtimeLog, CountAnalysisTableHelper.SourceType.Monthly, startTime, endTime);
        }

        /// <summary>
        /// 获取停机统计信息（年查询，按月统计）
        /// </summary>
        /// <param name="organizationId">组织机构ID</param>
        /// <param name="year">需要统计的年份</param>
        /// <param name="reasonText">报警类型</param>
        /// <returns></returns>
        public static DataTable GetDowntimeLogYearly(string organizationId, int year, string reasonLevelCode = "")
        {
            DateTime startTime = DateTime.Parse(year + "-01-01 00:00:00");
            DateTime endTime = DateTime.Parse(year + "-12-31 23:59:59");

            DataTable downtimeLog = GetDowntimeLogGroupByMonth(organizationId, startTime, endTime, reasonLevelCode);

            return CountAnalysisTableHelper.VerticalToHorizontal(downtimeLog, CountAnalysisTableHelper.SourceType.Yearly, DateTime.Parse(year + "-01-01"));
        }

        #region 原始数据获取

        /// <summary>
        /// 获取停机统计信息（按天）
        /// </summary>
        /// <param name="organiztionId">组织机构ID</param>
        /// <param name="startTime">起始时间</param>
        /// <param name="endTime">终止时间</param>
        /// <param name="reasonText">报警类型</param>
        /// <returns></returns>
        private static DataTable GetDowntimeLogGroupByDay(string organiztionId, DateTime startTime, DateTime endTime, string reasonLevelCode = "")
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);

            string queryString = @"SELECT YEAR([C].[HaltTime]) AS [Year], MONTH([C].[HaltTime]) AS [Month], DAY([C].[HaltTime]) AS [Day], COUNT([C].[HaltTime]) AS [Count]
                                     FROM [shift_MachineHaltLog] AS [C]
                                    WHERE [C].[OrganizationID] IN (
		                                    SELECT [B].[OrganizationID]
		                                      FROM [system_Organization] AS [A], [system_Organization] AS [B] 
		                                     WHERE [A].[OrganizationID] = '{0}'
	 	                                       AND [B].[LevelCode] LIKE [A].[LevelCode] + '%'
		                                    )
                                      AND [C].[HaltTime] >= '{1}'
                                      AND [C].[HaltTime] <= '{2}'
                                      {3}
                                 GROUP BY YEAR([C].[HaltTime]), MONTH([C].[HaltTime]), DAY([C].[HaltTime])
                                 ORDER BY YEAR([C].[HaltTime]), MONTH([C].[HaltTime]), DAY([C].[HaltTime])";

            if (string.IsNullOrWhiteSpace(reasonLevelCode))
            {
                reasonLevelCode = "";
            }
            else
            {
                reasonLevelCode = string.Format("AND [C].[ReasonID] LIKE '{0}%'", reasonLevelCode);
            }
            queryString = string.Format(queryString, organiztionId, startTime, endTime, reasonLevelCode);

            return dataFactory.Query(queryString);
        }

        /// <summary>
        /// 获取停机统计信息（按月）
        /// </summary>
        /// <param name="organiztionId">组织机构ID</param>
        /// <param name="startTime">起始时间</param>
        /// <param name="endTime">终止时间</param>
        /// <param name="reasonText">报警类型</param>
        /// <returns></returns>
        private static DataTable GetDowntimeLogGroupByMonth(string organiztionId, DateTime startTime, DateTime endTime, string reasonLevelCode = "")
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);

            string queryString = @"SELECT YEAR([C].[HaltTime]) AS [Year], MONTH([C].[HaltTime]) AS [Month], COUNT([C].[HaltTime]) AS [Count]
                                     FROM [shift_MachineHaltLog] AS [C]
                                    WHERE [C].[OrganizationID] IN (
		                                    SELECT [B].[OrganizationID]
		                                      FROM [system_Organization] AS [A], [system_Organization] AS [B] 
		                                     WHERE [A].[OrganizationID] = '{0}'
	 	                                       AND [B].[LevelCode] LIKE [A].[LevelCode] + '%'
		                                    )
                                      AND [C].[HaltTime] >= '{1}'
                                      AND [C].[HaltTime] <= '{2}'
                                      {3}
                                 GROUP BY YEAR([C].[HaltTime]), MONTH([C].[HaltTime])
                                 ORDER BY YEAR([C].[HaltTime]), MONTH([C].[HaltTime])";

            if (string.IsNullOrWhiteSpace(reasonLevelCode))
            {
                reasonLevelCode = "";
            }
            else
            {
                reasonLevelCode = string.Format("AND [C].[ReasonID] LIKE '{0}%'", reasonLevelCode);
            }
            queryString = string.Format(queryString, organiztionId, startTime, endTime, reasonLevelCode);

            return dataFactory.Query(queryString);
        }

        #endregion

    }
}
