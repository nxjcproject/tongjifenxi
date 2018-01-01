<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="HorizontallyEnergyAlarmAnalysis.aspx.cs" Inherits="StatisticalAnalysis.Web.UI_HorizontallyAlarmAnalysis.HorizontallyEnergyAlarmAnalysis" %>

<%@ Register Src="~/UI_WebUserControls/OrganizationSelector/OrganisationTree.ascx" TagPrefix="uc1" TagName="OrganisationTree_ProductionLine" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>横向能耗报警对比分析</title>
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

    <script type="text/javascript" src="/UI_HorizontallyAlarmAnalysis/js/page/HorizontallyEnergyAlarmAnalysis.js"></script>

</head>
<body>
    <div class="easyui-layout" data-options="fit:true,border:false">
        <!-- 左侧组织机构目录树开始 -->
        <div class="easyui-panel" data-options="region:'west',border:false" style="width: 150px;">
            <uc1:OrganisationTree_ProductionLine runat="server" ID="OrganisationTree_ProductionLine" />
        </div>
        <div class="easyui-panel" data-options="region:'center',border:false">
            <div class="easyui-layout" data-options="fit:true,border:false">
                <div data-options="region:'west',border:false, collapsible:true, split:false" style="width: 230px">
                    <div id="toolbarId" class="easyui-panel" style="height: 100px; padding: 10px">
                        <table>
                            <tr>
                                <td>开始时间</td>
                                <td><input id="StartTime" class="easyui-datetimebox" data-options="validType:'md[\'2012-10\']', required:true" style="width: 150px" /></td>
                            </tr>
                            <tr>
                                <td>结束时间</td>
                                <td><input id="EndTime" class="easyui-datetimebox" data-options="validType:'md[\'2012-10-10\']', required:true" style="width: 150px;" /></td>
                            </tr>                      
                        </table>            
                        <div style="padding-top:5px">
                            <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-remove'" onclick="removeAll();" \>清空列表</a>
                            <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'icon-reload'" onclick="refresh();" \>刷新</a>
                            <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'ext-icon-chart_curve'" onclick="query();">分析</a>
                        </div>           
                    </div>
                    <table id="labelListId" class="easyui-datagrid" style="width: 250px;" data-options="fitColumns:true,singleSelect:true,fit:true,toolbar:'#toolbarId'"></table>
                </div>
                <div id="GridChartContainerId" data-options="region:'center', border:false, collapsible:false, split:false">
                    <div class="easyui-layout" data-options="fit:true,border:false">
                        <div data-options="region:'north',border:true" style="height: 100px;">
                            <table id="GridId" class="easyui-datagrid" data-options="fit:true,border:false"></table>
                        </div>
                        <div id="ChartId" data-options="region:'center',border:false">
                        </div>
                    </div>
                </div>
            </div>
            <%--</div>--%>
            <!-- 图表结束 -->
            <%--</div>--%>
        </div>
    </div>
    <form id="form1" runat="server"></form>
</body>
</html>
