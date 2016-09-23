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
    public static class ElectricityPVFUsageAnalysisService
    {
        /// <summary>
        /// 获取峰谷平用电量（自定义，按日统计）
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static DataTable GetElectricityPVFUsageCustom(string organizationId, string organizationName, DateTime startTime, DateTime endTime, bool price = true)
        {
            DataTable pvfUsage = GetElectricityPVFDailyUsageByOrganiztionId(organizationId, startTime, endTime, organizationName);

            //if (price) GetPriceTable(organizationId, pvfUsage);

            return PVFAnalysisTableHelper.VerticalToHorizontal(pvfUsage, PVFAnalysisTableHelper.SourceType.Daily, startTime, endTime);
        }

        /// <summary>
        /// 获取峰谷平用电量（月查询，按日统计）
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public static DataTable GetElectricityPVFUsageMonthly(string organizationId, string organizationName, int year, int month, bool price = true)
        {
            DateTime startTime = DateTime.Parse(year + "-" + month + "-01 00:00:00");
            DateTime endTime = new DateTime(year, month, startTime.AddMonths(1).AddDays(-1).Day, 23, 59, 59);

            DataTable pvfUsage = GetElectricityPVFDailyUsageByOrganiztionId(organizationId, startTime, endTime, organizationName);

            //if (price) GetPriceTable(organizationId, pvfUsage);

            return PVFAnalysisTableHelper.VerticalToHorizontal(pvfUsage, PVFAnalysisTableHelper.SourceType.Monthly, startTime, endTime);
        }

        /// <summary>
        /// 获取峰谷平用电量（年查询，按月统计）
        /// </summary>
        /// <param name="organizationId">组织机构ID</param>
        /// <param name="year">需要统计的年份</param>
        /// <returns></returns>
        public static DataTable GetElectricityPVFUsageYearly(string organizationId, string organizationName, int year, bool price = true)
        {
            DateTime startTime = DateTime.Parse(year + "-01-01 00:00:00");
            DateTime endTime = DateTime.Parse(year + "-12-31 23:59:59");

            DataTable pvfUsage = GetElectricityPVFMonthlyUsageByOrganiztionId(organizationId, startTime, endTime, organizationName);

            //if (price) GetPriceTable(organizationId, pvfUsage);

            return PVFAnalysisTableHelper.VerticalToHorizontal(pvfUsage, PVFAnalysisTableHelper.SourceType.Yearly, DateTime.Parse(year + "-01-01"));
        }

        /// <summary>
        /// 按照组织机构ID（生产线）获取按日的峰谷平用电量
        /// </summary>
        /// <param name="organizationId">组织机构ID（生产线）</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns>峰谷平用电量</returns>
        internal static DataTable GetElectricityPVFDailyUsageByOrganiztionId(string organizationId, DateTime startTime, DateTime endTime, string organizationName)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            if (organizationName.IndexOf("分厂")>-1)
            {
                string queryString = @"select C.[KeyId],A.[TimeStamp],  
						            sum(C.[PeakB]) as [PeakB],
						            sum(C.[ValleyB]) as [ValleyB],
						            sum(C.[FlatB]) as [FlatB] 
						            from [balance_Energy] C	,[tz_Balance] A						
						            where C.[KeyId] = A.[BalanceId]
                                    and A.OrganizationID like @organizationId+'%'
						            AND A.[StaticsCycle] = 'day'	
						            AND A.[TimeStamp] >=@startTime
						            AND A.[TimeStamp] <=@endTime
						            AND (C.[VariableId]='clinker_ElectricityQuantity' OR C.[VariableId]='cementmill_ElectricityQuantity'
                                            or C.[VariableId]='wasteHeatElectricityGeneration_ElectricityQuantity')
											group by [KeyId],[TimeStamp],A.OrganizationID
                                ";
                SqlParameter[] parameters = new SqlParameter[]{
                new SqlParameter("organizationId", organizationId),
                new SqlParameter("startTime", startTime),
                new SqlParameter("endTime", endTime)
            };
                DataTable dt = dataFactory.Query(queryString, parameters);
                return dt;
            }
            else
            {
                string queryString = @"SELECT [C].*, [A].[TimeStamp]
                                        FROM [balance_Energy] AS [C] , 
                                            [tz_Balance] AS [A] 
                                        WHERE 
                                        [C].[KeyId] = [A].[BalanceId]
                                            AND([C].[OrganizationID] = @organizationId) 
	                                        AND ([A].[StaticsCycle] = 'day') 	
                                            AND([A].[TimeStamp] >= @startTime) 
                                            AND([A].[TimeStamp] <= @endTime)	
	                                        AND([C].[VariableId]='clinker_ElectricityQuantity' OR [C].[VariableId]='cementmill_ElectricityQuantity'
                                            or [C].[VariableId]='wasteHeatElectricityGeneration_ElectricityQuantity')
                                ";
                SqlParameter[] parameters = new SqlParameter[]{
                new SqlParameter("organizationId", organizationId),
                new SqlParameter("startTime", startTime),
                new SqlParameter("endTime", endTime)
            };
                DataTable dt = dataFactory.Query(queryString, parameters);
                return dt;
            }
        }

        /// <summary>
        /// 按照组织机构ID（生产线）获取按月的峰谷平用电量
        /// </summary>
        /// <param name="organizationId">组织机构ID（生产线）</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns>峰谷平用电量</returns>
        internal static DataTable GetElectricityPVFMonthlyUsageByOrganiztionId(string organizationId, DateTime startTime, DateTime endTime, string organizationName)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            if (organizationName.IndexOf("分厂") > -1)
            {
                string queryString = @"select C.[KeyId],A.[TimeStamp],  
						            sum(C.[PeakB]) as [PeakB],
						            sum(C.[ValleyB]) as [ValleyB],
						            sum(C.[FlatB]) as [FlatB] 
						            from [balance_Energy] C	,[tz_Balance] A						
						            where C.[KeyId] = A.[BalanceId]
                                    and A.OrganizationID like @organizationId+'%'
						            AND A.[StaticsCycle] = 'month'	
						            AND A.[TimeStamp] >=@startTime
						            AND A.[TimeStamp] <=@endTime
						            AND (C.[VariableId]='clinker_ElectricityQuantity' OR C.[VariableId]='cementmill_ElectricityQuantity'
                                            or C.[VariableId]='wasteHeatElectricityGeneration_ElectricityQuantity')
											group by [KeyId],[TimeStamp],A.OrganizationID
                                ";

                SqlParameter[] parameters = new SqlParameter[]{
                new SqlParameter("organizationId", organizationId),
                new SqlParameter("startTime", startTime.ToString("yyyy-MM")),
                new SqlParameter("endTime", endTime.ToString("yyyy-MM"))
            };

                return dataFactory.Query(queryString, parameters);
            }
            else
            {              
                    string queryString = @"SELECT [C].*, [A].[TimeStamp]
                                        FROM [balance_Energy] AS [C] , 
                                            [tz_Balance] AS [A] 
                                        WHERE 
                                        [C].[KeyId] = [A].[BalanceId]
                                            AND([C].[OrganizationID] = @organizationId) 
	                                        AND ([A].[StaticsCycle] = 'month') 	
                                            AND([A].[TimeStamp] >= @startTime) 
                                            AND([A].[TimeStamp] <= @endTime)	
	                                        AND([C].[VariableId]='clinker_ElectricityQuantity' OR [C].[VariableId]='cementmill_ElectricityQuantity'
                                            or [C].[VariableId]='wasteHeatElectricityGeneration_ElectricityQuantity')
                                ";
                    SqlParameter[] parameters = new SqlParameter[]{
                new SqlParameter("organizationId", organizationId),
                new SqlParameter("startTime", startTime.ToString("yyyy-MM")),
                new SqlParameter("endTime", endTime.ToString("yyyy-MM"))
                                };
                    DataTable dt = dataFactory.Query(queryString, parameters);
                    return dt; 
            }
        }

        /// <summary>
        /// 乘以电价
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="originalTable"></param>
        /// <returns></returns>
//        private static DataTable GetPriceTable(string organizationId, DataTable originalTable)
//        {
//            string connectionString = ConnectionStringFactory.NXJCConnectionString;
//            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);

//            string queryString = @"SELECT *
//                                     FROM [system_PeakValleyFlatElectrovalence]
//                                    WHERE [OrganizationID] = @organizationId
//                                ";

//            SqlParameter[] parameters = new SqlParameter[]{
//                new SqlParameter("organizationId", organizationId)
//            };

//            DataTable price = dataFactory.Query(queryString, parameters);

//            if (price.Rows.Count != 1)
//                throw new ArgumentException("未找到该生产线的电价");

//            double peakPrice = double.Parse(price.Rows[0]["PeakElectrovalence"].ToString());
//            double valleyPrice = double.Parse(price.Rows[0]["ValleyElectrovalence"].ToString());
//            double flatPrice = double.Parse(price.Rows[0]["FlatElectrovalence"].ToString());

//            foreach (DataRow dr in originalTable.Rows)
//            {
//                if (originalTable.Columns.Contains("PeakB"))
//                    dr["PeakB"] = decimal.Parse((double.Parse(dr["PeakB"].ToString()) * peakPrice).ToString("0.00"));

//                if (originalTable.Columns.Contains("ValleyB"))
//                    dr["ValleyB"] = decimal.Parse((double.Parse(dr["ValleyB"].ToString()) * peakPrice).ToString("0.00"));

//                if (originalTable.Columns.Contains("FlatB"))
//                    dr["FlatB"] = decimal.Parse((double.Parse(dr["FlatB"].ToString()) * peakPrice).ToString("0.00"));
//            }

//            return originalTable;
//        }
    }
}
