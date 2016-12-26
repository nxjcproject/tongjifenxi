var MaxBarValue = 0;
$(function () {
    InitDateBoxComponent();
    InitEquipmentCommonComponent();
    InitReasonTypeComponent();
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
        url: "DowntimeHorizontallyAnalysis.aspx/GetEquipmentCommonInfo",
        data: "",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            if (m_MsgData != null && m_MsgData != undefined && m_MsgData["rows"].length > 0) {
                var m_ResultData = [];
                m_ResultData.push({ "id": "All", "text": "全部"});
                for (var i = 0; i < m_MsgData.rows.length; i++) {
                    m_ResultData.push(m_MsgData.rows[i]);
                }
                $('#Select_EquipmentCommonInfoF').combobox('loadData', m_ResultData);
                $('#Select_EquipmentCommonInfoF').combobox("setValue", m_ResultData[0].id);
            }
        }
    });
}
function InitReasonTypeComponent() {
    var m_ReasonTypeId = $('#Select_HaltTypeF').combobox('getValue');
    SetReasonType(m_ReasonTypeId);
}
function SetReasonType(myReasonTypeId) {
    $.ajax({
        type: "POST",
        url: "DowntimeGanttAnalysis.aspx/GetResonType",
        data: "{myReasonTypeId:'" + myReasonTypeId + "'}",
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
                $('#Select_ReasonTypeF').combobox('loadData', m_ResultData);
                $('#Select_ReasonTypeF').combobox("setValue", m_ResultData[0].id);
            }
            else {
                var m_ResultData = [];
                m_ResultData.push({ "id": "All", "text": "全部" });
                $('#Select_ReasonTypeF').combobox('loadData', m_ResultData);
                $('#Select_ReasonTypeF').combobox("setValue", m_ResultData[0].id);
                //$('#Select_ReasonTypeF').combobox('clear');
            }
        }
    });
}
function query() {
    var m_StartTime = $('#StartTimeF').datebox('getValue');
    var m_EndTime = $('#EndTimeF').datebox('getValue');
    var m_EquipmentCommonId = $('#Select_EquipmentCommonInfoF').combobox('getValue');
    var m_StaticsMethod = $('#Select_StaticsMethodF').combobox('getValue');
    var m_StaticsRange = $('#Select_StaticsRangeF').combobox('getValue');
    var m_HaltTypeId = $('#Select_HaltTypeF').combobox('getValue');
    var m_ReasonTypeId = $('#Select_ReasonTypeF').combobox('getValue');
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
    else if (m_StaticsRange == null || m_StaticsRange == undefined) {
        alert("请选择统计范围!");
    }
    else if (m_HaltTypeId == null || m_HaltTypeId == undefined) {
        alert("请请选择停机类型!");
    }
    else if (m_ReasonTypeId == null || m_ReasonTypeId == undefined) {
        alert("请请选择原因类型!");
    }
    else {
        var win = $.messager.progress({
            title: '请稍后',
            msg: '数据载入中...'
        });
        $.ajax({
            type: "POST",
            url: "DowntimeHorizontallyAnalysis.aspx/GetHaltReasonStaticsChart",
            data: "{myStartTime:'" + m_StartTime + "',myEndTime:'" + m_EndTime + "',myEquipmentCommonId:'" + m_EquipmentCommonId +
                "',myStaticsMethod:'" + m_StaticsMethod + "',myStaticsRange:'" + m_StaticsRange + "',myHaltTypeId:'" + m_HaltTypeId + "',myReasonTypeId:'" + m_ReasonTypeId + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                $.messager.progress('close');
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
            },
            beforeSend: function (XMLHttpRequest) {
                win;
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
            field: 'Id',
            hidden: true
        },{
            width: '140',
            title: '名称',
            field: 'Name'
        }]],
        columns: [[{
            width: '920',
            title: '值',
            field: 'Value',
            formatter: function (value, row) {
                var str = "";
                if (value != "" && value != null && value != undefined) {
                    str = "<table class = \"BarChartTable\"><tr><td class = \"BarChartTd\" style= \"width:" + 800 * value / MaxBarValue + "px;\"></td><td class = \"BarValue\">" + value + "</td></tr></table>";
                } else {
                    str = "";
                }
                return str;
            }
        },
        {
            width: '60',
            title: '所占比例',
            field: 'HaltScale'
        }]],
        toolbar: '#toolbar_' + myGridId
    });
}

