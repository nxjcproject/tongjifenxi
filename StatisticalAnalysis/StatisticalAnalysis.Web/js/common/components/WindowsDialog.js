var WindowsObjArray = new Array();
var MaxWindowsCount = 8;
var DialogChartOptionsId = "";
$(document).ready(function () {
    for (var i = 0; i < MaxWindowsCount; i++) {
        WindowsObjArray.push("");
    }
    InitializingChartAxesValue();
    getbrowser();          //获得浏览器名称和型号
});
function InitializingChartAxesValue() {
    var m_DlgHtml = '<div id="dlg_ChartAxesValueY" class="easyui-dialog" data-options="iconCls:\'icon-edit\',resizable:false,modal:true" style = "padding-top:5px;">' +
        '<table style="width: 100%;">' +
            '<tr>' +
                '<th style="width:65px;height: 30px;">最小量程</th>' +
                '<td style="width:90px;">' +
                    '<input id="MinAxesValueY" class="easyui-numberbox" data-options="required:false, min:-10000000, max:100000000, value:0,precision:2" style="width: 80px" />' +
                '</td>' +
                '<th style="width:65px;">最大量程</th>' +
                '<td style="width:90px;">' +
                    '<input id="MaxAxesValueY" class="easyui-numberbox" data-options="required:false, min:-100000000, max:100000000, value:100,precision:2" style="width: 80px" />' +
                '</td>' +
                '<td style="width:65px;">' +
                    '<a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:\'icon-reload\'" onclick="ConfirmChartOptionsChange();">刷新</a>' +
                '</td>' +
            '</tr>' +
        '</table>' +
    '</div>';
    $(document.body).append(m_DlgHtml);
    $('#dlg_ChartAxesValueY').dialog({
        title: '修改趋势量程',
        width: 390,
        height: 80,
        closed: true,
        cache: false,
        modal: true,
    });
    $.parser.parse($('#dlg_ChartAxesValueY'));
}
function ConfirmChartOptionsChange() {
    setChartAxesValueY("dlg_ChartAxesValueY", DialogChartOptionsId, $('#MinAxesValueY').numberbox('getValue'), $('#MaxAxesValueY').numberbox('getValue'));
}
function OpenWindows(myParentObjId, myTitle, myWidth, myHeight, myLeft, myTop, myDraggable, myMaximizable, myMaximized, myResizable) {
    if (arguments.length == 9) {
        myResizable = false;
    }

    var m_WindowArrayIndex = GetEmptyWindowArray();
    var m_Title = myTitle + ' 视图' + (m_WindowArrayIndex + 1);
    if (WindowsObjArray.length <= MaxWindowsCount && m_WindowArrayIndex >= 0) {

        var m_WindowsId = 'Windows' + m_WindowArrayIndex;

        //var m_HtmlString = '<div id="' + m_WindowsId + '" class="easyui-window" title="' + myTitle + '" data-options="iconCls:\'icon-save\'" style="width:' + myWidth + 'px;height:' + myHeight + 'px;padding:10px;"></div>';
        var m_HtmlString = '<div id = "' + m_WindowsId + '" style = "overflow:hidden; text-align:center;"><div id = "' + m_WindowsId + '_CustomTools"><a href="javascript:void(0)" class="icon-edit" onclick = "OpenChartAxesValueDialog(\'' + m_WindowsId + '\')"></a></div></div>';
        $('#' + myParentObjId).append(m_HtmlString);                    //添加一个window

        CreateWindow(m_WindowsId, m_Title, myWidth, myHeight, myLeft, myTop, myDraggable, myMaximizable, myMaximized, myResizable);
        WindowsObjArray[m_WindowArrayIndex] = m_WindowsId;                 //window进入队列

        return m_WindowsId;
    }
    else {
        alert("最多允许" + MaxWindowsCount + "个窗口弹出!");
        return "";
    }
}
function OpenChartAxesValueDialog(myWindowsId)
{
    DialogChartOptionsId =  myWindowsId + '_Chart';
    $('#dlg_ChartAxesValueY').dialog('open');
}
function CreateWindow(myWindowId, myTitle, myWidth, myHeight) {
    $('#' + myWindowId).window({   
        width: myWidth,
        height: myHeight,
        title: myTitle,
        collapsible: false,
        minimizable: false,
        maximizable: false,
        resizable: false,
        inline: true,
        iconCls: 'ext-icon-chart_bar',
        padding: 10,
        tools: '#' + myWindowId + '_CustomTools'
        //modal:true  
    }); 
}
function CreateWindow(myWindowId, myTitle, myWidth, myHeight, myLeft, myTop) {
    $('#' + myWindowId).window({
        width: myWidth,
        height: myHeight,
        title: myTitle,
        left: myLeft,
        top: myTop,
        collapsible: false,
        minimizable: false,
        maximizable: false,
        resizable: false,
        inline: true,
        iconCls: 'ext-icon-chart_bar',
        padding: 10,
        tools: '#' + myWindowId + '_CustomTools'

        //modal:true  
    });
} 
function CreateWindow(myWindowId, myTitle, myWidth, myHeight, myLeft, myTop, myDraggable, myMaximizable) {
    $('#' + myWindowId).window({
        width: myWidth,
        height: myHeight,
        title: myTitle,
        left: myLeft,
        top: myTop,
        collapsible: false,
        minimizable: false,
        resizable: false,
        inline: true,
        draggable: myDraggable,
        maximizable: myMaximizable, 
        iconCls: 'ext-icon-chart_bar',
        padding: 10,
        tools: '#' + myWindowId + '_CustomTools'
        //modal:true  
    });
}

function CreateWindow(myWindowId, myTitle, myWidth, myHeight, myLeft, myTop, myDraggable, myMaximizable, myMaximized, myResizable) {
    if (arguments.length == 9) {
        myResizable = false;
    }
    $('#' + myWindowId).window({
        width: myWidth,
        height: myHeight,
        title: myTitle,
        left: myLeft != undefined ? myLeft : 0,
        top: myTop != undefined ? myTop : 0,
        collapsible: false,
        minimizable: false,
        resizable: myResizable != undefined ? myResizable : false,
        inline: true,
        draggable: myDraggable != undefined ? myDraggable : false,
        maximizable: myMaximizable != undefined ? myMaximizable : false,
        maximized: myMaximized != undefined ? myMaximized : false,
        iconCls: 'ext-icon-chart_bar',
        padding: 10,
        tools: '#' + myWindowId + '_CustomTools'

        //modal:true  
    });
}

function GetEmptyWindowArray() {
    var m_ArrayIndex = -1;
    for (var i = 0; i < MaxWindowsCount; i++) {
        if (WindowsObjArray[i] == "") {
            m_ArrayIndex = i;
            break;
        }
    }
    return m_ArrayIndex;
}
function CloseWindow(myObj) {
    for (var i = 0; i < WindowsObjArray.length; i++) {
        if (WindowsObjArray[i] == myObj.attr("id")) {
            WindowsObjArray[i] = "";
        }
    }
    myObj.unbind();
    myObj.parents().next('.window-shadow').remove();        //.css('background', 'red');
    myObj.window('destroy', true);
}
function CloseAllWindows() {
    for (var i = 0; i < MaxWindowsCount; i++) {
        var m_WindowObj = $('#' + WindowsObjArray[i]);
        m_WindowObj.unbind();
        m_WindowObj.parents().next('.window-shadow').remove();        //.css('background', 'red');
        m_WindowObj.window('destroy', true);
        WindowsObjArray[i] = "";
    }
}
function GetWindowIdByObj(myWindowObj) {
    if (myWindowObj != null && myWindowObj != undefined && myWindowObj != "") {
        return myWindowObj.attr("id");
    }
    else {
        return "";
    }
}
function TopWindow(myWindowId) {

    for (var i = 0; i < WindowsObjArray.length; i++) {
        if (WindowsObjArray[i] == myWindowId) {
            $('#' + myWindowId).parent().css('zIndex', 9010);
        }
        else if (WindowsObjArray[i] != "") {
            $('#' + WindowsObjArray[i]).parent().css('zIndex', 9008);
        }
        $('#' + WindowsObjArray[i]).parent().next('.window-shadow').css('zIndex', 9000);
    }
}
function BottomWindow(myWindowId) {
    for (var i = 0; i < WindowsObjArray.length; i++) {
        if (WindowsObjArray[i] == myWindowId) {
            $('#' + myWindowId).parent().css('zIndex', 9010);
        }
        else if (WindowsObjArray[i] != "") {
            $('#' + WindowsObjArray[i]).parent().css('zIndex', 9008);
        }
    }
}
function GetWindowsIdArray() {
    return WindowsObjArray;
}

