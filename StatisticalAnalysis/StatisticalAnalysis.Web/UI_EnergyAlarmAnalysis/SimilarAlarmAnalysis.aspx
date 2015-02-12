<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SimilarAlarmAnalysis.aspx.cs" Inherits="StatisticalAnalysis.Web.UI_EnergyAlarmAnalysis.SimilarAlarmAnalysis" %>

<%@ Register Src="~/UI_WebUserControls/OrganizationSelector/OrganisationTree.ascx" TagPrefix="uc1" TagName="OrganisationTree_ProductionLine" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>同类报警分析</title>
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/gray/easyui.css" />
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/icon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtIcon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtCss.css" />

    <link rel="stylesheet" type="text/css" href="/lib/pllib/themes/jquery.jqplot.min.css" />
    <link type="text/css" rel="stylesheet" href="/lib/pllib/syntaxhighlighter/styles/shCoreDefault.min.css" />
    <link type="text/css" rel="stylesheet" href="/lib/pllib/syntaxhighlighter/styles/shThemejqPlot.min.css" />
    <link type="text/css" rel="stylesheet" href="/css/common/charts.css" />
    <link type="text/css" rel="stylesheet" href="/css/common/NormalPage.css" />
    <link type="text/css" rel="stylesheet" href="/UI_EnergyAlarmAnalysis/css/page/SimilarAlarmAnalysis.css" />

    <script type="text/javascript" src="/lib/ealib/jquery.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/jquery.easyui.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/easyui-lang-zh_CN.js" charset="utf-8"></script>

    <!--[if lt IE 9]><script type="text/javascript" src="/lib/pllib/excanvas.js"></script><![endif]-->
    <script type="text/javascript" src="/lib/pllib/jquery.jqplot.min.js"></script>
    <!--<script type="text/javascript" src="/lib/pllib/syntaxhighlighter/scripts/shCore.min.js"></script>
    <script type="text/javascript" src="/lib/pllib/syntaxhighlighter/scripts/shBrushJScript.min.js"></script>
    <script type="text/javascript" src="/lib/pllib/syntaxhighlighter/scripts/shBrushXml.min.js"></script>-->

    <!-- Additional plugins go here -->
    <script type="text/javascript" src="/lib/pllib/plugins/jqplot.barRenderer.min.js"></script>
    <script type="text/javascript" src="/lib/pllib/plugins/jqplot.pieRenderer.min.js"></script>
    <script type="text/javascript" src="/lib/pllib/plugins/jqplot.canvasTextRenderer.min.js"></script>
    <script type="text/javascript" src="/lib/pllib/plugins/jqplot.canvasAxisTickRenderer.min.js"></script>
    <script type="text/javascript" src="/lib/pllib/plugins/jqplot.categoryAxisRenderer.min.js"></script>
    <script type="text/javascript" src="/lib/pllib/plugins/jqplot.cursor.min.js"></script>
    <script type="text/javascript" src="/lib/pllib/plugins/jqplot.dateAxisRenderer.min.js"></script>
    <script type="text/javascript" src="/lib/pllib/plugins/jqplot.pointLabels.min.js"></script>

    <!--[if lt IE 8 ]><script type="text/javascript" src="/js/common/json2.min.js"></script><![endif]-->

    <script type="text/javascript" src="/js/common/components/Charts.js" charset="utf-8"></script>
    <script type="text/javascript" src="/js/common/components/DataGrid.js" charset="utf-8"></script>
    <script type="text/javascript" src="/js/common/components/WindowsDialog.js" charset="utf-8"></script>
    <script type="text/javascript" src="/js/common/components/GridChart.js" charset="utf-8"></script>

    <script type="text/javascript" src="/UI_EnergyAlarmAnalysis/js/page/SimilarAlarmAnalysis.js"></script>
</head>
<body>
    <div class="easyui-layout" data-options="fit:true,border:false">
        <!-- 左侧组织机构目录树开始 -->
        <div class="easyui-panel" data-options="region:'west',border:false" style="width: 230px;">
            <uc1:OrganisationTree_ProductionLine runat="server" id="OrganisationTree_ProductionLine" />
        </div>
        <!-- 左侧组织机构目录树结束 -->
        <div class="easyui-panel" data-options="region:'center',border:false">
            <div class="easyui-layout" data-options="fit:true,border:false" style="margin-left:5px;">
                <!-- 工具栏开始 -->
                <div class="easyui-panel queryPanel" data-options="region:'north', border:true, collapsible:false, split:false" style="height: 80px;">
                    <table>
                        <tr>
                            <td>组织机构：</td>
                            <td><input id="txtOrganization" class="easyui-textbox" data-options="editable:false" style="width: 150px;" /><input id="organizationId" readonly="true" style="display:none;"/></td>
                            <td style="width:10px;">|</td>
                            <td>统计区间：</td>
                            <td>
                                <input type="radio" id="rdoYearly" name="analysisType" value="yearly"/><label for="rdoYearly">年统计</label>  
                                <input type="radio" id="rdoMonthly" name="analysisType" value="monthly" checked="checked"/><label for="rdoMonthly">月统计</label>
                                <input type="radio" id="rdoCustom" name="analysisType" value="custom"/><label for="rdoCustom">自定义</label>
                            </td>
                            <td style="width:10px;">|</td>
                            <td>起止时间：</td>
                            <td>
                                <span id="startTimeWrapper" style="display:none;"><input id="StartTime" class="easyui-datebox" data-options="validType:'md[\'2012-10\']', required:true" style="width: 100px" />
                                <span id="InnerlLine">---</span></span>
                                <input id="EndTime" class="easyui-datebox" data-options="validType:'md[\'2012-10-10\']', required:true" style="width: 100px" />
                            </td>
                        </tr>
                        <tr><td style="height:5px;"></td></tr>
                        <tr>
                            <td>报警类型：</td>
                            <td>
                                <select id="alarmType" class="easyui-combobox" data-options="panelHeight: 'auto'" name="imageType" style="width:150px;">
                                    <option value="电耗超标">电耗超标</option>
                                    <option value="煤耗超标">煤耗超标</option>
                                    <option value="功率超标">功率超标</option>
                                </select>
                            </td>
                            <td style="width:10px;">|</td>
                            <td>图表类型：</td>
                            <td>
                                <select id="imageType" class="easyui-combobox" data-options="panelHeight: 'auto'" name="imageType" style="width:150px;">
                                    <option value="Line">趋势图</option>
                                    <option value="Bar">柱状图</option>
                                </select>
                            </td>
                            <td style="width:10px;">|</td>
                            <td><a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'ext-icon-chart_curve'" onclick="Query();">分析</a></td>
                        </tr>
                    </table>
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
