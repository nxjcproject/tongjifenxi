var IsFirstLoadChart = "true";

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

            startTime = startTime.toLocaleString();
            endTime = endTime.toLocaleString();
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

            startTime = startTime.toLocaleString();
            endTime = endTime.toLocaleString();
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
            startTime = startTime.toLocaleString();
            endTime = endTime.toLocaleString();
            break;
    }

    $.ajax({
        type: "POST",
        url: "ComprehensiveAnalysis.aspx/GetDowntimCount",
        data: "{organizationId:'" + organizationId + "',startTime:'" + startTime + "',endTime:'" + endTime + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            updateCountPanel(JSON.parse(msg.d));
        }
    });

    $.ajax({
        type: "POST",
        url: "ComprehensiveAnalysis.aspx/GetDowntimeChart",
        data: "{organizationId:'" + organizationId + "',startTime:'" + startTime + "',endTime:'" + endTime + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            updateChart(JSON.parse(msg.d));
        }
    });

    $.ajax({
        type: "POST",
        url: "ComprehensiveAnalysis.aspx/GetReportData",
        data: "{organizationId:'" + organizationId + "',startTime:'" + startTime + "',endTime:'" + endTime + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_msg = JSON.parse(msg.d);
            loadDataGrid("last", m_msg);
        }
    });
}

function loadDataGrid(type, myData) {
    if ("first" == type) {
        $("#Windows_Report").treegrid({
            striped: true,
            rownumbers: true,
            singleSelect: true,
            fit: true,
            columns: [[                       
                		{ field: 'EquipmentName', title: '报警名称', width: 250 },
                        { field: 'Name', title: '产线名称', width: 150 },
		                { field: 'Type', title: '报警类别', width: 150 },
                        { field: 'Label', title: '报警标签', width: 150 },
		                { field: 'Count', title: '报警次数', width: 100 }
            ]],
            idField:"id",
            treeField: "EquipmentName"
        });
    }
    else {
        $("#Windows_Report").treegrid("loadData", myData);
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

        if (data.rows[i].Name == '总停机数') {
            total = data.rows[i].Count;
            continue;
        }

        // 否则，生成表格元素

        str += '<tr><th class="countPanelItemsCol">' + data.rows[i].Name + '：</th><td>' + data.rows[i].Count + ' 次</td></tr>';
    }

    // 如果返回记录为空，则生成空的提示

    if (data.total == 0) {
        str = '<tr><td>无停机记录</td></tr>';
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

        str += '<tr><th class="countPanelItemsCol">' + data.rows[i].RowName + '：</th><td>' + data.rows[i].Count + ' 次</td></tr>';
    }

    // 如果返回记录为空，则生成空的提示

    if (data.total == 0) {
        str = '<tr><td>无停机记录</td></tr>';
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
        ReleaseGridChartObj(m_WindowContainerId);
    }

    CloseAllWindows();

    var m_Postion = GetWindowPostion(0, m_WindowContainerId);
    WindowsDialogOpen(data, m_WindowContainerId, false, 'Pie', m_Postion[0], m_Postion[1], m_Postion[2], m_Postion[3], false, m_Maximizable, m_Maximized);
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


///////////////////////获取window初始位置////////////////////////////
function GetWindowPostion(myWindowIndex, myWindowContainerId) {
    var m_ParentObj = $('#' + myWindowContainerId);
    var m_ParentWidth = m_ParentObj.width();
    var m_ParentHeight = m_ParentObj.height();
    var m_ZeroLeft = 0;
    var m_ZeroTop = 0;
    var m_Padding = 5;
    var m_Width = (m_ParentWidth - m_Padding) / 2;
    var m_Height = (m_ParentHeight - m_Padding) / 2;
    var m_Left = 0;
    var m_Top = 0;
    if (myWindowIndex == 0) {
        m_Left = m_ZeroLeft;
        m_Top = m_ZeroTop;
    }
    else if (myWindowIndex == 1) {
        m_Left = m_ZeroLeft + m_Width + m_Padding;
        m_Top = m_ZeroTop;
    }
    else if (myWindowIndex == 2) {
        m_Left = m_ZeroLeft;
        m_Top = m_ZeroTop + m_Height + m_Padding;
    }
    else if (myWindowIndex == 3) {
        m_Left = m_ZeroLeft + m_Width + m_Padding;
        m_Top = m_ZeroTop + m_Height + m_Padding;
    }

    return [m_Width, m_Height, m_Left, m_Top]
}
///////////////////////////////////////////打开window窗口//////////////////////////////////////////
function WindowsDialogOpen(myData, myContainerId, myIsShowGrid, myChartType, myWidth, myHeight, myLeft, myTop, myDraggable, myMaximizable, myMaximized) {
    ;
    var m_WindowId = OpenWindows(myContainerId, '数据分析', myWidth, myHeight, myLeft, myTop, myDraggable, myMaximizable, myMaximized); //弹出windows
    var m_WindowObj = $('#' + m_WindowId);
    if (myMaximized != true) {
        CreateGridChart(myData, m_WindowId, myIsShowGrid, myChartType);               //生成图表
    }

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
            CreateGridChart(myData, m_WindowId, myIsShowGrid, myChartType);

        },
        onRestore: function () {
            //TopWindow(m_WindowId);
            ChangeSize(m_WindowId);
            CreateGridChart(myData, m_WindowId, myIsShowGrid, myChartType);
        }
    });
}
