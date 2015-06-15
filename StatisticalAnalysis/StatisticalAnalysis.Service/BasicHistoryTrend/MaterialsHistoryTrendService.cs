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
    public class MaterialsHistoryTrendService
    {
        private readonly static string connectionString = ConnectionStringFactory.NXJCConnectionString;
        private readonly static ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
        public static DataTable GetMaterialsByOrganizationID(string myOrganizationId)
        {
            string queryString = @"select F.* from (
                                    select A.KeyID as KeyId,A.Name as Name, C.Type as ProductionType, A.OrganizationID as OrganizationId, C.LevelType as LevelType, '' as VariableId, '0' as ParentKeyId, D.MeterDatabase as TagTableName, '' as TagColumnName
                                    from tz_Material A, system_Organization B, system_Organization C, system_Database D
                                    where A.Type = 2
                                    and A.Enable = 1
                                    and A.State = 0
                                    and A.OrganizationID in (C.OrganizationID)
                                    and B.OrganizationID = @OrganizationId
                                    and C.LevelCode like B.LevelCode + '%'
                                    and C.DatabaseID = D.DatabaseID
                                    union 
                                    select D.MaterialId as KeyId, A.Name + D.Name as Name, C.Type as ProductionType, A.OrganizationID as OrganizationId, 'Materials' as LevelType, D.VariableId as VariableId, cast(D.KeyID as varchar(64)) as ParentKeyId, E.MeterDatabase + '.dbo.' + D.TagTableName as TagTableName, D.Formula as TagColumnName 
                                    from tz_Material A, system_Organization B, system_Organization C, material_MaterialDetail D, system_Database E
                                    where A.Type = 2
                                    and A.Enable = 1
                                    and A.State = 0
                                    and A.OrganizationID in (C.OrganizationID)
                                    and B.OrganizationID =  @OrganizationId
                                    and C.LevelCode like B.LevelCode + '%'
                                    and A.KeyID = D.KeyID
                                    and C.DatabaseID = E.DatabaseID
                                    ) F
                                    order by F.ProductionType, F.Name";

            SqlParameter[] parameters = new SqlParameter[]{
                new SqlParameter("OrganizationId", myOrganizationId)
            };

            return dataFactory.Query(queryString, parameters);
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
            return dataFactory.Query(m_Sql, parameters);
        }
    }
}
