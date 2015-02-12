var IsFirstLoadChart = "true";

$(document).ready(function () {

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
    initCementTypeList();
});

function initCementTypeList() {
    $.ajax({
        type: "POST",
        url: "CementCostAnalysis.aspx/GetCementTypesList",
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var json = JSON.parse(msg.d);
            $('#cementItem').combobox({
                data: json,
                valueField: 'CementTypes',
                textField: 'CementTypes'
            });
        }
    });
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
    var organizationId = $('#organizationId').val();
    var cementType = $('#cementItem').combobox('getText');
    if (organizationId == '') {
        $.messager.alert('提示', '请先选择需要分析的组织机构。');
        return;
    }

    var startTime = new Date($('#StartTime').datebox('getValue')).toLocaleDateString();
    var endTime = new Date($('#EndTime').datebox('getValue')).toLocaleDateString();

    // 获取分析类型
    var analysisType = $("input[name='analysisType']:checked").val();

    $.ajax({
        type: "POST",
        url: "CementCostAnalysis.aspx/GetCementCostAnalysisChart",
        data: "{organizationId:'" + organizationId + "',analysisType:'" + analysisType + "',startTime:'" + startTime + "',endTime:'" + endTime + "',cementType:'" +cementType+ "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            updateChart(JSON.parse(msg.d));
        }
    });
}


function updateChart(data) {

    // 更新Chart

    var imageType = $('#imageType').combobox('getValue');

    var m_WindowContainerId = 'Windows_Container';
    var m_Maximizable = false;
    var m_Maximized = true;

    var m_WindowsIdArray = GetWindowsIdArray();
    for (var i = 0; i < m_WindowsIdArray.length; i++) {
        if (m_WindowsIdArray[i] != "") {
            ReleaseAllGridChartObj(m_WindowsIdArray[i]);
        }
    }
    CloseAllWindows();

    var m_Postion = GetWindowPostion(0, m_WindowContainerId);
    WindowsDialogOpen(data, m_WindowContainerId, true, imageType, m_Postion[0], m_Postion[1], m_Postion[2], m_Postion[3], false, m_Maximizable, m_Maximized);
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
