using SqlServerDataAdapter;
using StatisticalAnalysis.Infrastruture.Configuration;
using StatisticalReport.Infrastructure.Report;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace StatisticalAnalysis.Service.PlanAndActual
{
    public static class TargetCompletion
    {
        private const string NormalCaculateType = "Normal";               //普通计算方式
        private const string ComprehensiveCaculateType = "Comprehensive";        //综合计算方式

        public static DataTable TableQuery(string organizationId, string date, string item)
        {
            string _connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory _dataFactory = new SqlServerDataFactory(_connectionString);
            DataTable result = new DataTable();
            DataColumn[] columns = new DataColumn[14];
            columns[0] = new DataColumn("类别", Type.GetType("System.String"));
            for (int i = 1; i <= 12; i++)
            {
                columns[i] = new DataColumn(i + "月", Type.GetType("System.Decimal"));
            }
            columns[13] = new DataColumn("项目指标", Type.GetType("System.String"));
            //columns[14] = new DataColumn("Type", Type.GetType("System.String"));
            foreach (DataColumn dc in columns)
            {
                result.Columns.Add(dc);
            }
            string sql = "SELECT QuotasID, VariableId, ValueType, CaculateType FROM [dbo].[plan_EnergyConsumptionPlan_Template] where QuotasID = '{0}'";
            sql = string.Format(sql, item);
            DataTable itemTable = _dataFactory.Query(sql);
            IDictionary<string, string> m_dictionary = new Dictionary<string, string>();

            result.Columns.Add("CaculateType",typeof(string));          //临时加上返回前去掉
            foreach (DataRow dr in itemTable.Rows)
            {
                DataRow row = result.NewRow();
                row["项目指标"] = dr["QuotasID"];
                row["类别"] = "完成情况";
                row["CaculateType"] = dr["CaculateType"];
                result.Rows.Add(row);

                //////////填充项目指标与变量映射字典///////////
                if (dr["CaculateType"].ToString() == NormalCaculateType)         //普通的工序计算MaterialWeight
                {
                    if (dr["ValueType"].ToString() == "MaterialWeight")         //如果是产量则不需要加后缀
                    {
                        m_dictionary.Add(dr["QuotasID"].ToString(), dr["VariableId"].ToString());
                    }
                    else
                    {
                        m_dictionary.Add(dr["QuotasID"].ToString(), dr["VariableId"].ToString() + "_" + dr["ValueType"].ToString());
                    }
                }
                else                  //综合能耗计算
                {
                    m_dictionary.Add(dr["QuotasID"].ToString(), dr["VariableId"].ToString() + "_" + dr["ValueType"].ToString() + "_" + dr["CaculateType"].ToString());
                }

            }
            ///////取数部分未完待续---------
            int curMonth = DateTime.Now.Month;
            int year = DateTime.Now.Year;
            
            DataRow[] m_NormalCaculateRows = result.Select(string.Format("CaculateType = '{0}'", NormalCaculateType));
            DataRow[] m_ComprehensiveCaculateRows = result.Select(string.Format("CaculateType = '{0}'", ComprehensiveCaculateType));
            foreach (DataRow dr in m_NormalCaculateRows)             //普通能耗计算方式
            {
                string variable=dr["项目指标"].ToString();
                for (int i = 1; i <= 12; i++)
                {
                    string v_sql = @"SELECT SUM(B.TotalPeakValleyFlatB)
                                FROM [dbo].[tz_Balance] AS A INNER JOIN [dbo].[balance_Energy] AS B
                                ON 
                                A.[BalanceId]=B.KeyId 
                                WHERE
                                A.TimeStamp=@TimeStamp                                
                                AND
                                B.OrganizationID=@organizationId
                                AND
                                B.VariableId=@variableId";
                    string timeStamp = date + '-' + i.ToString("00");
                    SqlParameter[] parameters = { new SqlParameter("TimeStamp", timeStamp), new SqlParameter("organizationId", organizationId),
                                                new SqlParameter("variableId", m_dictionary[variable])};
                    DataTable temp= _dataFactory.Query(v_sql, parameters);

                    if (temp.Rows.Count == 0)
                    {
                        dr[i] = 0;
                    }
                    else
                    {
                        dr[i] =ReportHelper.MyToDecimal(temp.Rows[0][0]);
                    }
                }
            }

            foreach (DataRow dr in m_ComprehensiveCaculateRows)             //综合能耗计算方式
            {
                string variable = dr["项目指标"].ToString();
                for (int i = 1; i <= 12; i++)
                {
                    string v_sql = @"SELECT SUM(B.TotalPeakValleyFlatB) Value, B.VariableId as VariableId
                                FROM [dbo].[tz_Balance] AS A INNER JOIN [dbo].[balance_Energy] AS B
                                ON 
                                A.[BalanceId]=B.KeyId 
                                WHERE
                                A.TimeStamp=@TimeStamp
                                AND A.StaticsCycle = 'month'                                 
                                AND B.OrganizationID=@organizationId
                                AND B.ValueType in ('ElectricityQuantity','MaterialWeight')
                                group by B.VariableId";
                    string timeStamp = date + '-' + i.ToString("00");
                    SqlParameter[] parameters = { new SqlParameter("TimeStamp", timeStamp), new SqlParameter("organizationId", organizationId)};
                    DataTable temp = _dataFactory.Query(v_sql, parameters);

                    if (temp.Rows.Count == 0)
                    {
                        dr[i] = 0;
                    }
                    else
                    {
                        dr[i] = GetComprehensiveData(timeStamp, organizationId, _dataFactory, temp, m_dictionary[variable]);
                    }
                }
            }
            //TZHelper tzHelper=new TZHelper(_connectionString);
            //DataTable clinkerOutput = tzHelper.GetReportData("tz_Report", organizationId, date, "table_ClinkerYearlyOutput");
            //DataTable clinkerElec = tzHelper.GetReportData("tz_Report", organizationId, date, "table_ClinkerYearlyElectricity_sum");
            //int curMonth = DateTime.Now.Month;
            //for (int i = 1; i <= curMonth; i++)
            //{
            //    DataRow rowOutput =MyFindRow(clinkerOutput,i.ToString("00"));
            //    DataRow rowElec = MyFindRow(clinkerElec, i.ToString("00"));
            //}
            ///////
            result.Columns.Remove("CaculateType");          //临时加上返回前去掉
            return result;
        }
        /// <summary>
        /// 根据日期返回具体行
        /// </summary>
        /// <param name="table">要查找的表</param>
        /// <param name="date">日期</param>
        /// <returns></returns>
        public static DataRow MyFindRow(DataTable table, string date)
        {
            DataRow[] rows = table.Select("vDate='" + date + "'");
            if (rows.Count() > 0)
            {
                if (rows.Count() > 1)
                {
                    throw new Exception("不只有一行");
                }
                return rows[0];
            }
            else
            {
                return table.NewRow();
            }
        }

        public static Decimal GetComprehensiveData(string myDate, string myOrganizationId, ISqlServerDataFactory myDataFactory, DataTable myActualMonthResultTable, string myVariableId)
        {
            ////计算综合电耗、煤耗
            decimal Value = 0.0m;
            AutoSetParameters.AutoGetEnergyConsumptionRuntime_V1 m_AutoGetEnergyConsumption_V1 = new AutoSetParameters.AutoGetEnergyConsumptionRuntime_V1(new SqlServerDataAdapter.SqlServerDataFactory(ConnectionStringFactory.NXJCConnectionString));
            DateTime m_StartTime = DateTime.Parse(myDate + "-01");
            DateTime m_EndTime = m_StartTime.AddMonths(1).AddDays(-1);

            //Standard_GB16780_2012.Parameters_ComprehensiveData m_Parameters_ComprehensiveData = AutoSetParameters.AutoSetParameters_V1.SetComprehensiveParametersFromSql("month",
            //    myDate, myDate, new List<string>() { myOrganizationId }, myDataFactory);
            //Standard_GB16780_2012.Function_EnergyConsumption_V1 m_Function_EnergyConsumption_V1 = new Standard_GB16780_2012.Function_EnergyConsumption_V1();
            //m_Function_EnergyConsumption_V1.LoadComprehensiveData(myActualMonthResultTable, m_Parameters_ComprehensiveData, "VariableId", "Value");
            DataTable m_OrganizationLevelCodeTable = GetOrganizationInfo(myOrganizationId);
            if (m_OrganizationLevelCodeTable != null && m_OrganizationLevelCodeTable.Rows.Count > 0)
            {
                string m_OrganizationLevelCode = m_OrganizationLevelCodeTable.Rows[0]["LevelCode"].ToString();

                if (myVariableId == "clinker_ElectricityConsumption_Comprehensive")              //熟料综合电耗
                {
                    Value = m_AutoGetEnergyConsumption_V1.GetClinkerPowerConsumptionWithFormula("day", m_StartTime.ToString("yyyy-MM-dd"), m_EndTime.ToString("yyyy-MM-dd"), m_OrganizationLevelCode).CaculateValue;
                }
                else if (myVariableId == "clinker_CoalConsumption_Comprehensive")              //熟料综合煤耗
                {
                    Value = m_AutoGetEnergyConsumption_V1.GetClinkerCoalConsumptionWithFormula("day", m_StartTime.ToString("yyyy-MM-dd"), m_EndTime.ToString("yyyy-MM-dd"), m_OrganizationLevelCode).CaculateValue;
                }
                else if (myVariableId == "clinker_EnergyConsumption_Comprehensive")              //熟料能耗电耗
                {
                    Value = m_AutoGetEnergyConsumption_V1.GetClinkerEnergyConsumptionWithFormula("day", m_StartTime.ToString("yyyy-MM-dd"), m_EndTime.ToString("yyyy-MM-dd"), m_OrganizationLevelCode).CaculateValue;
                }
                else if (myVariableId == "cementmill_ElectricityConsumption_Comprehensive")              //水泥综合电耗
                {
                    Value = m_AutoGetEnergyConsumption_V1.GetCementPowerConsumptionWithFormula("day", m_StartTime.ToString("yyyy-MM-dd"), m_EndTime.ToString("yyyy-MM-dd"), m_OrganizationLevelCode).CaculateValue;
                }
                else if (myVariableId == "cementmill_CoalConsumption_Comprehensive")                   //水泥综合煤耗
                {
                    Value = m_AutoGetEnergyConsumption_V1.GetCementCoalConsumptionWithFormula("day", m_StartTime.ToString("yyyy-MM-dd"), m_EndTime.ToString("yyyy-MM-dd"), m_OrganizationLevelCode).CaculateValue;
                }
                else if (myVariableId == "cementmill_EnergyConsumption_Comprehensive")                //水泥综合能耗
                {
                    Value = m_AutoGetEnergyConsumption_V1.GetCementEnergyConsumptionWithFormula("day", m_StartTime.ToString("yyyy-MM-dd"), m_EndTime.ToString("yyyy-MM-dd"), m_OrganizationLevelCode).CaculateValue;
                }
            }
            return Value;
        }
        private static DataTable GetOrganizationInfo(string myOrganizationId)
        {
            string _connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory _dataFactory = new SqlServerDataFactory(_connectionString);

            string m_Sql = @"select A.LevelCode as LevelCode, A.LevelType as LevelType from system_Organization A
                     where A.OrganizationID = @OrganizationID";
            List<SqlParameter> m_Parameters = new List<SqlParameter>();
            m_Parameters.Add(new SqlParameter("@OrganizationID", myOrganizationId));
            DataTable table = _dataFactory.Query(m_Sql, m_Parameters.ToArray());
            return table;
        }
    }
}
