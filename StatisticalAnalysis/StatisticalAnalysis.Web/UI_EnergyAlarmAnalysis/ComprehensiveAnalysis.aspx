<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ComprehensiveAnalysis.aspx.cs" Inherits="StatisticalAnalysis.Web.UI_EnergyAlarmAnalysis.ComprehensiveAnalysis" %>

<%@ Register Src="~/UI_WebUserControls/OrganizationSelector/OrganisationTree.ascx" TagPrefix="uc1" TagName="OrganisationTree_ProductionLine" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>报警综合分析</title>
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/gray/easyui.css" />
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/icon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtIcon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtCss.css" />

    <link rel="stylesheet" type="text/css" href="/lib/pllib/themes/jquery.jqplot.min.css" />
    <link type="text/css" rel="stylesheet" href="/lib/pllib/syntaxhighlighter/styles/shCoreDefault.min.css" />
    <link type="text/css" rel="stylesheet" href="/lib/pllib/syntaxhighlighter/styles/shThemejqPlot.min.css" />
    <!--    <link type="text/css" rel="stylesheet" href="/css/common/charts.css" />-->
    <link type="text/css" rel="stylesheet" href="/css/common/NormalPage.css" />
    <link type="text/css" rel="stylesheet" href="/UI_EnergyAlarmAnalysis/css/page/ComprehensiveAnalysis.css" />

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

    <script type="text/javascript" src="/UI_EnergyAlarmAnalysis/js/page/ComprehensiveAnalysis.js"></script>
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
                <div class="easyui-panel queryPanel" data-options="region:'north', border:true, collapsible:false, split:false" style="height:50px;">
                    组织机构：
                    <input id="txtOrganization" class="easyui-textbox" data-options="editable:false" style="width: 100px;" />
                    <input id="organizationId" readonly="true" style="display:none;"/> | 
                    <input type="radio" id="rdoYearly" name="analysisType" value="yearly"/><label for="rdoYearly">年分析</label>  
                    <input type="radio" id="rdoMonthly" name="analysisType" value="monthly" checked="checked"/><label for="rdoMonthly">月分析</label>
                    <input type="radio" id="rdoCustom" name="analysisType" value="custom"/><label for="rdoCustom">自定义</label> | 
                    <span>
                        起止时间：                            
                        <span id="startTimeWrapper" style="display:none;"><input id="StartTime" class="easyui-datebox" data-options="validType:'md[\'2012-10\']', required:true" style="width: 100px" />
                        <span id="InnerlLine">---</span></span>
                        <input id="EndTime" class="easyui-datebox" data-options="validType:'md[\'2012-10-10\']', required:true" style="width: 100px" />
                    </span>
                    | 
                    <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'ext-icon-chart_curve'" onclick="query();">分析</a>
                </div>
                <!-- 工具栏结束 -->
                <!-- 统计信息开始 -->
                <div class="easyui-panel countPanel" data-options="region:'west',title:'报警信息',border:true,collapsible:true,split:false">
                    <fieldset>
                        <legend>合计</legend>
                        <table class="table" style="width:100%;">
                            <tr>
                                <th class="countPanelItemsCol">总报警数：</th>
                                <td><span id="countTotal">0</span> 次</td>
                            </tr>
                        </table>
                    </fieldset>
                    <!--<br /><br />-->
                    <fieldset>
                        <legend>按生产线统计</legend>
                        <table id="countByFactory" class="table" style="width:100%;">
                            <tr>
                                <td>请先选择条件进行分析</td>
                            </tr>
                        </table>
                    </fieldset>
                    <!--<br /><br />-->
                    <fieldset>
                        <legend>按报警类别统计</legend>
                        <table id="countByTypes" class="table" style="width:100%;">
                            <tr>
                                <td>请先选择条件进行分析</td>
                            </tr>
                        </table>
                    </fieldset>
                </div>
                <!-- 统计信息结束 -->
                <!-- 图表开始 -->
                <div id="Windows_Container" class="easyui-panel" data-options="region:'center', border:true, collapsible:false, split:false">
                </div>
                <!-- 图表结束 -->
                <!--报表开始-->
                <div class="easyui-panel"  data-options="region:'south',title:'报警详细信息', border:true, collapsible:true, split:false" style="height:170px;">
                    <table id="Windows_Report"></table>
                </div>
                <!--报表结束-->
            </div>
        </div>
    </div>

    <form id="form_Main" runat="server"></form>
</body>
</html>
