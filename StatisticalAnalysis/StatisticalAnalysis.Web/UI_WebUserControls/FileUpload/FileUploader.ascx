<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FileUploader.ascx.cs" Inherits="WebUserContorls.Web.UI_WebUserControls.FileUpload.FileUploader" %>
<div id ="UploadUserControlDiv">
    <input id="fileToUpload0" type="file" name="fileToUpload" style ="width:300px; "  />
    <a href="#" class="easyui-linkbutton" data-options="iconCls:'icon-save',plain:false" onclick="AddFile();">上传文件</a>
    <%--<a href="#" class="easyui-linkbutton" data-options="iconCls:'icon-save',plain:false" onclick="return ajaxFileUpload();">上传文件</a>--%>

    <%--<table id="grid_FilesList" data-options="border:true" style ="width:540px;"></table>--%>
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

            var bb = typeof (ajaxFileUpload);
            if (typeof (ajaxFileUpload) != "function")
            {
                var m_Script = document.createElement("script");
                m_Script.id = 'UploadFileScript';
                m_Script.type = "text/javascript";
                m_Script.src = "/UI_WebUserControls/FileUpload/js/page/FileUpload.js";
                m_Script.charset = "utf-8";
                m_Head.appendChild(m_Script);
            }


        }
    });
    /////////////////////////由调用的父页面决定什么时候传值/////////////
    function SubScriptLoad(myFileGroup, myFileClassify, myUserId, myFolderPath, myPageName) {
        UserId = myUserId;
        FolderPath = myFolderPath;
        FileGroup = myFileGroup;
        FileClassify = myFileClassify;
        PageName = myPageName;
        ControlDiv = "UploadUserControlDiv";

        EmptyFileList();
        LoadFileList(false);
    }
</script>


