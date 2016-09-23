<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FileViewList.ascx.cs" Inherits="WebUserContorls.Web.UI_WebUserControls.FileUpload.FileViewList" %>
<div id ="ViewFileUserControlDiv">

</div>

<!--脚本-->
<script>
    $(document).ready(function () {
        var m_Head = document.getElementsByTagName('HEAD').item(0);
        if (m_Head) {

            if ($.ajaxFileUpload == undefined) {
                var m_ajaxfileuploadScript = document.createElement("script");
                m_ajaxfileuploadScript.id = 'ajaxfileuploadScript';
                m_ajaxfileuploadScript.type = "text/javascript";
                m_ajaxfileuploadScript.src = "/js/common/ajaxfileupload.js";
                m_ajaxfileuploadScript.charset = "utf-8";
                m_Head.appendChild(m_ajaxfileuploadScript);
            }
            var aa = typeof (ajaxFileUpload);
            if (typeof (ajaxFileUpload) != "function")
            {
                var m_Script = document.createElement("script");
                m_Script.id = 'ViewFilesScript';
                m_Script.type = "text/javascript";
                m_Script.src = "/UI_WebUserControls/FileUpload/js/page/FileUpload.js";
                m_Script.charset = "utf-8";
                m_Head.appendChild(m_Script);
            }
        }
    });
    /////////////////////////由调用的父页面决定什么时候传值/////////////
    function SubViewScriptLoad(myFileGroup, myFileClassify, myUserId, myFolderPath, myPageName) {
        UserId = myUserId;
        FolderPath = myFolderPath;
        FileGroup = myFileGroup;
        FileClassify = myFileClassify;
        PageName = myPageName;
        ControlDiv = "ViewFileUserControlDiv";
        EmptyFileList();
        LoadFileList(true);
    }
</script>