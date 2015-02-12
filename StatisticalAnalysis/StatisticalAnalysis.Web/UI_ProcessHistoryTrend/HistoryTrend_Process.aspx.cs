using EasyUIJsonParser;
using StatisticalAnalysis.Infrastruture.Utility;
using StatisticalAnalysis.Service.HistoryTrend;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace StatisticalAnalysis.Web.UI_ProcessHistoryTrend
{
    public partial class HistoryTrend_Process : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

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
            tagTable.Columns.Add("LevelCode", typeof(string));
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