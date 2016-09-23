<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DowntimeReasonAnalysis.aspx.cs" Inherits="StatisticalAnalysis.Web.UI_DowntimeAnalysis.DowntimeReasonAnalysis" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>停机原因排序</title>
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/gray/easyui.css" />
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/icon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtIcon.css" />

    <link rel="stylesheet" type="text/css" href="/lib/pllib/themes/jquery.jqplot.min.css" />
    <link type="text/css" rel="stylesheet" href="/lib/pllib/syntaxhighlighter/styles/shCoreDefault.min.css" />
    <link type="text/css" rel="stylesheet" href="/lib/pllib/syntaxhighlighter/styles/shThemejqPlot.min.css" />
    <link type="text/css" rel="stylesheet" href="/css/common/charts.css" />
    <link type="text/css" rel="stylesheet" href="/css/common/NormalPage.css" />
    <link type="text/css" rel="stylesheet" href="/UI_DowntimeAnalysis/css/page/DowntimeReasonAnalysis.css" />

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

    <script type="text/javascript" src="/UI_DowntimeAnalysis/js/page/DowntimeReasonAnalysis.js"></script>
</head>
<body>
    <div class="easyui-layout" data-options="fit:true,border:false">
        <div id="toolbar_MachineHaltInfo" style="display: none;">
            <!-- 工具栏开始 -->
            <table>
                <tr>
                    <th style="width: 80px; height: 35px;">开始时间</th>
                    <td style="width: 110px;">
                        <input id="StartTimeF" class="easyui-datebox" data-options="validType:'md[\'2012-10\']', required:true" style="width: 100px" />
                    </td>
                    <th style="width: 80px;">结束时间</th>
                    <td style="width: 110px;">
                        <input id="EndTimeF" class="easyui-datebox" data-options="validType:'md[\'2012-10\']', required:true" style="width: 100px" />
                    </td>
                    <th style="width: 80px;">统计范围</th>
                    <td style="width: 150px;">
                        <select id="ComboTree_LevelCodeF" class="easyui-combotree" data-options="valueField: 'id', textField: 'text', required: false 
                                   ,panelHeight: 300, editable: false"
                            name="LevelCode" style="width: 140px;">
                        </select>
                    </td>
                    <th style="width: 80px; height: 35px;">统计方式</th>
                    <td style="width: 110px;">
                        <select id="Select_StaticsMethodF" class="easyui-combobox" data-options="panelHeight: 'auto', editable:false" name="StaticsType" style="width: 100px;">
                            <option value="StaticsCount" selected="selected">按次数统计</option>
                            <option value="StaticsTime">按时间统计</option>
                        </select>
                    </td>
                    <th style="width: 80px;"></th>
                    <td></td>
                </tr>
                <tr>

                    <th style="width: 80px;">选择设备</th>
                    <td style="width: 110px;">
                        <select id="Select_EquipmentCommonInfoF" class="easyui-combobox" data-options="valueField: 'id', textField: 'text',panelHeight: 'auto', editable:false" name="EquipmentCommonInfo" style="width: 100px;">
                        </select>
                    </td>
                    <th style="width: 80px;">选择停机类型</th>
                    <td style="width: 110px;">
                        <select id="Select_HaltTypeF" class="easyui-combobox" data-options="valueField: 'id', textField: 'text',panelHeight: 'auto', editable:false, onSelect:function(record){SetReasonType(record.id);}" name="HaltTypeInfo" style="width: 100px;">
                            <option value="failure" selected="selected">故障停机</option>
                            <option value="normal">非故障停机</option>
                            <option value="null">无原因停机</option>
                        </select>
                    </td>
                    <th style="width: 80px;">选择原因类型</th>
                    <td style="width: 150px;">
                        <select id="Select_ReasonTypeF" class="easyui-combobox" data-options="valueField: 'id', textField: 'text',panelHeight: 'auto', editable:false" name="ReasonTypeInfo" style="width: 140px;">
                        </select>
                    </td>
                    <th style="width: 80px;">
                        <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'ext-icon-chart_curve'" onclick="query();">分析</a>
                    </th>
                    <td></td>
                    <th style="width: 80px;"></th>
                    <td></td>
                </tr>
            </table>

        </div>
        <!-- 工具栏结束 -->
        <!-- 图表开始 -->
        <div class="easyui-panel" data-options="region:'center', border:true, collapsible:false, split:false">
            <table id="grid_MachineHaltInfo"></table>
        </div>

        <!-- 图表结束 -->
    </div>
    <form id="form_Main" runat="server"></form>


</body>
</html>
