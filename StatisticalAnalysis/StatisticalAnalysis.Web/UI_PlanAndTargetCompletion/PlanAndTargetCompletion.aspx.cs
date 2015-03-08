using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace StatisticalAnalysis.Web.UI_PlanAndTargetCompletion
{
    public partial class PlanAndTargetCompletion :  WebStyleBaseForEnergy.webStyleBase
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
                this.OrganisationTree_ProductionLine.PageName = "PlanAndTargetCompletion.aspx";                                     //向web用户控件传递当前调用的页面名称

            }
        }
        [WebMethod]
        public static string GetItemList(string type)
        {
            DataTable table = StatisticalAnalysis.Service.PlanAndActual.PlanAndTargetCompletionService.GetItemList(type);
            return EasyUIJsonParser.ComboboxJsonParser.DataTableToJson(table);
        }
        [WebMethod]
        public static string GetChart(string organizationId,string item,string date)
        {
            DataTable table = StatisticalAnalysis.Service.PlanAndActual.PlanAndTargetCompletionService.GetChartTable(organizationId, item, date);
            string[] columnsName=new string[12];
            for(int i=0;i<12;i++)
            {
                columnsName[i]=table.Columns[i].ColumnName;
            }
            string[] rowsName={"完成情况","计划"};
            string Json = EasyUIJsonParser.ChartJsonParser.GetGridChartJsonString(table, columnsName, rowsName, "时间", "", 1);
            return Json;
        }
    }
}