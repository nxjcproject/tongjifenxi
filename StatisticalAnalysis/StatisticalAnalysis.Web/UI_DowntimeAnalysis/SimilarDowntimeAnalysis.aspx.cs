using EasyUIJsonParser;
using StatisticalAnalysis.Service.DowntimeAnalysis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace StatisticalAnalysis.Web.UI_DowntimeAnalysis
{
    public partial class SimilarDowntimeAnalysis : WebStyleBaseForEnergy.webStyleBase
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
                this.OrganisationTree_ProductionLine.PageName = "AlarmCountAnalysis.aspx";                                     //向web用户控件传递当前调用的页面名称

            }
        }

        /// <summary>
        /// 获取停机记录原因
        /// </summary>
        /// <returns></returns>
        [WebMethod]
        public static string GetMachineHaltReasonsWithCombotreeFormat()
        {
            DataTable dt = DowntimeCountAnalysisService.GetMachineHaltReasons();
            return TreeJsonParser.DataTableToJsonByLevelCode(dt, "MachineHaltReasonID", "ReasonText");
        }
    }
}