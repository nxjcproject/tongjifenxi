

var UserId = "";          //用户ID信息
var FolderPath = "";      //根目录路径
var FileGroup = "";       //组信息
var FileClassify = "";    //类别
var PageName = "";        //父页面的名称
var ControlDiv = "";      //用户自定义框架的ID
var UploadFileIdArray = [];

var FileTalbeListCount = 0;                     //文件列表的行数
var UploadFileIdIndex = 0;                      //上传控件索引，用户生成上传控件ID



///////////////////////////添加文件/////////////////////////
function AddFile() {
    ///////////////////////////处理需要上传文件的控件/////////////////////////
    //alert('#fileToUpload' + UploadFileIdIndex.toString());
    if ($('#fileToUpload' + UploadFileIdIndex.toString()).val() != "") {
        $('#fileToUpload' + UploadFileIdIndex.toString()).hide();
        var m_FileName = $('#fileToUpload' + UploadFileIdIndex.toString()).val();
        var m_RowId = 'FileRowItem' + UploadFileIdIndex.toString();
        AddToFileList(m_RowId, m_FileName, '', '','', 'new', false);
        UploadFileIdArray.push('fileToUpload' + UploadFileIdIndex);

        UploadFileIdIndex = UploadFileIdIndex + 1;

        ///////////////////////////构架新上传文件控件/////////////////////////////
        var m_NewFileDom = $('<input id="fileToUpload' + UploadFileIdIndex.toString() + '" type="file" name="fileToUpload" style = "width:300px; "></input>');
        m_NewFileDom.prependTo($('#' + ControlDiv));
    }
    else {
        alert("请选择要上传的文件!");
    }


}
////////////////////////增加文件列表//////////////////////
function AddToFileList(myFileItemId, myFileName, myFileUploadTime, myFilePath, myFileType, myFileFlag, myReadonly) {
    //////////////////////////////构造显示列表
    if (FileTalbeListCount == 0) {
        var m_FileListTable = $('<table id = "' + ControlDiv + 'FileListTable" class = "table" style = "font-size:9pt; margin-top:3px;"></table>');
        var m_FileListRowHtml = '<tr id = "' + ControlDiv + 'FileListTitleRow"><th style = "width:30px; height:20px; text-align:center;">' + '序号';
        m_FileListRowHtml = m_FileListRowHtml + '</th><th style = "width:260px; text-align:center;">' + '文件名称';
        m_FileListRowHtml = m_FileListRowHtml + '</th><th style = "width:120px; text-align:center;">' + '上传时间';
        m_FileListRowHtml = m_FileListRowHtml + '</th><th style = "width:50px; text-align:center;">' + '操作';
        m_FileListRowHtml = m_FileListRowHtml + '</th></tr>';
        var m_FileListRow = $(m_FileListRowHtml);
        m_FileListRow.appendTo(m_FileListTable);
        m_FileListTable.appendTo($("#" + ControlDiv));
    }
    var m_DownloadFileOpStr = "";
    var m_DeleteFileOpStr = "";
    if (myFileFlag == 'old') {                //当已上传的页面,则可以查看文件
        m_DownloadFileOpStr = 'onclick = "DownloadFileFun(\'' + myFileName + '\',\'' + myFilePath + '\');"';
        if (myReadonly == false) {
            m_DeleteFileOpStr = '<img class="iconImg" src = "/lib/extlib/themes/images/ext_icons/notes/note_delete.png" title="删除" onclick="DeleteFileFun(\'' + myFileItemId + '\',\'' + myFilePath + '\');"/>';
        }
        m_DeleteFileOpStr = m_DeleteFileOpStr + '<img class="iconImg" src = "/lib/extlib/themes/images/ext_icons/map/magnifier.png" title="查看" onclick="DownloadFileFun(\'' + myFileName + '\',\'' + myFilePath + '\',\'' + myFileType + '\');"/>';
    }
    else {
        m_DeleteFileOpStr = '<img class="iconImg" src = "/lib/extlib/themes/images/ext_icons/notes/note_delete.png" title="删除" onclick="DeleteTempFileFun(\'' + myFileItemId + '\',\'' + UploadFileIdIndex + '\');"/>';
    }

    var m_FileListRowHtml = '<tr id = "' + myFileItemId + '"><th class = "' + ControlDiv + 'RowIndexColumn" style = "height:20px;text-align:center;">'
    m_FileListRowHtml = m_FileListRowHtml + '</th><td style = "text-align:right;"' + m_DownloadFileOpStr + '>' + myFileName;
    m_FileListRowHtml = m_FileListRowHtml + '</td><td style = "text-align:right;">' + myFileUploadTime;
    m_FileListRowHtml = m_FileListRowHtml + '</td><td style = "text-align:left;">' + m_DeleteFileOpStr;
    m_FileListRowHtml = m_FileListRowHtml + '</td></tr>';
    var m_FileListRow = $(m_FileListRowHtml);
    $('#' + ControlDiv + 'FileListTitleRow').after(m_FileListRow);

    //////////////////////////////////////重新设置序号///////////////////////////////
    SetFileListRowIndex(ControlDiv);
    //////////////////////////////////////增加索引/////////////////////////////////// 
    FileTalbeListCount = FileTalbeListCount + 1;
    ///////////////////////////////新上传的
}
///////////////////////设置文件列表序号/////////////////////////
function SetFileListRowIndex() {
    $('.' + ControlDiv + 'RowIndexColumn').each(function (index) {
        $(this).empty();
        $(this).append((index + 1).toString());
    });
}
/////////////////下载已经上传到服务器的文件///////////////
function DownloadFileFun(myFileName, myFilePath, myFileType) {
    DownFileFun(myFileName, myFilePath, myFileType);
}
/////////////////删除刚选择将要上传的文件//////////////////
function DeleteTempFileFun(myFileItemId, myUploadFileIdIndex) {
    for (var i = 0; i < UploadFileIdArray.length; i++) {              //删除队列中的行和创建的元素
        if (UploadFileIdArray[i] == 'fileToUpload' + myUploadFileIdIndex) {
            $('#' + UploadFileIdArray[i]).remove();
            UploadFileIdArray.splice(i, 1);
            break;
        }
    }
    DeleteFileList(myFileItemId);
}

function DeleteFileList(myFileItemId) {
    $('#' + myFileItemId).remove();////////////删除列表中的行
    SetFileListRowIndex();
    FileTalbeListCount = FileTalbeListCount - 1;

    if (FileTalbeListCount == 0) {                //如果列表为零则删除表格
        $('#' + ControlDiv + 'FileListTable').remove();
    }
}
////////////////删除已保存到数据库中的文件/////////////////
function DeleteFileFun(myFileItemId, myFilePath) {
    var m_Parmaters = { "FunctionName": "DeleteFile", "FileId": myFileItemId, "FilePath": myFilePath, "FileClassify": FileClassify };
    $.ajax({
        type: "POST",
        url: PageName,
        data: m_Parmaters,
        //contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = msg;                    //jQuery.parseJSON(msg);
            if (m_MsgData.Message == '1') {
                alert("删除成功!");
                DeleteFileList(myFileItemId);          //删除表中的list
            }
            else if (m_MsgData.Message == '0') {
                alert("删除失败!");
            }
            else {
                alert(m_MsgData.Message);
            }
        }
    });
}
///////////////////////清空列表///////////////////////
function EmptyFileList() {
    for (var i = 0; i < UploadFileIdArray.length; i++) {              //删除所有上传文件生成的file控件
        $('#' + UploadFileIdArray[i]).remove();
    }
    UploadFileIdArray = [];          //清空数组
    FileTalbeListCount = 0;
    $('#' + ControlDiv + 'FileListTable').remove();    //清空列表
}
function ajaxFileUpload() {

    var m_Parmaters = { "FunctionName": "UploadFile", "UserId": UserId, "FileGroup": FileGroup, "FileClassify": FileClassify, "FolderPath": FolderPath };
    for (var i = 0; i < UploadFileIdArray.length; i++) {
        $.ajaxFileUpload(
        {
            url: PageName,                          // '/UI_WebUserControls/FileUpload/FileUploadHandler.ashx',
            secureuri: false,
            //fileElementId: 'fileToUpload',
            fileElementId: UploadFileIdArray[i],
            dataType: 'json',
            type: 'post',
            data: m_Parmaters,            //"{FilePath:'" + "fasdfasdf" + "'}",
            success: function (data, status) {
                //if (typeof (data.error) !== 'undefined') {
                //    if (data.error !== '') {
                //        alert(data.error);
                //    } else {
                //        alert(data.msg);
                //    }
                //}
            },
            error: function (data, status, e) {
                //alert(e);
            }
        })
    }
    EmptyFileList();         //先清空File列表
    //LoadFileList(false);          //再重新加载
}

function LoadFileList(myReadonly) {            //当myReadonly为true则表示只读，即只能查看
    var m_Parmaters = { "FunctionName": "GetFileList", "FileGroup": FileGroup, "FileClassify": FileClassify };
    $.ajax({
        type: "POST",
        url: PageName,
        data: m_Parmaters,
        //contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg) {
            var m_MsgData = msg['rows'];           //jQuery.parseJSON(msg);
            $(m_MsgData).each(function (index) {
                AddToFileList(this.FileItemId, this.FileName, this.CreateTime, this.FilePath, this.FileType, "old", myReadonly);
            });
        }
    });
}

function DownFileFun(myFileName, myFilePath, myFileType) {

    var m_FunctionName = "DownloadFile";

    var form = $('<form id="ExportFile" name="ExportFile"></form>');   //定义一个form表单
    //var form = $('#formMain');
    //form.attr('style', 'display:none');   //在form表单中添加查询参数
    form.attr('enctype', 'multipart/form-data');
    form.attr('target', '_self');
    form.attr('method', 'post');
    form.attr('action', "");

    var input_Method = $('<input></input>');
    input_Method.attr('type', 'hidden');
    input_Method.attr('name', 'FunctionName');
    input_Method.attr('value', m_FunctionName);
    var input_Data1 = $('<input></input>');
    input_Data1.attr('type', 'hidden');
    input_Data1.attr('name', 'FileName');
    input_Data1.attr('value', myFileName);
    var input_Data2 = $('<input></input>');
    input_Data2.attr('type', 'hidden');
    input_Data2.attr('name', 'FilePath');
    input_Data2.attr('value', myFilePath);
    var input_Data3 = $('<input></input>');
    input_Data3.attr('type', 'hidden');
    input_Data3.attr('name', 'FileType');
    input_Data3.attr('value', myFileType);

    

    form.append(input_Method);   //将查询参数控件提交到表单上
    form.append(input_Data1);   //将查询参数控件提交到表单上
    form.append(input_Data2);   //将查询参数控件提交到表单上
    form.append(input_Data3);   //将查询参数控件提交到表单上
    $('body').append(form);  //将表单放置在web中 

    form.submit();
    //释放生成的资源
    form.remove();
}
