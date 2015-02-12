using StatisticalAnalysis.Service.DowntimeAnalysis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace StatisticalAnalysis.Web.UI_DowntimeAnalysis
{
    public partial class ComprehensiveAnalysis : WebStyleBaseForEnergy.webStyleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
            if (!IsPostBack)
            {
#if DEBUG
                ////////////////////调试用,自定义的数据授权
                List<string> m_DataValidIdItems = new List<string>() { "zc_nxjc_qtx" };
                AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
#elif RELEASE
#endif
                this.OrganisationTree_ProductionLine.Organizations = GetDataValidIdGroup("ProductionOrganization");                 //向web用户控件传递数据授权参数
                this.OrganisationTree_ProductionLine.PageName = "ComprehensiveAnalysis.aspx";                                     //向web用户控件传递当前调用的页面名称
                this.OrganisationTree_ProductionLine.LeveDepth = 5;

            }

        }


        [WebMethod]
        public static string GetDowntimCount(string organizationId, string startTime, string endTime)
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

            // 获取生产线的报警计数
            DataTable downtimeCountTable = DowntimeComprehensiveAnalysisService.GetDowntimeCountGroupByOrganization(organizationId, DateTime.Parse(startTime), DateTime.Parse(endTime));

            // 当生产线行数不为 0 时， 生成总报警数
            if (downtimeCountTable.Rows.Count != 0)
            {
                int total = 0;

                // 合计计算
                foreach (DataRow dr in downtimeCountTable.Rows)
                    total += (int)dr["Count"];

                // 添加合计行
                DataRow totalRow = downtimeCountTable.NewRow();
                totalRow["Name"] = "总停机数";
                totalRow["Count"] = total;
                downtimeCountTable.Rows.Add(totalRow);
            }

            return EasyUIJsonParser.DataGridJsonParser.DataTableToJson(downtimeCountTable);
        }

        [WebMethod]
        public static string GetDowntimeChart(string organizationId, string startTime, string endTime)
        {
            // 获取生产线的报警计数
            DataTable downtimeCountTable = DowntimeComprehensiveAnalysisService.GetDowntimeCountGroupByAlarmType(organizationId, DateTime.Parse(startTime), DateTime.Parse(endTime));

            IList<string> rowNames = new List<string>();
            foreach (DataRow dr in downtimeCountTable.Rows)
            {
                rowNames.Add(dr["Name"].ToString());
            }

            downtimeCountTable.Columns.Remove("Name");

            string json = EasyUIJsonParser.ChartJsonParser.GetGridChartJsonString(downtimeCountTable, new string[] { "值" }, rowNames.ToArray(), "A", "B", 1);

            return json;
        }
    }
}