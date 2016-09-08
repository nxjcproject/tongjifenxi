// inner variables
var canvas, ctx;
var clockRadius = 90;
var clockImage;
var centerPosX = 100;
var centerPosY = 100;
var PVFData = null;
var PVFColor = { "Peak": "#ff0000", "Flat": "#ffff00", "Valley": "#00ff00" };
// draw functions :
function clear() { // clear canvas function
    ctx.clearRect(0, 0, ctx.canvas.width, ctx.canvas.height);
}

function drawScene() { // main drawScene function
    clear(); // clear canvas

    drawProcess();

    // get current time
    var date = new Date();
    var hours = date.getHours();
    var minutes = date.getMinutes();
    var seconds = date.getSeconds();
    hours = hours > 12 ? hours - 12 : hours;
    var hour = hours + minutes / 60;
    var minute = minutes + seconds / 60;

    // save current context
    ctx.save();

    // draw clock image (as background)
    ctx.drawImage(clockImage, centerPosX - clockRadius, centerPosY - clockRadius, 2 * clockRadius, 2 * clockRadius);

    ctx.translate(canvas.width / 2, canvas.height / 2);
    ctx.beginPath();

    // draw numbers
    ctx.font = 'bold 11pt Arial';
    ctx.fillStyle = '#000';
    ctx.textAlign = 'center';
    ctx.textBaseline = 'middle';
    for (var n = 1; n <= 12; n++) {
        var theta = (n - 3) * (Math.PI * 2) / 12;
        var x = clockRadius * 0.7 * Math.cos(theta);
        var y = clockRadius * 0.7 * Math.sin(theta);
        ctx.fillText(n, x, y);
    }

    // draw hour
    ctx.save();
    var theta = (hour - 3) * 2 * Math.PI / 12;
    ctx.rotate(theta);
    ctx.beginPath();
    ctx.moveTo(-15, -5);
    ctx.lineTo(-15, 5);
    ctx.lineTo(clockRadius * 0.5, 1);
    ctx.lineTo(clockRadius * 0.5, -1);
    ctx.fill();
    ctx.restore();

    // draw minute
    ctx.save();
    var theta = (minute - 15) * 2 * Math.PI / 60;
    ctx.rotate(theta);
    ctx.beginPath();
    ctx.moveTo(-15, -4);
    ctx.lineTo(-15, 4);
    ctx.lineTo(clockRadius * 0.8, 1);
    ctx.lineTo(clockRadius * 0.8, -1);
    ctx.fill();
    ctx.restore();

    // draw second
    ctx.save();
    var theta = (seconds - 15) * 2 * Math.PI / 60;
    ctx.rotate(theta);
    ctx.beginPath();
    ctx.moveTo(-15, -3);
    ctx.lineTo(-15, 3);
    ctx.lineTo(clockRadius * 0.9, 1);
    ctx.lineTo(clockRadius * 0.9, -1);
    //ctx.fillStyle = '#000';
    ctx.fill();
    ctx.restore();
    ctx.restore();

    //写文字
    var year = date.getFullYear();
    var month = date.getMonth() + 1 >= 10 ? (date.getMonth() + 1).toString() : '0' + (date.getMonth() + 1).toString();
    var day = date.getDate() >= 10 ? (date.getDate()).toString() : '0' + (date.getDate()).toString();
    var m_AmpmText = hours > 12 ? "AM" : "PM";

    //ctx.font = 'bold 144px consolas';
    //ctx.textAlign = 'left';
    //ctx.textBaseline = 'top';
    //ctx.strokeStyle = '#DF5326';
    //ctx.strokeText('Hello', 100, 100);
    ctx.font = 'bold 9pt arial';
    ctx.fillStyle = 'black';
    ctx.fillText(m_AmpmText, centerPosX - 10, centerPosY - 30);
    ctx.fillText(year + '-' + month + '-' + day, centerPosX - 30, centerPosY + 35);
}
function drawProcess() {
    PVFData = [];
    PVFData.push({ "id": "Peak", "start": 0, "end": 12012 });
    PVFData.push({ "id": "Flat", "start": 12012, "end": 23523 });
    PVFData.push({ "id": "Valley", "start": 23523, "end": 34532 });
    PVFData.push({ "id": "Flat", "start": 34532, "end": 43200 });
    if (PVFData != null) {
        var deg = Math.PI / 180;
        var m_FVPFlagColor = '#aaaaaa';
        var date = new Date();
        var hours = date.getHours();
        var minutes = date.getMinutes();
        var seconds = date.getSeconds();
        var SencondValue = hours * 3600 + minutes * 60 + seconds;
        for (var i = 0; i < PVFData.length; i++) {
            var m_StartValue = PVFData[i].start >= 43200 ? PVFData[i].start - 43200 : PVFData[i].start;
            var m_EndValue = PVFData[i].end > 43200 ? PVFData[i].end - 43200 : PVFData[i].end;
            var m_StartAngle = parseInt(360 * m_StartValue / 43200) + 270;
            var m_EndAngle = parseInt(360 * m_EndValue / 43200) + 270;
            ctx.fillStyle = PVFColor[PVFData[i].id];
            ctx.sector(centerPosX, centerPosY, ctx.canvas.width / 2, m_StartAngle * deg, m_EndAngle * deg).fill();
            if (PVFData[i].start <= SencondValue && SencondValue < PVFData[i].end) {
                m_FVPFlagColor = PVFColor[PVFData[i].id];
            }
        }
        var rGrd = ctx.createRadialGradient(centerPosX, centerPosY, 0, centerPosX, centerPosY, clockRadius);
        rGrd.addColorStop(0, m_FVPFlagColor);
        rGrd.addColorStop(1, 'white');

        // ***开始画一个灰色的圆  
        ctx.beginPath();
        // 坐标移动到圆心  
        ctx.moveTo(centerPosX, centerPosY);
        // 画圆,圆心是24,24,半径24,从角度0开始,画到2PI结束,最后一个参数是方向顺时针还是逆时针  
        ctx.arc(centerPosX, centerPosY, clockRadius, 0, Math.PI * 2, false);
        ctx.closePath();
        // 填充颜色  
        ctx.fillStyle = rGrd;
        ctx.fill();
        // ***灰色的圆画完  
    }
    else {
        var deg = Math.PI / 180;
        ctx.fillStyle = "#ff0000";
        ctx.sector(centerPosX, centerPosY, ctx.canvas.width / 2, 30 * deg, 111 * deg).fill();
        ctx.fillStyle = "#00ff00";
        ctx.sector(centerPosX, centerPosY, ctx.canvas.width / 2, 111 * deg, 190 * deg).fill();
        ctx.fillStyle = "#0000ff";
        ctx.sector(centerPosX, centerPosY, ctx.canvas.width / 2, 190 * deg, 233 * deg).fill();
        ctx.fillStyle = "#ffff00";
        ctx.sector(centerPosX, centerPosY, ctx.canvas.width / 2, 233 * deg, 280 * deg).fill();
        ctx.fillStyle = "#ff00ff";
        ctx.sector(centerPosX, centerPosY, ctx.canvas.width / 2, 280 * deg, 345 * deg).fill();
        ctx.fillStyle = "#00ffff";
        ctx.sector(centerPosX, centerPosY, ctx.canvas.width / 2, 345 * deg, 30 * deg).fill();

        var rGrd = ctx.createRadialGradient(centerPosX, centerPosY, 0, centerPosX, centerPosY, clockRadius);
        rGrd.addColorStop(0, '#aaaaaa');
        rGrd.addColorStop(1, 'white');

        // ***开始画一个灰色的圆  
        ctx.beginPath();
        // 坐标移动到圆心  
        ctx.moveTo(centerPosX, centerPosY);
        // 画圆,圆心是24,24,半径24,从角度0开始,画到2PI结束,最后一个参数是方向顺时针还是逆时针  
        ctx.arc(centerPosX, centerPosY, clockRadius, 0, Math.PI * 2, false);
        ctx.closePath();
        // 填充颜色  
        ctx.fillStyle = rGrd;
        ctx.fill();
        // ***灰色的圆画完  
    }
}
CanvasRenderingContext2D.prototype.sector = function (x, y, radius, sDeg, eDeg) {
    // 初始保存
    this.save();
    // 位移到目标点
    this.translate(x, y);
    this.beginPath();
    // 画出圆弧
    this.arc(0, 0, radius, sDeg, eDeg);
    // 再次保存以备旋转
    this.save();
    // 旋转至起始角度
    this.rotate(eDeg);
    // 移动到终点，准备连接终点与圆心
    this.moveTo(radius, 0);
    // 连接到圆心
    this.lineTo(0, 0);
    // 还原
    this.restore();
    // 旋转至起点角度
    this.rotate(sDeg);
    // 从圆心连接到起点
    this.lineTo(radius, 0);
    this.closePath();
    // 还原到最初保存的状态
    this.restore();
    return this;
}

// initialization
$(function () {
    canvas = document.getElementById('canvas');
    //ctx = canvas.getContext('2d');
    if (typeof window.G_vmlCanvasManager != "undefined") {
        canvas = window.G_vmlCanvasManager.initElement(canvas);
        ctx = canvas.getContext("2d");
    } else {
        ctx = canvas.getContext("2d");
    }
    // var width = canvas.width;
    // var height = canvas.height;

    clockImage = new Image();
    clockImage.src = '/images/page/clockPanel.png';

    setInterval(drawScene, 1000); // loop drawScene
});