using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using SqlServerDataAdapter;
using StatisticalAnalysis.Infrastruture.Configuration;

namespace StatisticalAnalysis.Service.BasicHistoryTrend
{
    public class AmmetersHistoryTrendService
    {
        private readonly static string connectionString = ConnectionStringFactory.NXJCConnectionString;
        private readonly static ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);

        /// <summary>
        /// 获取所有分厂的数据库
        /// </summary>
        /// <param name="organizationId">组织机构ID（公司级别）</param>
        /// <returns></returns>
        private static DataTable GetDatabaseByOrganizationId(string organizationId)
        {
            string sql = @"SELECT [D].[MeterDatabase]
                             FROM [NXJC].[dbo].[system_Organization] AS [O] INNER JOIN
                                  [NXJC].[dbo].[system_Database] AS [D] ON [O].[DatabaseID] = [D].[DatabaseID]
                            WHERE [O].[LevelCode] LIKE (
                                  SELECT [L].[LevelCode]
	                                FROM [NXJC].[dbo].[system_Organization] AS [L]
		                           WHERE [L].[organizationId] = @organizationId
	                              ) + '%' AND
                                  [O].[LevelType] = 'Factory'";

            SqlParameter[] parameters = new SqlParameter[]{
                new SqlParameter("OrganizationId", organizationId)
            };

            return dataFactory.Query(sql, parameters);
        }

        /// <summary>
        /// 获取电表
        /// </summary>
        /// <param name="organizationId">组织机构ID（公司级别）</param>
        /// <returns></returns>
        public static DataTable GetAmmetersByOrganizationId(string organizationId)
        {
            DataTable factoryDatabases = GetDatabaseByOrganizationId(organizationId);

            StringBuilder queryBuilder = new StringBuilder();

            queryBuilder.Append(@"SELECT [O].[OrganizationID] AS [KeyId], [O].[OrganizationID] AS[OrganizationId], [O].[Name] AS [Name], '0' AS [ParentKeyId], '' AS [VariableId], 'Company' AS [LevelType], '' AS [TagTableName], '' AS [TagColumnName]
                                    FROM [NXJC].[dbo].[system_Organization] AS [O]
                                   WHERE [O].[OrganizationID] = @organizationId AND
                                         [O].[LevelType] = 'Company'

                                   UNION
                             
                                  SELECT [O].[OrganizationID] AS [KeyId], [O].[OrganizationID] AS[OrganizationId], [O].[Name] AS [Name], @organizationId AS [ParentKeyId], '' AS [VariableId], 'Factory' AS [LevelType], '' AS [TagTableName], '' AS [TagColumnName]
                                    FROM [NXJC].[dbo].[system_Organization] AS [O]
                                   WHERE [O].[LevelCode] LIKE (
                                         SELECT [L].[LevelCode]
	                                       FROM [NXJC].[dbo].[system_Organization] AS [L]
		                                  WHERE [L].[OrganizationID] = @organizationId
	                                     ) + '%' AND
                                         [O].[LevelType] = 'Factory'
                                  ");

            foreach (DataRow factoryDatabase in factoryDatabases.Rows)
            {
                queryBuilder.Append("  UNION ");
                queryBuilder.Append(@"SELECT DISTINCT [AC].[ElectricRoom] AS [KeyId], [AC].[OrganizationID] AS[OrganizationId], [AC].[ElectricRoom] AS [Name], [AC].[OrganizationID] AS [ParentKeyId], '' AS [VariableId], 'ElectricRoom' AS [LevelType], '' AS [TagTableName], '' AS [TagColumnName]
                                        FROM [" + factoryDatabase[0].ToString().Trim() + @"].[dbo].[AmmeterContrast] AS [AC]
                                  
                                       UNION
                                  
                                      SELECT CAST(NEWID() AS VARCHAR(64)) AS [KeyId], [AC].[OrganizationID] AS[OrganizationId], RTRIM([AC].[AmmeterName]) AS [Name], [AC].[ElectricRoom] AS [ParentKeyId], RTRIM([AC].[AmmeterName]) AS [VariableId], 'Ammeters' AS [LevelType], '" + factoryDatabase[0].ToString().Trim() + @".dbo.HistoryAmmeter' AS [TagTableName], RTRIM([AC].[ElectricEnergyFieldNameSave]) AS [TagColumnName]
                                        FROM [" + factoryDatabase[0].ToString().Trim() + @"].[dbo].[AmmeterContrast] AS [AC]
                                    ");
            }


            SqlParameter[] parameters = new SqlParameter[]{
                new SqlParameter("organizationId", organizationId)
            };

            return dataFactory.Query(queryBuilder.ToString(), parameters);
        }


        public static DataTable GetQuantityUsageGroup(DataTable tagTable, DateTime startTime, DateTime endTime, string myTimeInterval)
        {
            string queryString = @"SELECT {5}, '{0}' as OrganizationId, '{1}' as VariableId, SUM({2}) AS [Sum]
                                     FROM {3}
                                    WHERE [vDate] >= @startTime
                                      AND [vDate] <= @endTime
                                 GROUP BY {4}";
            string m_TimeInterval = "";
            string m_vDateColumn = "";
            if (myTimeInterval == "10")             //按10分钟
            {
                m_vDateColumn = "YEAR([vDate]) as [Year], MONTH([vDate]) as [Month], DAY([vDate]) as [Day], DATEPART(HOUR, [vDate]) as [Hour], (DATEPART(mi,[vDate]) / 10) * 10 as [Minute]";
                m_TimeInterval = "YEAR([vDate]), MONTH([vDate]), DAY([vDate]), DATEPART(HOUR, [vDate]), (DATEPART(mi,[vDate]) / 10) * 10";
            }
            else if (myTimeInterval == "20")         //按1小时
            {
                m_vDateColumn = "YEAR([vDate]) as [Year], MONTH([vDate]) as [Month], DAY([vDate]) as [Day], DATEPART(HOUR, [vDate]) as [Hour]";
                m_TimeInterval = "YEAR([vDate]), MONTH([vDate]), DAY([vDate]), DATEPART(HOUR, [vDate])";
            }
            else if (myTimeInterval == "30")        //按天
            {
                m_vDateColumn = "YEAR([vDate]) as [Year], MONTH([vDate]) as [Month], DAY([vDate]) as [Day]";
                m_TimeInterval = "YEAR([vDate]), MONTH([vDate]), DAY([vDate])";
            }
            StringBuilder queryBuilder = new StringBuilder();

            foreach (DataRow dr in tagTable.Rows)
            {
                queryBuilder.Append(string.Format(queryString, dr["OrganizationId"], dr["VariableId"], dr["TagColumnName"], dr["TagTableName"], m_TimeInterval, m_vDateColumn));
                queryBuilder.Append(" UNION ");
            }

            queryBuilder.Remove(queryBuilder.Length - 7, 7);

            SqlParameter[] parameters = new SqlParameter[]{
                new SqlParameter("startTime", startTime),
                new SqlParameter("endTime", endTime)
            };
            string m_Sql = queryBuilder.ToString();

            DataTable dt = dataFactory.Query(m_Sql, parameters);
            return dt;
        }
    }
}
