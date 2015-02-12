using SqlServerDataAdapter;
using StatisticalAnalysis.Infrastruture.Configuration;
//using StatisticalAnalysis.Infrastruture.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace StatisticalAnalysis.Service.CementCostAnalysis
{
    public static class CementCostAnalysisService
    {
        public static DataTable GetCementTypes()
        {
            ISqlServerDataFactory _dataFactory = new SqlServerDataFactory(ConnectionStringFactory.NXJCConnectionString);
            string sql = "SELECT CementTypes FROM [dbo].[system_CementTypesAndConvertCoefficient] GROUP BY CementTypes";
            DataTable table = _dataFactory.Query(sql);
            return table;
        }
        /// <summary>
        /// 获取甲乙丙煤耗（自定义，按日统计）
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="variableId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static DataTable GetCementCostPVFCustom(string organizationId, DateTime startTime, DateTime endTime, string cementType)
        {
            DataTable pvfUsage = GetCementCostFSTDailyByOrganiztionId(organizationId, startTime, endTime, cementType);

            return CementCostFSTAnalysisTableHelper.VerticalToHorizontal(pvfUsage, CementCostFSTAnalysisTableHelper.SourceType.Daily, startTime, endTime);
        }

        /// <summary>
        /// 获取甲乙丙煤耗（月查询，按日统计）
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="variableId"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public static DataTable GetCementCostPVFMonthly(string organizationId, int year, int month, string cementType)
        {
            DateTime startTime = DateTime.Parse(year + "-" + month + "-01 00:00:00");
            DateTime endTime = new DateTime(year, month, startTime.AddMonths(1).AddDays(-1).Day, 23, 59, 59);

            DataTable pvfUsage = GetCementCostFSTDailyByOrganiztionId(organizationId, startTime, endTime, cementType);

            return CementCostFSTAnalysisTableHelper.VerticalToHorizontal(pvfUsage, CementCostFSTAnalysisTableHelper.SourceType.Monthly, startTime, endTime);
        }

        /// <summary>
        /// 获取甲乙丙煤耗（年查询，按月统计）
        /// </summary>
        /// <param name="organizationId">组织机构ID</param>
        /// <param name="variableId"></param>
        /// <param name="year">需要统计的年份</param>
        /// <returns></returns>
        public static DataTable GetCementCostPVFYearly(string organizationId, int year,string cementType)
        {
            DateTime startTime = DateTime.Parse(year + "-01-01 00:00:00");
            DateTime endTime = DateTime.Parse(year + "-12-31 23:59:59");

            DataTable pvfUsage = GetCementCostFSTMonthlyByOrganiztionId(organizationId, startTime, endTime,cementType);

            return CementCostFSTAnalysisTableHelper.VerticalToHorizontal(pvfUsage, CementCostFSTAnalysisTableHelper.SourceType.Yearly, DateTime.Parse(year + "-01-01"));
        }

        /// <summary>
        /// 按照组织机构ID（生产线）获取按日的甲乙丙煤耗
        /// </summary>
        /// <param name="organizationId">组织机构ID（生产线）</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns>峰谷平用电量</returns>
        public static DataTable GetCementCostFSTDailyByOrganiztionId(string organizationId, DateTime startTime, DateTime endTime, string cementType)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);

            string queryString = @"SELECT [B].*, [A].[TimeStamp]
                                     FROM [balance_Energy] AS [B] INNER JOIN
                                          [tz_Balance] AS [A] ON [B].[KeyId] = [A].[BalanceId]
                                    WHERE ([B].[VariableId] = @cementType) AND 
                                          ([B].[OrganizationID] = @organizationId) AND 
				                          ([A].[StaticsCycle] = 'day') AND 
                                          ([A].[TimeStamp] >= @startTime) AND
                                          ([A].[TimeStamp] <= @endTime)
                                     ORDER BY [A].[TimeStamp]
                                ";

            SqlParameter[] parameters = new SqlParameter[]{
                new SqlParameter("organizationId", organizationId),
                new SqlParameter("startTime", startTime.ToString("yyyy-MM-dd")),
                new SqlParameter("endTime", endTime.ToString("yyyy-MM-dd")),
                new SqlParameter("cementType",cementType)
            };

            return dataFactory.Query(queryString, parameters);
        }

        /// <summary>
        /// 按照组织机构ID（生产线）获取按月的甲乙丙煤耗
        /// </summary>
        /// <param name="organiztionId">组织机构ID（生产线）</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns>峰谷平用电量</returns>
        public static DataTable GetCementCostFSTMonthlyByOrganiztionId(string organiztionId, DateTime startTime, DateTime endTime, string cementType)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);

            string queryString = @"SELECT [B].*, [A].[TimeStamp]
                                     FROM [balance_Energy] AS [B] INNER JOIN
                                          [tz_Balance] AS [A] ON [B].[KeyId] = [A].[BalanceId]
                                    WHERE ([B].[VariableId] =@cementType) AND 
                                          ([B].[OrganizationID] = @organizationId) AND 
				                          ([A].[StaticsCycle] = 'month') AND 
                                          ([A].[TimeStamp] >= @startTime) AND
                                          ([A].[TimeStamp] <= @endTime)
                                    ORDER BY [A].[TimeStamp]
                                ";

            SqlParameter[] parameters = new SqlParameter[]{
                new SqlParameter("organizationId", organiztionId),
                new SqlParameter("startTime", startTime),
                new SqlParameter("endTime", endTime),
                new SqlParameter("cementType",cementType)
            };

            return dataFactory.Query(queryString, parameters);
        }
    }
}
