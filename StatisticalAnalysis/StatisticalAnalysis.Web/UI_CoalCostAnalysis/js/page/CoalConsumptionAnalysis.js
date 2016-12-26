var IsFirstLoadChart = "true";
var windowID = '';
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
    $('#StartTime').datebox('setValue', tenDaysEarly.toString().replace(/\//g, '-'));
    $('#EndTime').datebox('setValue', now.toString().replace(/\//g, '-'));
});


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
    $('#' + windowID).empty();

    var organizationId = $('#organizationId').val();

    if (organizationId == '') {
        $.messager.alert('提示', '请先选择需要分析的组织机构。');
        return;
    }

    var startTime = $('#StartTime').datebox('getValue');
    var endTime = $('#EndTime').datebox('getValue');

    // 获取分析类型
    var analysisType = $("input[name='analysisType']:checked").val();
    var win = $.messager.progress({
        title: '请稍后',
        msg: '数据载入中...'
    });
    $.ajax({
        type: "POST",
        url: "CoalConsumptionAnalysis.aspx/GetCoalConsumptionAnalysisChart",
        data: "{organizationId:'" + organizationId + "',analysisType:'" + analysisType + "',startTime:'" + startTime + "',endTime:'" + endTime + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $.messager.progress('close');
            updateChart(JSON.parse(msg.d));
        },
        beforeSend: function (XMLHttpRequest) {
            win;
        }
    });
}


function updateChart(data) {

    // 更新Chart

    var imageType = $('#imageType').combobox('getValue');

    var m_WindowContainerId = 'Windows_Container';

    var m_WindowsIdArray = GetWindowsIdArray();
    for (var i = 0; i < m_WindowsIdArray.length; i++) {
        if (m_WindowsIdArray[i] != "") {
            ReleaseAllGridChartObj(m_WindowsIdArray[i]);
        }
    }
    CloseAllWindows();
    /////////////////////显示图表///////////////////////
    var m_ContainerObj = $('#Windows_Container');
    var m_ContainerObjWidth = m_ContainerObj.width();
    var m_ContainerObjHeight = m_ContainerObj.height();
    WindowsDialogOpen(m_WindowContainerId, data, m_ContainerObjWidth, m_ContainerObjHeight, imageType);
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

//将chart转化为图片
function chartToImage(){
    //alert("");
    var j = $('#' + windowID + '_Chart').jqplotToImageElem();
    $('#' + windowID + '_Chart').empty();
    $('#' + windowID + '_Chart').append(j);
}


///////////////////////////////////////////打开window窗口//////////////////////////////////////////
function WindowsDialogOpen(myContainerId, myData, myWidth, myHeight, myImageType) {
    var m_WindowId = OpenWindows(myContainerId, '熟料生产线煤耗分析', myWidth, myHeight); //弹出windows
    windowID = m_WindowId;
    var m_WindowObj = $('#' + m_WindowId);
    CreateGridChart(myData, m_WindowId, true, myImageType);               //生成图表
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