using SqlServerDataAdapter;
using StatisticalAnalysis.Infrastruture.Configuration;
using StatisticalAnalysis.Infrastruture.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace StatisticalAnalysis.Service.ElectricityCostAnalysis
{
    public static class ElectricityConsumptionAnalysisService
    {

        /// <summary>
        /// 按照生产线组织机构ID（生产线）获取所有的电耗变量标签
        /// </summary>
        /// <param name="organizationId">组织机构ID（生产线）</param>
        /// <returns></returns>
        public static DataTable GetElectricityConsumptionVariableByOrganizationId(string organizationId)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);

            string queryString = @"SELECT [T].[VariableId] AS [id], [T].[VariableName] AS [text]
                                     FROM [system_Organization] AS [O] INNER JOIN
                                          [balance_Energy_Template] AS [T] ON [O].[Type] = [T].[ProductionLineType]
                                    WHERE ([O].[OrganizationID] = @organizationId) AND ([T].[ValueType] = 'ElectricityConsumption')
                                ";

            SqlParameter[] parameters = new SqlParameter[]{
                new SqlParameter("organizationId", organizationId)
            };

            return dataFactory.Query(queryString, parameters);
        }

        /// <summary>
        /// 获取甲乙丙电耗（自定义，按日统计）
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="variableId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static DataTable GetElectricityPVFUsageCustom(string organizationId, string variableId, DateTime startTime, DateTime endTime)
        {
            DataTable pvfUsage = GetElectricityConsumptionFSTDailyByOrganiztionId(organizationId, variableId, startTime, endTime);

            return FSTAnalysisTableHelper.VerticalToHorizontal(pvfUsage, FSTAnalysisTableHelper.SourceType.Daily, startTime, endTime);
        }

        /// <summary>
        /// 获取甲乙丙电耗（月查询，按日统计）
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="variableId"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public static DataTable GetElectricityPVFUsageMonthly(string organizationId, string variableId, int year, int month)
        {
            DateTime startTime = DateTime.Parse(year + "-" + month + "-01 00:00:00");
            DateTime endTime = new DateTime(year, month, startTime.AddMonths(1).AddDays(-1).Day, 23, 59, 59);

            DataTable pvfUsage = GetElectricityConsumptionFSTDailyByOrganiztionId(organizationId, variableId, startTime, endTime);

            return FSTAnalysisTableHelper.VerticalToHorizontal(pvfUsage, FSTAnalysisTableHelper.SourceType.Monthly, startTime, endTime);
        }

        /// <summary>
        /// 获取甲乙丙电耗（年查询，按月统计）
        /// </summary>
        /// <param name="organizationId">组织机构ID</param>
        /// <param name="variableId"></param>
        /// <param name="year">需要统计的年份</param>
        /// <returns></returns>
        public static DataTable GetElectricityPVFUsageYearly(string organizationId, string variableId, int year)
        {
            DateTime startTime = DateTime.Parse(year + "-01-01 00:00:00");
            DateTime endTime = DateTime.Parse(year + "-12-31 23:59:59");

            DataTable pvfUsage = GetElectricityConsumptionFSTMonthlyByOrganiztionId(organizationId, variableId, startTime, endTime);

            return FSTAnalysisTableHelper.VerticalToHorizontal(pvfUsage, FSTAnalysisTableHelper.SourceType.Yearly, DateTime.Parse(year + "-01-01"));
        }

        /// <summary>
        /// 按照组织机构ID（生产线）获取按日的甲乙丙电耗
        /// </summary>
        /// <param name="organizationId">组织机构ID（生产线）</param>
        /// <param name="variableId">变量ID</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns>峰谷平用电量</returns>
        public static DataTable GetElectricityConsumptionFSTDailyByOrganiztionId(string organizationId, string variableId, DateTime startTime, DateTime endTime)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);

            string queryString = @"SELECT [B].*, [A].[TimeStamp]
                                     FROM [balance_Energy] AS [B] INNER JOIN
                                          [tz_Balance] AS [A] ON [B].[KeyId] = [A].[BalanceId]
                                    WHERE ([B].[VariableId] = @variableId) AND 
                                          ([B].[OrganizationID] = @organizationId) AND 
				                          ([A].[StaticsCycle] = 'day') AND 
                                          ([A].[TimeStamp] >= @startTime) AND
                                          ([A].[TimeStamp] <= @endTime)
                                ";

            SqlParameter[] parameters = new SqlParameter[]{
                new SqlParameter("organizationId", organizationId),
                new SqlParameter("variableId", variableId),
                new SqlParameter("startTime", startTime),
                new SqlParameter("endTime", endTime)
            };

            return dataFactory.Query(queryString, parameters);
        }

        /// <summary>
        /// 按照组织机构ID（生产线）获取按月的甲乙丙电耗
        /// </summary>
        /// <param name="organiztionId">组织机构ID（生产线）</param>
        /// <param name="variableId">变量ID</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns>峰谷平用电量</returns>
        public static DataTable GetElectricityConsumptionFSTMonthlyByOrganiztionId(string organiztionId, string variableId, DateTime startTime, DateTime endTime)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);

            string queryString = @"SELECT [B].*, [A].[TimeStamp]
                                     FROM [balance_Energy] AS [B] INNER JOIN
                                          [tz_Balance] AS [A] ON [B].[KeyId] = [A].[BalanceId]
                                    WHERE ([B].[VariableId] = @variableId) AND 
                                          ([B].[OrganizationID] = @organizationId) AND 
				                          ([A].[StaticsCycle] = 'month') AND 
                                          ([A].[TimeStamp] >= @startTime) AND
                                          ([A].[TimeStamp] <= @endTime)
                                ";

            SqlParameter[] parameters = new SqlParameter[]{
                new SqlParameter("organizationId", organiztionId),
                new SqlParameter("variableId", variableId),
                new SqlParameter("startTime", startTime.ToString("yyyy-MM")),
                new SqlParameter("endTime", endTime.ToString("yyyy-MM"))
            };

            return dataFactory.Query(queryString, parameters);
        }
    }
}
