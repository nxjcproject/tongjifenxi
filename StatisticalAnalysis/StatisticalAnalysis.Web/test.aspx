<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="test.aspx.cs" Inherits="StatisticalAnalysis.Web.test" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>

    <!--下面excanvas.js需下载才能在IE下支持canvas-->
<!--[if IE]> 
        <script src="/js/common/html5.js"></script> 
        <script src="/js/common/html5media.min.js"></script> 
        <script type="text/javascript" src="/js/common/json2.min.js"></script>
        <script type="text/javascript" src="/lib/pllib/excanvas.js"></script>
<![endif]-->
    <!--[if lt IE 8 ]><script type="text/javascript" src="/js/common/json2.min.js"></script><![endif]-->
    <script type="text/javascript" src="/lib/ealib/jquery.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="/js/common/components/PVFClock.js" charset="utf-8"></script>
    <style>
        .clocks {
            width: 300px;
            height: 300px;
            /*margin: 25px auto;*/
            position: relative;
        }
    </style>
</head>
<body>
    <div class="clocks">
        <canvas id="canvas" width="200" height="200"></canvas>
    </div>
    <form id="form1" runat="server">
        <div>
        </div>
    </form>
</body>
</html>
