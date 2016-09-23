using StatisticalAnalysis.Service.CoalCostAnalysis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace StatisticalAnalysis.Web.UI_CoalCostAnalysis
{
    public partial class CoalConsumptionAnalysis : WebStyleBaseForEnergy.webStyleBase
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
                this.OrganisationTree_ProductionLine.PageName = "ElectricityConsumptionAnalysis.aspx";                                     //向web用户控件传递当前调用的页面名称
                this.OrganisationTree_ProductionLine.OrganizationTypeItems.Add("熟料");
                //this.OrganisationTree_ProductionLine.OrganizationTypeItems.Add("余热发电");
            }
        }


        [WebMethod]
        public static string GetCoalConsumptionAnalysisChart(string organizationId, string analysisType, string startTime, string endTime)
        {
            DateTime begin = DateTime.Parse(startTime);
            DateTime end = DateTime.Parse(endTime);
            string xaxisLabel = "";

            DataTable coalConsumptionTable = new DataTable();

            switch (analysisType)
            {
                case "yearly":
                    xaxisLabel = "月";
                    coalConsumptionTable = CoalConsumptionAnalysisService.GetCoalConsumptionPVFYearly(organizationId, end.Year);
                    break;
                case "monthly":
                    xaxisLabel = "月-日";
                    coalConsumptionTable = CoalConsumptionAnalysisService.GetCoalConsumptionPVFMonthly(organizationId, end.Year, end.Month);
                    break;
                case "custom":
                    xaxisLabel = "年-月-日";
                    coalConsumptionTable = CoalConsumptionAnalysisService.GetCoalConsumptionPVFCustom(organizationId, begin, end);
                    break;
                default:
                    break;
            }

            IList<string> colNames = new List<string>();
            foreach (DataColumn dc in coalConsumptionTable.Columns)
            {
                colNames.Add(dc.ColumnName.ToString());
            }

            string json = EasyUIJsonParser.ChartJsonParser.GetGridChartJsonString(coalConsumptionTable, colNames.ToArray(), new string[] { "甲班煤耗", "乙班煤耗", "丙班煤耗" }, xaxisLabel, "kg/t", 1);

            return json;
        }
    }
}