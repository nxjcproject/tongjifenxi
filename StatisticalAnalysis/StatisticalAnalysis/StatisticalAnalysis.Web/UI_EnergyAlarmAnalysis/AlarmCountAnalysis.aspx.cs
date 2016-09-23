using StatisticalAnalysis.Service.EnergyAlarmAnalysis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace StatisticalAnalysis.Web.UI_EnergyAlarmAnalysis
{
    public partial class AlarmCountAnalysis : WebStyleBaseForEnergy.webStyleBase
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
        public static string GetAlarmChart(string organizationId, string analysisType, string startTime, string endTime, string alarmType = "")
        {
            DateTime begin = DateTime.Parse(startTime);
            DateTime end = DateTime.Parse(endTime);
            string xaxisLabel = "";

            DataTable alarmCountTable = new DataTable();

            switch (analysisType)
            {
                case "yearly":
                    xaxisLabel = "月";
                    alarmCountTable = AlarmCountAnalysisService.GetAlarmLogYearly(organizationId, end.Year, alarmType);
                    break;
                case "monthly":
                    xaxisLabel = "月-日";
                    alarmCountTable = AlarmCountAnalysisService.GetAlarmLogMonthly(organizationId, end.Year, end.Month, alarmType);
                    break;
                case "custom":
                    xaxisLabel = "年-月-日";
                    alarmCountTable = AlarmCountAnalysisService.GetAlarmLogCustom(organizationId, begin, end, alarmType);
                    break;
                default:
                    break;
            }

            IList<string> colNames = new List<string>();
            foreach (DataColumn dc in alarmCountTable.Columns)
            {
                colNames.Add(dc.ColumnName.ToString());
            }

            string json = EasyUIJsonParser.ChartJsonParser.GetGridChartJsonString(alarmCountTable, colNames.ToArray(), new string[] { "报警次数" }, xaxisLabel, "次数", 1);

            return json;
        }
    }
}