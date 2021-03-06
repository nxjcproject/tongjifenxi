﻿var IsFirstLoadChart = "true";

$(document).ready(function () {

    loadDataGrid("first");
    // 为分析类型挂载change事件

    $("input[type=radio][name=analysisType]").change(function () {
        if ($("input[type=radio][name=analysisType]").get(2).checked === true) {
            $('#startTimeWrapper').show();
        }
        else {
            $('#startTimeWrapper').hide();
        }
    });
    
    // 设定默认值

    var now = new Date();
    var tenDaysEarly = new Date((now / 1000 - 86400 * 10) * 1000);
    $('#StartTime').datebox('setValue', tenDaysEarly.toLocaleDateString().replace(/\//g, '-'));
    $('#EndTime').datebox('setValue', now.toLocaleDateString().replace(/\//g, '-'));
});

// 获取双击组织机构时的组织机构信息

function onOrganisationTreeClick(node) {

    // 仅能选中分厂级别
    // 即组织机构ID的层次码 = 5

    //if (node.id.length != 5) {
    //    $.messager.alert('提示', '仅能选择分厂级别。');
    //    return;
    //}

    // 设置组织机构ID
    // organizationId为其它任何函数提供当前选中的组织机构ID

    $('#organizationId').val(node.OrganizationId);

    // 设置组织机构名称
    // 用于呈现，在界面上显示当前的组织机构名称

    $('#txtOrganization').textbox('setText', node.text);
}

function query() {
    // 获取组织机构ID
    var organizationId = $('#organizationId').val();

    if (organizationId == '') {
        $.messager.alert('提示', '请先选择需要分析的组织机构。');
        return;
    }
    
    // 获取起止时间段
    var startTime = $('#StartTime').datebox('getValue');
    var endTime = $('#EndTime').datebox('getValue');
    var m_StartTimeString = "";
    var m_EndTimeString = "";
    // 获取分析类型
    var analysisType = $("input[name='analysisType']:checked").val();

    switch (analysisType) {

            // 如果是年分析
            // 结束时间月日应为年的最后一天
            // 起始时间月日应为年的1月1号

        case 'yearly':
            var array = endTime.split('-');
            endTime = new Date(array[0], array[1] - 1, array[2]);
            endTime.setMonth(12 - 1, getLastDayOfMonth(endTime.getFullYear(), 12));
            endTime.setHours(23, 59, 59, 999);
            startTime = new Date(endTime.getFullYear(), 0, 1);
            startTime.setHours(00, 00, 00, 000);
            break;

            // 如果是月分析
            // 结束时间月日应为选定月的最后一天
            // 起始时间月日应为选定月的1号

        case 'monthly':
            var array = endTime.split('-');
            endTime = new Date(array[0], array[1] - 1, array[2]);
            endTime.setDate(getLastDayOfMonth(endTime.getFullYear(), endTime.getMonth() + 1));
            endTime.setHours(23, 59, 59, 999);
            startTime = new Date();
            startTime.setFullYear(endTime.getFullYear(), endTime.getMonth(), 1);
            startTime.setHours(00, 00, 00, 000);

            break;

            // 如果是自定义，则
            // 开始时间设定为所选时间的凌晨0点0分0秒
            // 结束时间设定为所选时间的23点59分59秒

        case 'custom':
            var array = endTime.split('-');
            endTime = new Date(array[0], array[1] - 1, array[2]);
            var arrayStart = startTime.split('-');
            startTime = new Date(arrayStart[0], arrayStart[1] - 1, arrayStart[2]);
            endTime.setHours(23, 59, 59, 999);
            startTime.setHours(00, 00, 00, 000);
            break;
    }
    m_StartTimeString = startTime.getFullYear() + "-" + parseInt(startTime.getMonth() + 1) + "-" + startTime.getDate() + " " + startTime.getHours() + ":" + startTime.getMinutes() + ":" + +startTime.getSeconds();
    m_EndTimeString = endTime.getFullYear() + "-" + parseInt(endTime.getMonth() + 1) + "-" + endTime.getDate() + " " + endTime.getHours() + ":" + endTime.getMinutes() + ":" + +endTime.getSeconds();
    $.ajax({
        type: "POST",
        url: "ComprehensiveAnalysis.aspx/GetAlarmCount",
        data: "{organizationId:'" + organizationId + "',startTime:'" + m_StartTimeString + "',endTime:'" + m_EndTimeString + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            updateCountPanel(JSON.parse(msg.d));
        }
    });

    $.ajax({
        type: "POST",
        url: "ComprehensiveAnalysis.aspx/GetAlarmChart",
        data: "{organizationId:'" + organizationId + "',startTime:'" + m_StartTimeString + "',endTime:'" + m_EndTimeString + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {       
            updateChart(JSON.parse(msg.d));
        }
    });

    $.ajax({
        type: "POST",
        url: "ComprehensiveAnalysis.aspx/GetReportData",
        data: "{organizationId:'" + organizationId + "',startTime:'" + m_StartTimeString + "',endTime:'" + m_EndTimeString + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_msg = JSON.parse(msg.d);
            loadDataGrid("last",m_msg);
        }
    });
}

function loadDataGrid(type, myData) {
    if ("first" == type) {
        $("#Windows_Report").datagrid({
            striped:true,
            rownumbers: true,
            singleSelect: true,
            fit:true,
            columns: [[
                        { field: 'ProductLineName', title: '产线名称', width: 150 },
                		{ field: 'Name', title: '报警名称', width: 150 },
		                { field: 'EnergyConsumptionType', title: '报警类别', width: 150 },
		                { field: 'Count', title: '报警次数', width: 100 }
            ]]
        });
    }
    else {
        $("#Windows_Report").datagrid("loadData", myData);
    }
}

function updateCountPanel(data) {

    // 详细计数的HTML

    var str = '';

    // 总计的数目

    var total = 0;

    // 遍历data中的元素

    for (var i = 0; i < data.total; i++) {

        // 如果是总报警数
        // 则更新total

        if (data.rows[i].Name == '总报警数') {
            total = data.rows[i].Count;
            continue;
        }

        // 否则，生成表格元素

        str += '<tr><th class="countPanelItemsCol" style="width:60%;">' + data.rows[i].Name + '：</th><td>' + data.rows[i].Count + ' 次</td></tr>';
    }

    // 如果返回记录为空，则生成空的提示

    if (data.total == 0) {
        str = '<tr><td>无报警记录</td></tr>';
        total = 0;
    }

    // 更新占位符

    $('#countByFactory').html(str);
    $('#countTotal').html(total);
}

function updateChart(data) {

    // 详细计数的HTML

    var str = '';

    // 更新Table
    // 遍历data中的元素

    for (var i = 0; i < data.total; i++) {

        // 否则，生成表格元素

        str += '<tr><th class="countPanelItemsCol" style="width:60%;">' + data.rows[i].RowName + '：</th><td>' + data.rows[i].Count + ' 次</td></tr>';
    }

    // 如果返回记录为空，则生成空的提示

    if (data.total == 0) {
        str = '<tr><td>无报警记录</td></tr>';
        total = 0;
    }

    // 更新占位符

    $('#countByTypes').html(str);


    // 更新Chart

    var m_WindowContainerId = 'Windows_Container';
    var m_Maximizable = false;
    var m_Maximized = true;

    if (IsFirstLoadChart == true) {
        IsFirstLoadChart = false;
    }
    else {
        var m_WindowsIdArray = GetWindowsIdArray();
        for (var i = 0; i < m_WindowsIdArray.length; i++) {
            if (m_WindowsIdArray[i] != "") {
                ReleaseAllGridChartObj(m_WindowsIdArray[i]);
            }
        }
        CloseAllWindows();
    }
    /////////////////////显示图表///////////////////////
    var m_ContainerObj = $('#Windows_Container');
    var m_ContainerObjWidth = m_ContainerObj.width();
    var m_ContainerObjHeight = m_ContainerObj.height();
    WindowsDialogOpen(m_WindowContainerId, data, m_ContainerObjWidth, m_ContainerObjHeight, "Pie");
}

// 获取月份的最后一天
// 注意参数中的月份以1开始

function getLastDayOfMonth(year, month) {

    //获取本年本月的第一天日期

    var date = new Date(year, month, 1);

    //获取本月的最后一天

    lastDayOfMonth = new Date(date.getTime() - 1000 * 60 * 60 * 24);

    //返回结果
    // alert(lastDayOfMonth.getFullYear() + "年" + (lastDayOfMonth.getMonth()) + 1) + "月最后一天为：" + lastDayOfMonth.getDate() + "日");

    return lastDayOfMonth.getDate();
}

///////////////////////////////////////////打开window窗口//////////////////////////////////////////
function WindowsDialogOpen(myContainerId, myData, myWidth, myHeight, myImageType) {
    var m_WindowId = OpenWindows(myContainerId, '数据分析', myWidth, myHeight); //弹出windows
    windowID = m_WindowId;
    var m_WindowObj = $('#' + m_WindowId);
    CreateGridChart(myData, m_WindowId, false, myImageType);               //生成图表
    //if (myMaximized != true) {
    //    ChangeSize(m_WindowId);
    //}
    m_WindowObj.window({
        onBeforeClose: function () {
            ///////////////////////释放图形空间///////////////
            //var m_ContainerId = GetWindowIdByObj($(this));
            ReleaseGridChartObj(m_WindowId);
            CloseWindow($(this))
        },
        onMaximize: function () {
            TopWindow(m_WindowId);
            ChangeSize(m_WindowId);
            //CreateGridChart(myData, m_WindowId, myIsShowGrid, myChartType);

        },
        onRestore: function () {
            //TopWindow(m_WindowId);
            ChangeSize(m_WindowId);
            //CreateGridChart(myData, m_WindowId, myIsShowGrid, myChartType);
        }
    });
}