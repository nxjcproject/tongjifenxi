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
        public static DataTable TableQuery(string organizationId, string date)
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
            string sql = "SELECT QuotasID FROM [dbo].[plan_EnergyConsumptionPlan_Template]";
            DataTable itemTable = _dataFactory.Query(sql);
            foreach (DataRow dr in itemTable.Rows)
            {
                DataRow row = result.NewRow();
                row["项目指标"] = dr["QuotasID"];
                row["类别"] = "完成情况";
                result.Rows.Add(row);
            }
            ///////取数部分未完待续---------
            int curMonth = DateTime.Now.Month;
            int year = DateTime.Now.Year;
            IDictionary<string, string> m_dictionary = new Dictionary<string,string>(); 
            m_dictionary.Add("吨熟料发电量","cementmill_ElectricityConsumption");///临时
            m_dictionary.Add("发电量","cementmill_ElectricityConsumption");//临时
            m_dictionary.Add("煤磨电耗","coalPreparation_ElectricityConsumption");
            m_dictionary.Add("生料磨电耗","rawMaterialsPreparation_ElectricityConsumption");
            m_dictionary.Add("熟料产量","clinker_ClinkerOutput");
            m_dictionary.Add("熟料电耗","clinkerPreparation_ElectricityConsumption");
            m_dictionary.Add("熟料煤耗","clinker_CoalConsumption");
            m_dictionary.Add("水泥产量","cement_CementOutput");
            m_dictionary.Add("水泥电耗","cementmill_ElectricityConsumption");
            m_dictionary.Add("水泥磨电耗","cementPreparation_ElectricityConsumption");
            foreach (DataRow dr in result.Rows)
            {
                string variable=dr["项目指标"].ToString();
                for (int i = 1; i <= 12; i++)
                {
                    string v_sql = @"SELECT SUM(B.TotalPeakValleyFlatB)
                                FROM [dbo].[tz_Balance] AS A INNER JOIN [dbo].[balance_Energy] AS B
                                ON 
                                A.[BalanceId]=B.KeyId 
                                WHERE
                                MONTH(A.TimeStamp)=@month
                                AND
                                YEAR(A.TimeStamp)=@date
                                AND
                                B.OrganizationID=@organizationId
                                AND
                                B.VariableId=@variableId";
                    SqlParameter[] parameters = { new SqlParameter("date", date), new SqlParameter("organizationId", organizationId),
                                                new SqlParameter("month",i.ToString("00")),new SqlParameter("variableId", m_dictionary[variable])};
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
    }
}
