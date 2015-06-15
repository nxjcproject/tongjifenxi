<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProcessSelector.aspx.cs" Inherits="StatisticalAnalysis.Web.UI_BasicHistoryTrend.ProcessSelector" %>

<%@ Register Src="~/UI_WebUserControls/OrganizationSelector/OrganisationTree.ascx" TagPrefix="uc1" TagName="OrganisationTree_ProductionLine" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>工序选择器</title>
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/gray/easyui.css" />
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/icon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtIcon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtCss.css" />
    <link type="text/css" rel="stylesheet" href="/UI_ProcessHistoryTrend/css/page/ProcessSelector.css" />

    <script type="text/javascript" src="/lib/ealib/jquery.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/jquery.easyui.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/easyui-lang-zh_CN.js" charset="utf-8"></script>

    <!--[if lt IE 8 ]><script type="text/javascript" src="/js/common/json2.min.js"></script><![endif]-->

    <script type="text/javascript">

        function onTagItemSelect(row) {
            if (typeof (window.parent.onTagItemSelect) == "function") {
                var m_TagName = row.Name;
                row.OrganizationID = $('#organizationId').val();
                var m_TreeData = $('#processTable').treegrid('getParent', row.id)
                while (m_TreeData != null && m_TreeData != undefined && m_TreeData != NaN) {
                    m_TagName = m_TreeData.Name + '>>' + m_TagName;
                    m_TreeData = $('#processTable').treegrid('getParent', m_TreeData.id);
                }
                row.Name = m_TagName;
                window.parent.onTagItemSelect(row);
            }
        }

        // 获取双击组织机构时的组织机构信息

        function onOrganisationTreeClick(node) {

            // 设置组织机构ID
            // organizationId为其它任何函数提供当前选中的组织机构ID

            $('#organizationId').val(node.OrganizationId);

            // 设置组织机构名称
            // 用于呈现，在界面上显示当前的组织机构名称

            $('#txtOrganization').textbox('setText', node.text);
        }

        function query() {
            loadProcess();
        }


        // 所有公式组
        function loadProcess() {
            var organizationId = $('#organizationId').val();
            var queryUrl = 'ProcessSelector.aspx/GetProcessWithTreeGridFormat';
            var dataToSend = '{organizationId: "' + organizationId + '"}';

            $.ajax({
                type: "POST",
                url: queryUrl,
                data: dataToSend,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {
                    initializeProcessTable(jQuery.parseJSON(msg.d));
                }
            });
        }

        function initializeProcessTable(data) {
            $('#processTable').treegrid({
                data: data,
                dataType: "json"
            });
        }
    </script>
</head>

<body class="easyui-layout" data-options="fit:true,border:false">
    <!-- 左侧组织机构目录树开始 -->
    <div data-options="region:'west',border:false" style="width: 210px;">
        <uc1:OrganisationTree_ProductionLine runat="server" id="OrganisationTree_ProductionLine" />
    </div>
    <!-- 左侧组织机构目录树结束 -->
    <div data-options="region:'center',border:false">
        <div class="easyui-layout" data-options="fit:true,border:false" style="margin-left:5px;">
            <!-- 工具栏开始 -->
            <div class="queryPanel" data-options="region:'north', border:true, collapsible:false, split:false" style="height:50px;">
                组织机构：
                <input id="txtOrganization" class="easyui-textbox" data-options="editable:false" style="width: 100px;" />
                <input id="organizationId" readonly="true" style="display:none;"/> | 
                <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="query();">搜索</a>
            </div>
            <!-- 工具栏结束 -->
            <div data-options="region:'center', border:true, collapsible:false, split:false" style="height:50px; padding:0px;">
                <!-- 工序表格开始 -->
                <table id="processTable"class="easyui-treegrid"
			            data-options="
				            iconCls: 'icon-edit',
				            rownumbers: true,
				            animate: true,
				            collapsible: true,
				            fitColumns: true,
				            idField: 'LevelCode',
				            treeField: 'Name',
                            fit: true,
                            onDblClickRow: onTagItemSelect
			            ">
		            <thead>
			            <tr>
                            <th data-options="field:'LevelCode',hidden:true">层次码</th>
				            <th data-options="field:'Name',width:100,editor:'text'">工序名称</th>
			            </tr>
		            </thead>
                </table>
                <!-- 工序表格结束 -->
            </div>
        </div>
    </div>
</body>
</html>
