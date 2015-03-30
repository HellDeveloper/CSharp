<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SendLetter.aspx.cs" Inherits="TestInWeb.Pages.SendLetter" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div style="width: 200px;">
            <div>
                <input runat="server" type="text" id="ToName" data-fieldname="ToName" placeholder="收信人" maxlength="49" />
            </div>
            <div style="text-align: center;">
                <input runat="server" type="text" id="Title" data-fieldname="Title" placeholder="标题" maxlength="49" />
            </div>
            <div>
                <textarea runat="server" id="Contents" placeholder="内容" data-fieldname="Contents" style=" text-indent: 10px; overflow: hidden;"></textarea>
            </div>
            <div style="text-align: right;">
                <input runat="server" type="text" id="FromName" data-fieldname="FromName" placeholder="发送人" maxlength="49" />
                <br />
                <input runat="server" type="text" id="SendTime" data-fieldname="SendTime" placeholder="发送时间" maxlength="49" />
            </div>
            <div>
                <asp:Button runat="server" ID="btnSend" OnClick="btnSend_Click" Text="发送" />
            </div>
        </div>
    </form>
</body>
</html>
