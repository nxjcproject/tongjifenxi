var MaxBarValue = 0;
$(function () {
    InitDateBoxComponent();
    InitEquipmentCommonComponent();
    InitHaltReasonTypeInfo();
    InitLevelCodeInfo();
    InitializeMachineHaltGrid("MachineHaltInfo", { "rows": [], "total": 0 });
});
function InitDateBoxComponent() {
    var m_MonthDate = new Date();  //上月日期  
    var m_FullYear = m_MonthDate.getFullYear();
    var m_Month = m_MonthDate.getMonth() + 1;
    var m_Day = m_MonthDate.getDate();
    var m_StartTimeString = m_FullYear.toString();
    var m_EndTimeString = m_FullYear.toString();
    if (m_Month >= 10) {
        m_StartTimeString = m_StartTimeString + "-" + m_Month.toString();
        m_EndTimeString = m_EndTimeString + "-" + m_Month.toString();
    }
    else {
        m_StartTimeString = m_StartTimeString + "-0" + m_Month.toString();
        m_EndTimeString = m_EndTimeString + "-0" + m_Month.toString();
    }
    if (m_Day >= 10) {
        m_EndTimeString = m_EndTimeString + "-" + m_Day.toString();
    }
    else {
        m_EndTimeString = m_EndTimeString + "-0" + m_Day.toString();
    }
    m_StartTimeString = m_StartTimeString + "-01";
    $('#StartTimeF').datebox('setValue', m_StartTimeString);
    $('#EndTimeF').datebox('setValue', m_EndTimeString);
}
function InitEquipmentCommonComponent() {
    $.ajax({
        type: "POST",
        url: "DowntimeReasonAnalysis.aspx/GetEquipmentCommonInfo",
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            if (m_MsgData != null && m_MsgData != undefined && m_MsgData["rows"].length > 0) {
                var m_ResultData = [];
                m_ResultData.push({ "id": "All", "text": "全部" });
                for (var i = 0; i < m_MsgData.rows.length; i++) {
                    m_ResultData.push(m_MsgData.rows[i]);
                }
                $('#Select_EquipmentCommonInfoF').combobox('loadData', m_ResultData);
                $('#Select_EquipmentCommonInfoF').combobox("setValue", m_ResultData[0].id);
            }
        }
    });
}
function InitLevelCodeInfo() {
    $.ajax({
        type: "POST",
        url: "DowntimeReasonAnalysis.aspx/GetLevelCodeInfo",
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            var m_ResultData = [];
            if (m_MsgData != null && m_MsgData != undefined) {
                m_ResultData.push({ "id": "O", "text": "全部", "children": [] });
                for (var i = 0; i < m_MsgData.length; i++) {
                    m_ResultData.push(m_MsgData[i]);
                }
                $('#ComboTree_LevelCodeF').combotree('loadData', m_ResultData);
                $('#ComboTree_LevelCodeF').combotree('tree').tree("collapseAll");
                $('#ComboTree_LevelCodeF').combotree("setValue", m_ResultData[0].id);
            }

        }
    });
}
function InitHaltReasonTypeInfo() {
    $.ajax({
        type: "POST",
        url: "DowntimeReasonAnalysis.aspx/GetHaltReasonTypeInfo",
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            var m_ResultData = [];
            if (m_MsgData != null && m_MsgData != undefined) {
                m_ResultData.push({ "id": "All", "text": "全部", "children": [] });
                for (var i = 0; i < m_MsgData.length; i++) {
                    m_ResultData.push(m_MsgData[i]);
                }
                m_ResultData.push({ "id": "Null", "text": "无原因", "children": [] });
                $('#ComboTree_ReasonTypeInfoF').combotree('loadData', m_ResultData);
                $('#ComboTree_ReasonTypeInfoF').combotree('tree').tree("collapseAll");
                $('#ComboTree_ReasonTypeInfoF').combotree("setValue", m_ResultData[0].id);
            }

        }
    });
}
function query() {
    var m_StartTime = $('#StartTimeF').datebox('getValue');
    var m_EndTime = $('#EndTimeF').datebox('getValue');
    var m_EquipmentCommonId = $('#Select_EquipmentCommonInfoF').combobox('getValue');
    var m_StaticsMethod = $('#Select_StaticsMethodF').combobox('getValue');
    var m_LevelCode = $('#ComboTree_LevelCodeF').combotree('getValue');
    var m_ReasonType = $('#ComboTree_ReasonTypeInfoF').datebox('getValue');
    if (m_StartTime == null || m_StartTime == undefined) {
        alert("请选择开始时间!");
    }
    else if (m_EndTime == null || m_EndTime == undefined) {
        alert("请选择结束时间!");
    }
    else if (m_EquipmentCommonId == null || m_EquipmentCommonId == undefined) {
        alert("请选择设备名称!");
    }
    else if (m_StaticsMethod == null || m_StaticsMethod == undefined) {
        alert("请选择统计方式!");
    }
    else if (m_LevelCode == null || m_LevelCode == undefined) {
        alert("请选择生产单位!");
    }
    else if (m_ReasonType == null || m_ReasonType == undefined) {
        alert("请请选择停机类型!");
    }
    else {
        $.ajax({
            type: "POST",
            url: "DowntimeReasonAnalysis.aspx/GetHaltReasonStaticsChart",
            data: "{myStartTime:'" + m_StartTime + "',myEndTime:'" + m_EndTime + "',myEquipmentCommonId:'" + m_EquipmentCommonId +
                "',myStaticsMethod:'" + m_StaticsMethod + "',myLevelCode:'" + m_LevelCode + "',myReasonType:'" + m_ReasonType + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                var m_MsgData = jQuery.parseJSON(msg.d);
                if (m_MsgData != null && m_MsgData != undefined) {
                    if (m_MsgData["rows"] != null && m_MsgData["rows"] != undefined) {
                        MaxBarValue = 0;
                        for (var i = 0; i < m_MsgData["rows"].length; i++) {
                            if (parseFloat(m_MsgData["rows"][i]["Value"]) > MaxBarValue) {
                                MaxBarValue = parseFloat(m_MsgData["rows"][i]["Value"]);
                            }
                        }
                    }
                    if (MaxBarValue == 0) {
                        MaxBarValue = 1;
                    }
                    $('#grid_MachineHaltInfo').datagrid('loadData', m_MsgData);
                }
                else {
                    if (MaxBarValue == 0) {
                        MaxBarValue = 1;
                    }
                    $('#grid_MachineHaltInfo').datagrid('loadData', { "rows": [], "total": 0 });
                }


                //$('#BarChart').empty();
                //if (m_MsgData != null && m_MsgData != undefined && m_MsgData["rows"] != null && m_MsgData["rows"] != undefined) {
                //    var m_BarChartHtml = '<table class = "BarChartTable">';
                //    for (var i = 0; i < m_MsgData["rows"].length; i++) {
                //        m_BarChartHtml = m_BarChartHtml + '<tr><td class = "RowName">' + m_MsgData["rows"][0]["Name"] + '</td><td></td></tr>';
                //    }
                //    m_BarChartHtml = m_BarChartHtml + '</table>';
                //    $('#BarChart').html(m_BarChartHtml);
                //}
            }
        });
    }
}
function InitializeMachineHaltGrid(myGridId, myData) {
    $('#grid_' + myGridId).datagrid({
        title: '',
        data: myData,
        fit: true,
        border: true,
        dataType: "json",
        striped: true,
        //loadMsg: '',   //设置本身的提示消息为空 则就不会提示了的。这个设置很关键的
        rownumbers: true,
        singleSelect: true,
        idField: 'StandardItemId',
        frozenColumns: [[{
            width: '200',
            title: 'ID',
            field: 'MachineHaltReasonId',
            hidden: true
        }, {
            width: '400',
            title: '名称',
            field: 'ReasonText'
        }]],
        columns: [[{
            width: '720',
            title: '值',
            field: 'Value',
            formatter: function (value, row) {
                var str = "";
                if (value != "" && value != null && value != undefined) {
                    str = "<table class = \"BarChartTable\"><tr><td class = \"BarChartTd\" style= \"width:" + 600 * value / MaxBarValue + "px;\"></td><td class = \"BarValue\">" + value + "</td></tr></table>";
                } else {
                    str = "";
                }
                return str;
            }
        }]],
        toolbar: '#toolbar_' + myGridId
    });
}

