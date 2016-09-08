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
    public class DowntimeReasonAnalysis
    {
        private static readonly ISqlServerDataFactory _dataFactory = new SqlServerDataFactory(ConnectionStringFactory.NXJCConnectionString);
        public static DataTable GetEquipmentCommonInfo(List<string> myOrganizations)
        {
            string m_Condition = "";
            string m_Sql = @"select distinct C.EquipmentCommonId as id, C.Name as text from system_MasterMachineDescription A, equipment_EquipmentDetail B, equipment_EquipmentCommonInfo C, system_Organization D, system_Organization E
                                where A.ID = B.EquipmentId
                                and B.EquipmentCommonId = C.EquipmentCommonId
                                and A.OrganizationID = D.OrganizationID
                                and D.LevelCode like E.LevelCode + '%'
                                and E.OrganizationID in ({0})";

            try
            {
                if (myOrganizations != null && myOrganizations.Count > 0)
                {
                    for (int i = 0; i < myOrganizations.Count; i++)
                    {
                        if (i == 0)
                        {
                            m_Condition = "'" + myOrganizations[i] + "'";
                        }
                        else
                        {
                            m_Condition = m_Condition + ",'" + myOrganizations[i] + "'";
                        }
                    }
                    m_Sql = string.Format(m_Sql, m_Condition);
                    DataTable m_EquipmentCommonInfoTable = _dataFactory.Query(m_Sql);
                    return m_EquipmentCommonInfoTable;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static DataTable GetLevelCodeInfo(List<string> myOrganizations)
        {
            string m_Condition = "";
            string m_Sql = @"SELECT A.LevelCode
                                  ,A.Name
                             FROM system_Organization A, system_Organization B
                                 where A.Enabled = 1
                                 and B.OrganizationID in ({0})
                                 and A.LevelCode like B.LevelCode + '%'
                                 order by A.LevelCode";
            try
            {
                if (myOrganizations != null && myOrganizations.Count > 0)
                {
                    for (int i = 0; i < myOrganizations.Count; i++)
                    {
                        if (i == 0)
                        {
                            m_Condition = "'" + myOrganizations[i] + "'";
                        }
                        else
                        {
                            m_Condition = m_Condition + ",'" + myOrganizations[i] + "'";
                        }
                    }
                    m_Sql = string.Format(m_Sql, m_Condition);
                    DataTable m_LevelCodeInfoTable = _dataFactory.Query(m_Sql);
                    return m_LevelCodeInfoTable;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static DataTable GetReasonTypeInfo()
        {
            string m_Sql = @"SELECT A.ReasonStatisticsTypeId as Id
                                  ,A.Name
                                  ,Rtrim(Ltrim(A.LevelCode)) as LevelCode
                                  ,A.Enabled
                                  ,A.Creator
                                  ,A.CreateTime
                              FROM system_MachineHaltReasonStatisticsType A
                              where A.Enabled = 1
                              order by A.LevelCode";

            try
            {
                DataTable m_ReasonTypeInfoTable = _dataFactory.Query(m_Sql);
                return m_ReasonTypeInfoTable;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static DataTable GetHaltReasonStatics(string myStartTime, string myEndTime, string myEquipmentCommonId, string myStaticsMethod, string myLevelCode, string myReasonType, List<string> myValidOrganizationIds)
        {
            DataTable m_HaltReasonInfoTable = GetHaltReasonInfo();
            string m_OrganizationIds = "";
            string m_Sql = @"Select X.ReasonText +'(' + X.ReasonType + ')' as ReasonText, X.MachineHaltReasonID as MachineHaltReasonId, X.Value from
                               (Select M.MachineHaltReasonID, M.ReasonText, N.Name as ReasonType, {6} 
                                 from
									(Select case when F.MachineHaltReasonID is null then '' else F.MachineHaltReasonID end as MachineHaltReasonID,
									        case when F.ReasonText is null then '不明原因' else F.ReasonText end as ReasonText,
                                        F.ReasonStatisticsTypeId,
                                        (case when C.HaltTime is null or C.HaltTime < '{0} 00:00:00' then '{0} 00:00:00'
                                              when C.HaltTime > '{1} 23:59:59' then '{1} 23:59:59'
										      else C.HaltTime end) as HaltTimeF,
                                        (case when C.RecoverTime is null or C.RecoverTime > '{1} 23:59:59' then '{1} 23:59:59'
                                              when  C.RecoverTime < '{0} 00:00:00' then '{0} 00:00:00'
                                              else C.RecoverTime end)  as RecoverTimeF,
                                        convert(decimal(18,4), DATEDIFF (second,(case when C.HaltTime is null or C.HaltTime < '{0} 00:00:00' then '{0} 00:00:00'
                                              when C.HaltTime > '{1} 23:59:59' then '{1} 23:59:59'
										      else C.HaltTime end),
                                             (case when C.RecoverTime is null or C.RecoverTime > '{1} 23:59:59' then '{1} 23:59:59'
                                              when  C.RecoverTime < '{0} 00:00:00' then '{0} 00:00:00'
                                              else C.RecoverTime end))) / 3600 as HaltLongF    
								    from shift_MachineHaltLog C
										   left join system_MachineHaltReason F on C.ReasonID = F.MachineHaltReasonID {4}, 
										   system_Organization D, system_Organization E, equipment_EquipmentDetail G
										where ((C.HaltTime >= '{0} 00:00:00' and C.HaltTime <= '{1} 23:59:59')
										   or (C.RecoverTime >= '{0} 00:00:00' and C.RecoverTime <= '{1} 23:59:59'))
										and E.OrganizationID in ({2})
                                        and D.LevelCode like E.LevelCode + '%'
                                        and D.LevelCode like '{3}%'
										and C.OrganizationID = D.OrganizationID
                                        and C.EquipmentID = G.EquipmentID
                                        and C.ReasonID is not null
                                        {5}) M
                                    left join system_MachineHaltReasonStatisticsType N on M.ReasonStatisticsTypeId = N.ReasonStatisticsTypeId
                                    where M.ReasonStatisticsTypeId is not null
									group by M.MachineHaltReasonID, M.ReasonText, N.Name) X
									order by X.Value desc";
            try
            {
                if (myValidOrganizationIds != null)
                {
                    for (int i = 0; i < myValidOrganizationIds.Count; i++)
                    {
                        if (i == 0)
                        {
                            m_OrganizationIds = "'" + myValidOrganizationIds[i] + "'";
                        }
                        else
                        {
                            m_OrganizationIds = m_OrganizationIds + ",'" + myValidOrganizationIds[i] + "'";
                        }
                    }
                    ///////当选择具体原因类别的时候
                    string m_ReasonStatisticsType = "";
                    if (myReasonType == "Null")
                    {
                        m_ReasonStatisticsType = " and (F.ReasonStatisticsTypeId is null or F.ReasonStatisticsTypeId = '') ";
                    }
                    else if (myReasonType != "All" && myReasonType != "Null")
                    {
                        m_ReasonStatisticsType = string.Format(" and F.ReasonStatisticsTypeId = '{0}'", myReasonType);
                    }

                    //当选择次数还是时间的时候
                    string m_DisplayStaticsMethod = "";
                    if (myStaticsMethod == "StaticsCount")
                    {
                        m_DisplayStaticsMethod = " count(M.MachineHaltReasonID) as Value ";
                    }
                    else if (myStaticsMethod == "StaticsTime")
                    {
                        m_DisplayStaticsMethod = " sum( case when M.RecoverTimeF > M.HaltTimeF then convert(decimal(18,4), DATEDIFF (second, M.HaltTimeF, M.RecoverTimeF))/3600 else 0 end) as Value ";
                    }
                    //当选择了具体设备的时候
                    string m_EquipmentId = "";
                    if (myEquipmentCommonId != "All")
                    {
                        m_EquipmentId = string.Format(" and G.EquipmentCommonId = '{0}' ", myEquipmentCommonId);
                    }
                    m_Sql = m_Sql.Replace("{0}", myStartTime);
                    m_Sql = m_Sql.Replace("{1}", myEndTime);
                    m_Sql = m_Sql.Replace("{2}", m_OrganizationIds);
                    m_Sql = m_Sql.Replace("{3}", myLevelCode);
                    m_Sql = m_Sql.Replace("{4}", m_ReasonStatisticsType);
                    m_Sql = m_Sql.Replace("{5}", m_EquipmentId);
                    m_Sql = m_Sql.Replace("{6}", m_DisplayStaticsMethod);

                    DataTable m_ResultTable = _dataFactory.Query(m_Sql);

                    if (m_HaltReasonInfoTable != null && m_ResultTable != null)
                    {
                        for (int i = 0; i < m_ResultTable.Rows.Count; i++)
                        {
                            string m_ReasonText = m_ResultTable.Rows[i]["ReasonText"].ToString();
                            string m_ReasonLevelCode = m_ResultTable.Rows[i]["MachineHaltReasonId"].ToString().Replace(" ", "");
                            while (m_ReasonLevelCode.Length > 2)
                            {
                                m_ReasonLevelCode = m_ReasonLevelCode.Substring(0, m_ReasonLevelCode.Length - 2);
                                DataRow[] m_DataRowTemp = m_HaltReasonInfoTable.Select(string.Format("MachineHaltReasonId = '{0}'", m_ReasonLevelCode));
                                if (m_DataRowTemp.Length > 0)
                                {
                                    m_ReasonText = m_DataRowTemp[0]["ReasonText"].ToString() + ">>" + m_ReasonText;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            m_ResultTable.Rows[i]["ReasonText"] = m_ReasonText;
                        }
                    }
                    return m_ResultTable;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        private static DataTable GetHaltReasonInfo()
        {
            string m_Sql = @"select Ltrim(Rtrim(A.MachineHaltReasonID)) as MachineHaltReasonId, A.ReasonText from system_MachineHaltReason A where A.Enabled = 1";
            try
            {
                DataTable m_HaltReasonInfoTable = _dataFactory.Query(m_Sql);
                return m_HaltReasonInfoTable;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
