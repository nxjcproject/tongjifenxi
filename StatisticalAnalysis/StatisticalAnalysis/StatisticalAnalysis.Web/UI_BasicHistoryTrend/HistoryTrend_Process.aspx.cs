using EasyUIJsonParser;
using StatisticalAnalysis.Infrastruture.Utility;
using StatisticalAnalysis.Service.BasicHistoryTrend;
using StatisticalReport.Service.StatisticalReportServices;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace StatisticalAnalysis.Web.UI_BasicHistoryTrend
{
    public partial class HistoryTrend_Process : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            ///以下是接收js脚本中post过来的参数
            string m_FunctionName = Request.Form["myFunctionName"] == null ? "" : Request.Form["myFunctionName"].ToString();             //方法名称,调用后台不同的方法
            string m_Parameter1 = Request.Form["myParameter1"] == null ? "" : Request.Form["myParameter1"].ToString();                   //方法的参数名称1
            string m_Parameter2 = Request.Form["myParameter2"] == null ? "" : Request.Form["myParameter2"].ToString();                   //方法的参数名称2
            if (m_FunctionName == "ExcelStream")
            {
                //ExportFile("xls", "导出报表1.xls");
                string m_ExportTable = m_Parameter1.Replace("&lt;", "<");
                m_ExportTable = m_ExportTable.Replace("&gt;", ">");
                StatisticalReportHelper.ExportExcelFile("xls", m_Parameter2 + "综合对标数据.xls", m_ExportTable);
            }
        }

        [WebMethod]
        public static string GetChartDataJson(string startTime, string endTime, string tags)
        {
            // tagItems 格式：
            // OrganizationID（组织机构ID）   LevelCode（层次码）   Source（标签添加方式）  Name（名称）
            DataTable tagTable = DataGridJsonParser.JsonToDataTable(tags.JsonPickArray("rows"), GetTagTable());

            DataTable dt = GetElectricityUsage(tagTable, startTime, endTime);

            // 获取横坐标（列，时间信息）
            IList<string> colNames = new List<string>();
            foreach (DataColumn dc in dt.Columns)
            {
                colNames.Add(dc.ColumnName.ToString());
            }

            // 获取图例（行，数据信息）
            IList<string> rowName = new List<string>();
            foreach (DataRow dr in tagTable.Rows)
            {
                rowName.Add(dr["Name"].ToString());
            }

            return ChartJsonParser.GetGridChartJsonString(dt, colNames.ToArray(), rowName.ToArray(), "年-月-日-时", "kWh", 1);
        }

        /// <summary>
        /// 获取标签列表的表格结构
        /// </summary>
        /// <returns></returns>
        private static DataTable GetTagTable()
        {
            DataTable tagTable = new DataTable();
            tagTable.Columns.Add("OrganizationID", typeof(string));
            tagTable.Columns.Add("VariableId", typeof(string));
            tagTable.Columns.Add("Name", typeof(string));
            tagTable.Columns.Add("LevelType", typeof(string));
            return tagTable;
        }

        /// <summary>
        /// 获取用电量信息
        /// </summary>
        /// <param name="tagTable"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private static DataTable GetElectricityUsage(DataTable tagTable, string startTime, string endTime)
        {
            #region 参数验证
            try
            {
                DateTime.Parse(startTime);
                DateTime.Parse(endTime);
            }
            catch
            {
                throw new ArgumentException("时间参数不正确");
            }
            #endregion

            DataTable electricityUsageSumTable = HorizontalAnalysisService.GetElectricityUsageGroupByHour(tagTable, DateTime.Parse(startTime), DateTime.Parse(endTime));

            return TempAnalysisTableHelper.VerticalToHorizontalHourly(electricityUsageSumTable, DateTime.Parse(startTime), DateTime.Parse(endTime));
        }
    }
}