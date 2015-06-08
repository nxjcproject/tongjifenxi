using StatisticalAnalysis.Service.HorizontallyAlarmAnalysis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace StatisticalAnalysis.Web.UI_HorizontallyAlarmAnalysis
{
    public partial class HorizontallyEnergyAlarmAnalysis : WebStyleBaseForEnergy.webStyleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
            if (!IsPostBack)
            {
#if DEBUG
                ////////////////////调试用,自定义的数据授权
                List<string> m_DataValidIdItems = new List<string>() { "zc_nxjc_byc","zc_nxjc_qtx" };
                AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
#elif RELEASE
#endif
                this.OrganisationTree_ProductionLine.Organizations = GetDataValidIdGroup("ProductionOrganization");                 //向web用户控件传递数据授权参数
                this.OrganisationTree_ProductionLine.PageName = "UI_HorizontallyAlarmAnalysis.aspx";                                     //向web用户控件传递当前调用的页面名称

            }
        }
        [WebMethod]
        public static string GetLableName()
        {
            string json = LabelNameService.GetLableNames();
            return json;
        }

        [WebMethod]
        public static string GetData(string levelCodeString,string startTime,string endTime,string labelLength)
        {
            string[] levelCodeList = levelCodeString.Split(',');
            DataTable table = HorizontallyEnergyAlarmAnalysisService.GetGridData(levelCodeList, startTime, endTime,labelLength);
            IList<string> colList=new List<string>();
            foreach(DataColumn cName in table.Columns){
                colList.Add(cName.ColumnName);
            }
            string json = EasyUIJsonParser.ChartJsonParser.GetGridChartJsonString(table, colList.ToArray(),new string[]{ "报警次数"}, "", "", 1);
            return json;
        }
    }
}