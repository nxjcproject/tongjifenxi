using SqlServerDataAdapter;
using StatisticalAnalysis.Infrastruture.Configuration;
using StatisticalAnalysis.Infrastruture.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace StatisticalAnalysis.Service.ElectricityCostAnalysis
{
    public class ElectricityABCDAnalysisService
    {
        /// <summary>
        /// 按照生产线组织机构ID（生产线）获取所有的电耗变量标签
        /// </summary>
        /// <param name="organizationId">组织机构ID（生产线）</param>
        /// <returns></returns>
        public static DataTable GetElectricityConsumptionVariableByOrganizationId(string organizationId)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);

            string queryString = @"SELECT [T].[VariableId] AS [id], [T].[VariableName] AS [text]
                                     FROM [system_Organization] AS [O] INNER JOIN
                                          [balance_Energy_Template] AS [T] ON [O].[Type] = [T].[ProductionLineType]
                                    WHERE ([O].[OrganizationID] = @organizationId) AND ([T].[ValueType] = 'ElectricityConsumption')
                                ";

            SqlParameter[] parameters = new SqlParameter[]{
                new SqlParameter("organizationId", organizationId)
            };

            return dataFactory.Query(queryString, parameters);
        }

        /// <summary>
        /// 获取ABCD班电耗（自定义，按日统计）
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="variableId"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public static DataTable GetABCDElectricityConsumptionCustom(string organizationId, string variableId, DateTime startTime, DateTime endTime)
        {
            DataTable pvfUsage = GetElectricityConsumptionABCDDailyByOrganiztionId(organizationId, variableId, startTime, endTime);

            return ABCDAnalysisTableHelper.VerticalToHorizontal(pvfUsage, ABCDAnalysisTableHelper.SourceType.Daily, startTime, endTime);
        }

        /// <summary>
        /// 获取ABCD班电耗（月查询，按日统计）
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="variableId"></param>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public static DataTable GetABCDElectricityConsumptionMonthly(string organizationId, string variableId, int year, int month)
        {
            DateTime startTime = DateTime.Parse(year + "-" + month + "-01 00:00:00");
            DateTime endTime = new DateTime(year, month, startTime.AddMonths(1).AddDays(-1).Day, 23, 59, 59);

            DataTable pvfUsage = GetElectricityConsumptionABCDDailyByOrganiztionId(organizationId, variableId, startTime, endTime);

            return ABCDAnalysisTableHelper.VerticalToHorizontal(pvfUsage, ABCDAnalysisTableHelper.SourceType.Monthly, startTime, endTime);
        }

        /// <summary>
        /// 获取ABCD班电耗（年查询，按月统计）
        /// </summary>
        /// <param name="organizationId">组织机构ID</param>
        /// <param name="variableId"></param>
        /// <param name="year">需要统计的年份</param>
        /// <returns></returns>
        public static DataTable GetABCDElectricityConsumptionYearly(string organizationId, string variableId, int year)
        {
            DateTime startTime = DateTime.Parse(year + "-01-01 00:00:00");
            DateTime endTime = DateTime.Parse(year + "-12-31 23:59:59");

            DataTable pvfUsage = GetElectricityConsumptionABCDMonthlyByOrganiztionId(organizationId, variableId, startTime, endTime);

            return ABCDAnalysisTableHelper.VerticalToHorizontal(pvfUsage, ABCDAnalysisTableHelper.SourceType.Yearly, DateTime.Parse(year + "-01-01"));
        }

        /// <summary>
        /// 按照组织机构ID（生产线）获取按日的ABCD电耗
        /// </summary>
        /// <param name="organizationId">组织机构ID（生产线）</param>
        /// <param name="variableId">变量ID</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns>峰谷平用电量</returns>
        public static DataTable GetElectricityConsumptionABCDDailyByOrganiztionId(string organizationId, string variableId, DateTime startTime, DateTime endTime)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);

            string queryString = @"Select 
                                    (case when A.FirstWorkingTeam = 'A班' then B.[FirstB] when SecondWorkingTeam = 'A班' then B.[SecondB] when A.ThirdWorkingTeam = 'A班' then B.ThirdB else 0 end) as A班,
                                    (case when FirstWorkingTeam = 'B班' then FirstB when SecondWorkingTeam = 'B班' then SecondB when ThirdWorkingTeam = 'B班' then B.ThirdB  else 0 end) as B班,
                                    (case when FirstWorkingTeam = 'C班' then FirstB when SecondWorkingTeam ='C班' then SecondB when ThirdWorkingTeam = 'C班' then B.ThirdB  else 0 end) as C班,
                                    (case when FirstWorkingTeam = 'D班' then FirstB when SecondWorkingTeam =  'D班' then SecondB when ThirdWorkingTeam =  'D班' then B.ThirdB else 0 end) as D班,
                                    A.TimeStamp
                                    From tz_Balance AS A,
                                    balance_Energy AS B
                                    Where A.BalanceId=B.KeyId AND
                                    ([B].[VariableId] = @variableId) AND 
                                    ([B].[OrganizationID] = @organizationId) AND 
                                    ([A].[StaticsCycle] = 'day') AND 
                                    ([A].[TimeStamp] >= @startTime) AND
                                    ([A].[TimeStamp] <= @endTime)
                                ";

            SqlParameter[] parameters = new SqlParameter[]{
                new SqlParameter("organizationId", organizationId),
                new SqlParameter("variableId", variableId),
                new SqlParameter("startTime", startTime),
                new SqlParameter("endTime", endTime)
            };

            return dataFactory.Query(queryString, parameters);
        }

        /// <summary>
        /// 按照组织机构ID（生产线）获取按月的ABCD电耗
        /// </summary>
        /// <param name="organiztionId">组织机构ID（生产线）</param>
        /// <param name="variableId">变量ID</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns>峰谷平用电量</returns>
        public static DataTable GetElectricityConsumptionABCDMonthlyByOrganiztionId(string organiztionId, string variableId, DateTime startTime, DateTime endTime)
        {
            string connectionString = ConnectionStringFactory.NXJCConnectionString;
            ISqlServerDataFactory dataFactory = new SqlServerDataFactory(connectionString);
            //找该VariableId的电耗公式
            string myFormulaSQL = @"select a.ValueFormula
                                    from balance_Energy_Template a
                                    where a.VariableId=@myVariableId";
            SqlParameter parameter = new SqlParameter("myVariableId",variableId);
            DataTable formulaTable = dataFactory.Query(myFormulaSQL, parameter);            
            string myFormula="";
            if (0 == formulaTable.Rows.Count)
                throw new Exception("没有找到该ID对应的电耗公式");
            else
                myFormula=formulaTable.Rows[0]["ValueFormula"].ToString().Trim();
            //拆分公式，找出产量、电量因式
            string[] myArray = myFormula.Split('/');
            if (myArray.Count() != 2)
                throw new Exception("该电耗公式不是有两个因式");
            //电量ID
            string electricityQuantityId = myArray[0].Trim('[',']');
            //产量ID
            string outputId = myArray[1].Trim('[', ']'); 
            string queryString = @"Select 
                                    sum(case when A.FirstWorkingTeam = 'A班' then B.[FirstB] when SecondWorkingTeam = 'A班' then B.[SecondB] when A.ThirdWorkingTeam = 'A班' then B.ThirdB else 0 end) as A班,
                                    sum(case when FirstWorkingTeam = 'B班' then FirstB when SecondWorkingTeam = 'B班' then SecondB when ThirdWorkingTeam = 'B班' then B.ThirdB  else 0 end) as B班,
                                    sum(case when FirstWorkingTeam = 'C班' then FirstB when SecondWorkingTeam ='C班' then SecondB when ThirdWorkingTeam = 'C班' then B.ThirdB  else 0 end) as C班,
                                    sum(case when FirstWorkingTeam = 'D班' then FirstB when SecondWorkingTeam =  'D班' then SecondB when ThirdWorkingTeam =  'D班' then B.ThirdB else 0 end) as D班,
                                    LEFT(A.TimeStamp,7) AS TimeStamp
                                    From tz_Balance AS A,
                                    balance_Energy AS B
                                    Where A.BalanceId=B.KeyId AND
                                    ([B].[VariableId] = @variableId) AND 
                                    ([B].[OrganizationID] = @organizationId) AND 
                                    ([A].[StaticsCycle] = 'day') AND 
                                    ([A].[TimeStamp] >= @startTime) AND
                                    ([A].[TimeStamp] <= @endTime)
                                    GROUP BY LEFT(A.TimeStamp,7) 
                                    order by TimeStamp";
            //电量参数
            SqlParameter[] parameters01 = new SqlParameter[]{
                new SqlParameter("organizationId", organiztionId),
                new SqlParameter("variableId", electricityQuantityId),
                new SqlParameter("startTime", startTime.ToString("yyyy-MM")),
                new SqlParameter("endTime", endTime.ToString("yyyy-MM"))
            };
            //产量参数
            SqlParameter[] parameters02 = new SqlParameter[]{
                new SqlParameter("organizationId", organiztionId),
                new SqlParameter("variableId", outputId),
                new SqlParameter("startTime", startTime.ToString("yyyy-MM")),
                new SqlParameter("endTime", endTime.ToString("yyyy-MM"))
            };
            //电量表
            DataTable table01= dataFactory.Query(queryString, parameters01);
            //产量表
            DataTable table02 = dataFactory.Query(queryString, parameters02);
            
            //求电耗
            DataTable result = table01.Clone();
            //int littleCount = table01.Rows.Count <= table02.Rows.Count ? table01.Rows.Count : table02.Rows.Count;
            //for (int i = 0; i < littleCount; i++)
            //{
            //    string myTimeStamp=table01.Rows[i]["TimeStamp"].ToString().Trim();
            //    DataRow row = result.NewRow();
            //    row["TimeStamp"] = myTimeStamp;
            //    row["A班"]=
            //}
            foreach (DataRow dr in table01.Rows)
            {
                string myTimeStamp = dr["TimeStamp"].ToString().Trim();
                //产量行
                DataRow[] rows = table02.Select("TimeStamp='"+myTimeStamp+"'");
                if (rows.Count() == 1)
                {
                    DataRow row = result.NewRow();
                    row["TimeStamp"] = myTimeStamp;
                    decimal ele=dr["A班"] is DBNull?0:Convert.ToDecimal(dr["A班"]);
                    row["A班"] = rows[0]["A班"] is DBNull || Convert.ToDecimal(rows[0]["A班"]) == 0 ? 0 : ele / Convert.ToDecimal(rows[0]["A班"]);
                    ele = dr["B班"] is DBNull ? 0 : Convert.ToDecimal(dr["B班"]);
                    row["B班"] = rows[0]["B班"] is DBNull || Convert.ToDecimal(rows[0]["B班"]) == 0 ? 0 : ele / Convert.ToDecimal(rows[0]["B班"]);
                    ele = dr["C班"] is DBNull ? 0 : Convert.ToDecimal(dr["C班"]);
                    row["C班"] = rows[0]["C班"] is DBNull || Convert.ToDecimal(rows[0]["C班"]) == 0 ? 0 : ele / Convert.ToDecimal(rows[0]["C班"]);
                    ele = dr["D班"] is DBNull ? 0 : Convert.ToDecimal(dr["D班"]);
                    row["D班"] = rows[0]["D班"] is DBNull || Convert.ToDecimal(rows[0]["D班"]) == 0 ? 0 : ele / Convert.ToDecimal(rows[0]["D班"]);
                    result.Rows.Add(row);
                }
            }
            return result;
        }
    }
}

