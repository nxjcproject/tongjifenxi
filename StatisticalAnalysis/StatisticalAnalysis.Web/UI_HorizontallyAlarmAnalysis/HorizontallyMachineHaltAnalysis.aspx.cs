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
    public partial class HorizontallyMachineHaltAnalysis : WebStyleBaseForEnergy.webStyleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
            if (!IsPostBack)
            {
#if DEBUG
                ////////////////////调试用,自定义的数据授权
                List<string> m_DataValidIdItems = new List<string>() { "zc_nxjc_byc", "zc_nxjc_qtx" };
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
        //获取停机原因信息
        [WebMethod]
        public static string GetMachineHaltReason()
        {
            DataTable table= HorizontallyMachineHaltAnalysisService.GetMachineHaltReasonInfo();
            string json = EasyUIJsonParser.TreeJsonParser.DataTableToJsonByLevelCode(table, "MachineHaltReasonID", "ReasonText");          
            return json;
        }
        /// <summary>
        /// 统计停机次数
        /// </summary>
        /// <param name="levelCodeString"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="labelLength"></param>
        /// <param name="type"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        [WebMethod]
        public static string GetMachineHaltCount(string levelCodeString, string startTime, string endTime, string labelLength, string type, string reason)
        {
            string[] levelCodeList = levelCodeString.Split(',');
            DataTable table = HorizontallyMachineHaltAnalysisService.GetDataService(levelCodeList, startTime, endTime, labelLength, type, reason);
            IList<string> colList=new List<string>();
            foreach(DataColumn cName in table.Columns){
                colList.Add(cName.ColumnName);
            }
            string json = EasyUIJsonParser.ChartJsonParser.GetGridChartJsonString(table, colList.ToArray(), new string[] { "报警次数" }, "", "", 1);
            return json;
        }
    }
}