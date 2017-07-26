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
                                             AND [B].[LevelCode] LIKE [A].[LevelCode] + '%')
                                      AND [C].[HaltTime] >= @startTime
                                      AND [C].[HaltTime] <= @endTime";
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
            result.DefaultView.Sort = "Count desc";
            result = result.DefaultView.ToTable();
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
//            string mySql = @"select A.OrganizationID,A.Label,B.Name as ProductLineName,A.EquipmentName,'停机报警' EnergyConsumptionType,count(1) as [Count]
//                                from shift_MachineHaltLog A,system_Organization B,
//                                (select LevelCode from system_Organization where OrganizationID=@organizationId) C
//                                where A.OrganizationID=B.OrganizationID
//                                and B.LevelCode like C.LevelCode+'%'
//                                and A.HaltTime>=@startTime
//                                and A.HaltTime<=@endTime
//                                group by A.OrganizationID,A.Label,B.Name,A.EquipmentName
//                                order by EnergyConsumptionType,A.OrganizationID";
            string mySql = @"select A.OrganizationID,C.Name,B.MasterEquipmentName,B.MasterLabel,B.EquipmentName as SlaveEquipmentName,B.Label as SlaveLabel,
                                    '从机延时停机' as Type,count(1) as [Count]
                                    from [dbo].[shift_MachineHaltLog] A,[dbo].[shift_SlaverHaltDelayAlarmLog] B,system_Organization C,
                                    (select LevelCode from system_Organization where OrganizationID=@organizationId) D
                                    where A.MachineHaltLogID=B.KeyID
                                    and A.OrganizationID=C.OrganizationID
                                    and C.LevelCode like D.LevelCode+'%'
                                    and B.MasterHaltTime>=@startTime
                                    and B.MasterHaltTime<=@endTime
                                    group by A.OrganizationID,C.Name,B.MasterEquipmentName,B.MasterLabel,B.EquipmentName,B.Label
                                    union all
                                    select A.OrganizationID,B.Name,A.EquipmentName as MasterEquipmentName,A.Label as MasterLabel,'无' as SlaveEquipmentName,
                                    '无' as SlaveLabel,'主机停机' as Type,count(1) as [Count]
                                    from [dbo].[shift_MachineHaltLog] A,system_Organization B,
                                    (select LevelCode from system_Organization where OrganizationID=@organizationId) C
                                    where A.OrganizationID=B.OrganizationID
                                    and B.LevelCode like C.LevelCode+'%'
                                    and A.HaltTime>=@startTime
                                    and A.HaltTime<=@endTime
                                    group by A.OrganizationID,B.Name,A.EquipmentName,A.Label";
            SqlParameter[] parameters = { new SqlParameter("organizationId", organizationId), 
                                            new SqlParameter("startTime", startTime), new SqlParameter("endTime", endTime) };
            DataTable originalTable= dataFactory.Query(mySql, parameters);
            DataColumn levelColumn = new DataColumn("LevelCode",typeof(string));
            originalTable.Columns.Add(levelColumn);
            DataRow[] masterRows = originalTable.Select("Type='主机停机'");
            int length = masterRows.Count();
            for(int i=0;i<length;i++)
            {
                DataRow dr = masterRows[i];
                dr["LevelCode"] = "M01" + (i+1).ToString("00");
                string masterName=dr["MasterEquipmentName"].ToString().Trim();
                string myOrganizationId = dr["OrganizationID"].ToString().Trim();
                DataRow[] slaveRows = originalTable.Select("MasterEquipmentName='" + masterName + "' and SlaveEquipmentName<>'无' and OrganizationID='" + myOrganizationId + "'");
                int slaveLength = slaveRows.Count();
                for (int j = 0; j < slaveLength; j++)
                {
                    slaveRows[j]["LevelCode"] = "M01" + (i+1).ToString("00") + (j+1).ToString("00");
                }
            }
            DataColumn labelColumn = new DataColumn("Label",typeof(string));
            DataColumn equipmentColumn = new DataColumn("EquipmentName", typeof(string));
            DataColumn stateColumn = new DataColumn("state",typeof(string));
            originalTable.Columns.Add(labelColumn);
            originalTable.Columns.Add(equipmentColumn);
            originalTable.Columns.Add(stateColumn);
            //DataRow factoryRow = originalTable.NewRow();
            //factoryRow["Name"] = "分厂";
            //factoryRow["LevelCode"] = "M01";
            foreach (DataRow dr in originalTable.Rows)
            {
                if (dr["SlaveLabel"].ToString() == "无")//主机设备停机
                {
                    dr["Label"] = dr["MasterLabel"];
                    dr["EquipmentName"] = dr["MasterEquipmentName"];
                    dr["state"] = "closed";
                }
                else//从机报警
                {
                    dr["Label"] = dr["SlaveLabel"];
                    dr["EquipmentName"] = dr["SlaveEquipmentName"];
                    dr["state"] = "open";
                }
            }
            return originalTable;
        }
    }
}
