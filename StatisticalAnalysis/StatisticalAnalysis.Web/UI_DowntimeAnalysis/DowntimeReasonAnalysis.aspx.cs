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
    public partial class DowntimeReasonAnalysis : WebStyleBaseForEnergy.webStyleBase
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
            DataTable m_EquipmentCommonInfoTable = StatisticalAnalysis.Service.DowntimeAnalysis.DowntimeReasonAnalysis.GetEquipmentCommonInfo(m_DataValidIdGroup);
            if (m_EquipmentCommonInfoTable != null)
            {
                m_EquipmentCommonInfoString = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(m_EquipmentCommonInfoTable, new string[] { "id", "text" });
            }
            return m_EquipmentCommonInfoString;
        }
        [WebMethod]
        public static string GetLevelCodeInfo()
        {
            string m_LevelCodeInfoString = "{\"rows\":[],\"total\":0}";
            List<string> m_DataValidIdGroup = GetDataValidIdGroup("ProductionOrganization");
            DataTable m_LevelCodeInfoTable = StatisticalAnalysis.Service.DowntimeAnalysis.DowntimeReasonAnalysis.GetLevelCodeInfo(m_DataValidIdGroup);
            if (m_LevelCodeInfoTable != null)
            {
                m_LevelCodeInfoString = EasyUIJsonParser.TreeJsonParser.DataTableToJsonByLevelCode(m_LevelCodeInfoTable,"LevelCode","Name");
            }
            return m_LevelCodeInfoString;
        }
        [WebMethod]
        public static string GetHaltReasonTypeInfo()
        {
            DataTable m_ReasonTypeTable = StatisticalAnalysis.Service.DowntimeAnalysis.DowntimeReasonAnalysis.GetReasonTypeInfo();
            string m_ReasonInfoString = EasyUIJsonParser.TreeJsonParser.DataTableToJsonByLevelCodeWithIdColumn(m_ReasonTypeTable, "LevelCode", "Id", "Name");
            return m_ReasonInfoString;
        }
        [WebMethod]
        public static string GetHaltReasonStaticsChart(string myStartTime, string myEndTime, string myEquipmentCommonId, string myStaticsMethod, string myLevelCode, string myReasonType)
        {
            List<string> m_DataValidIdGroup = GetDataValidIdGroup("ProductionOrganization");
            DataTable m_ResultTable = StatisticalAnalysis.Service.DowntimeAnalysis.DowntimeReasonAnalysis.GetHaltReasonStatics(myStartTime, myEndTime, myEquipmentCommonId, myStaticsMethod, myLevelCode, myReasonType, m_DataValidIdGroup);

            string m_ReturnString = EasyUIJsonParser.DataGridJsonParser.DataTableToJson(m_ResultTable);
            return m_ReturnString;
        }
    }
}