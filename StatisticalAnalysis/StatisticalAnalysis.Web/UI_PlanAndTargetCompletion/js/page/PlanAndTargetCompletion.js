var IsFirstLoadChart = "true";
var productLineType;
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
    $('#StartTime').datebox('setValue', tenDaysEarly.toLocaleDateString().replace(/\//g, '-'));
    $('#EndTime').datebox('setValue', now.toLocaleDateString().replace(/\//g, '-'));
    $('#year').numberspinner('setValue', now.getFullYear());
});


// 获取双击组织机构时的组织机构信息
function onOrganisationTreeClick(node) {

    // 设置组织机构ID
    // organizationId为其它任何函数提供当前选中的组织机构ID

    $('#organizationId').val(node.OrganizationId);

    // 设置组织机构名称
    // 用于呈现，在界面上显示当前的组织机构名称

    $('#txtOrganization').textbox('setText', node.text);
    productLineType = node.OrganizationType;
    $.ajax({
        type: "POST",
        url: "PlanAndTargetCompletion.aspx/GetItemList",
        data: "{type:'" + productLineType+"'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var json=JSON.parse(msg.d);
            $('#itemName').combobox({
                data: json,
                valueField: 'QuotasID',
                textField: 'QuotasID'
            });
        }
          }
        );
}

function Query() {
    $('#' + windowID).empty();
    var organizationId = $('#organizationId').val();

    if (organizationId == '') {
        $.messager.alert('提示', '请先选择需要分析的组织机构。');
        return;
    }


    // 获取分析类型
    var analysisType = $("input[name='analysisType']:checked").val();
    var item = $('#itemName').combobox('getValue');
    var date = $('#year').numberspinner('getValue');
    var win = $.messager.progress({
        title: '请稍后',
        msg: '数据载入中...'
    });
    $.ajax({
        type: "POST",
        url: "PlanAndTargetCompletion.aspx/GetChart",
        data: "{organizationId:'" + organizationId + "',item:'" + item + "',date:'" + date + "'}",
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
    var imageType = $('#imageType').combobox('getValue');

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
        str = '<tr><td>无报警记录</td></tr>';
        total = 0;
    }

    // 更新占位符

    //$('#countByTypes').html(str);


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


///////////////////////////////////////////打开window窗口//////////////////////////////////////////
function WindowsDialogOpen(myContainerId, myData, myWidth, myHeight, myImageType) {
    var m_WindowId = OpenWindows(myContainerId, '数据分析', myWidth, myHeight); //弹出windows
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
//将chart转化为图片
function chartToImage() {
    //alert("");

    var j = $('#' + windowID + '_Chart').jqplotToImageElem();
    $('#' + windowID).empty();
    $('#' + windowID).append(j);
}