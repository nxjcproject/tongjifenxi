<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HistoryTrend_Ammeters.aspx.cs" Inherits="StatisticalAnalysis.Web.UI_BasicHistoryTrend.HistoryTrend_Ammeters" %>

<%@ Register Src="~/UI_WebUserControls/OrganizationSelector/OrganisationTree.ascx" TagPrefix="uc1" TagName="OrganisationTree_ProductionLine" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>电表历史趋势</title> 
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/gray/easyui.css" />
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/icon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtIcon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtCss.css" />

    <link rel="stylesheet" type="text/css" href="/lib/pllib/themes/jquery.jqplot.min.css" />
    <link type="text/css" rel="stylesheet" href="/lib/pllib/syntaxhighlighter/styles/shCoreDefault.min.css" />
    <link type="text/css" rel="stylesheet" href="/lib/pllib/syntaxhighlighter/styles/shThemejqPlot.min.css" />
    <!--    <link type="text/css" rel="stylesheet" href="/css/common/charts.css" />-->
    <link type="text/css" rel="stylesheet" href="/css/common/NormalPage.css" />


    <script type="text/javascript" src="/lib/ealib/jquery.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/jquery.easyui.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/easyui-lang-zh_CN.js" charset="utf-8"></script>

    <!--[if gt IE 8]><script type="text/javascript" src="/lib/ealib/extend/easyUI.WindowsOverrange.js" charset="utf-8"></script>-->
    <!--[if !IE]><!-->
    <script type="text/javascript" src="/lib/ealib/extend/easyUI.WindowsOverrange.js" charset="utf-8"></script>
    <!--<![endif]-->

    <!--[if lt IE 9]><script type="text/javascript" src="/lib/pllib/excanvas.js"></script><![endif]-->
    <script type="text/javascript" src="/lib/pllib/jquery.jqplot.min.js"></script>

    <script type="text/javascript" src="/lib/pllib/plugins/jqplot.trendline.min.js"></script>
    <script type="text/javascript" src="/lib/pllib/plugins/jqplot.barRenderer.min.js"></script>
    <script type="text/javascript" src="/lib/pllib/plugins/jqplot.pieRenderer.min.js"></script>

    <!-- Additional plugins go here -->
    <script type="text/javascript" src="/lib/pllib/plugins/jqplot.canvasAxisTickRenderer.min.js"></script>
    <script type="text/javascript" src="/lib/pllib/plugins/jqplot.categoryAxisRenderer.min.js"></script>
    <script type="text/javascript" src="/lib/pllib/plugins/jqplot.canvasTextRenderer.min.js"></script>
    <script type="text/javascript" src="/lib/pllib/plugins/jqplot.canvasAxisLabelRenderer.min.js"></script>
    <script type="text/javascript" src="/lib/pllib/plugins/jqplot.dateAxisRenderer.min.js"></script>
    <script type="text/javascript" src="/lib/pllib/plugins/jqplot.pointLabels.min.js"></script>
    <script type="text/javascript" src="/lib/pllib/plugins/jqplot.enhancedLegendRenderer.min.js"></script>
    <script type="text/javascript" src="/lib/pllib/plugins/jqplot.canvasOverlay.min.js"></script> 
    <script type="text/javascript" src="/lib/pllib/plugins/jqplot.cursor.min.js"></script>
    <script type="text/javascript" src="/lib/pllib/plugins/jqplot.highlighter.min.js"></script>
    <!--[if lt IE 8 ]><script type="text/javascript" src="/lib/pllib/plugins/jqplot.json2.min"></script><![endif]-->


    <!--[if lt IE 8 ]><script type="text/javascript" src="/js/common/json2.min.js"></script><![endif]-->

    <script type="text/javascript" src="/js/common/components/Charts.js" charset="utf-8"></script>
    <script type="text/javascript" src="/js/common/components/DataGrid.js" charset="utf-8"></script>
    <script type="text/javascript" src="/js/common/components/WindowsDialog.js" charset="utf-8"></script>
    <script type="text/javascript" src="/js/common/components/GridChart.js" charset="utf-8"></script>

    <script type="text/javascript" src="/js/common/format/DateTimeFormat.js" charset="utf-8"></script>

    <script type="text/javascript" src="/js/common/PrintFile.js" charset="utf-8"></script>

    <script type="text/javascript" src="js/page/HistoryTrend_Ammeters.js" charset="utf-8"></script>
</head>
<body>
    <div class="easyui-layout" data-options="fit:true,border:false">
        <div class="easyui-panel" data-options="region:'west',border:true" style="width: 230px;">
            <div id="MainSelect_Toolbar" style="display: none; text-align: center; padding-top: 10px;">
                <table style="width: 220px;">
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td style="width: 60px; height: 30px;">开始时间</td>
                                    <td style="text-align: left;">
                                        <input id="StartTime" class="easyui-datetimebox" data-options="required:true, validType:'md[\'2014-07-28 10:10:10\']',editable:false" value="2014-07-28 12:13:56" style="width: 150px" />
                                    </td>
                                </tr>
                                <tr>
                                    <td style="height: 30px;">终止时间</td>
                                    <td style="text-align: left;">
                                        <input id="EndTime" class="easyui-datetimebox" data-options="required:true, validType:'md[\'2014-07-28 10:10:10\']', editable:false" value="2014-07-28 12:13:56" style="width: 150px" />
                                    </td>
                                </tr>
                                <tr>
                                    <td style="height: 30px;">时间间隔</td>
                                    <td style="text-align: left;">
                                        <select id="TimeInterval" class="easyui-combobox" name="TimeInterval" data-options="panelHeight:'auto'" style="width: 80px;">
                                            <option value="10">10分钟</option>
                                            <option value="20">1小时</option>
                                            <option value="30">1天</option>
                                        </select>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table>
                                <tr>
                                    <td style="height: 30px; text-align: left;">
                                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-add',plain:true" onclick="popupTagSelector();">添加</a>                                   
                                    </td>
                                    <td style="height: 30px; text-align: left;">
                                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-remove',plain:true" onclick="clearTagItems();">清空</a>
                                    </td>
                                    <td>
                                        <div class="datagrid-btn-separator"></div>
                                    </td>
                                    <td>
                                        <a href="#" class="easyui-linkbutton" data-options="iconCls:'icon-search',plain:true" onclick="queryHistoryTrend();">查询</a>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <a href="#" class="easyui-linkbutton" data-options="iconCls:'ext-icon-page_white_excel',plain:true" onclick="ExportFileFun();">导出</a>
                                    </td>
                                    <td>
                                        <a href="#" class="easyui-linkbutton" data-options="iconCls:'ext-icon-printer',plain:true" onclick="PrintFileFun();">打印</a>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </div>
            <table id="grid_SelectedObj" class="easyui-datagrid" title="已选择数据项" data-options="fit:true,border:false,toolbar: '#MainSelect_Toolbar',
                    onDblClickRow: function (rowIndex, rowData) {
                        $(this).datagrid('deleteRow', rowIndex);
                    }">
                <thead>
                    <tr>
                        <th data-options="field:'OrganizationId', hidden:true"></th>
                        <th data-options="field:'TagTableName', hidden:true"></th>
                        <th data-options="field:'TagColumnName', hidden:true"></th>
                        <th data-options="field:'Name'" style="width: 220px;">项目名称</th>
                    </tr>
                </thead>
            </table>
        </div>
        <div class="easyui-panel" data-options="region:'center',border:false" style="padding: 5px;">
            <div class="easyui-layout" data-options="fit:true,border:false">
                <div id="Windows_Container" class="easyui-panel" data-options="region:'center', border:false, collapsible:false, split:false"></div>
                <!-- 标签选择器开始 -->
                <div id="dlg_SelectProcessTags" class="easyui-dialog" data-options="title:'电表选择', iconCls:'icon-search',resizable:false,modal:true,closed:true" style="width: 600px; height: 400px; overflow: hidden">
                    <div class="easyui-layout" data-options="fit:true,border:false">
                        <!-- 左侧组织机构目录树开始 -->
                        <div data-options="region:'west',border:false" style="width: 210px;">
                            <uc1:OrganisationTree_ProductionLine runat="server" ID="OrganisationTree_ProductionLine" />
                        </div>
                        <!-- 左侧组织机构目录树结束 -->
                        <div data-options="region:'center',border:false">
                            <!-- 工具栏开始 -->
                            <div id="toolbar_processTable" style="display: none; height: 27px; padding: 5px;">
                                <table>
                                    <tr>
                                        <td>组织机构</td>
                                        <td>
                                            <input id="txtOrganization" class="easyui-textbox" data-options="editable:false" style="width: 100px;" />
                                            <input id="organizationId" readonly="true" style="display: none;" />
                                        </td>
                                        <td>
                                            <div class="datagrid-btn-separator"></div>
                                        </td>
                                        <td>
                                            <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-search'" onclick="query();">搜索</a>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <!-- 工具栏结束 -->
                            <!-- 工序表格开始 -->
                            <table id="processTable" class="easyui-treegrid"
                                data-options="
				                            iconCls: 'icon-edit',
				                            rownumbers: true,
				                            animate: true,
				                            collapsible: true,
				                            fitColumns: true, 
				                            idField: 'LevelCode',
				                            treeField: 'Name',
                                            fit: true,
                                            toolbar: '#toolbar_processTable',
                                            onDblClickRow: onTagItemSelect
			                            ">
                                <thead>
                                    <tr>
                                        <th data-options="field:'TagTableName',hidden:true">所属数据表</th>
                                        <th data-options="field:'TagColumnName',hidden:true">字段名</th>
                                        <th data-options="field:'VariableId',hidden:true">字段名</th>
                                        <th data-options="field:'Name',width:100,editor:'text'">电表名称</th>
                                    </tr>
                                </thead>
                            </table>
                            <!-- 工序表格结束 -->
                        </div>
                    </div>
                </div>
                <!-- 标签选择器结束 -->
            </div>
        </div>
    </div>
    <form id="form_MaterialsQuantityTrend" runat="server">
        <div>
        </div>
    </form>
</body>
</html>
