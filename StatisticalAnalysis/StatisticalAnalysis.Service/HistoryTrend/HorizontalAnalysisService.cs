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
    public static class HorizontalAnalysisService
    {
        public static DataTable GetElectricityUsageGroupByHour(DataTable tagTable, DateTime startTime, DateTime endTime)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);

            string queryString = @"SELECT [OrganizationID], [LevelCode], YEAR([vDate]) AS [Year], MONTH([vDate]) AS [Month], DAY([vDate]) AS [Day], DATEPART(HOUR, [vDate]) AS [Hour], SUM([FormulaValue]) AS [Sum]
                                     FROM [{2}].[dbo].[HistoyFormulaValue]
                                    WHERE [OrganizationID] = '{0}'
                                      AND [LevelCode] = '{1}'
                                      AND [vDate] >= @startTime
                                      AND [vDate] <= @endTime
                                 GROUP BY [OrganizationID], [LevelCode], YEAR([vDate]), MONTH([vDate]), DAY([vDate]), DATEPART(HOUR, [vDate])
                                ";

            StringBuilder queryBuilder = new StringBuilder();

            foreach (DataRow dr in tagTable.Rows)
            {
                queryBuilder.Append(string.Format(queryString, dr["OrganizationID"], dr["LevelCode"], ConnectionStringFactory.GetAmmeterDatabaseName(dr["OrganizationID"].ToString())));
                queryBuilder.Append(" UNION ");
            }

            queryBuilder.Remove(queryBuilder.Length - 8, 7);

            SqlParameter[] parameters = new SqlParameter[]{
                new SqlParameter("startTime", startTime),
                new SqlParameter("endTime", endTime)
            };

            return dataFactory.Query(queryBuilder.ToString(), parameters);
        }
    }

}
