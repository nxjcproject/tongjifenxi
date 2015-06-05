<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HorizontallyMachineHaltAnalysis.aspx.cs" Inherits="StatisticalAnalysis.Web.UI_HorizontallyAlarmAnalysis.HorizontallyDowntimeAnalysis" %>

<%@ Register Src="~/UI_WebUserControls/OrganizationSelector/OrganisationTree.ascx" TagPrefix="uc1" TagName="OrganisationTree_ProductionLine" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <div class="easyui-layout" data-options="fit:true,border:false">
        <!-- 左侧组织机构目录树开始 -->
        <div class="easyui-panel" data-options="region:'west',border:false" style="width: 230px;">
            <uc1:OrganisationTree_ProductionLine runat="server" ID="OrganisationTree_ProductionLine" />
        </div>
        <div class="easyui-panel" data-options="region:'center',border:false">
        </div>
    </div>
    <form id="form1" runat="server">
    </form>
</body>
</html>
