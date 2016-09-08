using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using SqlServerDataAdapter;
using StatisticalAnalysis.Infrastruture.Configuration;
using StatisticalAnalysis.Infrastruture.Utility;
namespace StatisticalAnalysis.Service.DowntimeAnalysis
{
    public class DowntimeHorizontallyAnalysis
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
        public static DataTable GetHaltReasonStatics(string myStartTime, string myEndTime, string myEquipmentCommonId, string myStaticsMethod, string myStaticsRange, string myReasonType, List<string> myValidOrganizationIds)
        {
            string m_OrganizationIds = "";
            string m_Sql = @"Select Y.Name {7} as Name, X.LevelCode as Id, X.Value from
                               (Select {3}, {8} 
                                 from
									(Select C.MachineHaltLogID, C.ReasonID, D.OrganizationID, D.LevelCode, C.HaltTime, C.RecoverTime, C.EquipmentID
										,F.ReasonStatisticsTypeId,
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
                                              else C.RecoverTime end))) / 3600.00 as HaltLongF    
								    from shift_MachineHaltLog C
										   left join system_MachineHaltReason F on C.ReasonID = F.MachineHaltReasonID, 
										   system_Organization D, system_Organization E, equipment_EquipmentDetail G, system_Organization J, system_Organization K
										where ((C.HaltTime >= '{0} 00:00:00' and C.HaltTime <= '{1} 23:59:59')
										   or (C.RecoverTime >= '{0} 00:00:00' and C.RecoverTime <= '{1} 23:59:59'))
										and K.OrganizationID in ({2})
                                        and J.LevelCode like K.LevelCode + '%'
                                        and J.LevelType = 'Factory' 
										and D.OrganizationID = J.OrganizationID
										and E.LevelCode like D.LevelCode + '%'
										and C.OrganizationID = E.OrganizationID
                                        and C.EquipmentID = G.EquipmentID
                                        {9}
                                        {5}) M
									group by {4}) X
									{6}
									,system_Organization Y
									where X.LevelCode = Y.LevelCode
									order by X.Value desc, X.LevelCode";
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
                    //当按设备显示的时候
                    string m_GroupByCondition = string.Format("substring(M.LevelCode,1,{0})", myStaticsRange);
                    string m_DisplayColumn = string.Format("substring(M.LevelCode,1,{0}) as LevelCode", myStaticsRange);
                    string m_EquipmentCondition = "";
                    string m_DisplayEquimentName = "";
                    if (myStaticsRange == "7") 
                    {
                        m_GroupByCondition = m_GroupByCondition + ", M.EquipmentID";
                        m_DisplayColumn = m_DisplayColumn + ", M.EquipmentID";
                        m_EquipmentCondition = "left join equipment_EquipmentDetail Z on Z.EquipmentID = X.EquipmentID";
                        m_DisplayEquimentName = " + (case when Z.EquipmentName is null then '未知设备' else Z.EquipmentName end)";
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
                        m_DisplayStaticsMethod = " count(M.OrganizationID) as Value ";
                    }
                    else if (myStaticsMethod == "StaticsTime")
                    {
                        m_DisplayStaticsMethod = " sum( case when M.RecoverTimeF > M.HaltTimeF then convert(decimal(18,4), DATEDIFF (second, M.HaltTimeF, M.RecoverTimeF))/3600 else 0.0 end) as Value ";
                    }
                    //当选择了具体设备的时候
                    string m_EquipmentId = "";
                    if (myEquipmentCommonId != "All")
                    {
                        m_EquipmentId = string.Format(" and G.EquipmentCommonId = '{0}' ", myEquipmentCommonId);
                    }
                    m_Sql = m_Sql.Replace("{0}",myStartTime);
                    m_Sql = m_Sql.Replace("{1}",myEndTime);
                    m_Sql = m_Sql.Replace("{2}",m_OrganizationIds);
                    m_Sql = m_Sql.Replace("{3}",m_DisplayColumn);
                    m_Sql = m_Sql.Replace("{4}",m_GroupByCondition);
                    m_Sql = m_Sql.Replace("{5}",m_ReasonStatisticsType);
                    m_Sql = m_Sql.Replace("{6}",m_EquipmentCondition);
                    m_Sql = m_Sql.Replace("{7}",m_DisplayEquimentName);
                    m_Sql = m_Sql.Replace("{8}",m_DisplayStaticsMethod);
                    m_Sql = m_Sql.Replace("{9}", m_EquipmentId);
                   
                    DataTable m_ResultTable = _dataFactory.Query(m_Sql);
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
    }
}
