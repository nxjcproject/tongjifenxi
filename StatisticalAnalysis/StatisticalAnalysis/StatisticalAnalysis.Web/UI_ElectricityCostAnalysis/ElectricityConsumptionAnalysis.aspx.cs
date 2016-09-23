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
    public partial class ElectricityConsumptionAnalysis : WebStyleBaseForEnergy.webStyleBase
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

            }
        }

        [WebMethod]
        public static string GetElectricityConsumptionVariableByOrganizationIdWithComoboboxFormat(string organizationId)
        {
            DataTable dt = ElectricityConsumptionAnalysisService.GetElectricityConsumptionVariableByOrganizationId(organizationId);

            return EasyUIJsonParser.ComboboxJsonParser.DataTableToJson(dt);
        }

        [WebMethod]
        public static string GetElectricityConsumptionAnalysisChart(string organizationId, string variableId, string analysisType, string startTime, string endTime)
        {
            DateTime begin = DateTime.Parse(startTime);
            DateTime end = DateTime.Parse(endTime);
            string xaxisLabel = "";

            DataTable electricityConsumptionTable = new DataTable();

            switch (analysisType)
            {
                case "yearly":
                    xaxisLabel = "月";
                    electricityConsumptionTable = ElectricityConsumptionAnalysisService.GetElectricityPVFUsageYearly(organizationId, variableId, end.Year);
                    break;
                case "monthly":
                    xaxisLabel = "月-日";
                    electricityConsumptionTable = ElectricityConsumptionAnalysisService.GetElectricityPVFUsageMonthly(organizationId, variableId, end.Year, end.Month);
                    break;
                case "custom":
                    xaxisLabel = "年-月-日";
                    electricityConsumptionTable = ElectricityConsumptionAnalysisService.GetElectricityPVFUsageCustom(organizationId, variableId, begin, end);
                    break;
                default:
                    break;
            }

            IList<string> colNames = new List<string>();
            foreach (DataColumn dc in electricityConsumptionTable.Columns)
            {
                colNames.Add(dc.ColumnName.ToString());
            }

            string json = EasyUIJsonParser.ChartJsonParser.GetGridChartJsonString(electricityConsumptionTable, colNames.ToArray(), new string[] { "甲班电耗", "乙班电耗", "丙班电耗" }, xaxisLabel, "kW·h/t", 1);

            return json;
        }
    }
}