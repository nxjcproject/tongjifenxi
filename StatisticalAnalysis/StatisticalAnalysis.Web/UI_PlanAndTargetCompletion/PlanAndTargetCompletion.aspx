<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PlanAndTargetCompletion.aspx.cs" Inherits="StatisticalAnalysis.Web.UI_PlanAndTargetCompletion.PlanAndTargetCompletion" %>
<%@ Register Src="~/UI_WebUserControls/OrganizationSelector/OrganisationTree.ascx" TagPrefix="uc1" TagName="OrganisationTree_ProductionLine" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>能源计划实绩</title>
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/gray/easyui.css" />
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/icon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtIcon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtCss.css" />

    <link rel="stylesheet" type="text/css" href="/lib/pllib/themes/jquery.jqplot.min.css" />
    <link type="text/css" rel="stylesheet" href="/lib/pllib/syntaxhighlighter/styles/shCoreDefault.min.css" />
    <link type="text/css" rel="stylesheet" href="/lib/pllib/syntaxhighlighter/styles/shThemejqPlot.min.css" />
    <!--    <link type="text/css" rel="stylesheet" href="/css/common/charts.css" />-->
    <link type="text/css" rel="stylesheet" href="/css/common/NormalPage.css" />
    <link type="text/css" rel="stylesheet" href="/UI_PlanAndTargetCompletion/css/page/PlanAndTargetCompletion.css" />

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

    <script type="text/javascript" src="/UI_PlanAndTargetCompletion/js/page/PlanAndTargetCompletion.js"></script>
</head>
<body>
    <div class="easyui-layout" data-options="fit:true,border:false">
        <!-- 左侧组织机构目录树开始 -->
        <div class="easyui-panel" data-options="region:'west',border:false" style="width: 150px;">
            <uc1:OrganisationTree_ProductionLine runat="server" id="OrganisationTree_ProductionLine" />
        </div>
        <!-- 左侧组织机构目录树结束 -->
        <div class="easyui-panel" data-options="region:'center',border:false">
            <div class="easyui-layout" data-options="fit:true,border:false" style="margin-left:5px;">
                <!-- 工具栏开始 -->
                <div class="easyui-panel queryPanel" data-options="region:'north', border:true, collapsible:false, split:false" style="height: 50px;">
                    组织机构
                    <input id="txtOrganization" class="easyui-textbox" data-options="editable:false" style="width: 90px;" />
                    <input id="organizationId" readonly="true" style="display:none;"/>                                      
                    &nbsp 项目指标
                    <select id="itemName" class="easyui-combobox" name="item" style="width:95px"></select>
                    &nbsp 选择年份
                    <input id="year" class="easyui-numberspinner" style="width:70px;"required="required" data-options="editable:false"/>  
                    <select id="imageType" class="easyui-combobox" name="imageType" data-options="panelHeight:'auto'"  style="width:75px;">
                        <option value="Line">趋势图</option>
                        <option value="Bar">柱状图</option>
                    </select>
                    <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'ext-icon-chart_curve'" onclick="Query();">分析</a>
                    <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'ext-icon-picture_save'" onclick="chartToImage();">生成图片</a>
                </div>
                <!-- 工具栏结束 -->
                <!-- 图表开始 -->
                <div id="Windows_Container" class="easyui-panel" data-options="region:'center', border:true, collapsible:false, split:false">
                </div>
                <!-- 图表结束 -->
            </div>
        </div>
    </div>

    <form id="form_Main" runat="server"></form>

</body>
</html>
