using SqlServerDataAdapter;
using StatisticalAnalysis.Infrastruture.Configuration;
using StatisticalReport.Infrastructure.Report;
using StatisticalReport.Service.StatisticalReportServices.Daily;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace StatisticalAnalysis.Service.PlanAndActual
{
    public static class PlanAndTargetCompletionService
    {
        /// <summary>
        /// 获取项目指标
        /// </summary>
        /// <param name="type">生产线类型</param>
        /// <returns></returns>
        public static DataTable GetItemList(string type)
        {
            string _connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory _dataFactory = new SqlServerDataFactory(_connectionString);
            DataTable itemtable;
            if (type == "")
            {
                string sql = "SELECT [QuotasID] as QuotasID FROM [plan_EnergyConsumptionPlan_Template] ORDER BY [ProductionLineType],[DisplayIndex]";
                Query query = new Query("plan_EnergyConsumptionPlan_Template");
                itemtable = _dataFactory.Query(sql);
            }
            else
            {
                string sql = "SELECT [QuotasID] as QuotasID FROM [plan_EnergyConsumptionPlan_Template] WHERE [ProductionLineType]=@type ORDER BY [ProductionLineType],[DisplayIndex]";
                SqlParameter parameter = new SqlParameter("type", type);
                itemtable = _dataFactory.Query(sql, parameter);
            }
            return itemtable;
        }
        public static DataTable GetChartTable(string organizationId, string item, string date)
        {
            //获取计划表
            string _connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory _dataFactory = new SqlServerDataFactory(_connectionString);
            DataTable planTable;
            string sqlPlan = @"SELECT  
                                 B.* 
                              FROM [dbo].[tz_Plan] AS A 
                              INNER JOIN [dbo].[plan_EnergyConsumptionYearlyPlan] AS B 
                                 ON A.KeyID=B.KeyID 
                              WHERE A.Date=@date AND A.OrganizationID LIKE '"+organizationId+"%'";
            SqlParameter paramaterPlan = new SqlParameter("date", date);
            DataTable temp = _dataFactory.Query(sqlPlan, paramaterPlan);
            planTable = ReportHelper.MyTotalOn(temp, "QuotasID", "January,February,March,April,May,June,July,August,September,October,November,December,Totals");
            //取出完成情况表
            DataTable targetCompletionTable;
            targetCompletionTable = TargetCompletion.TableQuery(organizationId, date, item);
            DataTable result = targetCompletionTable.Clone();
            DataRow[] rows;
            rows = targetCompletionTable.Select("项目指标='" + item + "'");
            foreach (DataRow dr in rows)
            {
                DataRow drow = result.NewRow();
                drow.ItemArray= dr.ItemArray;
                result.Rows.Add(drow);
            }
            ///取计划到目标表
            DataRow[] itemPlan = planTable.Select("QuotasID='" + item + "'");
            DataRow planRow = targetCompletionTable.NewRow();
            planRow["项目指标"] = "计划";
            int startPlan = planTable.Columns.IndexOf("January");
            int startTarger = targetCompletionTable.Columns.IndexOf("1月");
            for (int i = 0; i < 12; i++)
            {
                planRow[startTarger + i] = itemPlan[0][startPlan + i];
            }
            DataRow m_row = result.NewRow();
            m_row.ItemArray = planRow.ItemArray;
            result.Rows.Add(m_row);
            result.Columns.Remove("项目指标");
            result.Columns.Remove("类别");
            return result;
        }

    }
}
    //public enum myType
    //{
    //    plan=0,
    //    targetCompletion=1
    //}

