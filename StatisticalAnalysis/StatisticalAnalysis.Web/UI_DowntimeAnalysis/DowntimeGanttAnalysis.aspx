<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DowntimeGanttAnalysis.aspx.cs" Inherits="StatisticalAnalysis.Web.UI_DowntimeAnalysis.DowntimeGanttAnalysis" %>

<%@ Register Src="~/UI_WebUserControls/OrganizationSelector/OrganisationTree.ascx" TagPrefix="uc1" TagName="OrganisationTree_ProductionLine" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>停机甘特图分析</title>
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/gray/easyui.css" />
    <link rel="stylesheet" type="text/css" href="/lib/ealib/themes/icon.css" />
    <link rel="stylesheet" type="text/css" href="/lib/extlib/themes/syExtIcon.css" />

    <link type="text/css" rel="stylesheet" href="/css/common/charts.css" />
    <link type="text/css" rel="stylesheet" href="/css/common/NormalPage.css" />
    <link type="text/css" rel="stylesheet" href="/css/common/GanttChart.css" />

    <script type="text/javascript" src="/lib/ealib/jquery.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/jquery.easyui.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/lib/ealib/easyui-lang-zh_CN.js" charset="utf-8"></script>


    <!--[if lt IE 8 ]><script type="text/javascript" src="/js/common/json2.min.js"></script><![endif]-->

    <script type="text/javascript" src="/js/common/components/GanttChart.js" charset="utf-8"></script>

    <script type="text/javascript" src="/UI_DowntimeAnalysis/js/page/DowntimeGanttAnalysis.js"></script>
</head>
<body>
    <div class="easyui-layout" data-options="fit:true,border:false">
        <!-- 左侧组织机构目录树开始 -->
        <div class="easyui-panel" data-options="region:'west',border:false" style="width: 150px;">
            <uc1:OrganisationTree_ProductionLine runat="server" ID="OrganisationTree_ProductionLine" />
        </div>
        <!-- 左侧组织机构目录树结束 -->
        <div class="easyui-panel" data-options="region:'center',border:false">
            <div class="easyui-layout" data-options="fit:true,border:false" style="margin-left: 5px;">
                <div class="easyui-panel" data-options="region:'north', border:false, collapsible:false, split:false" style="background-color: #f7f7f7">
                    <!-- 工具栏开始 -->
                    <table>
                        <tr>
                            <th style="width: 80px;">选择分厂</th>
                            <td style="width: 110px;">
                                <input id="OrganizationIdF" style="width: 90px; display: none;" readonly="readonly" />
                                <input id="OrganizationNameF" class="easyui-textbox" data-options="editable:false" style="width: 100px" readonly="readonly" />
                            </td>
                            <th style="width: 80px; height: 35px;">开始时间</th>
                            <td style="width: 110px;">
                                <input id="StartTimeF" class="easyui-datebox" data-options="validType:'md[\'2012-10\']', required:true" style="width: 100px" />
                            </td>
                            <th style="width: 80px;">结束时间</th>
                            <td style="width: 110px;">
                                <input id="EndTimeF" class="easyui-datebox" data-options="validType:'md[\'2012-10\']', required:true" style="width: 100px" />
                            </td>
                            <th style="width: 80px;">显示顺序</th>
                            <td style="width: 110px;">
                                <select id="Select_DisplayOrderF" class="easyui-combobox" data-options="valueField: 'id', textField: 'text',panelHeight: 'auto', editable:false, onSelect:function(record){SetReasonType(record.id);}" name="HaltTypeInfo" style="width: 100px;">
                                    <option value="time" selected="selected">停机时间</option>
                                    <option value="count">停机次数</option>
                                </select>
                            </td>
                        </tr>
                        <tr>
                            <th style="width: 80px;">选择设备</th>
                            <td style="width: 110px;">
                                <select id="Select_EquipmentInfoF" class="easyui-combobox" data-options="valueField: 'id', textField: 'text',panelHeight: 'auto', editable:false" name="EquipmentInfo" style="width: 100px;">
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
                            <td style="width: 110px;">
                                <select id="Select_ReasonTypeF" class="easyui-combobox" data-options="valueField: 'id', textField: 'text',panelHeight: 'auto', editable:false" name="ReasonTypeInfo" style="width: 100px;">
                                </select>
                            </td>
                            <th style="width: 80px;">
                                <a href="javascript:void(0);" class="easyui-linkbutton" data-options="iconCls:'ext-icon-chart_curve'" onclick="query();">分析</a>
                            </th>
                            <td></td>
                        </tr>
                    </table>

                </div>
                <!-- 工具栏结束 -->
                <!-- 图表开始 -->
                <div class="easyui-panel" data-options="region:'center', border:true, collapsible:false, split:false" style="padding: 5px; padding-right: 8px;">
                    <div id="GanttChartTest" class="easyui-layout" data-options="fit:true,border:false" style="overflow: auto;">
                    </div>
                </div>
            </div>
        </div>

        <!-- 图表结束 -->
    </div>
    <form id="form_Main" runat="server"></form>


</body>
</html>
