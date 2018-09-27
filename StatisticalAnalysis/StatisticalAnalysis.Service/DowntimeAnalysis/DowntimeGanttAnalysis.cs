using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SqlServerDataAdapter;
using StatisticalAnalysis.Infrastruture.Configuration;
using StatisticalAnalysis.Infrastruture.Utility;
namespace StatisticalAnalysis.Service.DowntimeAnalysis
{
    public class DowntimeGanttAnalysis
    {
        private static readonly ISqlServerDataFactory _dataFactory = new SqlServerDataFactory(ConnectionStringFactory.NXJCConnectionString);
        public static DataTable GetEquipmentInfo(string myOrganizationId)
        {
            string m_Sql = @"select A.EquipmentId as id,
                                A.EquipmentName as text
                                from equipment_EquipmentDetail A, equipment_EquipmentCommonInfo B, system_MasterMachineDescription C, system_Organization D, system_Organization E
                                where E.OrganizationID = '{0}'
                                and D.LevelCode like E.LevelCode + '%'
                                and A.OrganizationId = D.OrganizationID
                                and A.EquipmentCommonId = B.EquipmentCommonId
                                and A.EquipmentId = C.ID
                                order by B.DisplayIndex, A.EquipmentName";

            try
            {
                m_Sql = string.Format(m_Sql, myOrganizationId);
                DataTable m_EquipmentInfoTable = _dataFactory.Query(m_Sql);
                return m_EquipmentInfoTable;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static DataTable GetResonType(string myReasonTypeId)
        {
            string m_Sql = @"Select A.ReasonStatisticsTypeId as id,
                                A.Name as text
                                from 
                                system_MachineHaltReasonStatisticsType A
                                where A.Type = '{0}'";
            try
            {
                m_Sql = string.Format(m_Sql, myReasonTypeId);
                DataTable m_ReasonTypeTable = _dataFactory.Query(m_Sql);
                return m_ReasonTypeTable;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static DataTable GetHaltReasonStaticsGanttChart(string myStartTime, string myEndTime, string myEquipmentId, string myHaltTypeId, string myReasonTypeId, string myOrganizationId)
        {
            string m_Sql = @"Select M.MachineHaltLogID, M.OrganizationID, M.HaltTimeF as HaltTime, M.RecoverTimeF as RecoverTime, M.EquipmentID, M.ReasonStatisticsTypeId, M.ReasonID, N.Name from 
                                (Select C.MachineHaltLogID, D.OrganizationID, C.EquipmentID, ltrim(rtrim(C.ReasonID)) as ReasonID
					                                ,F.ReasonStatisticsTypeId,
                                                    (case when C.HaltTime is null or C.HaltTime < '{0} 00:00:00' then '{0} 00:00:00'
                                                            when C.HaltTime > '{1} 23:59:59' then '{1} 23:59:59'
							                                else C.HaltTime end) as HaltTimeF,
                                                    (case when C.RecoverTime is null or C.RecoverTime > '{1} 23:59:59' then '{1} 23:59:59'
                                                            when  C.RecoverTime < '{0} 00:00:00' then '{0} 00:00:00'
                                                            else C.RecoverTime end)  as RecoverTimeF 
			                                from shift_MachineHaltLog C
						                                left join system_MachineHaltReason F on C.ReasonID = F.MachineHaltReasonID, 
						                                system_Organization D, system_Organization E, system_Organization J, system_Organization K
					                                where ((C.HaltTime >= '{0} 00:00:00' and C.HaltTime <= '{1} 23:59:59')
						                                or (C.RecoverTime >= '{0} 00:00:00' and C.RecoverTime <= '{1} 23:59:59')
                                                        or (C.HaltTime < '{0} 00:00:00' and C.RecoverTime > '{1} 23:59:59')
                                                        or (C.HaltTime <= '{1} 23:59:59' and C.RecoverTime is null)
										                or (C.RecoverTime >= '{0} 00:00:00' and C.HaltTime is null))
					                                and K.OrganizationID = '{2}'
                                                    and J.LevelCode like K.LevelCode + '%'
                                                    and J.LevelType = 'Factory' 
					                                and D.OrganizationID = J.OrganizationID
					                                and E.LevelCode like D.LevelCode + '%'
					                                and C.OrganizationID = E.OrganizationID
                                                    and CONVERT(varchar(64), C.EquipmentID) = '{3}') M, system_MachineHaltReasonStatisticsType N
                                            where M.ReasonStatisticsTypeId = N.ReasonStatisticsTypeId
                                                and N.Type = '{4}'
                                                {5}
                                            order by M.OrganizationID, M.EquipmentID, N.LevelCode, M.ReasonID, M.HaltTimeF";
            string m_ReasonTypeCondition = " and M.ReasonStatisticsTypeId = '{0}'";
            if (myReasonTypeId == "All")
            {
                m_ReasonTypeCondition = "";
            }
            else
            {
                m_ReasonTypeCondition = string.Format(m_ReasonTypeCondition, myReasonTypeId);
            }
            try
            {
                m_Sql = string.Format(m_Sql, myStartTime, myEndTime, myOrganizationId, myEquipmentId, myHaltTypeId, m_ReasonTypeCondition);
                DataTable m_HaltLogTable = _dataFactory.Query(m_Sql);
                return m_HaltLogTable;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static DataTable GetHaltReasonStaticsGanttChartNoReason(string myStartTime, string myEndTime, string myEquipmentId, string myOrganizationId)
        {
            string m_Sql = @"Select M.MachineHaltLogID, M.OrganizationID, M.HaltTimeF as HaltTime, M.RecoverTimeF as RecoverTime, M.EquipmentID, M.ReasonStatisticsTypeId, M.ReasonID, '无故障原因' as Name from 
                                (Select C.MachineHaltLogID, D.OrganizationID, C.EquipmentID, ltrim(rtrim(C.ReasonID)) as ReasonID
					                                ,F.ReasonStatisticsTypeId,
                                                    (case when C.HaltTime is null or C.HaltTime < '{0} 00:00:00' then '{0} 00:00:00'
                                                            when C.HaltTime > '{1} 23:59:59' then '{1} 23:59:59'
							                                else C.HaltTime end) as HaltTimeF,
                                                    (case when C.RecoverTime is null or C.RecoverTime > '{1} 23:59:59' then '{1} 23:59:59'
                                                            when  C.RecoverTime < '{0} 00:00:00' then '{0} 00:00:00'
                                                            else C.RecoverTime end)  as RecoverTimeF 
			                                from shift_MachineHaltLog C
						                                left join system_MachineHaltReason F on C.ReasonID = F.MachineHaltReasonID, 
						                                system_Organization D, system_Organization E, system_Organization J, system_Organization K
					                                where ((C.HaltTime >= '{0} 00:00:00' and C.HaltTime <= '{1} 23:59:59')
						                                or (C.RecoverTime >= '{0} 00:00:00' and C.RecoverTime <= '{1} 23:59:59')
                                                        or (C.HaltTime < '{0} 00:00:00' and C.RecoverTime > '{1} 23:59:59')
                                                        or (C.HaltTime <= '{1} 23:59:59' and C.RecoverTime is null)
										                or (C.RecoverTime >= '{0} 00:00:00' and C.HaltTime is null))
					                                and K.OrganizationID = '{2}'
                                                    and J.LevelCode like K.LevelCode + '%'
                                                    and J.LevelType = 'Factory' 
					                                and D.OrganizationID = J.OrganizationID
					                                and E.LevelCode like D.LevelCode + '%'
					                                and C.OrganizationID = E.OrganizationID
                                                    and CONVERT(varchar(64), C.EquipmentID) = '{3}') M
                                            where M.ReasonID is null
                                            order by M.OrganizationID, M.EquipmentID, M.HaltTimeF";
            try
            {
                m_Sql = string.Format(m_Sql, myStartTime, myEndTime, myOrganizationId, myEquipmentId);
                DataTable m_HaltLogTable = _dataFactory.Query(m_Sql);
                return m_HaltLogTable;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static DataTable GetMachineHaltReason()
        {
            string m_Sql = @"SELECT A.MachineHaltReasonID
                                  ,A.ReasonText
                                  ,A.Remarks
                                  ,A.ReasonStatisticsTypeId
                                  ,A.OrganizationID
                              FROM system_MachineHaltReason A";
            try
            {
                DataTable m_MachineHaltReasonTable = _dataFactory.Query(m_Sql);
                return m_MachineHaltReasonTable;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static string GetMachineHaltLogChartString(DataTable myMachineHaltLogTable, DataTable myMachineHaltReasonTable, bool myAllReason, string myDisplayOrder)
        {
            DataTable m_MachineHaltLogTable = new DataTable();
            m_MachineHaltLogTable.Columns.Add("DisplayOrderValue", typeof(decimal));               //排序用累计数
            m_MachineHaltLogTable.Columns.Add("ReasonName", typeof(string));
            m_MachineHaltLogTable.Columns.Add("ChartString", typeof(string));

            string m_ReturnString = "[]";
            if (myMachineHaltLogTable != null && myMachineHaltReasonTable != null)
            {
                string m_ReasonName = "";
                int m_NewRowsIndex = -1;       //新建行的索引值
                for (int i = 0; i < myMachineHaltLogTable.Rows.Count; i++)
                {
                    string m_ReasonId = myMachineHaltLogTable.Rows[i]["ReasonID"].ToString();
                    string m_ReasonNameTemp = "-1";
                    if (myAllReason == true)
                    {
                        m_ReasonNameTemp = GetReasonName(m_ReasonId, myMachineHaltReasonTable) + "(" + myMachineHaltLogTable.Rows[i]["Name"].ToString() + ")";
                    }
                    else
                    {
                        m_ReasonNameTemp = GetReasonName(m_ReasonId, myMachineHaltReasonTable);
                    }
                    if (m_ReasonName != m_ReasonNameTemp)
                    {
                        m_ReasonName = m_ReasonNameTemp;
                        DataRow m_NewDataRow = m_MachineHaltLogTable.NewRow();
                        m_NewDataRow["ReasonName"] = m_ReasonName;
                        m_NewDataRow["ChartString"] = "";
                        m_NewDataRow["DisplayOrderValue"] = 0.0m;
                        m_MachineHaltLogTable.Rows.Add(m_NewDataRow);
                        m_NewRowsIndex = m_NewRowsIndex + 1;
                    }
                    ///////////////填入排序用累计数/////////////
                    if (myDisplayOrder == "count")
                    {
                        decimal m_CountValue = m_MachineHaltLogTable.Rows[m_NewRowsIndex]["DisplayOrderValue"] != DBNull.Value ? (decimal)m_MachineHaltLogTable.Rows[m_NewRowsIndex]["DisplayOrderValue"] : 0.0m;
                        m_MachineHaltLogTable.Rows[m_NewRowsIndex]["DisplayOrderValue"] = m_CountValue + 1.0m;
                    }
                    else if (myDisplayOrder == "time")
                    {
                        decimal m_CountValue = m_MachineHaltLogTable.Rows[m_NewRowsIndex]["DisplayOrderValue"] != DBNull.Value ? (decimal)m_MachineHaltLogTable.Rows[m_NewRowsIndex]["DisplayOrderValue"] : 0.0m;
                        try
                        {
                            DateTime m_StartTime = (DateTime)myMachineHaltLogTable.Rows[i]["HaltTime"];
                            DateTime m_EndTime = (DateTime)myMachineHaltLogTable.Rows[i]["RecoverTime"];
                            TimeSpan m_StartTime_ts = new TimeSpan(m_StartTime.Ticks);
                            TimeSpan m_EndTime_ts = new TimeSpan(m_EndTime.Ticks);
                            TimeSpan m_Diff_ts = m_EndTime_ts.Subtract(m_StartTime_ts).Duration();
                            m_MachineHaltLogTable.Rows[m_NewRowsIndex]["DisplayOrderValue"] = m_CountValue + (decimal)m_Diff_ts.TotalSeconds;
                        }
                        catch
                        {
                            m_MachineHaltLogTable.Rows[m_NewRowsIndex]["DisplayOrderValue"] = m_CountValue;
                        }

                    }
                    ////////////////////填入开始时间和结束之间json字符串////////////////////

                    if (m_MachineHaltLogTable.Rows[m_NewRowsIndex]["ChartString"].ToString() == "")          //如果该行第一次填写json字符串
                    {
                        m_MachineHaltLogTable.Rows[m_NewRowsIndex]["ChartString"] = "{\"start\":\""
                                + ((DateTime)myMachineHaltLogTable.Rows[i]["HaltTime"]).ToString("yyyy-MM-dd HH:mm:ss") + "\",\"end\":\""
                                + ((DateTime)myMachineHaltLogTable.Rows[i]["RecoverTime"]).ToString("yyyy-MM-dd HH:mm:ss") + "\"}";
                    }
                    else
                    {
                        m_MachineHaltLogTable.Rows[m_NewRowsIndex]["ChartString"] = m_MachineHaltLogTable.Rows[m_NewRowsIndex]["ChartString"].ToString() + "," + "{\"start\":\""
                                + ((DateTime)myMachineHaltLogTable.Rows[i]["HaltTime"]).ToString("yyyy-MM-dd HH:mm:ss") + "\",\"end\":\""
                                + ((DateTime)myMachineHaltLogTable.Rows[i]["RecoverTime"]).ToString("yyyy-MM-dd HH:mm:ss") + "\"}";
                    }
                }

                DataView m_MachineHaltLogView = m_MachineHaltLogTable.DefaultView;
                m_MachineHaltLogView.Sort = "DisplayOrderValue desc";
                DataTable m_MachineHaltLogTableResult = m_MachineHaltLogView.ToTable();
                for (int i = 0; i < m_MachineHaltLogTableResult.Rows.Count; i++)
                {
                    if (i == 0)
                    {
                        m_ReturnString = "{\"" + m_MachineHaltLogTableResult.Rows[i]["ReasonName"].ToString() + "\":[" + m_MachineHaltLogTableResult.Rows[i]["ChartString"].ToString() + "]";
                    }
                    else
                    {
                        m_ReturnString = m_ReturnString + ",\"" + m_MachineHaltLogTableResult.Rows[i]["ReasonName"].ToString() + "\":[" + m_MachineHaltLogTableResult.Rows[i]["ChartString"].ToString() + "]";
                    }
                }
                if (m_ReturnString != "[]")
                {
                    m_ReturnString = m_ReturnString + "}";
                }
                //for (int i = 0; i < m_MachineHaltLogTable.Rows.Count; i++)
                //{
                //    string m_ReasonNameTemp = m_MachineHaltLogTable.Rows[i]["ReasonName"].ToString();
                //    if (m_ReasonName != m_ReasonNameTemp)
                //    {
                //        m_ReasonName = m_ReasonNameTemp;
                //        if (m_ReturnString == "[]")            //第一次赋值
                //        {
                //            m_ReturnString = "{\"" + m_ReasonName + "\":[{\"start\":\""
                //                + ((DateTime)myMachineHaltLogTable.Rows[i]["HaltTime"]).ToString("yyyy-MM-dd HH:mm:ss") + "\",\"end\":\""
                //                + ((DateTime)myMachineHaltLogTable.Rows[i]["RecoverTime"]).ToString("yyyy-MM-dd HH:mm:ss") + "\"}";
                //        }
                //        else
                //        {
                //            m_ReturnString = m_ReturnString + "],\"" + m_ReasonName + "\":[{\"start\":\""
                //                + ((DateTime)myMachineHaltLogTable.Rows[i]["HaltTime"]).ToString("yyyy-MM-dd HH:mm:ss") + "\",\"end\":\""
                //                + ((DateTime)myMachineHaltLogTable.Rows[i]["RecoverTime"]).ToString("yyyy-MM-dd HH:mm:ss") + "\"}";
                //        }
                //    }
                //    else
                //    {
                //        m_ReturnString = m_ReturnString + ",{\"start\":\""
                //                + ((DateTime)myMachineHaltLogTable.Rows[i]["HaltTime"]).ToString("yyyy-MM-dd HH:mm:ss") + "\",\"end\":\""
                //                + ((DateTime)myMachineHaltLogTable.Rows[i]["RecoverTime"]).ToString("yyyy-MM-dd HH:mm:ss") + "\"}";
                //    }
                //}
                //if (m_ReturnString != "[]")
                //{
                //    m_ReturnString = m_ReturnString + "]}";
                //}
            }

            return m_ReturnString;
        }
        private static string GetReasonName(string myReasonId, DataTable myMachineHaltReasonTable)
        {
            string m_ReasonName = "";
            if (myReasonId.Length >= 5)
            {
                DataRow[] m_DataRow = myMachineHaltReasonTable.Select(string.Format("MachineHaltReasonID = '{0}'", myReasonId));
                if (m_DataRow.Length > 0)
                {
                    string m_UpperReasonId = myReasonId.Substring(0, myReasonId.Length - 2);
                    if (myReasonId.Length == 5)
                    {
                        m_ReasonName = m_DataRow[0]["ReasonText"].ToString();
                    }
                    else
                    {
                        m_ReasonName = GetReasonName(m_UpperReasonId, myMachineHaltReasonTable) + ">>" + m_DataRow[0]["ReasonText"].ToString();
                    }
                }
            }
            return m_ReasonName;
        }
    }
}
