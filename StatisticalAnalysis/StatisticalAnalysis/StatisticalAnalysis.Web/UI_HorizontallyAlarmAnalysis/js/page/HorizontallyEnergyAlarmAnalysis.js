//全局变量
//var g_labelName = {
//    O0301: "白银公司白银分厂", O030101: '白银公司1#水泥磨',
//    O030102: '白银公司2#水泥磨', O030103: '白银公司1#熟料线',O030104:'白银公司1#余热发电'
//};//格式{OrganizationID：名字}
var g_labelName;
var g_labelList = [];//标签列表数组（参与对比的标签LevelCode）
var g_labelLengh = 0;
var g_plot1=new Object();

$(document).ready(function () {
    initDate();
    initDatagrid('', 'first');
    initLabelName();
})

function initDate() {
    var date = new Date();
    var startDate = new Date();
    startDate.setDate(startDate.getDate() - 10);
    var formateDate = date.getFullYear() + '-' + (date.getMonth() + 1)+'-' + date.getDate() + " " + date.toTimeString();
    var formateStartDate = startDate.getFullYear() + '-' + (startDate.getMonth() + 1)+'-' + startDate.getDate() + " " + startDate.toTimeString();
    $('#StartTime').datetimebox('setValue', formateStartDate);
    $('#EndTime').datetimebox('setValue', formateDate);
}
//加载datagrid
function initDatagrid(myData,myType) {
    if (myType == 'first') {
        $('#labelListId').datagrid({
            data: myData,
            rownumbers: true,
            singleSelect: true,
            striped:true,
            columns: [[
                { field: 'Name', title: '名称', width: 250 },
                { field: 'LevelCode', title: '组织机构', width: 100, hidden: true }
            ]],
            onDblClickRow: function (index, field, value) {
                $('#labelListId').datagrid('deleteRow', index);
                g_labelList.splice(index, 1);
                //若标签数组为空，则置标签长度为0
                if (g_labelList.length == 0) {
                    g_labelLengh = 0;
                }
            }
        });
    }
    else {
        $('#labelListId').datagrid('reload', myData);
    }
}
//获取标签名称
function initLabelName() {
    var myURL = "HorizontallyEnergyAlarmAnalysis.aspx/GetLableName";
    var sendData = '';
    $.ajax({
        type: "POST",
        url: myURL,
        data: sendData,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            g_labelName=JSON.parse(msg.d);
        }
    });
}

//目录树双击事件
function onOrganisationTreeClick(node) {
    
    //var myOrganizationId = node.OrganizationId;
    var myLevelCode = node.id;
    //获取标签的长度
    if (g_labelLengh == 0) {
        g_labelLengh = myLevelCode.length;
    }
    else {
        if (g_labelLengh != myLevelCode.length) {
            $.messager.alert('提示', '该标签与已添加的标签级别不同！');
            return;
        }
    }
    var myName = g_labelName[myLevelCode];
    //var t_json = labelObj(myOrganizationId, myName);
    if (g_labelList.contains(myLevelCode) == true) {
        $.messager.alert('提示', '该标签已经存在！');
        return;
    }
    g_labelList.push(myLevelCode);
    datagridAppendRow(myLevelCode, myName);
}

//标签对象构造函数
function labelObj(myOrganizationId, myName) {
    var t_label = new Object();
    t_label.OrganizationID = myOrganizationId;
    t_label.Name = myName;
    return t_label;
}
//追加新行
function datagridAppendRow(levelCode,name) {
    $('#labelListId').datagrid('appendRow',
        {
            LevelCode: levelCode,
            Name: name
        });
}
function refresh() {
    query();
}
function query() {
    var startTime = $("#StartTime").datetimebox('getValue');
    var endTime = $("#EndTime").datetimebox('getValue');
    var myURL = "HorizontallyEnergyAlarmAnalysis.aspx/GetData";
    var sendData = '{levelCodeString:"' + g_labelList + '",startTime:"' + startTime + '",endTime:"' + endTime + '",labelLength:"' + g_labelLengh + '"}';
    $.ajax({
        type: "POST",
        url: myURL,
        data: sendData,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var myData = JSON.parse(msg.d);
            var count = g_labelList.length;
            for (var i = 0; i < count; i++) {               
                var t_label = g_labelName[g_labelList[i]];
                if (myData['rows'][0][t_label] == undefined) {
                    myData['rows'][0][t_label] = 0;
                    var t_obj = new Object();
                    t_obj.title = t_label;
                    t_obj.field = t_label;
                    myData['columns'].splice(i + 1, 0, t_obj);
                }
                myData['columns'][i + 1]['width'] = 150;
            }
            updateGridChart(myData);
        }
    });
}

function updateGridChart(myData) {   
    updateGrid(myData);
    updateChart(myData);
}
function updateGrid(myData) {
    $('#GridId').unbind(); // for iexplorer  
    $('#GridId').empty();
    $('#GridId').datagrid({
        columns: [myData["columns"]],
        data: myData['rows']
    });
}
function updateChart(myData) {
    var chartData = [];
    for (var item in myData["rows"][0]) {
        if (item == "RowName") {
            continue;
        }
        var t_array = [];
        t_array.push(item);
        t_array.push(parseInt(myData['rows'][0][item]));
        chartData.push(t_array);
    }
    $('#ChartId').unbind(); // for iexplorer  
    $('#ChartId').empty();
    //g_plot1.destroy();
    g_plot1 = jQuery.jqplot('ChartId', [chartData],
    {
        seriesDefaults: {
            // Make this a pie chart.
            renderer: jQuery.jqplot.PieRenderer,
            rendererOptions: {
                // Put data labels on the pie slices.
                // By default, labels show the percentage of the slice.
                showDataLabels: true
            }
        },
        legend: { show: true, location: 'e' }
    }
  );
}

//清空列表
function removeAll() {
    g_labelLengh = 0;
    var count = g_labelList.length;
    g_labelList = [];
    for (var i = 0; i < count; i++) {
        $('#labelListId').datagrid('deleteRow',0);
    }
}
//判断数组内元素是否存在
Array.prototype.contains = function (arr) {
    for (var i = 0; i < this.length; i++) {//this指向真正调用这个方法的对象  
        if (this[i] == arr) {
            return true;
        }
    }
    return false;
}