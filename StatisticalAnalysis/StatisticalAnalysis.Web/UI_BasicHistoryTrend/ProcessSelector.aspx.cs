using EasyUIJsonParser;
using StatisticalAnalysis.Service.BasicHistoryTrend;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace StatisticalAnalysis.Web.UI_BasicHistoryTrend
{
    public partial class ProcessSelector : WebStyleBaseForEnergy.webStyleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            base.InitComponts();
            if (!IsPostBack)
            {
                ////////////////////调试用,自定义的数据授权
#if DEBUG
                List<string> m_DataValidIdItems = new List<string>() { "zc_nxjc_byc_byf" };
                AddDataValidIdGroup("ProductionOrganization", m_DataValidIdItems);
#elif RELEASE
#endif
                this.OrganisationTree_ProductionLine.Organizations = GetDataValidIdGroup("ProductionOrganization");                 //向web用户控件传递数据授权参数
                this.OrganisationTree_ProductionLine.PageName = "ComprehensiveAnalysis.aspx";                                     //向web用户控件传递当前调用的页面名称
            }
        }

        [WebMethod]
        public static string GetProcessWithTreeGridFormat(string organizationId)
        {
            DataTable dt = ProcessService.GetSystemProcessByOrganizationID(organizationId);
            return TreeGridJsonParser.DataTableToJsonByLevelCode(dt, "LevelCode");
        }
    }
}