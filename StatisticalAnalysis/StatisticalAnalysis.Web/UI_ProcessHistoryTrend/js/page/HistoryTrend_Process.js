var IsFirstLoadChart;

$(function () {
    initializeDateTimePickers();
    IsFirstLoadChart = true;
});

// 标签选择器选择事件

function onTagItemSelect(item) {
    
    // 将选中标签添加至标签列表

    AddTagItem(item);
}

// 初始化时间选择器

function initializeDateTimePickers() {
    var m_DateTime = new Date();
    var m_NowStr = m_DateTime.format("yyyy-MM-dd hh:mm:ss");
    $('#StartTime').datetimebox('setValue', m_NowStr);
    $('#EndTime').datetimebox('setValue', m_NowStr);
}

// 查询历史趋势

function queryHistoryTrend() {

    // 获取起始时间段

    var m_StartTime = $('#StartTime').datetimebox('getValue');
    var m_EndTime = $('#EndTime').datetimebox('getValue');

    // 获取标签里诶博阿

    var m_TagInfoObject = $('#grid_SelectedObj').datagrid('getData');

    // 条件检测

    if (m_StartTime == "" || m_EndTime == "") {
        $.messager.alert('提示', '请选择查询时间段!');
        return;
    }

    if (m_TagInfoObject['rows'].length == 0) {
        $.messager.alert('提示', "您还没有选择工序!");
        return;
    }

    var m_TagInfoJson = JSON.stringify(m_TagInfoObject);

    $.ajax({
        type: "POST",
        url: "HistoryTrend_Process.aspx/GetChartDataJson",
        data: "{startTime:'" + m_StartTime + "',endTime:'" + m_EndTime + "',tags:'" + m_TagInfoJson + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);

            if (m_MsgData == null || m_MsgData == undefined || m_MsgData == NaN) {
                $.messager.alert('提示', "生成趋势失败!");
                return;
            }
            if (m_MsgData["rows"].length == 0) {
                $.messager.alert('提示', "生成趋势失败!");
                return;
            }

            var m_WindowContainerId = 'Windows_Container';
            if (IsFirstLoadChart == true) {
                IsFirstLoadChart = false;
            }
            else {
                ReleaseGridChartObj(m_WindowContainerId);
            }

            CreateGridChart(m_MsgData, m_WindowContainerId, true, "Line");
        }
    });
}

// 弹出标签选择窗口

function popupTagSelector() {
    $('#dlg_SelectProcessTags').dialog('open');
    if ($('#tagSelector').attr('src') == 'about:blank')
        $('#tagSelector').attr('src', 'ProcessSelector.aspx?PageId=5CE25714-15AE-490B-947E-13C28BA20316');
}

// 添加标签

function AddTagItem(tag) {

    // 最多只允许添加8个标签

    if($('#grid_SelectedObj').datagrid('getRows').length >= 8){
        $.messager.alert('提示','最多允许添加8个标签!');
        return;
    }

    // 添加标签

    $('#grid_SelectedObj').datagrid('appendRow', {
        OrganizationID: tag.OrganizationID,
        LevelCode: tag.LevelCode,
        Name: tag.Name
    });
}


// 清空标签列表

function clearTagItems() {
    $('#grid_SelectedObj').datagrid('loadData', { 'rows': [], 'total': 0 });
}