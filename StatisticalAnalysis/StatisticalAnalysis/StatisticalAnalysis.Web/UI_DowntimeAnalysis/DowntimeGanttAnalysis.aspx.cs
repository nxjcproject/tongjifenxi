using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.Services;
using EasyUIJsonParser;
namespace StatisticalAnalysis.Web.UI_DowntimeAnalysis
{
    public partial class DowntimeGanttAnalysis : WebStyleBaseForEnergy.webStyleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
            if (!IsPostBack)
            {
#if DEBUG
                ////////////////////调试用,自定义的数据授权
                List<string> m_DataValidIdItems = new List<string>() { "zc_nxjc_qtx", "zc_nxjc_byc", "zc_nxjc_tsc","zc_nxjc_szsc" };
                AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
#elif RELEASE
#endif
                this.OrganisationTree_ProductionLine.Organizations = GetDataValidIdGroup("ProductionOrganization");                 //向web用户控件传递数据授权参数
                this.OrganisationTree_ProductionLine.PageName = "DowntimeGanttAnalysis.aspx";                                     //向web用户控件传递当前调用的页面名称
                this.OrganisationTree_ProductionLine.LeveDepth = 5;
            }
        }
        /// <summary>
        /// 获取停机记录原因
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public static string GetEquipmentInfo(string myOrganizationId)
        {
            DataTable dt = StatisticalAnalysis.Service.DowntimeAnalysis.DowntimeGanttAnalysis.GetEquipmentInfo(myOrganizationId);
            return EasyUIJsonParser.DataGridJsonParser.DataTableToJson(dt);
        }
        [WebMethod]
        public static string GetResonType(string myReasonTypeId)
        {
            DataTable dt = StatisticalAnalysis.Service.DowntimeAnalysis.DowntimeGanttAnalysis.GetResonType(myReasonTypeId);
            return EasyUIJsonParser.DataGridJsonParser.DataTableToJson(dt);
        }
        [WebMethod]
        public static string GetHaltReasonStaticsGanttChart(string myStartTime, string myEndTime, string myEquipmentId, string myDisplayOrder, string myHaltTypeId, string myReasonTypeId, string myOrganizationId)
        {
            bool m_AllReason = false;
            DataTable m_MachineHaltLogTable = null;
            if (myHaltTypeId == "null")
            {
                m_MachineHaltLogTable = StatisticalAnalysis.Service.DowntimeAnalysis.DowntimeGanttAnalysis.GetHaltReasonStaticsGanttChartNoReason(myStartTime, myEndTime, myEquipmentId, myOrganizationId);
            }
            else
            {
                m_MachineHaltLogTable = StatisticalAnalysis.Service.DowntimeAnalysis.DowntimeGanttAnalysis.GetHaltReasonStaticsGanttChart(myStartTime, myEndTime, myEquipmentId, myHaltTypeId, myReasonTypeId, myOrganizationId);
            }
               
            DataTable m_MachineHaltReasonTable = StatisticalAnalysis.Service.DowntimeAnalysis.DowntimeGanttAnalysis.GetMachineHaltReason();
            if (myReasonTypeId == "All")
            {
                m_AllReason = true;
            }
            string m_MachineHaltLogChartString = StatisticalAnalysis.Service.DowntimeAnalysis.DowntimeGanttAnalysis.GetMachineHaltLogChartString(m_MachineHaltLogTable, m_MachineHaltReasonTable, m_AllReason, myDisplayOrder);
            return m_MachineHaltLogChartString;
        }
    }

}