using SqlServerDataAdapter;
using StatisticalAnalysis.Infrastruture.Configuration;
using StatisticalAnalysis.Infrastruture.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace StatisticalAnalysis.Service.EnergyAlarmAnalysis
{
    public static class AlarmCountAnalysisService
    {
        /// <summary>
        /// 获取报警统计信息（自定义，按日统计）
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="alarmType">报警类型</param>
        /// <returns></returns>
        public static DataTable GetAlarmLogCustom(string organizationId, DateTime startTime, DateTime endTime, string alarmType = "")
        {
            DataTable alarmLog = GetAlarmLogGroupByDay(organizationId, startTime, endTime, alarmType);

            return CountAnalysisTableHelper.VerticalToHorizontal(alarmLog, CountAnalysisTableHelper.SourceType.Daily, startTime, endTime);
        }

        /// <summary>
        /// 获取报警统计信息（月查询，按日统计）
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="alarmType">报警类型</param>
        /// <returns></returns>
        public static DataTable GetAlarmLogMonthly(string organizationId, int year, int month, string alarmType = "")
        {
            DateTime startTime = DateTime.Parse(year + "-" + month + "-01 00:00:00");
            DateTime endTime = new DateTime(year, month, startTime.AddMonths(1).AddDays(-1).Day, 23, 59, 59);

            DataTable alarmLog = GetAlarmLogGroupByDay(organizationId, startTime, endTime, alarmType);

            return CountAnalysisTableHelper.VerticalToHorizontal(alarmLog, CountAnalysisTableHelper.SourceType.Monthly, startTime, endTime);
        }

        /// <summary>
        /// 获取报警统计信息（年查询，按月统计）
        /// </summary>
        /// <param name="organizationId">组织机构ID</param>
        /// <param name="year">需要统计的年份</param>
        /// <param name="alarmType">报警类型</param>
        /// <returns></returns>
        public static DataTable GetAlarmLogYearly(string organizationId, int year, string alarmType = "")
        {
            DateTime startTime = DateTime.Parse(year + "-01-01 00:00:00");
            DateTime endTime = DateTime.Parse(year + "-12-31 23:59:59");

            DataTable alarmLog = GetAlarmLogGroupByMonth(organizationId, startTime, endTime, alarmType);

            return CountAnalysisTableHelper.VerticalToHorizontal(alarmLog, CountAnalysisTableHelper.SourceType.Yearly, DateTime.Parse(year + "-01-01"));
        }

        #region 原始数据获取

        /// <summary>
        /// 获取报警统计信息（按天）
        /// </summary>
        /// <param name="organiztionId">组织机构ID</param>
        /// <param name="startTime">起始时间</param>
        /// <param name="endTime">终止时间</param>
        /// <param name="alarmType">报警类型</param>
        /// <returns></returns>
        private static DataTable GetAlarmLogGroupByDay(string organiztionId, DateTime startTime, DateTime endTime, string alarmType = "")
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);

            string queryString = @"SELECT YEAR([C].[StartTime]) AS [Year], MONTH([C].[StartTime]) AS [Month], DAY([C].[StartTime]) AS [Day], COUNT([C].[StartTime]) AS [Count]
                                     FROM [shift_EnergyConsumptionAlarmLog] AS [C]
                                    WHERE [C].[OrganizationID] IN (
		                                    SELECT [B].[OrganizationID]
		                                      FROM [system_Organization] AS [A], [system_Organization] AS [B] 
		                                     WHERE [A].[OrganizationID]=@organizationId
	 	                                       AND [B].[LevelCode] LIKE [A].[LevelCode] + '%'
		                                    )
                                      AND [C].[StartTime] >= @startTime
                                      AND [C].[StartTime] <= @endTime
                                      AND [C].[EnergyConsumptionType] LIKE @alarmType
                                 GROUP BY YEAR([C].[StartTime]), MONTH([C].[StartTime]), DAY([C].[StartTime])
                                 ORDER BY YEAR([C].[StartTime]), MONTH([C].[StartTime]), DAY([C].[StartTime])";

            if (string.IsNullOrWhiteSpace(alarmType))
                alarmType = "%";

            SqlParameter[] parameters = new SqlParameter[]{
                new SqlParameter("organizationId", organiztionId),
                new SqlParameter("startTime", startTime),
                new SqlParameter("endTime", endTime),
                new SqlParameter("alarmType", alarmType)
            };

            return dataFactory.Query(queryString, parameters);
        }

        /// <summary>
        /// 获取报警统计信息（按月）
        /// </summary>
        /// <param name="organiztionId">组织机构ID</param>
        /// <param name="startTime">起始时间</param>
        /// <param name="endTime">终止时间</param>
        /// <param name="alarmType">报警类型</param>
        /// <returns></returns>
        private static DataTable GetAlarmLogGroupByMonth(string organiztionId, DateTime startTime, DateTime endTime, string alarmType = "")
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);

            string queryString = @"SELECT YEAR([C].[StartTime]) AS [Year], MONTH([C].[StartTime]) AS [Month], COUNT([C].[StartTime]) AS [Count]
                                     FROM [shift_EnergyConsumptionAlarmLog] AS [C]
                                    WHERE [C].[OrganizationID] IN (
		                                    SELECT [B].[OrganizationID]
		                                      FROM [system_Organization] AS [A], [system_Organization] AS [B] 
		                                     WHERE [A].[OrganizationID]=@organizationId
	 	                                       AND [B].[LevelCode] LIKE [A].[LevelCode] + '%'
		                                    )
                                      AND [C].[StartTime] >= @startTime
                                      AND [C].[StartTime] <= @endTime
                                      AND [C].[EnergyConsumptionType] LIKE @alarmType
                                 GROUP BY YEAR([C].[StartTime]), MONTH([C].[StartTime])
                                 ORDER BY YEAR([C].[StartTime]), MONTH([C].[StartTime])";

            if (string.IsNullOrWhiteSpace(alarmType))
                alarmType = "%";

            SqlParameter[] parameters = new SqlParameter[]{
                new SqlParameter("organizationId", organiztionId),
                new SqlParameter("startTime", startTime),
                new SqlParameter("endTime", endTime),
                new SqlParameter("alarmType", alarmType)
            };

            return dataFactory.Query(queryString, parameters);
        }

        #endregion
    }
}