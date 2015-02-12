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
    public partial class DowntimeCountAnalysis : WebStyleBaseForEnergy.webStyleBase
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
                this.OrganisationTree_ProductionLine.PageName = "AlarmCountAnalysis.aspx";                                     //向web用户控件传递当前调用的页面名称

            }
        }

        [WebMethod]
        public static string GetDowntimeChart(string organizationId, string analysisType, string startTime, string endTime, string reasonText = "")
        {
            DateTime begin = DateTime.Parse(startTime);
            DateTime end = DateTime.Parse(endTime);
            string xaxisLabel = "";

            DataTable downtimeCountTable = new DataTable();

            switch (analysisType)
            {
                case "yearly":
                    xaxisLabel = "月";
                    downtimeCountTable = DowntimeCountAnalysisService.GetDowntimeLogYearly(organizationId, end.Year, reasonText);
                    break;
                case "monthly":
                    xaxisLabel = "月-日";
                    downtimeCountTable = DowntimeCountAnalysisService.GetDowntimeLogMonthly(organizationId, end.Year, end.Month, reasonText);
                    break;
                case "custom":
                    xaxisLabel = "年-月-日";
                    downtimeCountTable = DowntimeCountAnalysisService.GetDowntimeLogCustom(organizationId, begin, end, reasonText);
                    break;
                default:
                    break;
            }

            IList<string> colNames = new List<string>();
            foreach (DataColumn dc in downtimeCountTable.Columns)
            {
                colNames.Add(dc.ColumnName.ToString());
            }

            string json = EasyUIJsonParser.ChartJsonParser.GetGridChartJsonString(downtimeCountTable, colNames.ToArray(), new string[] { "停机次数" }, xaxisLabel, "次数", 1);

            return json;
        }
    }
}