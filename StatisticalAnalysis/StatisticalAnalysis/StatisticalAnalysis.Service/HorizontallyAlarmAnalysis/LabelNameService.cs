using SqlServerDataAdapter;
using StatisticalReport.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace StatisticalAnalysis.Service.HorizontallyAlarmAnalysis
{
    public class LabelNameService
    {
        public static string GetLableNames()
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string mySql = @"SELECT (CASE WHEN C.LevelType='Company' THEN C.Name ELSE (D.Name+C.Name) END) AS Name,C.OrganizationID,C.LevelCode
                                FROM
                                (select a.OrganizationID,a.LevelCode,a.Name,a.LevelType from system_Organization a) C
                                LEFT JOIN
                                (select b.OrganizationID,b.LevelCode,b.Name,b.LevelType from system_Organization b where b.LevelType='Company') D
                                ON CHARINDEX(D.LevelCode,C.LevelCode)>0
                                ORDER BY C.LevelCode";
            DataTable table = dataFactory.Query(mySql);
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("{");
            foreach (DataRow dr in table.Rows)
            {
                jsonBuilder.Append("\"");
                jsonBuilder.Append(dr["LevelCode"].ToString().Trim());
                jsonBuilder.Append("\":\"");
                jsonBuilder.Append(dr["Name"].ToString().Trim());
                jsonBuilder.Append("\",");
            }
            jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            jsonBuilder.Append("}");
            string json = jsonBuilder.ToString();
            return json;
        }
    }
}
