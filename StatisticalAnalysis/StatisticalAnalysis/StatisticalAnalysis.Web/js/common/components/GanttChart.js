(function ($) {
    //存放插件所需的属性字段 
    var data = [];
    var options = {
        "seriesColors": ["#01b3f9", "#fef102", "#f8000e", "#a400ed", "#aaf900", "#fe0072", "#0c6c92", "#fea002", "#c1020a", "#62008d", "#3c8300"],
        "seriesColors-high": ["#21d3ff", "#ffff22", "#ff606e", "#c420ff", "#caff20", "#ff50a2", "#2c8cb2", "#ffc022", "#e1222a", "#8220ad", "#5ca320"],
        "xaxis": { "color": "black", "font-family": "宋体", "size": 9, "font-weight": "normal", "bottom": 0, "type": "time", "scale-space": "120", "showGridLine": true },
        "yaxis": { "color": "black", "font-family": "宋体", "size": 9, "font-weight": "normal", "left": 150, "type": "text", "showGridLine": true },
        "backGround-color": "white",
        "title": "",
        "width": 0,
        "height": 0
    };
    var object = null;
    //插件的私有函数
    function DrawChart(myFirstLoad) {
        if (object != null && object != undefined) {
            if (myFirstLoad != "first") {
                destory();
            }
            var m_ContentWidth = options.width > 0 ? options.width : $(object).width();            //如果没有定义宽度则默认容器宽度
            var m_ContentHeight = options.height > 0 ? options.height : $(object).height();        //如果没有定义高度则默认容器高度
            ////////////////////横坐标范围生成坐标轴/////////////////////
            var m_MinValue = undefined;
            var m_MaxValue = undefined;
            var m_RowCount = 0;                                                                     //甘特图的行数

            $.each(data, function (key, value) {                                                   //每一行一个Key,value里面有多个时间段
                m_RowCount = m_RowCount + 1;
                for (var i = 0; i < data[key].length; i++) {
                    if (m_MinValue == undefined || m_MaxValue == undefined) {
                        if (data[key][i].start < data[key][i].end) {
                            m_MinValue = data[key][i].start;
                            m_MaxValue = data[key][i].end;
                        }
                    }
                    else {
                        if (data[key][i].start < data[key][i].end) {
                            if (m_MinValue > data[key][i].start) {
                                m_MinValue = data[key][i].start;
                            }
                            if (m_MaxValue < data[key][i].end) {
                                m_MaxValue = data[key][i].end;
                            }
                        }
                    }
                }
            });
            ///////////////////////////////////////////////////////////
            if (m_RowCount > 0) {
                //////////////////////////形成外观框架系///////////////////////
                var m_ChartTable = GetChartMainTable(m_ContentWidth, m_ContentHeight, m_RowCount);
                $(object).append(m_ChartTable);
                GetChartMainDiv(m_ContentWidth);
                /////////////////////////形成坐标系////////////////////////
                if (options["xaxis"]["showGridLine"] == true || options["yaxis"]["showGridLine"] == true) {
                    var m_GridLineTable = GetGridLineTable(m_ContentWidth, m_ContentHeight, m_RowCount);
                    if (m_GridLineTable != null) {
                        $('#GanttChartMain').append(m_GridLineTable);
                    }
                }
                var m_xaxisScaleTable = GetxaxisScaleTable(m_ContentWidth, m_ContentHeight)
                if (m_xaxisScaleTable != null) {
                    $('#xaxisScale').append(m_xaxisScaleTable);

                }

                //////////////////////////////////////////////////////////
                if (options["xaxis"].type == "time") {                                                 //横坐标是time格式
                    var m_xaxisScaleTextTable = GetxaxisScaleTimeTable(m_ContentWidth, m_ContentHeight, m_MinValue, m_MaxValue);   //生成横坐标坐标系
                    if (m_xaxisScaleTextTable != null) {
                        $('#xaxisScaleText').append(m_xaxisScaleTextTable);
                    }
                    //$('#GanttChartMain').append($('<div class = "FloatChartDiv" style = "width:80px;height:60px;left:20px;top:0px; background-color:red;"></div>'));
                    //$('#GanttChartMain').append($('<div class = "FloatChartDiv" style = "width:80px;height:70px;left:40px;top:-50px; background-color:blue;"></div>'));
                    //$('#GanttChartMain').append($('<div class = "FloatChartDiv" style = "width:80px;height:70px;left:80px;top:-100px; background-color:yellow;"></div>'));
                    SetTimeGattChartItem(data, m_ContentWidth, m_ContentHeight, m_RowCount, m_MinValue, m_MaxValue);
                }
                else if (options["xaxis"].type == "number") {                                           //横坐标是数值格式
                    $.each(data, function (key, value) {                                                   //每一行一个Key,value里面有多个时间段




                    });
                }
            }
        }
        else {
            alert("没有为甘特图提供合适的容器!");
        }
    }
    ////////////////////基础函数/////////////////
    function GetChartRowHeight(myContentHeight, myRowCount, myxaxisScaleHeight) {
        var m_xaxisScaleHeight = myxaxisScaleHeight;
        var m_Height = parseInt((myContentHeight - 1 * (myRowCount + 1) - (2 * options["xaxis"]["size"] + 4) - m_xaxisScaleHeight) / myRowCount);
        if (m_Height > 30) {
            m_Height = 30;
        }
        else if (m_Height < 22) {
            m_Height = 22;
        }
        return m_Height;
    }
    function GetxaxisHeight() {
        var m_xaxisHeight = parseInt(2 * options["xaxis"]["size"] + 4);
        return m_xaxisHeight;
    }
    function GetChartColumnWidth(myRightTdWidth, myColumnCount) {
        var m_Width = parseInt((myRightTdWidth - 1 * (myColumnCount - 1)) / myColumnCount);
        return m_Width
    }
    function GetRightTdWidth(myContentWidth) {
        var m_RightTdWidth = parseInt(myContentWidth - options["yaxis"].left - 3);          //减去3是因为减去三个纵边框的宽度
        if (m_RightTdWidth < 400) {
            m_RightTdWidth = 400;
        }
        return m_RightTdWidth;
    }
    function GetDiffMilliSecond(myMinValue, myMaxValue) {
        var m_StartTime = new Date(myMinValue.replace(/-/g, "/"));
        var m_EndTime = new Date(myMaxValue.replace(/-/g, "/"));
        var m_DiffMilliSecond = m_EndTime - m_StartTime;
        return m_DiffMilliSecond;
    }
    ////////////////////生成甘特图以及相关框架////////////////
    function GetChartMainTable(myContentWidth, myContentHeight, myRowCount) {
        var m_ChartTable = $('<table class = "GanttChartTable"></table>');
        var m_LeftTdWidth = options["yaxis"].left;
        var m_RightTdWidth = GetRightTdWidth(myContentWidth);
        var m_xaxisScaleHeight = 3;
        var m_Height = GetChartRowHeight(myContentHeight, myRowCount, m_xaxisScaleHeight);               //设置行高
        var m_RowSpanHeight = (m_Height + 1) * myRowCount;
        for (var i = 0; i < myRowCount; i++) {
            if (i == 0) {
                var aa = '<tr><td id = "ChartTitle_' + (i + 1).toString() + '" class = "ChartTitleTd" style = "width:' + m_LeftTdWidth + 'px;height:' + m_Height + 'px; font-size:' +
                    options["yaxis"].size + 'pt;color:' + options["yaxis"].color + '; font-family:' + options["yaxis"]["font-family"] + '; font-weight: ' + options["yaxis"]["font-weight"] + ';">&nbsp;</td>' +
                    '<td  id = "GanttChartMainTd"  rowspan = ' + myRowCount + ' style = "width:' + m_RightTdWidth + 'px; background-color: ' + options["backGround-color"] +
                    '"><div id = "GanttChartMain" style = "width:' + m_RightTdWidth + 'px;height:' + m_RowSpanHeight + 'px;"></div></td></tr>';
                var m_TitleRow = $('<tr><td id = "ChartTitle_' + (i + 1).toString() + '" class = "ChartTitleTd" style = "width:' + m_LeftTdWidth + 'px;height:' + m_Height + 'px; font-size:' +
                    options["yaxis"].size + 'pt;color:' + options["yaxis"].color + '; font-family:' + options["yaxis"]["font-family"] + '; font-weight: ' + options["yaxis"]["font-weight"] + ';">&nbsp;</td>' +
                    '<td  id = "GanttChartMainTd"  rowspan = ' + myRowCount + ' style = "width:' + m_RightTdWidth + 'px; background-color: ' + options["backGround-color"] +'"></td></tr>');
                m_ChartTable.append(m_TitleRow);
            }
            else {
                var m_TitleRow = $('<tr><td id = "ChartTitle_' + (i + 1).toString() + '" class = "ChartTitleTd" style = "width:' + m_LeftTdWidth + 'px;height:' + m_Height + 'px; font-size:' +
                    options["yaxis"].size + 'pt;color:' + options["yaxis"].color + '; font-family:' + options["yaxis"]["font-family"] + '; font-weight: ' + options["yaxis"]["font-weight"] + ';">&nbsp;</td></tr>');
                m_ChartTable.append(m_TitleRow);
            }
        }
        var m_xaxisScale = $('<tr><td class = "xaxisTd" style = "height:' + m_xaxisScaleHeight + 'px;">&nbsp;</td><td id = "xaxisScale" class = "xaxisTd" style = "height:' + m_xaxisScaleHeight + 'px;">&nbsp;</td></tr>');
        m_ChartTable.append(m_xaxisScale);
        var m_xaxisHeight = GetxaxisHeight();
        var m_xaxis = $('<tr><td class = "xaxisTd" style = "height:' + m_xaxisHeight + 'px;">adfa</td><td id = "xaxisScaleText"  class = "xaxisTd" style = "font-size:' +
                    options["xaxis"].size + 'pt;color:' + options["xaxis"].color + '; font-family:' + options["xaxis"]["font-family"] + '; font-weight: ' + options["xaxis"]["font-weight"] + ';">&nbsp;</td></tr>');
        m_ChartTable.append(m_xaxis);
        return m_ChartTable;
    }
    function GetChartMainDiv(myContentWidth) {
        var m_RightTdWidth = GetRightTdWidth(myContentWidth);
        var m_RightTdHeight = $('#GanttChartMainTd').height() + 2;
        var m_ChartMainDiv = $('<div id = "GanttChartMain" style = "width:' + m_RightTdWidth + 'px;height:' + m_RightTdHeight + 'px;"></div>');
        $('#GanttChartMainTd').append(m_ChartMainDiv);
    }
    function GetGridLineTable(myContentWidth, myContentHeight, myRowCount) {
        var m_RowGridLine = options["xaxis"]["showGridLine"];
        var m_ColumnGridLine = options["yaxis"]["showGridLine"];
        var m_ScaleSpace = options["xaxis"]["scale-space"];
        var m_LeftTdWidth = options["yaxis"].left;
        var m_xaxisScaleHeight = 3;
        var m_RightTdWidth = GetRightTdWidth(myContentWidth);
        if (m_ScaleSpace != 0) {
            var m_ColumnCount = parseInt(myContentWidth / m_ScaleSpace);
            if (m_ColumnCount <= 0) {
                m_ColumnCount = 1;
            }
            var m_Height = 0;
            if ($.browser.msie) { //IE浏览器 
                //var aa = parseInt($.browser.version);
                m_Height = GetChartRowHeight(myContentHeight, myRowCount, m_xaxisScaleHeight);
            }
            else {
                m_Height = GetChartRowHeight(myContentHeight, myRowCount, m_xaxisScaleHeight);
            }
            var m_Width = GetChartColumnWidth(m_RightTdWidth, m_ColumnCount);
            var m_GridLineTable = $('<table class = "GridLineTable"></table>');
            for (var i = 0; i < myRowCount; i++) {
                var m_TableRow = "<tr>";
                if (m_RowGridLine == true && m_ColumnGridLine == true) {            //显示横向与纵向
                    for (var j = 0; j < m_ColumnCount; j++) {
                        if (i < myRowCount - 1 && j < m_ColumnCount - 1) {        //不在第一行也不在第一列
                            m_TableRow = m_TableRow + '<td class = "XYGridLine" style = "width:' + m_Width + 'px; height:' + m_Height + 'px;">&nbsp;</td>';
                        }
                        else if (i == myRowCount - 1 && j < m_ColumnCount - 1) {   //最后一行除了最后一列
                            m_TableRow = m_TableRow + '<td class = "YGridLine" style = "width:' + m_Width + 'px; height:' + m_Height + 'px;">&nbsp;</td>';
                        }
                        else if (i < myRowCount - 1 && j == m_ColumnCount - 1) {   //最后一列除了最后一行
                            m_TableRow = m_TableRow + '<td class = "XGridLine" style = "width:' + m_Width + 'px; height:' + m_Height + 'px;">&nbsp;</td>';
                        }
                        else {
                            m_TableRow = m_TableRow + '<td class = "NoneGridLine" style = "width:' + m_Width + 'px; height:' + m_Height + 'px;">&nbsp;</td>';
                        }
                    }
                }
                else if (m_RowGridLine == true && m_ColumnGridLine == false) {             //只显示纵向
                    for (var j = 0; j < m_ColumnCount; j++) {
                        if (i < myRowCount - 1 && j < m_ColumnCount - 1) {        //不在第一行也不在第一列
                            m_TableRow = m_TableRow + '<td class = "XGridLine" style = "width:' + m_Width + 'px; height:' + m_Height + 'px;">&nbsp;</td>';
                        }
                        else if (i < myRowCount - 1 && j == m_ColumnCount - 1) {   //最后一列除了最后一行
                            m_TableRow = m_TableRow + '<td class = "XGridLine" style = "width:' + m_Width + 'px; height:' + m_Height + 'px;">&nbsp;</td>';
                        }
                        else {
                            m_TableRow = m_TableRow + '<td class = "NoneGridLine" style = "width:' + m_Width + 'px; height:' + m_Height + 'px;">&nbsp;</td>';
                        }
                    }
                }
                else if (m_RowGridLine == false && m_ColumnGridLine == true) {            //只显示横向
                    for (var j = 0; j < m_ColumnCount; j++) {
                        if (i < myRowCount - 1 && j < m_ColumnCount - 1) {        //不在第一行也不在第一列
                            m_TableRow = m_TableRow + '<td class = "YGridLine" style = "width:' + m_Width + 'px; height:' + m_Height + 'px;">&nbsp;</td>';
                        }
                        else if (i == myRowCount - 1 && j < m_ColumnCount - 1) {   //最后一行除了最后一列
                            m_TableRow = m_TableRow + '<td class = "YGridLine" style = "width:' + m_Width + 'px; height:' + m_Height + 'px;">&nbsp;</td>';
                        }
                        else {
                            m_TableRow = m_TableRow + '<td class = "NoneGridLine" style = "width:' + m_Width + 'px; height:' + m_Height + 'px;">&nbsp;</td>';
                        }
                    }
                }

                m_TableRow = m_TableRow + "</tr>"
                m_GridLineTable.append($(m_TableRow));
            }
            return m_GridLineTable;
        }
        else {
            return null;
        }
    }
    function GetxaxisScaleTable(myContentWidth, myContentHeight) {
        var m_ScaleSpace = options["xaxis"]["scale-space"];
        var m_LeftTdWidth = options["yaxis"].left;
        var m_xaxisScaleHeight = 3;
        var m_RightTdWidth = myContentWidth - options["yaxis"].left - 3;
        if (m_ScaleSpace != 0) {
            var m_ColumnCount = parseInt(myContentWidth / m_ScaleSpace);
            if (m_ColumnCount <= 0) {
                m_ColumnCount = 1;
            }
            var m_Width = GetChartColumnWidth(m_RightTdWidth, m_ColumnCount);

            var m_ScalexaxisTable = $('<table class = "ScalexaxisTable"></table>');
            m_TableRow = "<tr>"
            for (var i = 0; i < m_ColumnCount; i++) {
                if (i < m_ColumnCount - 1) {        //不在第一行也不在第一列
                    var m_TableRow = m_TableRow + '<td class = "YScaleLine" style = "width:' + m_Width + 'px; height:' + m_xaxisScaleHeight + 'px;">&nbsp;</td>';
                }
                else {
                    m_TableRow = m_TableRow + '<td class = "NoneScaleLine" style = "width:' + m_Width + 'px; height:' + m_xaxisScaleHeight + 'px;">&nbsp;</td>';
                }
            }
            m_TableRow = m_TableRow + "</tr>";
            m_ScalexaxisTable.append($(m_TableRow));
            return m_ScalexaxisTable;
        }
        else {
            return null;
        }
    }
    function GetxaxisScaleTimeTable(myContentWidth, myContentHeight, myMinValue, myMaxValue) {
        ///////////////////////生成横坐标title////////////////////
        var m_StartTime = new Date(myMinValue.replace(/-/g, "/"));
        var m_DiffMilliSecond = GetDiffMilliSecond(myMinValue, myMaxValue);
        ///////////////////////生成横坐标框架//////////////////
        var m_ScaleSpace = options["xaxis"]["scale-space"];
        var m_LeftTdWidth = options["yaxis"].left;
        var m_RightTdWidth = myContentWidth - options["yaxis"].left - 3;
        if (m_ScaleSpace != 0) {
            var m_ColumnCount = parseInt(myContentWidth / m_ScaleSpace);
            if (m_ColumnCount <= 0) {
                m_ColumnCount = 1;
            }
            var m_Width = GetChartColumnWidth(m_RightTdWidth, m_ColumnCount) - 2;

            var m_ScalexaxisTextTable = $('<table class = "ScalexaxisTextTable"></table>');
            m_TableRow = '<tr><td class = "NoneScaleLine" style = "width: ' + parseInt(m_Width / 2) + 'px;"></td>';
            for (var i = 0; i < m_ColumnCount - 1; i++) {
                m_StartTime.setMilliseconds(parseInt(m_DiffMilliSecond / m_ColumnCount));
                m_TableRow = m_TableRow + '<td class = "ScalexaxisTextTd" style = "width:' + m_Width + 'px;">' + m_StartTime.getFullYear().toString() +
                    '-' + (m_StartTime.getMonth() + 1).toString() + '-' + m_StartTime.getDate().toString() +
                    '<br/>' + m_StartTime.getHours() + ':' + m_StartTime.getMinutes() + ':' + m_StartTime.getSeconds() + '</td>';
            }
            m_TableRow = m_TableRow + "</tr>";
            m_ScalexaxisTextTable.append($(m_TableRow));
            return m_ScalexaxisTextTable;
        }
        else {
            return null;
        }
    }
    function SetTimeGattChartItem(value, myContentWidth, myContentHeight, myRowCount, myMinValue, myMaxValue) {
       
        var m_ChartTitleRowIndex = 1;
        var m_xaxisScaleHeight = 3;
        var m_RightTdWidth = GetRightTdWidth(myContentWidth) - 2;
        var m_StartTime = new Date(myMinValue.replace(/-/g, "/"));
        var m_MilliSecondValue = GetDiffMilliSecond(myMinValue, myMaxValue);

        var m_TransparentDiv = $('<div id = "TransparentDiv"></div>');                      //是为了找到正常流位置
        $('#GanttChartMain').append($(m_TransparentDiv));
        var m_ContentPositionTop = -$('#TransparentDiv').position().top + 2;
        var m_ContentPositionLeft = 0;
        $(m_TransparentDiv).remove();


        $.each(data, function (key, value) {                                                   //每一行一个Key,value里面有多个时间段
            $("#ChartTitle_" + m_ChartTitleRowIndex.toString()).text(key);
            $("#ChartTitle_" + m_ChartTitleRowIndex.toString()).attr("title", key);
            var m_Height = $('#ChartTitle_' + m_ChartTitleRowIndex).height();
            for (var i = 0; i < value.length; i++) {
                var m_DiffStartVulue = GetDiffMilliSecond(myMinValue, value[i]["start"]);
                var m_DiffEndValue = GetDiffMilliSecond(value[i]["start"], value[i]["end"]);
                var m_ChartLeft = m_ContentPositionLeft + parseInt(m_RightTdWidth * m_DiffStartVulue / m_MilliSecondValue);
                var m_ChartTop = m_ContentPositionTop + 2;
                var m_ChartWidth = parseInt(m_RightTdWidth * m_DiffEndValue / m_MilliSecondValue) + 2;
                var m_ChartHeight = m_Height - 2;
                var m_ColorSeriesIndex = (m_ChartTitleRowIndex - 1) % options["seriesColors"].length;
                var m_seriesColors = options["seriesColors"][m_ColorSeriesIndex];
                var m_TimeDay = parseInt(m_DiffEndValue / (1000 * 3600 * 24));
                var m_TimeHour = parseInt((m_DiffEndValue - m_TimeDay * 1000 * 3600 * 24) / (1000 * 3600));
                var m_TimeMinute = parseInt((m_DiffEndValue - m_TimeDay * 1000 * 3600 * 24 - m_TimeHour * 1000 * 3600) / (1000 * 60));
                var m_TimeSecond = parseInt((m_DiffEndValue - m_TimeDay * 1000 * 3600 * 24 - m_TimeHour * 1000 * 3600 - m_TimeMinute * 1000 * 60) / 1000);
                var m_TimeLongText = m_TimeDay + "天" + m_TimeHour + "时" + m_TimeMinute + "分" + m_TimeSecond + "秒";
                var m_ChartDiv = '<div data-options=\'{"seriesColorsIndex":"' + m_ChartTitleRowIndex + '"}\' class = "GanttChartItems" title="开始时间:' + value[i]["start"] + '\r\n结束时间:' + value[i]["end"] + '\r\n时长: ' + m_TimeLongText + '" style = "left:' + m_ChartLeft + 'px;top:' + m_ChartTop +
                    'px; width:' + m_ChartWidth + 'px;height:' + m_ChartHeight + 'px;background-color:' + m_seriesColors + ';"></div>';
                $('#GanttChartMain').append($(m_ChartDiv));
                m_ContentPositionTop = m_ContentPositionTop - m_ChartHeight - 2;
            }
            if ($.browser.msie) { //IE浏览器 
                //var aa = parseInt($.browser.version);
                m_ContentPositionTop = m_ContentPositionTop + GetChartRowHeight(myContentHeight, myRowCount, m_xaxisScaleHeight) + 3;
            }
            else {
                m_ContentPositionTop = m_ContentPositionTop + GetChartRowHeight(myContentHeight, myRowCount, m_xaxisScaleHeight) + 3;
            }
            m_ChartTitleRowIndex = m_ChartTitleRowIndex + 1;
        });

        $(".GanttChartItems").hover(function () {
            var m_ColorIndexString = $(this).data("options")["seriesColorsIndex"];
            var m_ColorIndex = parseInt(m_ColorIndexString - 1) % options["seriesColors-high"].length;
            $(this).css("background-color", options["seriesColors-high"][m_ColorIndex]);
        }, function () {
            var m_ColorIndex = (parseInt($(this).data("options")["seriesColorsIndex"]) - 1) % options["seriesColors"].length;
            $(this).css("background-color", options["seriesColors"][m_ColorIndex]);
        });
        

    }
    /////////////////////////////////////////////////////////////////
    function destory() {
        object.empty();
    }
    //插件的公共函数
    var methods = {
        init: function (myOptions) {
            $.each(myOptions, function (key, value) {
                if (key == "data") {
                    data = myOptions[key];
                }
                else if (key == "options") {
                    $.each(myOptions[key], function (subkey, subvalue) {
                        if (typeof (subvalue) == 'object' || !subvalue) {
                            options[subkey] = $.extend({}, options[subkey], myOptions[key][subkey]);
                        }
                        else {
                            options[subkey] = subvalue;
                        }
                    });


                }
            });
            object = $(this);

            DrawChart("first");

            return object;
        },
        destory: function () {
            destory();
            return null;
        },
        loadData: function (myData) {
            data = myData;
            DrawChart("last");
            return object;
        },
        resize: function () {
            DrawChart("last");
            return object;
        }
    };
    $.fn.GanttChart = function () {
        var method = arguments[0];
        if (methods[method]) {
            method = methods[method];
            arguments = Array.prototype.slice.call(arguments, 1);
        } else if (typeof (method) == 'object' || !method) {
            method = methods.init;
        } else {
            $.error('Method ' + method + ' does not exist on jQuery.pluginName');
            return this;
        }
        return method.apply(this, arguments);
    }
})(jQuery);