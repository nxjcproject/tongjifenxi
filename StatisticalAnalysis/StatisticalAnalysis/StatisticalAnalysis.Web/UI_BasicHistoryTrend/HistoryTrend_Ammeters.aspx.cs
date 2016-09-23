using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using EasyUIJsonParser;
using StatisticalAnalysis.Infrastruture.Utility;
using StatisticalAnalysis.Service.BasicHistoryTrend;
using StatisticalReport.Service.StatisticalReportServices;

namespace StatisticalAnalysis.Web.UI_BasicHistoryTrend
{
    public partial class HistoryTrend_Ammeters : WebStyleBaseForEnergy.webStyleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ////////////////////调试用,自定义的数据授权
#if DEBUG
                List<string> m_DataValidIdItems = new List<string>() { "zc_nxjc_qtx_tys" };
                AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
#elif RELEASE
#endif
                this.OrganisationTree_ProductionLine.Organizations = GetDataValidIdGroup("ProductionOrganization");                 //向web用户控件传递数据授权参数
                this.OrganisationTree_ProductionLine.PageName = "HistoryTrend_Ammeters.aspx";                                       //向web用户控件传递当前调用的页面名称
                this.OrganisationTree_ProductionLine.LeveDepth = 3;

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
        }

        [WebMethod]
        public static string GetChartDataJson(string startTime, string endTime, string timeInterval, string tags)
        {
            // tagItems 格式：
            // OrganizationID（组织机构ID）   TagTableName（数据库名+ .dbo. + 表名）   TagColumnName（字段名）  Name（名称）
            DataTable tagTable = EasyUIJsonParser.DataGridJsonParser.JsonToDataTable(tags.JsonPickArray("rows"), GetTagTable());

            string[] test = tags.JsonPickArray("rows");

            DataTable dt = GetQuantityUsage(tagTable, startTime, endTime, timeInterval);

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
            string m_UnitX = "";
            if (timeInterval == "10")
            {
                m_UnitX = "月-日-时-分";
            }
            else if (timeInterval == "20")
            {
                m_UnitX = "月-日-时";
            }
            else if (timeInterval == "30")
            {
                m_UnitX = "年-月-日";
            }
            return ChartJsonParser.GetGridChartJsonString(dt, colNames.ToArray(), rowName.ToArray(), m_UnitX, "kWh", 1);
        }

        /// <summary>
        /// 获取标签列表的表格结构
        /// </summary>
        /// <returns></returns>
        private static DataTable GetTagTable()
        {
            DataTable tagTable = new DataTable();
            tagTable.Columns.Add("OrganizationId", typeof(string));
            tagTable.Columns.Add("TagTableName", typeof(string));
            tagTable.Columns.Add("TagColumnName", typeof(string));
            tagTable.Columns.Add("VariableId", typeof(string));
            tagTable.Columns.Add("Name", typeof(string));
            return tagTable;
        }

        /// <summary>
        /// 获取用电量信息
        /// </summary>
        /// <param name="tagTable"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        private static DataTable GetQuantityUsage(DataTable tagTable, string startTime, string endTime, string timeInterval)
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

            DataTable QuantityUsageSumTable = AmmetersHistoryTrendService.GetQuantityUsageGroup(tagTable, DateTime.Parse(startTime), DateTime.Parse(endTime), timeInterval);

            return TempAnalysisTableHelper.VerticalToHorizontalByInterval(QuantityUsageSumTable, DateTime.Parse(startTime), DateTime.Parse(endTime), timeInterval);
        }

        [WebMethod]
        public static string GetAmmetersTreeGridFormat(string organizationId)
        {
            DataTable dt = AmmetersHistoryTrendService.GetAmmetersByOrganizationId(organizationId);

            return EasyUIJsonParser.TreeGridJsonParser.DataTableToJson(dt, "KeyId", "Name", "ParentKeyId", "0", new string[] { "OrganizationId", "TagTableName", "TagColumnName", "LevelType", "Name", "VariableId" });
        }
    }
}