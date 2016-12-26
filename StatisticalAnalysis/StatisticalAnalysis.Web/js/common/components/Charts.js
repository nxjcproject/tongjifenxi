﻿
function CreatChart(myChartObjId, myData, myChartType, myTitle) {
    var m_ChartObj;
    if (myChartType == 'Line') {
        m_ChartObj = GetLineChart(myChartObjId, myData, myTitle);
    }
    else if (myChartType == 'Bar') {
        m_ChartObj = GetBarChart(myChartObjId, myData, myTitle);
    }
    else if (myChartType == 'DateXLine') {
        m_ChartObj = GetDateXLineChart(myChartObjId, myData, myTitle);
    }
    else if (myChartType == 'MultiBar') {
        m_ChartObj = GetMultiBarChart(myChartObjId, myData, myTitle);
    }
    else if (myChartType == 'Pie') {
        m_ChartObj = GetPieChart(myChartObjId, myData, myTitle);
    }
    else if (myChartType == 'meterGauge') {
        m_ChartObj = GetMeterGauge(myChartObjId, myData, myTitle);
    }
    return m_ChartObj;
}

function GetLineChart(myChartObjId, myData, myTitle, temp) {
    if (myData['columns'][1]['field'].split('-').length > 1) {
        var m_DateXLineObj = GetDateXLineChart(myChartObjId, myData, myTitle);
        return m_DateXLineObj;
    }

    //var line1 = [6.5, 9.2, 14, 19.65, 26.4, 35, 51];
    //var line2 = [3.5, 3.2, 12, 13.65, 41.4, 21, 51]
    var m_ColumnName = "";
    var m_Rows = myData['rows'];
    var m_Labels = new Array();
    var m_AxisX = new Array();
    var m_MaxBarValue = 0;
    ////////////////////////////////获得颜色标签名////////////////////
    for (var i = 0; i < m_Rows.length; i++) {
        var m_LabelItem = { label: m_Rows[i][myData['columns'][0]['field']] };
        m_Labels.push(m_LabelItem);
    }
    /////////////////////////////获得x轴坐标名称//////////////////////
    for (var i = 1; i < myData['columns'].length; i++) {
        var m_AxisItem = [i, myData['columns'][i]['title']];
        m_AxisX.push(m_AxisItem);
    }
    var m_Lines = new Array();
    for (var i = 0; i < m_Rows.length; i++) {
        var m_LineTemp = new Array();
        for (var j = 1; j < myData['columns'].length; j++) {
            m_ColumnName = myData['columns'][j]['field'];
            for (var m_Name in m_Rows[i]) {
                if (m_ColumnName == m_Name) {
                    m_LineTemp.push(parseFloat(m_Rows[i][m_Name]));
                }
            }
        }
        m_Lines.push(m_LineTemp);
    }
    //////////////////////////////找到最大的line累加和////////////////////////////
    for (var i = 0; i < m_Rows.length; i++) {
        for (var j = 1; j < myData['columns'].length; j++) {
            var m_CurrentValue = parseFloat(m_Rows[i][myData['columns'][j].field]);
            if (m_CurrentValue > m_MaxBarValue) {
                m_MaxBarValue = m_CurrentValue;
            }
        }
    }
    m_MaxBarValue = GetYaxisMax(m_MaxBarValue);

    var ChartJqplot = $.jqplot(myChartObjId, m_Lines, {
        animate: true,
        seriesColors: ["#62fcf1", "#ff7471", "#fdf39d", "#8def77", "#4da5fe", "#7f82ec", "#eb3e68", "#f8a659", "#249090", "#fe9af6", "#e1e1e3", "#a6a6a7"],
        // Will animate plot on calls to plot1.replot({resetAxes:true})
        animateReplot: true,
        seriesDefaults: {
            lineWidth: 1,
            markerOptions: { size: 0 }
        },
        axesDefaults: {
            tickRenderer: $.jqplot.CanvasAxisTickRenderer
        },
        series: m_Labels,
        title: {
            text: myTitle,
            fontFamily: '"Times New Roman", cursive',
            fontSize: '11pt',
            textColor: '#000000'
        },
        legend: {
            renderer: $.jqplot.EnhancedLegendRenderer,
            show: true,
            location: 's',
            placement: 'outsideGrid',
            disableIEFading: true,
            seriesToggle: 'normal',
            seriesToggleReplot: true,
            rendererOptions: {
                numberColumns: 4
            }
        },
        axes: {
            xaxis: {
                tickRenderer: $.jqplot.CanvasAxisTickRenderer,
                tickOptions: {
                    //angle: 10
                    fontFamily: 'Times New Roman',
                    fontSize: '9pt',
                    fontWeight: 'normal',
                    textColor:'#000000'
                },
                ticks: m_AxisX,
                label: myData['Units']['UnitX'],
                labelOptions: {
                    fontFamily: 'Times New Roman',
                    fontSize: '10pt',
                    textColor: '#000000'
                },
                labelRenderer: $.jqplot.CanvasAxisLabelRenderer
            },
            yaxis: {
                tickRenderer: $.jqplot.CanvasAxisTickRenderer,
                tickOptions: {
                    fontFamily: 'Times New Roman',
                    fontSize: '9pt',
                    fontWeight: 'normal',
                    textColor: '#000000'
                },
                labelRenderer: $.jqplot.CanvasAxisLabelRenderer,
                labelOptions: {
                    fontFamily: 'Times New Roman',
                    fontSize: '10pt',
                    textColor: '#000000'
                },
                tickInterval: m_MaxBarValue / 10,
                min: 0,
                max: m_MaxBarValue,
                pad: 0,
                label: myData['Units']['UnitY']
            }
        },
        cursor: {
            show: true,
            zoom: true
        },
        highlighter: {
            show: true,
            showMarker: true,
            sizeAdjust: 16,  // 当鼠标移动到数据点上时，数据点扩大的增量
            tooltipAxes: 'y',
            fadeTooltip: true,// 设置提示信息栏出现和消失的方式（是否淡入淡出）  
            lineWidthAdjust: 2.5,   //当鼠标移动到放大的数据点上时，设置增大的数据点的宽度
            //tooltipOffset: 20,       // 提示信息栏据被高亮显示的数据点的偏移位置，以像素计
            tooltipLocation: 'n'
        },
        grid: {
            drawGridLines: true, // wether to draw lines across the grid or not.
            gridLineColor: '#cccccc', // 设置整个图标区域网格背景线的颜色
            background: '#ffffff', // 设置整个图表区域的背景色
            borderColor: '#666666', // 设置图表的(最外侧)边框的颜色
            borderWidth: 1.0, //设置图表的（最外侧）边框宽度
            shadow: false, // 为整个图标（最外侧）边框设置阴影，以突出其立体效果
            shadowAngle: 45, // 设置阴影区域的角度，从x轴顺时针方向旋转
            shadowOffset: 1.5, // 设置阴影区域偏移出图片边框的距离
            shadowWidth: 3, // 设置阴影区域的宽度
            shadowDepth: 3, // 设置影音区域重叠阴影的数量
            shadowAlpha: 0.07, // 设置阴影区域的透明度
            rendererOptions: {} // options to pass to the renderer. Note, the default
            // CanvasGridRenderer takes no additional options.
        },
        canvasOverlay: {
            show: true
        }
    });
    $('#' + myChartObjId).parent().bind('_resize', function (event, ui) {
        ChartJqplot.replot({ resetAxes: true });
    });
    var labels = $('table.jqplot-table-legend');
    labels.each(function (index) {
        $(this).css('border', '0px');
    });
    var LegendLabellabels = $('td.jqplot-table-legend-label');
    LegendLabellabels.each(function (index) {
        $(this).css('padding-right', '5px');
    });
    return ChartJqplot;
}


function GetDateXLineChart(myChartObjId, myData, myTitle) {
    //var line1 = [6.5, 9.2, 14, 19.65, 26.4, 35, 51];
    //var line2 = [3.5, 3.2, 12, 13.65, 41.4, 21, 51]
    var m_ColumnName = "";
    var m_Rows = myData['rows'];
    var m_Labels = new Array();
    var m_AxisX = new Array();
    var m_MaxBarValue = 0;
    var formatString = "";
    ////////////////////////////////获得颜色标签名////////////////////
    for (var i = 0; i < m_Rows.length; i++) {
        var m_LabelItem = { label: m_Rows[i][myData['columns'][0]['field']] };
        m_Labels.push(m_LabelItem);
    }
    /////////////////////////////获得x轴坐标名称//////////////////////
    for (var i = 1; i < myData['columns'].length; i++) {
        var m_AxisItem = [i, myData['columns'][i]['title']];
        m_AxisX.push(m_AxisItem);
    }
    var m_Lines = new Array();
    for (var i = 0; i < m_Rows.length; i++) {
        var m_LineTemp = new Array();
        for (var j = 1; j < myData['columns'].length; j++) {
            m_ColumnName = myData['columns'][j]['field'];
            for (var m_Name in m_Rows[i]) {
                if (m_ColumnName == m_Name) {
                    var timeArray = myData['columns'][j]['title'].split('-');
                    var date = null;
                    if (timeArray.length == 4) {
                        date = new Date(timeArray[0], timeArray[1] - 1, timeArray[2], timeArray[3]);
                        formatString = "%Y-%m-%d-%H";
                        m_LineTemp.push([date.toUTCString(), parseFloat(m_Rows[i][m_Name])]);
                    }
                    else if (timeArray.length == 3) {
                        date = new Date(timeArray[0], timeArray[1] - 1, timeArray[2]);
                        formatString = "%Y-%m-%d";
                        m_LineTemp.push([date.toUTCString(), parseFloat(m_Rows[i][m_Name])]);
                    }
                    else if (timeArray.length == 2 && timeArray[0].length == 4) {
                        date = new Date(timeArray[0], timeArray[1] - 1);
                        formatString = "%Y-%m";
                        m_LineTemp.push([date.toUTCString(), parseFloat(m_Rows[i][m_Name])]);
                    }
                    else if (timeArray.length == 2 && timeArray[0].length == 2) {
                        date = new Date((new Date()).getFullYear(), timeArray[0] - 1, timeArray[1]);
                        formatString = "%m-%d";
                        m_LineTemp.push([date.toUTCString(), parseFloat(m_Rows[i][m_Name])]);
                    }
                    else {
                        formatString = "";
                        var m_FirstFlag = true;
                        var m_DateString = myData['columns'][j]['title'];
                        var m_DateNow = new Date();
                        var m_FullYear = m_DateNow.getFullYear();
                        var m_Month = m_DateNow.getMonth() + 1;
                        var m_Day = m_DateNow.getDate();
                        var m_Hour = m_DateNow.getHours();
                        var m_Minute = m_DateNow.getMinutes();
                        var m_Second = m_DateNow.getSeconds();
                        //////////////////////字符串是否存在年的数据/////////////////
                        var m_StringIndex = m_DateString.indexOf("年");
                        if (m_StringIndex >= 0) {
                            m_FullYear = m_DateString.substring(0, m_StringIndex);
                            m_DateString = m_DateString.substring(m_StringIndex + 1);
                            if (m_FirstFlag == true) {
                                formatString = formatString + "%Y";
                                m_FirstFlag = false;
                            }
                        }
                        //////////////////////字符串是否存在月的数据/////////////////
                        var m_StringIndex = m_DateString.indexOf("月");
                        if (m_StringIndex >= 0) {
                            m_Month = m_DateString.substring(0, m_StringIndex);
                            m_DateString = m_DateString.substring(m_StringIndex + 1);
                            if (m_FirstFlag == true) {
                                formatString = formatString + "%m";
                                m_FirstFlag = false;
                            }
                            else {
                                formatString = formatString + "-%m";
                            }
                        }
                        //////////////////////字符串是否存在日的数据/////////////////
                        var m_StringIndex = m_DateString.indexOf("日");
                        if (m_StringIndex >= 0) {
                            m_Day = m_DateString.substring(0, m_StringIndex);
                            m_DateString = m_DateString.substring(m_StringIndex + 1);
                            if (m_FirstFlag == true) {
                                formatString = formatString + "%d";
                                m_FirstFlag = false;
                            }
                            else {
                                formatString = formatString + "-%d";
                            }
                        }
                        //////////////////////字符串是否存在时的数据/////////////////
                        var m_StringIndex = m_DateString.indexOf("时");
                        if (m_StringIndex >= 0) {
                            m_Hour = m_DateString.substring(0, m_StringIndex);
                            m_DateString = m_DateString.substring(m_StringIndex + 1);
                            if (m_FirstFlag == true) {
                                formatString = formatString + "%H";
                                m_FirstFlag = false;
                            }
                            else {
                                formatString = formatString + "-%H";
                            }
                        }
                        //////////////////////字符串是否存在分的数据/////////////////
                        var m_StringIndex = m_DateString.indexOf("分");
                        if (m_StringIndex >= 0) {
                            m_Minute = m_DateString.substring(0, m_StringIndex);
                            m_DateString = m_DateString.substring(m_StringIndex + 1);
                            if (m_FirstFlag == true) {
                                formatString = formatString + "%M";
                                m_FirstFlag = false;
                            }
                            else {
                                formatString = formatString + ":%M";
                            }
                        }
                        //////////////////////字符串是否存在秒的数据/////////////////
                        var m_StringIndex = m_DateString.indexOf("秒");
                        if (m_StringIndex >= 0) {
                            m_Second = m_DateString.substring(0, m_StringIndex);
                            m_DateString = m_DateString.substring(m_StringIndex + 1);
                            if (m_FirstFlag == true) {
                                formatString = formatString + "%S";
                                m_FirstFlag = false;
                            }
                            else {
                                formatString = formatString + ":%S";
                            }
                        }
                        var m_DateTemp = m_FullYear + "/" + m_Month.toString() + "/" + m_Day + " " + m_Hour + ":" + m_Minute + ":" + m_Second;
                        m_LineTemp.push([m_DateTemp, parseFloat(m_Rows[i][m_Name])]);
                    }
                }
            }
        }
        m_Lines.push(m_LineTemp);
    }
    //////////////////////////////找到最大的bar累加和////////////////////////////
    for (var i = 0; i < m_Rows.length; i++) {
        for (var j = 1; j < myData['columns'].length; j++) {
            var m_CurrentValue = parseFloat(m_Rows[i][myData['columns'][j].field]);
            if (m_CurrentValue > m_MaxBarValue) {
                m_MaxBarValue = m_CurrentValue;
            }
        }
    }
    m_MaxBarValue = GetYaxisMax(m_MaxBarValue);

    var ChartJqplot = $.jqplot(myChartObjId, m_Lines, {
        animate: true,
        // Will animate plot on calls to plot1.replot({resetAxes:true})
        seriesColors: ["#62fcf1", "#ff7471", "#fdf39d", "#8def77", "#4da5fe", "#7f82ec", "#eb3e68", "#f8a659", "#249090", "#fe9af6", "#e1e1e3", "#a6a6a7"],
        animateReplot: true,
        seriesDefaults: {
            lineWidth: 1,
            markerOptions: { size: 0 }
        },
        axesDefaults: {
            tickRenderer: $.jqplot.CanvasAxisTickRenderer
        },
        series: m_Labels,
        title: {
            text: myTitle,
            fontFamily: '"Times New Roman", cursive',
            fontSize: '11pt',
            textColor: '#000000'
        },
        legend: {
            renderer: $.jqplot.EnhancedLegendRenderer,
            show: true,
            location: 's',
            placement: 'outsideGrid',
            disableIEFading: true,
            seriesToggle: 'normal',
            seriesToggleReplot: true,
            rendererOptions: {
                numberColumns: 4
            }
        },
        axes: {
            xaxis: {
                renderer: $.jqplot.DateAxisRenderer,
                tickRenderer: $.jqplot.CanvasAxisTickRenderer,
                tickOptions: {
                    formatString: formatString,
                    fontFamily: 'Times New Roman',
                    fontSize: '9pt',
                    fontWeight: 'normal',
                    textColor: '#000000'
                },
                label: myData['Units']['UnitX'],
                labelOptions: {
                    fontFamily: 'Times New Roman',
                    fontSize: '10pt',
                    textColor: '#000000'
                },
                labelRenderer: $.jqplot.CanvasAxisLabelRenderer
            },
            yaxis: {
                tickRenderer: $.jqplot.CanvasAxisTickRenderer,
                tickOptions: {
                    formatString: "%.2f",
                    fontFamily: 'Times New Roman',
                    fontSize: '9pt',
                    fontWeight: 'normal',
                    textColor: '#000000'
                },
                labelRenderer: $.jqplot.CanvasAxisLabelRenderer,
                labelOptions: {
                    fontFamily: 'Times New Roman',
                    fontSize: '10pt',
                    textColor: '#000000'
                },
                tickInterval: m_MaxBarValue / 10,
                min: 0,
                max: m_MaxBarValue,
                pad: 0,
                label: myData['Units']['UnitY']
            }
        },
        cursor: {
            show: true,
            zoom: true
        },
        highlighter: {
            show: true,
            showMarker: true,
            sizeAdjust: 16,  // 当鼠标移动到数据点上时，数据点扩大的增量
            tooltipAxes: 'y',
            fadeTooltip: true,// 设置提示信息栏出现和消失的方式（是否淡入淡出）  
            lineWidthAdjust: 2.5,   //当鼠标移动到放大的数据点上时，设置增大的数据点的宽度
            //tooltipOffset: 20,       // 提示信息栏据被高亮显示的数据点的偏移位置，以像素计
            tooltipLocation: 'n'
        },
        grid: {
            drawGridLines: true, // wether to draw lines across the grid or not.
            gridLineColor: '#cccccc', // 设置整个图标区域网格背景线的颜色
            background: '#ffffff', // 设置整个图表区域的背景色
            borderColor: '#666666', // 设置图表的(最外侧)边框的颜色
            borderWidth: 1.0, //设置图表的（最外侧）边框宽度
            shadow: false, // 为整个图标（最外侧）边框设置阴影，以突出其立体效果
            shadowAngle: 45, // 设置阴影区域的角度，从x轴顺时针方向旋转
            shadowOffset: 1.5, // 设置阴影区域偏移出图片边框的距离
            shadowWidth: 3, // 设置阴影区域的宽度
            shadowDepth: 3, // 设置影音区域重叠阴影的数量
            shadowAlpha: 0.07, // 设置阴影区域的透明度
            rendererOptions: {} // options to pass to the renderer. Note, the default
        },
        canvasOverlay: {
            show: true
        }
    });

    $('#' + myChartObjId).parent().bind('_resize', function (event, ui) {
        ChartJqplot.replot({ resetAxes: true });
    });

    var labels = $('table.jqplot-table-legend');
    labels.each(function (index) {
        $(this).css('border', '0px');
    });
    var LegendLabellabels = $('td.jqplot-table-legend-label');
    LegendLabellabels.each(function (index) {
        $(this).css('padding-right', '10px');
    });
    return ChartJqplot;
}


function GetBarChart(myChartObjId, myData, myTitle) {
    var m_ColumnName = "";
    var m_Rows = myData['rows'];
    var m_Labels = new Array();
    var m_AxisX = new Array();
    var m_Bars = new Array();
    var m_MaxBarValue = 0;  //;
    ////////////////////////////////获得颜色标签名////////////////////
    for (var i = 0; i < m_Rows.length; i++) {
        var m_LabelItem = { label: m_Rows[i][myData['columns'][0]['field']] };
        m_Labels.push(m_LabelItem);
    }
    /////////////////////////////获得x轴坐标名称//////////////////////
    for (var i = 1; i < myData['columns'].length; i++) {

        m_AxisX.push(myData['columns'][i]['title']);
    }
    if (m_Rows.length == 1) {
        var m_BarTemp = [];
        for (var j = 1; j < myData['columns'].length; j++) {
            m_BarTemp.push(parseFloat(m_Rows[0][myData['columns'][j].field]));    //m_Rows[i][myData['columns'][j].field]
        }
        //////////////////////////////找到最大的bar累加和////////////////////////////
        for (var j = 0; j < m_BarTemp.length; j++) {
            if (m_BarTemp[j] > m_MaxBarValue) {
                m_MaxBarValue = m_BarTemp[j];
            }
        }
        m_Bars.push(m_BarTemp);
        m_MaxBarValue = GetYaxisMax(m_MaxBarValue);
    }
    else if (m_Rows.length > 1) {
        for (var i = 0; i < m_Rows.length; i++) {
            var m_BarTemp = new Array();
            for (var j = 1; j < myData['columns'].length; j++) {
                m_BarTemp.push(parseFloat(m_Rows[i][myData['columns'][j].field]));
            }
            m_Bars.push(m_BarTemp);
        }
        //////////////////////////////找到最大的bar累加和////////////////////////////
        for (var i = 0; i < m_Bars.length; i++) {
            for (var j = 0; j < m_Bars[i].length; j++) {
                if (m_Bars[i][j] > m_MaxBarValue) {
                    m_MaxBarValue = m_Bars[i][j];
                }
            }
        }
    }

    m_MaxBarValue = GetYaxisMax(m_MaxBarValue);
    var m_BarMargin = 5;   //计算每组之间的间距
    //if (m_Bars.length > 4) {
    //    m_BarMargin = 2;
    //}
    //else {
    //    m_BarMargin = 7 - m_Bars.length;
    //}
    ////////////////柱状图是否转角////////////////
    var m_ChartWidth = $('#' + myChartObjId).width();
    var m_SingleBarWidth = (m_ChartWidth - (myData['columns'].length - 1) * 10) / (myData['rows'].length * (myData['columns'].length - 1));
    var m_BarZoneWidth = 0;
    var m_AngleX = 0;
    if (m_SingleBarWidth < 15) {
        m_BarZoneWidth = myData['rows'].length * 15 + 10;
    }
    else {
        m_BarZoneWidth = myData['rows'].length * m_SingleBarWidth + 10;
    }
    var m_MaxWordLength = 0;
    for (var i = 0; i < m_AxisX.length; i++) {
        if (m_AxisX[i].length > m_MaxWordLength) {
            m_MaxWordLength = m_AxisX[i].length;
        }
    }
    if (m_BarZoneWidth <= 11) {
        m_AngleX = -90;
    }
    else if (m_BarZoneWidth >= m_MaxWordLength * 11) {
        m_AngleX = 0;
    }
    else {
        var m_Radian = Math.asin(m_BarZoneWidth / (m_MaxWordLength * 11));
        m_AngleX = 180 * m_Radian / Math.PI - 90;
        var m_ModifyParmeter = Math.pow((m_AngleX + 45), 2);
        var m_ModifyRadio = 45 - Math.sqrt(m_ModifyParmeter);
        m_AngleX = m_AngleX - m_AngleX * m_ModifyRadio / 45;
        if (m_AngleX > -12) {
            m_AngleX = -12;
        }
        else if (m_AngleX < -85) {
            m_AngleX = -90;
        }
    }
    
    //if (m_AngleX <= -45) {
        
    //}
    //else if (m_AngleX < -20) {
    //    m_AngleX = m_AngleX + (40 + m_AngleX) * 0.6;
    //}
    //else if (m_AngleX < -10) {
    //    m_AngleX = m_AngleX + 5;
    //}
    ////////////
    var BarJqplot = $.jqplot(myChartObjId, m_Bars, {
        animate: !$.jqplot.use_excanvas,
        //seriesColors: ["#4bb2c5", "#c5b47f", "#EAA228", "#579575", "#839557", "#958c12", "#953579", "#4b5de4", "#d8b83f", "#ff5800", "#0085cc"],
        seriesColors: ["#62fcf1", "#ff7471", "#fdf39d", "#8def77", "#4da5fe", "#7f82ec", "#eb3e68", "#f8a659", "#249090", "#fe9af6", "#e1e1e3", "#a6a6a7"],
        seriesDefaults: {
            renderer: $.jqplot.BarRenderer,
            //pointLabels: { show: true, location: 'e', edgeTolerance: -15 },
            rendererOptions: {
                barPadding: 1, //设置同一分类两个柱状条之间的距离(px)
                barMargin: m_BarMargin, //设置不同分类两个柱状条之间的距离(px)(同一个横坐标表点上)
                barDirection: 'vertical', //设置柱状图显示的方向：垂直显示和水平显示，默认垂直显示 vertical or horizontal.
                barWidth: null, //设置柱状图中每个柱状条的宽度
                shadowAngle: 30,
                shadowOffset: 2, //同grid相同属性设置
                shadowDepth: 1, //同grid相同属性设置
                shadowAlpha: 0.3, //同grid相同属性设置
            }
        },
        series: m_Labels,
        title: {
            text: myTitle,
            fontFamily: '"Times New Roman", cursive',
            fontSize: '11pt',
            textColor: '#000000'
        },
        legend: {
            renderer: $.jqplot.EnhancedLegendRenderer,
            show: true,
            location: 's',
            placement: 'outsideGrid',
            disableIEFading: true,
            seriesToggle: 'normal',
            seriesToggleReplot: true,
            rendererOptions: {
                numberColumns: 4
            }
        },
        axes: {
            xaxis: {
                renderer: $.jqplot.CategoryAxisRenderer,
                ticks: m_AxisX,
                mark: 'outside',
                tickRenderer: $.jqplot.CanvasAxisTickRenderer,
                tickOptions: {
                    angle: m_AngleX,
                    fontFamily: 'Times New Roman',
                    fontSize: '9pt',
                    fontWeight: 'normal',
                    textColor: '#000000'
                },
                label: myData['Units']['UnitX'],
                labelOptions: {
                    fontFamily: 'Times New Roman',
                    fontSize: '10pt',
                    textColor: '#000000'
                },
                labelRenderer: $.jqplot.CanvasAxisLabelRenderer
            },
            yaxis: {
                //renderer: $.jqplot.CategoryAxisRenderer,

                tickRenderer: $.jqplot.CanvasAxisTickRenderer,
                labelRenderer: $.jqplot.CanvasAxisLabelRenderer,
                tickOptions: {
                    fontFamily: 'Times New Roman',
                    fontSize: '9pt',
                    fontWeight: 'normal',
                    textColor: '#000000'
                },
                tickInterval: m_MaxBarValue / 10,
                min: 0,
                max: m_MaxBarValue,
                pad: 0,
                label: myData['Units']['UnitY'],
                tickRenderer: $.jqplot.CanvasAxisTickRenderer,
                labelRenderer: $.jqplot.CanvasAxisLabelRenderer,
                labelOptions: {
                    fontFamily: 'Times New Roman',
                    fontSize: '10pt',
                    textColor: '#000000'
                }
            }
        },
        grid: {
            drawGridLines: true, // wether to draw lines across the grid or not.
            gridLineColor: '#cccccc', // 设置整个图标区域网格背景线的颜色
            background: '#ffffff', // 设置整个图表区域的背景色
            borderColor: '#666666', // 设置图表的(最外侧)边框的颜色
            borderWidth: 1.0, //设置图表的（最外侧）边框宽度
            shadow: false, // 为整个图标（最外侧）边框设置阴影，以突出其立体效果
            shadowAngle: 45, // 设置阴影区域的角度，从x轴顺时针方向旋转
            shadowOffset: 1.5, // 设置阴影区域偏移出图片边框的距离
            shadowWidth: 3, // 设置阴影区域的宽度
            shadowDepth: 3, // 设置影音区域重叠阴影的数量
            shadowAlpha: 0.07, // 设置阴影区域的透明度
            rendererOptions: {} // options to pass to the renderer. Note, the default
            // CanvasGridRenderer takes no additional options.
        },
        cursor: {
            show: true,
            zoom: true
        },
        highlighter: {
            show: true,
            showMarker: true,
            tooltipAxes: 'y',
            fadeTooltip: true,// 设置提示信息栏出现和消失的方式（是否淡入淡出）  
            tooltipLocation: 'n'
        },
        canvasOverlay: {
            show: true
        }
    });
    /*
    var s1 = [2, 6, 7, 10];
    var s2 = [7, 5, 3, 2];
    var s3 = [14, 9, 3, 8];

    var BarJqplot = $.jqplot(myChartObjId, [s1, s2, s3], {
        stackSeries: true,
        captureRightClick: true,
        seriesDefaults: {
            renderer: $.jqplot.BarRenderer,
            rendererOptions: {
                highlightMouseDown: true
            },
            pointLabels: { show: true }
        },
        legend: {
            show: true,
            location: 'e',
            placement: 'outside'
        }
    });
    */
    var labels = $('table.jqplot-table-legend');
    labels.each(function (index) {
        //turn the label's text color to the swatch's color  
        $(this).css('border', '0px');
        //set type name as the label's text  

    });
    var LegendLabellabels = $('td.jqplot-table-legend-label');
    LegendLabellabels.each(function (index) {
        //turn the label's text color to the swatch's color  
        //$(this).css('font-size', '10pt');
        $(this).css('padding-right', '10px');
        //set type name as the label's text  

    });
    return BarJqplot;
}
function GetMultiBarChart(myChartObjId, myData, myTitle) {
    var m_ColumnName = "";
    var m_Rows = myData['rows'];
    var m_Labels = new Array();
    var m_AxisX = new Array();
    var m_Bars = new Array();
    var m_MaxBarValue = 0;  //;
    ////////////////////////////////获得颜色标签名////////////////////
    for (var i = 0; i < m_Rows.length; i++) {
        var m_LabelItem = { label: m_Rows[i][myData['columns'][0]['field']] };
        m_Labels.push(m_LabelItem);
    }
    /////////////////////////////获得x轴坐标名称//////////////////////
    for (var i = 1; i < myData['columns'].length; i++) {

        m_AxisX.push(myData['columns'][i]['title']);
    }
    ///////////////////////////////填充数据////////////////////////////////
    if (m_Rows.length == 1) {
        var m_BarTemp = [];
        for (var j = 1; j < myData['columns'].length; j++) {
            m_BarTemp.push(parseFloat(m_Rows[0][myData['columns'][j].field]));    //m_Rows[i][myData['columns'][j].field]
        }
        //////////////////////////////找到最大的bar累加和////////////////////////////
        for (var j = 0; j < m_BarTemp.length; j++) {
            if (m_BarTemp[j] > m_MaxBarValue) {
                m_MaxBarValue = m_BarTemp[j];
            }
        }
        m_Bars.push(m_BarTemp);
        m_MaxBarValue = GetYaxisMax(m_MaxBarValue);
    }
    else if (m_Rows.length > 1) {
        for (var i = 0; i < m_Rows.length; i++) {
            var m_BarTemp = new Array();
            for (var j = 1; j < myData['columns'].length; j++) {
                m_BarTemp.push(parseFloat(m_Rows[i][myData['columns'][j].field]));
            }
            m_Bars.push(m_BarTemp);
        }
        //////////////////////////////找到最大的bar累加和////////////////////////////
        for (var j = 0; j < m_Bars[0].length; j++) {
            var m_SumBarValue = 0;
            for (var i = 0; i < m_Bars.length; i++) {
                m_SumBarValue = m_SumBarValue + m_Bars[i][j];
            }
            if (m_SumBarValue > m_MaxBarValue) {
                m_MaxBarValue = m_SumBarValue;
            }
        }
        m_MaxBarValue = GetYaxisMax(m_MaxBarValue);
    }

    //var bb = JSON.stringify(m_Bars)

    var m_BarMargin = 10;   //计算每组之间的间距0
    //if (m_Bars.length > 4) {
    //    m_BarMargin = 2;
    //}
    //else {
    //    m_BarMargin = 7 - m_Bars.length;
    //}
    ////////////////柱状图是否转角////////////////
    var m_ChartWidth = $('#' + myChartObjId).width();
    var m_SingleBarWidth = (m_ChartWidth - (myData['columns'].length - 1) * 10) / (myData['columns'].length - 1);
    var m_BarZoneWidth = 0;
    var m_AngleX = 0;
    if (m_SingleBarWidth < 15) {
        m_BarZoneWidth = myData['rows'].length * 15 + 10;
    }
    else {
        m_BarZoneWidth = myData['rows'].length * m_SingleBarWidth + 10;
    }
    var m_MaxWordLength = 0;
    for (var i = 0; i < m_AxisX.length; i++) {
        if (m_AxisX[i].length > m_MaxWordLength) {
            m_MaxWordLength = m_AxisX[i].length;
        }
    }
    if (m_BarZoneWidth <= 11) {
        m_AngleX = -90;
    }
    else if (m_BarZoneWidth >= m_MaxWordLength * 11) {
        m_AngleX = 0;
    }
    else {
        var m_Radian = Math.asin(m_BarZoneWidth / (m_MaxWordLength * 11));
        m_AngleX = 180 * m_Radian / Math.PI - 90;
        var m_ModifyParmeter = Math.pow((m_AngleX + 45), 2);
        var m_ModifyRadio = 45 - Math.sqrt(m_ModifyParmeter);
        m_AngleX = m_AngleX - m_AngleX * m_ModifyRadio / 45;
        if (m_AngleX > -12) {
            m_AngleX = -12;
        }
        else if (m_AngleX < -85) {
            m_AngleX = -90;
        }
    }
    var MultiBarJqplot = $.jqplot(myChartObjId, m_Bars, {
        // Tell the plot to stack the bars.
        stackSeries: true,
        seriesColors: ["#62fcf1", "#ff7471", "#fdf39d", "#8def77", "#4da5fe", "#7f82ec", "#eb3e68", "#f8a659", "#249090", "#fe9af6", "#e1e1e3", "#a6a6a7"],
        captureRightClick: true,
        seriesDefaults: {
            renderer: $.jqplot.BarRenderer,
            rendererOptions: {
                // Put a 30 pixel margin between bars.
                // Highlight bars when mouse button pressed.
                // Disables default highlighting on mouse over.
            highlightMouseDown: true,
            barMargin: m_BarMargin, //设置不同分类两个柱状条之间的距离(px)(同一个横坐标表点上)
            barDirection: 'vertical', //设置柱状图显示的方向：垂直显示和水平显示，默认垂直显示 vertical or horizontal.
            barWidth: null, //设置柱状图中每个柱状条的宽度
            shadowAngle: 30,
            shadowOffset: 2, //同grid相同属性设置
            shadowDepth: 1, //同grid相同属性设置
            shadowAlpha: 0.3, //同grid相同属性设置
            },
            //pointLabels: { show: true }
        },
        axes: {
            xaxis: {
                renderer: $.jqplot.CategoryAxisRenderer,
                ticks: m_AxisX,
                tickRenderer: $.jqplot.CanvasAxisTickRenderer,
                tickOptions: {
                    angle: m_AngleX,
                    fontFamily: 'Times New Roman',
                    fontSize: '9pt',
                    fontWeight: 'normal',
                    textColor: '#000000'
                },
                label: myData['Units']['UnitX'],
                labelOptions: {
                    fontFamily: 'Times New Roman',
                    fontSize: '10pt',
                    textColor: '#000000'
                },
                labelRenderer: $.jqplot.CanvasAxisLabelRenderer
            },
            yaxis: {
                //renderer: $.jqplot.CategoryAxisRenderer,

                tickRenderer: $.jqplot.CanvasAxisTickRenderer,
                tickOptions: {
                    fontFamily: 'Times New Roman',
                    fontSize: '9pt',
                    fontWeight: 'normal',
                    textColor: '#000000'
                },
                labelRenderer: $.jqplot.CanvasAxisLabelRenderer,
                labelOptions: {
                    fontFamily: 'Times New Roman',
                    fontSize: '10pt',
                    textColor: '#000000'
                },
                tickInterval: m_MaxBarValue / 10,
                min: 0,
                max: m_MaxBarValue,
                pad: 0,
                // Don't pad out the bottom of the data range.  By default,
                // axes scaled as if data extended 10% above and below the
                // actual range to prevent data points right on grid boundaries.
                // Don't want to do that here.
                padMin: 0,
                label: myData['Units']['UnitY'],
                //tickRenderer: $.jqplot.CanvasAxisTickRenderer,
                //labelRenderer: $.jqplot.CanvasAxisLabelRenderer,
                labelOptions: {
                    fontFamily: 'Times New Roman',
                    fontSize: '10pt'
                }
            }
        },
        series: m_Labels,
        title: {
            text: myTitle,
            fontFamily: '"Times New Roman", cursive',
            fontSize: '11pt',
            textColor: '#000000'
        },
        legend: {
            renderer: $.jqplot.EnhancedLegendRenderer,
            show: true,
            location: 's',
            placement: 'outsideGrid',
            disableIEFading: true,
            seriesToggle: 'normal',
            seriesToggleReplot: true,
            rendererOptions: {
                numberColumns: 4
            }
        },
        highlighter: {
            show: true,
            showMarker: true,
            tooltipAxes: 'y',
            fadeTooltip: true,// 设置提示信息栏出现和消失的方式（是否淡入淡出）  
            tooltipLocation: 'n'
        },
        grid: {
            drawGridLines: true, // wether to draw lines across the grid or not.
            gridLineColor: '#cccccc', // 设置整个图标区域网格背景线的颜色
            background: '#ffffff', // 设置整个图表区域的背景色
            borderColor: '#666666', // 设置图表的(最外侧)边框的颜色
            borderWidth: 1.0, //设置图表的（最外侧）边框宽度
            shadow: false, // 为整个图标（最外侧）边框设置阴影，以突出其立体效果
            shadowAngle: 45, // 设置阴影区域的角度，从x轴顺时针方向旋转
            shadowOffset: 1.5, // 设置阴影区域偏移出图片边框的距离
            shadowWidth: 3, // 设置阴影区域的宽度
            shadowDepth: 3, // 设置影音区域重叠阴影的数量
            shadowAlpha: 0.07, // 设置阴影区域的透明度
            rendererOptions: {} // options to pass to the renderer. Note, the default
            // CanvasGridRenderer takes no additional options.
        },
        cursor: {
            show: true,
            zoom: true
        },
        canvasOverlay: {
            show: true
        }
    });
    // Bind a listener to the "jqplotDataClick" event.  Here, simply change
    // the text of the info3 element to show what series and ponit were
    // clicked along with the data for that point.
    //$('#chart3').bind('jqplotDataClick', 
    //  function (ev, seriesIndex, pointIndex, data) {
    //      $('#info3').html('series: '+seriesIndex+', point: '+pointIndex+', data: '+data);
    //  }
    //); 
    var labels = $('table.jqplot-table-legend');
    labels.each(function (index) {
        $(this).css('border', '0px');
    });
    var LegendLabellabels = $('td.jqplot-table-legend-label');
    LegendLabellabels.each(function (index) {
        $(this).css('padding-right', '10px');
    });
    return MultiBarJqplot;
}
function GetPieChart(myChartObjId, myData, myTitle) {

    var m_Rows = myData['rows'];
    var m_Labels = new Array();
    var m_Pie = new Array();
    ////////////////////////////////获得颜色标签名////////////////////
    for (var i = 0; i < m_Rows.length; i++) {
        var m_TitleTemp = m_Rows[i]['RowName'];
        var m_ValueTemp = 0;
        for (var j = 1; j < myData['columns'].length; j++) {
            m_ValueTemp = m_ValueTemp + parseFloat(m_Rows[i][myData['columns'][j].field]);
        }
        m_Labels.push(m_TitleTemp);
        m_Pie.push([m_TitleTemp, m_ValueTemp]);
    }
    var PieJqplot = jQuery.jqplot(myChartObjId, [m_Pie],
    {
        animate: !$.jqplot.use_excanvas,
        // Will animate plot on calls to plot1.replot({resetAxes:true})
        seriesColors: ["#62fcf1", "#ff7471", "#fdf39d", "#8def77", "#4da5fe", "#7f82ec", "#eb3e68", "#f8a659", "#249090", "#fe9af6", "#e1e1e3", "#a6a6a7"],
        animateReplot: true,
        seriesDefaults: {
            shadow: true,
            //color: '#FF0000',
            renderer: jQuery.jqplot.PieRenderer,
            rendererOptions: {
                startAngle: 180,
                showDataLabels: true,

                //diameter: undefined, // 设置饼的直径
                padding: 20,        // 饼距离其分类名称框或者图表边框的距离，变相该表饼的直径
                sliceMargin: 2,     // 饼的每个部分之间的距离
                fill: true,         // 设置饼的每部分被填充的状态
                shadow: true,       //为饼的每个部分的边框设置阴影，以突出其立体效果
                shadowOffset: 2,    //设置阴影区域偏移出饼的每部分边框的距离
                shadowDepth: 2,     // 设置阴影区域的深度
                shadowAlpha: 0.2   // 设置阴影区域的透明度
            }
        },

        title: {
            text: myTitle,
            fontFamily: '"Times New Roman", cursive',
            fontSize: '11pt',
            textColor: '#000000'
        },
        legend: {
            renderer: $.jqplot.EnhancedLegendRenderer,
            show: true,
            location: 'e',
            placement: 'inside',
            disableIEFading: true,
            seriesToggle: 'normal',
            seriesToggleReplot: true,
            rowSpacing: '1px',
            rendererOptions: {
                numberColumns: 1
            }
        },
        highlighter: {
            show: false,
            showMarker: false
        },
        //legend: { show: true, location: 'e', placement: 'inside', fontSize: '10pt', textColor: '#000000' },
        grid: {
            drawGridLines: true, // wether to draw lines across the grid or not.
            gridLineColor: '#cccccc', // 设置整个图标区域网格背景线的颜色
            background: '#ffffff', // 设置整个图表区域的背景色
            borderColor: '#666666', // 设置图表的(最外侧)边框的颜色
            borderWidth: 1.0, //设置图表的（最外侧）边框宽度
            shadow: false, // 为整个图标（最外侧）边框设置阴影，以突出其立体效果
            shadowAngle: 45, // 设置阴影区域的角度，从x轴顺时针方向旋转
            shadowOffset: 1.5, // 设置阴影区域偏移出图片边框的距离
            shadowWidth: 3, // 设置阴影区域的宽度
            shadowDepth: 3, // 设置影音区域重叠阴影的数量
            shadowAlpha: 0.07, // 设置阴影区域的透明度
            rendererOptions: {} // options to pass to the renderer. Note, the default
            // CanvasGridRenderer takes no additional options.
        }
    });
    $('.jqplot-target').css('font-size', '10pt');

    var labels = $('table.jqplot-table-legend');
    labels.each(function (index) {
        //turn the label's text color to the swatch's color  
        $(this).css('border', '0px');
        $(this).css('font-size', '9pt');
        $(this).css('text-align', 'left');
        //set type name as the label's text  
        
    });
   
    return PieJqplot;
}

// 获取仪表盘
function GetMeterGauge(myChartObjId, myData, myTitle) {

    var m_Rows = myData['rows'];    // 数据行
    var m_ActualValue = 0;
    var m_AlarmValue = 0;
    var m_MinimunVlaue = 0;
    var m_MaximunValue = 0;

    // 数据验证
    if (myData['rows'].length == 0)  // 无数据行
        return null;

    // 遍历数据表
    // 数据表为一个n行2列的表，列名为RowName是数据的类型，列名为Value是数据的值
    // 其中数据类型包括：实际值，报警值，表盘下限值（可选，为空时下限值为0）， 表盘上限值（可选，为空时，程序自动以报警值为表盘的70%的方式计算表盘上限）
    for (var i = 0; i < m_Rows.length; i++) {
        var m_ValueType = m_Rows[i]['RowName'];
        var m_Value = parseFloat(m_Rows[i]['value']);

        switch (m_ValueType) {
            case '表盘实际值':     // 当前的实际值
                m_ActualValue = m_Value;
                break;
            case '报警设定值':      // 设定的报警值
                m_AlarmValue = m_Value;
                break;
            case '表盘下限值':    // 表盘的下限值
                m_MinimunVlaue = m_Value;
                break;
            case '表盘上限值':    // 表盘的上限值
                m_MaximunValue = m_Value;
                break;
            default:                // 其他值忽略
                break;
        }
    }

    // 计算表盘的最大值
    if (m_MaximunValue == 0 && m_AlarmValue > 0) {  // 仅当表盘上限值为0，且设定报警值大于0时，才计算表盘的上限值
        m_MaximunValue = m_ActualValue / 0.7;
    }

    // 获取仪表盘
    var MeterGaugeJqplot = $.jqplot(myChartObjId, [[m_ActualValue]], {
        title: myTitle,
        seriesColors: ["#62fcf1", "#f75a57", "#fdf39d", "#8def77", "#4da5fe", "#7f82ec", "#f35a7f", "#f8a659", "#249090", "#fe9af6", "#e1e1e3", "#a6a6a7"],
        seriesDefaults: {
            renderer: $.jqplot.MeterGaugeRenderer,
            rendererOptions: {
                min: m_MinimunVlaue,                            // 表盘的下限值
                max: m_MaximunValue,                            // 表盘的上限值
                ringColor: '#4B4B4B',                           // 表盘的颜色（包括指针）
                intervals: [m_AlarmValue, m_MaximunValue],      // 表盘的区间（现仅有正常区间和报警区间）
                intervalColors: ['#99FF66', '#FF0033'],         // 表盘的区间颜色（正常区间为绿色，报警区间为红色）
                label: myData['Units']['UnitX'],                // 表盘的单位（仅从UnitX中取）
                hubRadius: 10                                   // 表盘指针下方圆点的大小
            }
        },
        cursor: {
            show: false,
            zoom: false
        },
        grid: {
            drawGridLines: true, // wether to draw lines across the grid or not.
            gridLineColor: '#cccccc', // 设置整个图标区域网格背景线的颜色
            background: '#fffdf6', // 设置整个图表区域的背景色
            borderColor: '#999999', // 设置图表的(最外侧)边框的颜色
            borderWidth: 2.0, //设置图表的（最外侧）边框宽度
            shadow: false, // 为整个图标（最外侧）边框设置阴影，以突出其立体效果
            shadowAngle: 45, // 设置阴影区域的角度，从x轴顺时针方向旋转
            shadowOffset: 1.5, // 设置阴影区域偏移出图片边框的距离
            shadowWidth: 3, // 设置阴影区域的宽度
            shadowDepth: 3, // 设置影音区域重叠阴影的数量
            shadowAlpha: 0.07, // 设置阴影区域的透明度
            renderer: $.jqplot.CanvasGridRenderer, // renderer to use to draw the grid.
            rendererOptions: {} // options to pass to the renderer. Note, the default
            // CanvasGridRenderer takes no additional options.
        }
    });

    return MeterGaugeJqplot;
}

function ReleasePlotChart(containerId, plot) {
    if (plot) {
        plot.destroy();

        var elementId = '#' + containerId;
        $(elementId).unbind(); // for iexplorer  
        $(elementId).empty();

        plot = null;
    }
}
//function GetYaxisMax(myMaxValue) {
//    var m_MaxValue = Math.abs(myMaxValue);
//    if (m_MaxValue >= 1) {
//        var m_MaxDigits = 1;
//        while ((m_MaxValue / 10) > 1 && m_MaxDigits < 10000000000000000000000000000) {
//            m_MaxValue = m_MaxValue / 10;
//            m_MaxDigits = m_MaxDigits * 10;
//        }
//        return Math.ceil((myMaxValue / m_MaxDigits)) * m_MaxDigits;
//    }
//    else if (m_MaxValue > 0 && m_MaxValue < 1) {
//        var m_MaxDigits = 1;
//        while ((m_MaxValue * 10) < 1 && m_MaxDigits < 10000000000000000000000000000) {
//            m_MaxValue = m_MaxValue * 10;
//            m_MaxDigits = m_MaxDigits * 10;
//        }
//        return Math.ceil((myMaxValue * m_MaxDigits)) / m_MaxDigits;
//    }
//    else {
//        return 1;
//    }

//}
function RefreshChartY(myChartObjId) {
    alert(myChartObjId)
}
function GetYaxisMax(myMaxValue) {
    var m_MaxValue = Math.abs(myMaxValue);
    var m_MaxValueResult = 1;
    var m_LoopFlag = true;
    if (m_MaxValue >= 1) {
        while (m_LoopFlag) {
            if (m_MaxValue >= 10) {             //当大于10的情况下
                m_MaxValueResult = m_MaxValueResult * 10;
            }
            else {                             //当大于1小于10的情况下
                m_MaxValueResult = m_MaxValueResult * Math.ceil(m_MaxValue * 12) / 10;
                m_LoopFlag = false;
            }
            m_MaxValue = m_MaxValue / 10;
        }
    }
    else if (m_MaxValue > 0 && m_MaxValue < 1) {
        while (m_LoopFlag) {
            if (m_MaxValue < 1) {             //当大于10的情况下
                m_MaxValueResult = m_MaxValueResult * 0.1;
            }
            else {                             //当大于1小于10的情况下
                m_MaxValueResult = m_MaxValueResult * Math.ceil(m_MaxValue * 12) / 10;
                m_LoopFlag = false;
            }
            m_MaxValue = m_MaxValue * 10;
        }
    }
    else {
        m_MaxValueResult = 0;
    }
    return m_MaxValueResult;
}
function GetYaxisMin(myMinValue) {
    var m_MinValue = Math.abs(myMinValue);
    var m_MinValueResult = 1;
    var m_LoopFlag = true;
    if (m_MinValue >= 1) {
        while (m_LoopFlag) {
            if (m_MinValue >= 10) {             //当大于10的情况下
                m_MinValueResult = m_MinValueResult * 10;
            }
            else {                             //当大于1小于10的情况下
                m_MinValueResult = m_MinValueResult * Math.floor(m_MinValue * 0.8) / 10;
                m_LoopFlag = false;
            }
            m_MinValue = m_MinValue / 10;
        }
    }
    else if (m_MinValue > 0 && m_MinValue < 1) {
        while (m_LoopFlag) {
            if (m_MinValue < 1) {             //当大于10的情况下
                m_MinValueResult = m_MinValueResult * 0.1;
            }
            else {                             //当大于1小于10的情况下
                m_MinValueResult = m_MinValueResult * Math.floor(m_MinValue * 0.8) / 10;
                m_LoopFlag = false;
            }
            m_MinValue = m_MinValue * 10;
        }
    }
    else {
        m_MinValueResult = 0;
    }
    return m_MinValueResult;
}

