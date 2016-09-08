var m_GanttChartObj = null;
var m_ChartFristLoad = true;
$(function () {
    InitDateBoxComponent();
    InitReasonTypeComponent();
    InitGanttChartTestLayout();
});
function InitGanttChartTestLayout() {
    $('#GanttChartTest').resize(function () {
        if (m_GanttChartObj != null) {
            $('#GanttChartTest').GanttChart("resize");
        }
    });
}
// 获取双击组织机构时的组织机构信息
function onOrganisationTreeClick(node) {

    // 设置组织机构ID
    // organizationId为其它任何函数提供当前选中的组织机构ID
    $('#OrganizationIdF').val(node.OrganizationId);

    // 设置组织机构名称
    // 用于呈现，在界面上显示当前的组织机构名称
    $('#OrganizationNameF').textbox('setText', node.text);
    LoadEquipmentData(node.OrganizationId);
}
function InitGanttChart(myData) {
    m_GanttChartObj = $('#GanttChartTest').GanttChart({
        data: myData,
        options: {
            "backGround-color": "#f1f1ff",
            "yaxis": {
                "size": 9,
                "font-weight": "normal",
                "left": 220
            },
            "xaxis": {
                "color": "black",
                "showGridLine": true
            }
        }
    });

    //{
    //    "aaaa": [{ "start": "2016-01-01 08:32:11", "end": "2016-01-02 11:21:23" }, { "start": "2016-01-04 08:32:11", "end": "2016-01-05 11:21:23" }, { "start": "2016-01-09 08:32:11", "end": "2016-01-12 11:21:23" }],
    //    "bbbb": [{ "start": "2016-01-11 08:32:11", "end": "2016-01-12 11:21:23" }, { "start": "2016-01-06 08:32:11", "end": "2016-01-08 11:21:23" }, { "start": "2016-01-21 08:32:11", "end": "2016-01-22 11:21:23" }],
    //    "cccc": [{ "start": "2016-01-11 08:32:11", "end": "2016-01-12 11:21:23" }, { "start": "2016-01-06 08:32:11", "end": "2016-01-08 11:21:23" }, { "start": "2016-01-21 08:32:11", "end": "2016-01-22 11:21:23" }],
    //    "dddd": [{ "start": "2016-01-11 08:32:11", "end": "2016-01-12 11:21:23" }, { "start": "2016-01-06 08:32:11", "end": "2016-01-08 11:21:23" }, { "start": "2016-01-21 08:32:11", "end": "2016-01-22 11:21:23" }],
    //    "eeee": [{ "start": "2016-01-11 08:32:11", "end": "2016-01-12 11:21:23" }, { "start": "2016-01-06 08:32:11", "end": "2016-01-08 11:21:23" }, { "start": "2016-01-21 08:32:11", "end": "2016-01-22 11:21:23" }],
    //    "ffff": [{ "start": "2016-01-11 08:32:11", "end": "2016-01-12 11:21:23" }, { "start": "2016-01-06 08:32:11", "end": "2016-01-08 11:21:23" }, { "start": "2016-01-21 08:32:11", "end": "2016-01-22 11:21:23" }],
    //    "gggg": [{ "start": "2016-01-11 08:32:11", "end": "2016-01-12 11:21:23" }, { "start": "2016-01-06 08:32:11", "end": "2016-01-08 11:21:23" }, { "start": "2016-01-21 08:32:11", "end": "2016-01-22 11:21:23" }],
    //    "hhhh": [{ "start": "2016-01-11 08:32:11", "end": "2016-01-12 11:21:23" }, { "start": "2016-01-06 08:32:11", "end": "2016-01-08 11:21:23" }, { "start": "2016-01-21 08:32:11", "end": "2016-01-22 11:21:23" }],
    //    "iiii": [{ "start": "2016-01-11 08:32:11", "end": "2016-01-12 11:21:23" }, { "start": "2016-01-06 08:32:11", "end": "2016-01-08 11:21:23" }, { "start": "2016-01-21 08:32:11", "end": "2016-01-22 11:21:23" }],
    //    "jjjj": [{ "start": "2016-01-11 08:32:11", "end": "2016-01-12 11:21:23" }, { "start": "2016-01-06 08:32:11", "end": "2016-01-08 11:21:23" }, { "start": "2016-01-21 08:32:11", "end": "2016-01-22 11:21:23" }]

    //}
    //$('#GanttChartTest').GanttChart("destory");
    //$('#GanttChartTest').GanttChart("loadData", { "title": "test" });
    //$('#GanttChartTest').GanttChart("resize");
}
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
function LoadEquipmentData(myOrganizationId) {
    $.ajax({
        type: "POST",
        url: "DowntimeGanttAnalysis.aspx/GetEquipmentInfo",
        data: "{myOrganizationId:'" + myOrganizationId + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = jQuery.parseJSON(msg.d);
            if (m_MsgData != null && m_MsgData != undefined && m_MsgData["rows"].length > 0) {
                $('#Select_EquipmentInfoF').combobox('loadData', m_MsgData.rows);
                $('#Select_EquipmentInfoF').combobox("setValue", m_MsgData.rows[0].id);
            }
        }
    });
}
function query() {
    var m_StartTime = $('#StartTimeF').datebox('getValue');
    var m_EndTime = $('#EndTimeF').datebox('getValue');
    var m_EquipmentId = $('#Select_EquipmentInfoF').combobox('getValue');
    var m_HaltTypeId = $('#Select_HaltTypeF').combobox('getValue');
    var m_ReasonTypeId = $('#Select_ReasonTypeF').combobox('getValue');
    var m_DisplayOrder = $('#Select_DisplayOrderF').combobox('getValue');
    var m_OrganizationId = $('#OrganizationIdF').val();
    if (m_ReasonTypeId == null || m_ReasonTypeId == undefined) {
        m_ReasonTypeId = "";
    }
   
    if (m_StartTime == null || m_StartTime == undefined) {
        alert("请选择开始时间!");
    }
    else if (m_EndTime == null || m_EndTime == undefined) {
        alert("请选择结束时间!");
    }
    else if (m_EquipmentId == null || m_EquipmentId == undefined) {
        alert("请选择设备名称!");
    }
    else {
        $.ajax({
            type: "POST",
            url: "DowntimeGanttAnalysis.aspx/GetHaltReasonStaticsGanttChart",
            data: "{myStartTime:'" + m_StartTime + "',myEndTime:'" + m_EndTime + "',myEquipmentId:'" + m_EquipmentId + "',myDisplayOrder:'" + m_DisplayOrder +
                "',myHaltTypeId:'" + m_HaltTypeId + "',myReasonTypeId:'" + m_ReasonTypeId + "',myOrganizationId:'" + m_OrganizationId + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg) {
                var m_MsgData = jQuery.parseJSON(msg.d);
                if (m_MsgData != null && m_MsgData != undefined) {
                    if (m_ChartFristLoad == true) {
                        InitGanttChart(m_MsgData);
                        m_ChartFristLoad = false;
                    }
                    else {
                        $('#GanttChartTest').GanttChart("loadData", m_MsgData);
                    }
                }
                else {
                    $('#GanttChartTest').GanttChart("destory");
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