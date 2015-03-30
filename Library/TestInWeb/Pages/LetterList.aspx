﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LetterList.aspx.cs" Inherits="TestInWeb.Pages.LetterList" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <asp:Panel runat="server" ID="pnlSearch">
        <input runat="server" id="Title" placeholder="标题" data-fieldname="Title LIKE"  />
        <input runat="server" id="FromName" placeholder="发送人" data-fieldname="FromName LIKE"  />
        <input runat="server" id="ToName" placeholder="收信人" data-fieldname="ToName LIKE"  />
        <input runat="server" id="BeginSendTime" placeholder="发送时间" data-fieldname="SendTime >="  />
        至
        <input runat="server" id="EndSendTime" placeholder="发送时间" data-fieldname="SendTime <="  />
    </asp:Panel>
    <asp:GridView runat="server" ID="gv" OnRowDataBound="gv_RowDataBound" OnDataBinding="gv_DataBinding" OnDataBound="gv_DataBound" OnRowCreated="gv_RowCreated">
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:Label runat="server" data-fieldname="Title" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:Label runat="server" data-fieldname="FromName" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:Label runat="server" data-fieldname="ToName" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:Label runat="server" data-fieldname="SendTime" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    </form>
</body>
</html>
