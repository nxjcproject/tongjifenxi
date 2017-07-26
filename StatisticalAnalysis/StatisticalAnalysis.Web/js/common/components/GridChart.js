var BrowserName = "IE";
var BrowserVersion = 8;
var GridObjContainerIdArray = new Array();
var GridObjArray = new Array();
var ChartObjContainerIdArray = new Array();
var ChartObjArray = new Array();

function GetChart(myGridObjContainerId, myChartObjContainerId, myData, myChartType, myTitle) {
    if (myGridObjContainerId != null && myGridObjContainerId != undefined && myGridObjContainerId != "") {
        ///////////////////////////////显示datagrid/////////////////////////
        GetGridObjById(myGridObjContainerId, myData, myTitle);
    }
    if (myChartObjContainerId != null && myChartObjContainerId != undefined && myChartObjContainerId != "") {
        ///////////////////////////////////显示chart/////////////////////////////
        GetChartObjById(myChartObjContainerId, myData, myChartType, myTitle);
    }
}
/////////////////////////获得grid/////////////////////////
function GetGridObjById(myGridObjContainerId, myData, myTitle) {
    var m_ExistsIndex = FindArrayIndexByContainerId(GridObjContainerIdArray, myGridObjContainerId);
    if (m_ExistsIndex < 0) {
        ////////////////////////////////////////第一次加载/////////////////////////////////////////
        GridObjArray.push(InitializeGrid(myGridObjContainerId, myData));
        GridObjContainerIdArray.push(myGridObjContainerId);
    }
    else {
        /////////////////////////////////////////////已经加载过一次////////////////////////////////
        //GridObjArray[m_ExistsIndex].datagrid('loadData', myData.rows);
        InitializeGrid(GridObjArray[m_ExistsIndex][0].id, myData);
    }
}
/////////////////////////////获得chart/////////////////////////
function GetChartObjById(myChartObjContainerId, myData, myChartType, myTitle) {
    var m_ExistsIndex = FindArrayIndexByContainerId(ChartObjContainerIdArray, myChartObjContainerId);
    if (m_ExistsIndex < 0) {
        ////////////////////////////////////////第一次加载/////////////////////////////////////////
        var m_ChartObj = CreatChart(myChartObjContainerId, myData, myChartType, myTitle)
        ChartObjArray.push(m_ChartObj);
        ChartObjContainerIdArray.push(myChartObjContainerId);
    }
    else {
        /////////////////////////////////////////////已经加载过一次////////////////////////////////
        ReleaseChartObj(myChartObjContainerId);

        ChartObjArray.push(CreatChart(myChartObjContainerId, myData, myChartType, myTitle));
        ChartObjContainerIdArray.push(myChartObjContainerId);
    }
}
/////////////////在队列中查找是否该对象已经存在，如果存在返回在array中的索引index///////////////
function FindArrayIndexByContainerId(myContainerIdArray, myContainerId)
{
    var m_ExistsIndex = -1;
    for (var i = 0; i < myContainerIdArray.length; i++) {
        if (myContainerIdArray[i] == myContainerId) {
            m_ExistsIndex = i;
            break;
        }
    }
    return m_ExistsIndex;
}
function ReleaseChartObj(myChartContainerId) {
    var m_ChartExistsIndex = FindArrayIndexByContainerId(ChartObjContainerIdArray, myChartContainerId);
    if (m_ChartExistsIndex >= 0) {
        ReleasePlotChart(myChartContainerId, ChartObjArray[m_ChartExistsIndex]);
        ChartObjContainerIdArray.splice(m_ChartExistsIndex, 1);
        ChartObjArray.splice(m_ChartExistsIndex, 1);
    }
}
function ReleaseGridObj(myGridContainerId) {
    var m_GridExistsIndex = FindArrayIndexByContainerId(GridObjContainerIdArray, myGridContainerId);
    if (m_GridExistsIndex >= 0) {
        ReleaseGird(myGridContainerId, GridObjArray[m_GridExistsIndex]);
        GridObjContainerIdArray.splice(m_GridExistsIndex, 1);
        GridObjArray.splice(m_GridExistsIndex, 1);
    }
}
function ReleaseGridChartObj(myChartContainerId) {
    ReleaseChartObj(myChartContainerId + '_Chart');
    ReleaseGridObj(myChartContainerId + '_Grid');
}
function ReleaseAllGridChartObj(myChartContainerId) {
    for (var i = 0; i < ChartObjArray.length; i++) {
        ReleasePlotChart(myChartContainerId, ChartObjArray[i]);
    }
    ChartObjContainerIdArray.splice(0, ChartObjContainerIdArray.length);
    ChartObjArray.splice(0, ChartObjArray.length);

    for (var i = 0; i < GridObjArray.length; i++) {
        ReleaseGird(myChartContainerId, GridObjArray[i]);
    }
    GridObjContainerIdArray.splice(0, GridObjContainerIdArray.length);
    GridObjArray.splice(0, GridObjArray.length);
}
function ChangeSize(myContainerId) {
    var m_ZoomObj = $('#' + myContainerId + '_WindowsLayout');

    var m_ParentContainerObj = $('#' + myContainerId).parent().parent();
    var m_ContainerObj = $('#' + myContainerId);
    var m_ParentContainerWidth = m_ParentContainerObj.width() - 14;
    var m_ParentContainerHeight = m_ParentContainerObj.height() - 36;
    var m_ContainerWidth = m_ContainerObj.width();
    var m_ContainerHeight = m_ContainerObj.height();
    var m_ZoomRadio = 1;
    if (m_ParentContainerWidth != undefined && m_ContainerWidth != undefined) {
        if (m_ContainerWidth / m_ParentContainerWidth < m_ZoomRadio) {
            m_ZoomRadio = m_ContainerWidth / m_ParentContainerWidth;
        }
    }
    if (m_ParentContainerHeight != undefined && m_ContainerHeight != undefined) {
        if (m_ContainerHeight / m_ParentContainerHeight < m_ZoomRadio) {
            m_ZoomRadio = m_ContainerHeight / m_ParentContainerHeight;
        }
    }
    m_ZoomRadio = m_ZoomRadio * 100;

    //m_ZoomObj.width(m_ContainerWidth);
    //m_ZoomObj.height(m_ContainerHeight);
    ChartZoom(m_ZoomObj[0], m_ZoomRadio, m_ZoomRadio);



}
//////////////////////////////////////////增加Grid与Chart//////////////////////////
function CreateGridChart(myData, myContainerId, myIsShowGrid, myChartType) {
    var m_ParentContainerObj = $('#' + myContainerId).parent().parent();
    var m_ParentContainerWidth = m_ParentContainerObj.width() - 14;
    var m_ParentContainerHeight = m_ParentContainerObj.height() - 36;

    var m_ContainerObj = $('#' + myContainerId);

    var m_WindowsHtml = ''
    var m_GridHeight = m_ParentContainerHeight;
    var m_ChartHeight = m_ParentContainerHeight;
    if (myIsShowGrid == true) {
        m_GridHeight = parseInt(m_GridHeight * 0.4);
        m_ChartHeight = parseInt(m_ParentContainerHeight - m_GridHeight);
        m_WindowsHtml = '<div id = "' + myContainerId + '_WindowsLayout" style = "width:' + m_ParentContainerWidth + 'px; height:' + m_ParentContainerHeight + 'px; " >' +
                '<div id = "' + myContainerId + '_ChartPanel" style="width:' + (m_ParentContainerWidth - 20) + 'px;height:' + (m_ChartHeight - 20) + 'px; padding:10px; margin:0px; background-color:#cccccc;">' +
	            '</div>' +
                '<div id="' + myContainerId + '_GridPanel" style="width:' + m_ParentContainerWidth + 'px;height:' + m_GridHeight + 'px;">' +
		            '<table id="' + myContainerId + '_Grid" data-options="fit:true,border:false"></table>' +
	            '</div>' +
            '</div>';
        /*
        var m_GridOptions = {        
            id: myContainerId + "_Grid",
            region: "north",
            height: m_GridHeight,
            border:true, 
            collapsible:false, 
            split:false
        };

        $('#Windows_Container').layout('add', m_GridOptions);

        var m_ChartOptions = {
            id: myContainerId + "_Chart",
            region: "center",
            height: m_ChartHeight,
            border: true,
            collapsible: false,
            split: false
        };
        m_ContainerObj.layout('add', m_ChartOptions);
        */

        //$('#' + myContainerId + "_GridPanel").append("<table></table>");
    }
    else {
        m_WindowsHtml = '<div id = "' + myContainerId + '_WindowsLayout" style = "width:' + m_ParentContainerWidth + 'px; height:' + m_ParentContainerHeight + 'px; " >' +
                '<div id = "' + myContainerId + '_ChartPanel" style="width:' + (m_ParentContainerWidth - 20) + 'px;height:' + (m_ChartHeight - 20) + 'px; padding:10px; margin:0px; background-color:#cccccc;">' +
	            '</div>' +
            '</div>';
    }

    m_ContainerObj.append(m_WindowsHtml);
    $.parser.parse(m_ContainerObj);

    var m_ChartContainerWidth = m_ParentContainerWidth - 30;
    var m_ChartContainerHeight = m_ChartHeight - 30;
    var m_ChartWidth = m_ChartContainerWidth - 30;
    var m_ChartHeight = m_ChartContainerHeight;
    if (myChartType == 'Bar') {  //如果是柱状图,需要考虑宽度,以免柱子太细
        var m_SingleBarWidth = (m_ChartWidth - (myData['columns'].length - 1) * 10 ) / (myData['rows'].length * (myData['columns'].length - 1));
        if (m_SingleBarWidth < 15) {
            m_ChartWidth = myData['rows'].length * (myData['columns'].length - 1) * 15 + (myData['columns'].length - 1) * 10;
            m_ChartHeight = m_ChartHeight - 20;
        }
    }
    else if (myChartType == 'MultiBar') {
        var m_SingleBarWidth = (m_ChartWidth - (myData['columns'].length - 1) * 10) / (myData['columns'].length - 1);
        if (m_SingleBarWidth < 15) {
            m_ChartWidth = (myData['columns'].length - 1) * 15 + (myData['columns'].length - 1) * 10;
            m_ChartHeight = m_ChartHeight - 20;
        }
    }

    var m_ChartContainer = $('<div style = "width:' + m_ChartContainerWidth + 'px; height:' + m_ChartContainerHeight + 'px;padding-left:10px; padding-top:10px;background-color:white; border:1px solid #cccccc; overflow:auto;"></div>');
    m_ChartContainer.append('<div id = "' + myContainerId + '_Chart" style = "width:' + m_ChartWidth + 'px; height:' + m_ChartHeight + 'px; font-size:12pt; color:black; font-family:\'Times New Roman\';"></div>');
    $("#" + myContainerId + "_ChartPanel").append(m_ChartContainer);
    if (myIsShowGrid == true) {

        if (myData["title"] != undefined && myData["title"] != null) {
            GetChart(myContainerId + "_Grid", myContainerId + "_Chart", myData, myChartType, myData["title"]);
        }
        else {
            GetChart(myContainerId + "_Grid", myContainerId + "_Chart", myData, myChartType, null);
        }
    }
    else {
        if (myData["title"] != undefined && myData["title"] != null) {
            GetChart(null, myContainerId + "_Chart", myData, myChartType, myData["title"]);
        }
        else
        {
            GetChart(null, myContainerId + "_Chart", myData, myChartType, null);
        }
    }

}
///////////////////////////刷新该容器内GridChart///////////////////////////////
function RefreshGridChart(myData, myContainerId, myIsShowGrid, myChartType) {
    if (myIsShowGrid == true) {
        //GetChart(m_WindowId + "_Grid", m_WindowId + "_Chart", MsgData, "Line", "asdfasdfafd");
        //GetChart(m_WindowId + "_Grid", m_WindowId + "_Chart", MsgData, "Bar", null);
        //GetChart(m_WindowId + "_Grid", m_WindowId + "_Chart", MsgData, "MultiBar", "afsdasdfa");
        //GetChart(m_WindowId + "_Grid", m_WindowId + "_Chart", myData, "Pie", null);
        if (myData["title"] != undefined && myData["title"] != null) {
            GetChart(myContainerId + "_Grid", myContainerId + "_Chart", myData, myChartType, myData["title"]);
        }
        else {
            GetChart(myContainerId + "_Grid", myContainerId + "_Chart", myData, myChartType, null);
        }
    }
    else {
        if (myData["title"] != undefined && myData["title"] != null) {
            GetChart(null, myContainerId + "_Chart", myData, myChartType, myData["title"]);
        }
        else {
            GetChart(null, myContainerId + "_Chart", myData, myChartType, null);
        }
    }
}

//////////改变chart纵坐标上下限
function setChartAxesValueY(myDialogId, myDialogChartOptionsId, myYaxisMin, myYaxisMax) {
    var DialogChartIdIndex = FindArrayIndexByContainerId(ChartObjContainerIdArray, myDialogChartOptionsId);
    if (DialogChartIdIndex >= 0) {
        var m_ChartObj = ChartObjArray[DialogChartIdIndex];
        var m_ChartOptions = m_ChartObj.options;
        m_ChartOptions.axes.yaxis.min = parseFloat(myYaxisMin);
        m_ChartOptions.axes.yaxis.max = parseFloat(myYaxisMax);
        m_ChartOptions.axes.yaxis.tickInterval = (myYaxisMax - myYaxisMin) / 10;
        m_ChartObj.replot(m_ChartOptions);
        $('#' + myDialogId).dialog('close');

        var labels = $('table.jqplot-table-legend');
        labels.each(function (index) {
            //turn the label's text color to the swatch's color  
            $(this).css('border', '0px');
            $(this).css('font-size', '10pt');
            $(this).css('text-align', 'left');
            //set type name as the label's text  

        });
    }
}

//改变缩放比例
function ChartZoom(myZoomObject, myNewValue, myOldValue) {

    zoomEle(myZoomObject, parseFloat(myNewValue) / 100, parseFloat(myNewValue) / 100);
}
//获得浏览器名称
function getbrowser() {
    //var userAgent = navigator.userAgent; //取得浏览器的userAgent字符串
    //var isOpera = userAgent.indexOf("Opera") > -1;
    //if (isOpera) { return "Opera" }; //判断是否Opera浏览器
    //if (userAgent.indexOf("Firefox") > -1) { return "FF"; } //判断是否Firefox浏览器
    //if (userAgent.indexOf("Safari") > -1) { return "Safari"; } //判断是否Safari浏览器
    //if (userAgent.indexOf("compatible") > -1 && userAgent.indexOf("MSIE") > -1 && !isOpera) { return "IE"; };var brow = $.browser;
    var brow = $.browser;
    if (brow.msie) {
        BrowserName = "IE";
        var m_Version = (brow.version).substring(0, brow.version.indexOf('.'));
        BrowserVersion = parseInt(m_Version, 0);
    }
    if (brow.mozilla) {
        BrowserName = "FF";
        var m_Version = (brow.version).substring(0, brow.version.indexOf('.'));
        BrowserVersion = parseInt(m_Version, 0);
    }
    if (brow.safari) {
        BrowserName = "Safari";
        var m_Version = (brow.version).substring(0, brow.version.indexOf('.'));
        BrowserVersion = parseInt(m_Version, 0);
    }
    if (brow.opera) {
        BrowserName = "Opera";
        var m_Version = (brow.version).substring(0, brow.version.indexOf('.'));
        BrowserVersion = parseInt(m_Version, 0);
    }
    if (brow.chrome) {
        BrowserName = "Chrome";
        var m_Version = (brow.version).substring(0, brow.version.indexOf('.'));
        BrowserVersion = parseInt(m_Version, 0);
    }
    //判断是否IE浏览器
}
//对DOM元素缩放
function zoomEle(el, xScale, yScale) {
    style = el.getAttribute('style') || "";
    if (BrowserName == "IE" && BrowserVersion < 9) {
        if (document.compatMode == "CSS1Compat") {//模式匹配 解决ie8下兼容模式
            el.style.width = el.clientWidth * 2.0;
            el.style.height = el.clientHeight * 2.0;
        }
        el.style.zoom = xScale;
    }
    else if (BrowserName == "IE" && BrowserVersion >= 9) {
        el.setAttribute('style', style + '-ms-transform: scale(' + xScale + ', ' + yScale + '); -ms-transform-origin: 0px 0px;');
    }
    else if (BrowserName == "FF") {
        el.style.transform = 'scale(' + xScale + ', ' + yScale + ')';
        el.style.transformOrigin = '0px 0px';
    }
    else if (BrowserName == "Opera") {
        el.setAttribute('style', style + '-o-transform: scale(' + xScale + ', ' + yScale + '); -o-transform-origin: 0px 0px;');
    }
    else {
        el.setAttribute('style', style + '-webkit-transform: scale(' + xScale + ', ' + yScale + '); -webkit-transform-origin: 0px 0px;');
    }
    //-webkit-transform: scale(0.5);     /* for Chrome || Safari */
    //-moz-transform: scale(0.5);        /* for Firefox */
    //-ms-transform: scale(0.5);         /* for IE */
    //-o-transform: scale(0.5);          /* for Opera */
    //transform: rotate(45deg);
    //transform-origin:20% 40%;

    //-ms-transform: rotate(45deg); 		/* IE 9 */
    //-ms-transform-origin:20% 40%; 		/* IE 9 */

    //-webkit-transform: rotate(45deg);	/* Safari 和 Chrome */
    //-webkit-transform-origin:20% 40%;	/* Safari 和 Chrome */

    //-moz-transform: rotate(45deg);		/* Firefox */
    //-moz-transform-origin:20% 40%;		/* Firefox */

    //-o-transform: rotate(45deg);		/* Opera */
    //-o-transform-origin:20% 40%;		/* Opera */
}