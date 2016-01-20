<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Work.aspx.cs" Inherits="Work" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="DataTime_Source_lbl" runat="server" Text="Label"></asp:Label>
            <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
            <br />
            <br />
            <asp:Button ID="Work_btn" runat="server" Text="執行" OnClick="Work_btn_Click" />
            <br />
            <br />
            <asp:Button ID="Analysis" runat="server" Text="總分析" OnClick="Analysis_Click" />
            <br />
            <br />
            <asp:Button ID="weeklinetest" runat="server" Text="月測試" OnClick="WeekLine_Click" />
            <br />
            <br />
            <div>
                <asp:TextBox ID="FeedBack_txt" runat="server">輸入回測次數</asp:TextBox>
                <br />
                <asp:TextBox ID="Return_value" runat="server">輸入預期報酬率</asp:TextBox>
                <br />
                <asp:TextBox ID="NDay_Count" runat="server">輸入預計算N日最高價</asp:TextBox>
                <br />
                <asp:Button ID="FeedBack_Test" runat="server" OnClick="FeedBack_Test_Click" Text="月測試回測" />
                <br />
            </div>
            <br />
            <asp:Button ID="Clear_btn" runat="server" OnClick="Clear_btn_Click" Text="清除資料" />
            <br />
            <br />
        </div>
    </form>
</body>
</html>
