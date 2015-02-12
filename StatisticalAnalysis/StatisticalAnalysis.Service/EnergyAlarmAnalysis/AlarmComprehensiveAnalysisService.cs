using SqlServerDataAdapter;
using StatisticalAnalysis.Infrastruture.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace StatisticalAnalysis.Service.EnergyAlarmAnalysis
{
    public static class AlarmComprehensiveAnalysisService
    {
        /// <summary>
        /// 按照组织机构ID（分厂级及以上）获取报警记录
        /// </summary>
        /// <param name="organizationId">组织机构ID（分厂级及以上）</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns>报警记录表</returns>
        public static DataTable GetAlarmLogByOrganiztionId(string organizationId, DateTime startTime, DateTime endTime)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);

            string queryString = @"SELECT [C].*, [D].[Name] AS [ProductionLineName]
                                     FROM [shift_EnergyConsumptionAlarmLog] AS [C] INNER JOIN 
                                          [system_Organization] AS [D] ON [C].[OrganizationID] = [D].[OrganizationID]
                                    WHERE [C].[OrganizationID] IN (
		                                  SELECT [B].[OrganizationID]
		                                    FROM [system_Organization] AS [A], [system_Organization] AS [B] 
		                                   WHERE [A].[OrganizationID] = @organizationId 
                                             AND [B].[LevelCode] LIKE [A].[LevelCode] + '%'
                                         )
                                      AND [C].[StartTime] >= @startTime
                                      AND [C].[StartTime] <= @endTime
                                ";

            SqlParameter[] parameters = new SqlParameter[]{
                new SqlParameter("organizationId", organizationId),
                new SqlParameter("startTime", startTime),
                new SqlParameter("endTime", endTime)
            };

            return dataFactory.Query(queryString, parameters);
        }

        /// <summary>
        /// 按照组织机构ID（分厂级及以上）获取按组织机构统计的报警数量
        /// </summary>
        /// <param name="organizationId">组织机构ID（分厂级及以上）</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns>统计的报警数量表格</returns>
        public static DataTable GetAlarmCountGroupByOrganization(string organizationId, DateTime startTime, DateTime endTime)
        {
            DataTable alarmLogs = GetAlarmLogByOrganiztionId(organizationId, startTime, endTime);

            var organizations = from r in alarmLogs.AsEnumerable()
                                select new {
                                    OrganizationID = r.Field<string>("OrganizationID"),
                                    Name = r.Field<string>("ProductionLineName")
                                };

            var alarmCounts = from r in alarmLogs.AsEnumerable()
                              group r by r.Field<string>("OrganizationID")
                                  into g
                                  select new
                                  {
                                      OrganizationID = g.Key,
                                      AlarmCount = g.Count()
                                  };

            DataTable result = new DataTable();
            result.Columns.Add("OrganizationID", typeof(string));
            result.Columns.Add("Name", typeof(string));
            result.Columns.Add("Count", typeof(int));

            foreach (var alarmCount in alarmCounts)
            {
                DataRow dr = result.NewRow();
                dr["OrganizationID"] = alarmCount.OrganizationID;
                dr["Name"] = organizations.FirstOrDefault(o => o.OrganizationID == alarmCount.OrganizationID).Name;
                dr["Count"] = alarmCount.AlarmCount;

                result.Rows.Add(dr);
            }

            return result;
        }

        /// <summary>
        /// 按照组织机构ID（分厂级及以上）获取按报警类型分类的报警数量
        /// </summary>
        /// <param name="organizationId">组织机构ID（分厂级及以上）</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns>按报警类型分类的报警数量表格</returns>
        public static DataTable GetAlarmCountGroupByAlarmType(string organizationId, DateTime startTime, DateTime endTime)
        {
            DataTable alarmLogs = GetAlarmLogByOrganiztionId(organizationId, startTime, endTime);

            var organizations = from r in alarmLogs.AsEnumerable()
                                select new
                                {
                                    OrganizationID = r.Field<string>("OrganizationID"),
                                    Name = r.Field<string>("ProductionLineName")
                                };

            var alarmCounts = from r in alarmLogs.AsEnumerable()
                              group r by r.Field<string>("EnergyConsumptionType")
                                  into g
                                  select new
                                  {
                                      EnergyConsumptionType = g.Key,
                                      AlarmCount = g.Count()
                                  };

            DataTable result = new DataTable();
            result.Columns.Add("Name", typeof(string));
            result.Columns.Add("Count", typeof(int));

            foreach (var alarmCount in alarmCounts)
            {
                DataRow dr = result.NewRow();
                dr["Name"] = alarmCount.EnergyConsumptionType;
                dr["Count"] = alarmCount.AlarmCount;

                result.Rows.Add(dr);
            }

            return result;
        }
    }
}
