using SqlServerDataAdapter;
using StatisticalAnalysis.Infrastruture.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace StatisticalAnalysis.Service.DowntimeAnalysis
{
    public static class DowntimeComprehensiveAnalysisService
    {
        /// <summary>
        /// 按照组织机构ID（分厂级及以上）获取停机记录
        /// </summary>
        /// <param name="organiztionId">组织机构ID（分厂级及以上）</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns>报警记录表</returns>
        public static DataTable GetDowntimeLogByOrganiztionId(string organiztionId, DateTime startTime, DateTime endTime)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);

            string queryString = @"SELECT [C].*, [D].[Name] AS [ProductionLineName]
                                     FROM [shift_MachineHaltLog] AS [C] INNER JOIN 
                                          [system_Organization] AS [D] ON [C].[OrganizationID] = [D].[OrganizationID]
                                    WHERE [C].[OrganizationID] IN (
		                                  SELECT [B].[OrganizationID]
		                                    FROM [system_Organization] AS [A], [system_Organization] AS [B] 
		                                   WHERE [A].[OrganizationID] = @organizationId 
                                             AND [B].[LevelCode] LIKE [A].[LevelCode] + '%'
                                         )
                                      AND [C].[HaltTime] >= @startTime
                                      AND [C].[HaltTime] <= @endTime
                                ";

            SqlParameter[] parameters = new SqlParameter[]{
                new SqlParameter("organizationId", organiztionId),
                new SqlParameter("startTime", startTime),
                new SqlParameter("endTime", endTime)
            };

            return dataFactory.Query(queryString, parameters);
        }

        /// <summary>
        /// 按照组织机构ID（分厂级及以上）获取按组织机构统计的停机次数
        /// </summary>
        /// <param name="organizationId">组织机构ID（分厂级及以上）</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns>统计的报警数量表格</returns>
        public static DataTable GetDowntimeCountGroupByOrganization(string organizationId, DateTime startTime, DateTime endTime)
        {
            DataTable downtimeLogs = GetDowntimeLogByOrganiztionId(organizationId, startTime, endTime);

            var organizations = from r in downtimeLogs.AsEnumerable()
                                select new
                                {
                                    OrganizationID = r.Field<string>("OrganizationID"),
                                    Name = r.Field<string>("ProductionLineName")
                                };

            var downtimeCounts = from r in downtimeLogs.AsEnumerable()
                              group r by r.Field<string>("OrganizationID")
                                  into g
                                  select new
                                  {
                                      OrganizationID = g.Key,
                                      DowntimeCount = g.Count()
                                  };

            DataTable result = new DataTable();
            result.Columns.Add("OrganizationID", typeof(string));
            result.Columns.Add("Name", typeof(string));
            result.Columns.Add("Count", typeof(int));

            foreach (var downtimeCount in downtimeCounts)
            {
                DataRow dr = result.NewRow();
                dr["OrganizationID"] = downtimeCount.OrganizationID;
                dr["Name"] = organizations.FirstOrDefault(o => o.OrganizationID == downtimeCount.OrganizationID).Name;
                dr["Count"] = downtimeCount.DowntimeCount;

                result.Rows.Add(dr);
            }

            return result;
        }

        /// <summary>
        /// 按照组织机构ID（分厂级及以上）获取按停机原因分类的停机次数
        /// </summary>
        /// <param name="organizationId">组织机构ID（分厂级及以上）</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns>按报警类型分类的报警数量表格</returns>
        public static DataTable GetDowntimeCountGroupByAlarmType(string organizationId, DateTime startTime, DateTime endTime)
        {
            DataTable downtimeLogs = GetDowntimeLogByOrganiztionId(organizationId, startTime, endTime);

            var organizations = from r in downtimeLogs.AsEnumerable()
                                select new
                                {
                                    OrganizationID = r.Field<string>("OrganizationID"),
                                    Name = r.Field<string>("ProductionLineName")
                                };

            var downtimeCounts = from r in downtimeLogs.AsEnumerable()
                              group r by r.Field<string>("ReasonText")
                                  into g
                                  select new
                                  {
                                      ReasonText = g.Key,
                                      DowntimeCount = g.Count()
                                  };

            DataTable result = new DataTable();
            result.Columns.Add("Name", typeof(string));
            result.Columns.Add("Count", typeof(int));

            foreach (var downtimeCount in downtimeCounts)
            {
                DataRow dr = result.NewRow();
                dr["Name"] = downtimeCount.ReasonText;
                dr["Count"] = downtimeCount.DowntimeCount;

                result.Rows.Add(dr);
            }

            return result;
        }
        /// <summary>
        /// 获得报警详细信息
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static DataTable GetAlarmDetailInfo(string organizationId, DateTime startTime, DateTime endTime)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            string mySql = @"select A.OrganizationID,A.Label,B.Name as ProductLineName,A.EquipmentName,'停机报警' EnergyConsumptionType,count(1) as [Count]
                                from shift_MachineHaltLog A,system_Organization B,
                                (select LevelCode from system_Organization where OrganizationID=@organizationId) C
                                where A.OrganizationID=B.OrganizationID
                                and B.LevelCode like C.LevelCode+'%'
                                and A.HaltTime>=@startTime
                                and A.HaltTime<=@endTime
                                group by A.OrganizationID,A.Label,B.Name,A.EquipmentName
                                order by EnergyConsumptionType,A.OrganizationID";
            SqlParameter[] parameters = { new SqlParameter("organizationId", organizationId), 
                                            new SqlParameter("startTime", startTime), new SqlParameter("endTime", endTime) };
            return dataFactory.Query(mySql, parameters);
        }
    }
}
