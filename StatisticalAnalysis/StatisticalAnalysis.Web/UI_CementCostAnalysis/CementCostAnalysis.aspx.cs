using StatisticalAnalysis.Service.CementCostAnalysis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace StatisticalAnalysis.Web.UI_CementCostAnalysis
{
    public partial class CementCostAnalysis : WebStyleBaseForEnergy.webStyleBase
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
                this.OrganisationTree_ProductionLine.PageName = "ElectricityConsumptionAnalysis.aspx";                                     //向web用户控件传递当前调用的页面名称
                this.OrganisationTree_ProductionLine.OrganizationTypeItems.Add("水泥磨");

            }
        }
        [WebMethod]
        public static string GetCementTypesList()
        {
            DataTable table = CementCostAnalysisService.GetCementTypes();
            string Json = EasyUIJsonParser.ComboboxJsonParser.DataTableToJson(table);            
            return Json;
            
        }
        [WebMethod]
        public static string GetCementCostAnalysisChart(string organizationId, string analysisType, string startTime, string endTime,string cementType)
        {
            DateTime begin = DateTime.Parse(startTime);
            DateTime end = DateTime.Parse(endTime);
            string xaxisLabel = "";

            DataTable coalConsumptionTable = new DataTable();
            
            switch (analysisType)
            {
                case "yearly":
                    xaxisLabel = "月";
                    CementCostFSTAnalysisTableHelper.SetElcPrice(organizationId);
                    coalConsumptionTable = CementCostAnalysisService.GetCementCostPVFYearly(organizationId, end.Year,cementType);
                    break;
                case "monthly":
                    xaxisLabel = "月-日";
                    CementCostFSTAnalysisTableHelper.SetElcPrice(organizationId);
                    coalConsumptionTable = CementCostAnalysisService.GetCementCostPVFMonthly(organizationId, end.Year, end.Month, cementType);
                    break;
                case "custom":
                    xaxisLabel = "年-月-日";
                    CementCostFSTAnalysisTableHelper.SetElcPrice(organizationId);
                    coalConsumptionTable = CementCostAnalysisService.GetCementCostPVFCustom(organizationId, begin, end, cementType);
                    break;
                default:
                    break;
            }

            IList<string> colNames = new List<string>();
            foreach (DataColumn dc in coalConsumptionTable.Columns)
            {
                colNames.Add(dc.ColumnName.ToString());
            }

            string json = EasyUIJsonParser.ChartJsonParser.GetGridChartJsonString(coalConsumptionTable, colNames.ToArray(), new string[] { "峰期成本", "谷期成本", "平期成本","平均成本" }, xaxisLabel, "kg/t", 1);

            return json;
        }
    }
}