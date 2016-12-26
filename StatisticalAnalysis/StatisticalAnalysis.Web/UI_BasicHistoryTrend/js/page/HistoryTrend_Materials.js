var IsFirstLoadChart;
var SelectDatetime = "";
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
    SelectDatetime = m_StartTime + ' 至 ' + m_EndTime;
    var m_TimeInterval = $('#TimeInterval').datetimebox('getValue');
    // 获取标签里诶博阿

    var m_TagInfoObject = $('#grid_SelectedObj').datagrid('getData');

    // 条件检测

    if (m_StartTime == "" || m_EndTime == "") {
        $.messager.alert('提示', '请选择查询时间段!');
        return;
    }

    if (m_TagInfoObject['rows'].length == 0) {
        $.messager.alert('提示', "您还没有选择物料!");
        return;
    }

    var m_TagInfoJson = JSON.stringify(m_TagInfoObject);
    var win = $.messager.progress({
        title: '请稍后',
        msg: '数据载入中...'
    });
    $.ajax({
        type: "POST",
        url: "HistoryTrend_Materials.aspx/GetChartDataJson",
        data: "{startTime:'" + m_StartTime + "',endTime:'" + m_EndTime + "',timeInterval:'" + m_TimeInterval + "',tags:'" + m_TagInfoJson + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            $.messager.progress('close');
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
            WindowsDialogOpen(m_WindowContainerId, m_MsgData, m_ContainerObjWidth, m_ContainerObjHeight);
        },
        beforeSend: function (XMLHttpRequest) {
            win;
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
    if (row.LevelType == "Materials") {
        row.Name = m_OrganizationName + '>>' + m_TagName;
        AddTagItem(row);
    }
    else {
        $.messager.alert('提示', '请选择具体物料!');
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
    var queryUrl = 'HistoryTrend_Materials.aspx/GetMaterialsTreeGridFormat';
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

function ExportFileFun() {
    var m_FunctionName = "ExcelStream";
    var m_Parameter1 = GetDataGridTableHtml("Windows_Container_Grid", "综合对标数据", SelectDatetime);
    var m_Parameter2 = "";

    var m_ReplaceAlllt = new RegExp("<", "g");
    var m_ReplaceAllgt = new RegExp(">", "g");
    m_Parameter1 = m_Parameter1.replace(m_ReplaceAlllt, "&lt;");
    m_Parameter1 = m_Parameter1.replace(m_ReplaceAllgt, "&gt;");

    var form = $("<form id = 'ExportFile'>");   //定义一个form表单
    form.attr('style', 'display:none');   //在form表单中添加查询参数
    form.attr('target', '');
    form.attr('method', 'post');
    form.attr('action', "HistoryTrend_Materials.aspx");

    var input_Method = $('<input>');
    input_Method.attr('type', 'hidden');
    input_Method.attr('name', 'myFunctionName');
    input_Method.attr('value', m_FunctionName);
    var input_Data1 = $('<input>');
    input_Data1.attr('type', 'hidden');
    input_Data1.attr('name', 'myParameter1');
    input_Data1.attr('value', m_Parameter1);
    var input_Data2 = $('<input>');
    input_Data2.attr('type', 'hidden');
    input_Data2.attr('name', 'myParameter2');
    input_Data2.attr('value', m_Parameter2);

    $('body').append(form);  //将表单放置在web中 
    form.append(input_Method);   //将查询参数控件提交到表单上
    form.append(input_Data1);   //将查询参数控件提交到表单上
    form.append(input_Data2);   //将查询参数控件提交到表单上
    form.submit();
    //释放生成的资源
    form.remove();
}
function PrintFileFun() {
    var m_ReportTableHtml = GetDataGridTableHtml("Windows_Container_Grid", "综合对标数据", SelectDatetime);
    PrintHtml(m_ReportTableHtml);
}


///////////////////////////////////////////打开window窗口//////////////////////////////////////////
function WindowsDialogOpen(myContainerId, myData, myWidth, myHeight) {
    var m_WindowId = OpenWindows(myContainerId, 'DCS数据分析', myWidth, myHeight); //弹出windows
    var m_WindowObj = $('#' + m_WindowId);
    CreateGridChart(myData, m_WindowId, true, "DateXLine");               //生成图表
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