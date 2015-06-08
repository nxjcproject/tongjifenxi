using SqlServerDataAdapter;
using StatisticalReport.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace StatisticalAnalysis.Service.HorizontallyAlarmAnalysis
{
    public static class HorizontallyEnergyAlarmAnalysisService
    {
        

        public static DataTable GetGridData(string[] levelCodes, string startTIme, string endTime,string labelLength)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string mySql = @"select h.Name,g.LevelCode,g.Count from
                            (select count(*) as Count,left(b.LevelCode,{1}) as LevelCode
                            from shift_EnergyConsumptionAlarmLog a,system_Organization b
                            where a.OrganizationID=b.OrganizationID
                            and ({0}) 
                            and a.StartTime>=@startTime and a.StartTime<=@endTime
                            group by left(b.LevelCode,{1}))as g,
                            (SELECT (CASE WHEN C.LevelType='Company' THEN C.Name ELSE (D.Name+C.Name) END) AS Name,C.OrganizationID,C.LevelCode
                            FROM
                            (select a.OrganizationID,a.LevelCode,a.Name,a.LevelType from system_Organization a) C
                            LEFT JOIN
                            (select b.OrganizationID,b.LevelCode,b.Name,b.LevelType from system_Organization b where b.LevelType='Company') D
                            ON CHARINDEX(D.LevelCode,C.LevelCode)>0
                            ) as h
                            where g.LevelCode=h.LevelCode";
            StringBuilder levelBuilder = new StringBuilder();
            foreach (string item in levelCodes)
            {
                levelBuilder.Append("CHARINDEX('");
                levelBuilder.Append(item.Trim());
                levelBuilder.Append("',b.LevelCode)>0");
                levelBuilder.Append(" or ");
            }
            levelBuilder.Remove(levelBuilder.Length - 4, 4);
            SqlParameter[] parameters = { new SqlParameter("startTime", startTIme), new SqlParameter("endTime", endTime) };
            DataTable table=dataFactory.Query(string.Format(mySql, levelBuilder.ToString(),labelLength), parameters);
            DataTable result = VerticallyToHorizontally.VerticallyToHorizontallyTable(table, "Name", "Count");
            return result;
        }
    }
}