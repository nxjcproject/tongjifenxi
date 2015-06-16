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
    m_DateTime.setDate(m_DateTime.getDate() - 1);
    var m_YestedayStr = m_DateTime.format("yyyy-MM-dd hh:mm:ss");
    $('#StartTime').datetimebox('setValue', m_YestedayStr);
    $('#EndTime').datetimebox('setValue', m_NowStr);
}

// 查询历史趋势

function queryHistoryTrend() {

    // 获取起始时间段

    var m_StartTime = $('#StartTime').datetimebox('getValue');
    var m_EndTime = $('#EndTime').datetimebox('getValue');
    var m_TimeInterval = $('#TimeInterval').datetimebox('getValue');
    // 获取标签里诶博阿

    var m_TagInfoObject = $('#grid_SelectedObj').datagrid('getData');

    // 条件检测

    if (m_StartTime == "" || m_EndTime == "") {
        $.messager.alert('提示', '请选择查询时间段!');
        return;
    }

    if (m_TagInfoObject['rows'].length == 0) {
        $.messager.alert('提示', "您还没有选择电表!");
        return;
    }

    var m_TagInfoJson = JSON.stringify(m_TagInfoObject);

    $.ajax({
        type: "POST",
        url: "HistoryTrend_Ammeters.aspx/GetChartDataJson",
        data: "{startTime:'" + m_StartTime + "',endTime:'" + m_EndTime + "',timeInterval:'" + m_TimeInterval + "',tags:'" + m_TagInfoJson + "'}",
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

            CreateGridChart(m_MsgData, m_WindowContainerId, true, "DateXLine");
        }
    });
}

// 弹出标签选择窗口

function popupTagSelector() {
    $('#dlg_SelectProcessTags').dialog('open');
    //if ($('#tagSelector').attr('src') == 'about:blank')
    //    $('#tagSelector').attr('src', 'ProcessSelector.aspx?PageId=5CE25714-15AE-490B-947E-13C28BA20316');
}

// 添加标签

function AddTagItem(tag) {

    // 最多只允许添加8个标签

    if ($('#grid_SelectedObj').datagrid('getRows').length >= 8) {
        $.messager.alert('提示', '最多允许添加8个标签!');
        return;
    }

    // 添加标签

    $('#grid_SelectedObj').datagrid('appendRow', {
        OrganizationId: tag.OrganizationId,
        TagTableName: tag.TagTableName,
        TagColumnName: tag.TagColumnName,
        VariableId: tag.VariableId,
        Name: tag.Name
    });
}


// 清空标签列表

function clearTagItems() {
    $('#grid_SelectedObj').datagrid('loadData', { 'rows': [], 'total': 0 });
}



//////////////////////////////////////////////标签选择器///////////////////////////////////////////////////
function onTagItemSelect(row) {
    var m_TagName = row.Name;
    var m_OrganizationName = $('#txtOrganization').textbox("getText");
    //var m_TreeData = $('#processTable').treegrid('getParent', row.id)
    //while (m_TreeData != null && m_TreeData != undefined && m_TreeData != NaN) {
    //    m_TagName = m_TreeData.Name + '>>' + m_TagName;
    //    m_TreeData = $('#processTable').treegrid('getParent', m_TreeData.id);
    //}
    if (row.LevelType == "Ammeters") {
        row.Name = m_OrganizationName + '>>' + m_TagName;
        AddTagItem(row);
    }
    else {
        $.messager.alert('提示', '请选择具体电表!');
    }
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
    loadProcess();
}


// 所有公式组
function loadProcess() {
    var organizationId = $('#organizationId').val();
    var queryUrl = 'HistoryTrend_Ammeters.aspx/GetAmmetersTreeGridFormat';
    var dataToSend = '{organizationId: "' + organizationId + '"}';

    $.ajax({
        type: "POST",
        url: queryUrl,
        data: dataToSend,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            initializeProcessTable(jQuery.parseJSON(msg.d));
        }
    });
}

function initializeProcessTable(data) {
    $('#processTable').treegrid({
        data: data,
        idField: 'id',
        treeField: 'Name',
        dataType: "json"
    });
}