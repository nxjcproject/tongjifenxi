using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.Services;

namespace StatisticalAnalysis.Web.UI_DowntimeAnalysis
{
    public partial class DowntimeHorizontallyAnalysis : WebStyleBaseForEnergy.webStyleBase
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
            }
        }
        [WebMethod]
        public static string GetEquipmentCommonInfo()
        {
            string m_EquipmentCommonInfoString = "{\"rows\":[],\"total\":0}";
            List<string> m_DataValidIdGroup = GetDataValidIdGroup("ProductionOrganization");
            DataTable m_EquipmentCommonInfoTable = StatisticalAnalysis.Service.DowntimeAnalysis.DowntimeHorizontallyAnalysis.GetEquipmentCommonInfo(m_DataValidIdGroup);
            if(m_EquipmentCommonInfoTable != null)
            {
                m_EquipmentCommonInfoString = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(m_EquipmentCommonInfoTable, new string[] { "id", "text" });
            }
            return m_EquipmentCommonInfoString;
        }
        [WebMethod]
        public static string GetHaltReasonStaticsChart(string myStartTime, string myEndTime, string myEquipmentCommonId, string myStaticsMethod, string myStaticsRange, string myHaltTypeId, string myReasonTypeId)
        {
            List<string> m_DataValidIdGroup = GetDataValidIdGroup("ProductionOrganization");
            DataTable m_ResultTable = StatisticalAnalysis.Service.DowntimeAnalysis.DowntimeHorizontallyAnalysis.GetHaltReasonStatics(myStartTime, myEndTime, myEquipmentCommonId, myStaticsMethod, myStaticsRange, myHaltTypeId, myReasonTypeId, m_DataValidIdGroup);

            string m_ReturnString = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(m_ResultTable);
            return m_ReturnString;
        }
    }
}