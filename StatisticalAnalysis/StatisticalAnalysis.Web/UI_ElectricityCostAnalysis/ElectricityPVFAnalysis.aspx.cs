using StatisticalAnalysis.Service.ElectricityCostAnalysis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace StatisticalAnalysis.Web.UI_ElectricityCostAnalysis
{
    public partial class ElectricityPVFAnalysis : WebStyleBaseForEnergy.webStyleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
            if (!IsPostBack)
            {
#if DEBUG
                ////////////////////调试用,自定义的数据授权
                List<string> m_DataValidIdItems = new List<string>() { "zc_nxjc_byc" };
                AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
#elif RELEASE
#endif
                this.OrganisationTree_ProductionLine.Organizations = GetDataValidIdGroup("ProductionOrganization");                 //向web用户控件传递数据授权参数
                this.OrganisationTree_ProductionLine.PageName = "ElectricityPVFAnalysis.aspx";                                     //向web用户控件传递当前调用的页面名称

            }
        }


        [WebMethod]
        public static string GetElectricityPVFAnalysisChart(string organizationId, string analysisType, string startTime, string endTime)
        {
            DateTime begin = DateTime.Parse(startTime);
            DateTime end = DateTime.Parse(endTime);
            string xaxisLabel = "";

            DataTable electricityPVFUsageTable = new DataTable();

            switch (analysisType)
            {
                case "yearly":
                    xaxisLabel = "月";
                    electricityPVFUsageTable = ElectricityPVFUsageAnalysisService.GetElectricityPVFUsageYearly(organizationId, end.Year);
                    break;
                case "monthly":
                    xaxisLabel = "月-日";
                    electricityPVFUsageTable = ElectricityPVFUsageAnalysisService.GetElectricityPVFUsageMonthly(organizationId, end.Year, end.Month);
                    break;
                case "custom":
                    xaxisLabel = "年-月-日";
                    electricityPVFUsageTable = ElectricityPVFUsageAnalysisService.GetElectricityPVFUsageCustom(organizationId, begin, end);
                    break;
                default:
                    break;
            }

            IList<string> colNames = new List<string>();
            foreach (DataColumn dc in electricityPVFUsageTable.Columns)
            {
                colNames.Add(dc.ColumnName.ToString());
            }

            return EasyUIJsonParser.ChartJsonParser.GetGridChartJsonString(electricityPVFUsageTable, colNames.ToArray(), new string[] { "峰期用电成本", "谷期用电成本", "平期用电成本" }, xaxisLabel, "元", 1);
        }
    }
}