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
        private const string MainMachine = "MainMachine";
        public static DataTable GetElectricityUsageGroupByHour(DataTable tagTable, DateTime startTime, DateTime endTime)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);

            string queryStringProductionOrProcess = @"SELECT [OrganizationID], [VariableId], YEAR([vDate]) AS [Year], MONTH([vDate]) AS [Month], DAY([vDate]) AS [Day], DATEPART(HOUR, [vDate]) AS [Hour], SUM([FormulaValue]) AS [Sum]
                                     FROM [{2}].[dbo].[HistoryFormulaValue]
                                    WHERE [OrganizationID] = '{0}'
                                      AND [VariableId] = '{1}'
                                      AND [vDate] >= @startTime
                                      AND [vDate] <= @endTime
                                 GROUP BY [OrganizationID], [VariableId], YEAR([vDate]), MONTH([vDate]), DAY([vDate]), DATEPART(HOUR, [vDate])
";
            string queryStringMainMachine = @"SELECT [OrganizationID], [VariableId], YEAR([vDate]) AS [Year], MONTH([vDate]) AS [Month], DAY([vDate]) AS [Day], DATEPART(HOUR, [vDate]) AS [Hour], SUM([FormulaValue]) AS [Sum]
                                     FROM [{2}].[dbo].[HistoryMainMachineFormulaValue]
                                    WHERE [OrganizationID] = '{0}'
                                      AND [VariableId] = '{1}'
                                      AND [vDate] >= @startTime
                                      AND [vDate] <= @endTime
                                 GROUP BY [OrganizationID], [VariableId], YEAR([vDate]), MONTH([vDate]), DAY([vDate]), DATEPART(HOUR, [vDate])
";
            StringBuilder queryBuilder = new StringBuilder();

            foreach (DataRow dr in tagTable.Rows)
            {
                if (dr["LevelType"].ToString().Replace(" ", "") == "MainMachine")
                {
                    queryBuilder.Append(string.Format(queryStringMainMachine, dr["OrganizationID"], dr["VariableId"], ConnectionStringFactory.GetAmmeterDatabaseName(dr["OrganizationID"].ToString())));
                    queryBuilder.Append(" UNION ");
                }
                else
                {
                    queryBuilder.Append(string.Format(queryStringProductionOrProcess, dr["OrganizationID"], dr["VariableId"], ConnectionStringFactory.GetAmmeterDatabaseName(dr["OrganizationID"].ToString())));
                    queryBuilder.Append(" UNION ");
                }
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
