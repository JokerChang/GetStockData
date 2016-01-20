<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PYS.aspx.cs" Inherits="Index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <br />
        <asp:FileUpload ID="PYS_FileUpload" runat="server" />
        <br />
        <asp:TextBox ID="DataTime_PYS_txt" runat="server"></asp:TextBox>
        <br />
        <asp:Button ID="GSD_btn" runat="server" Text="上傳" ValidateRequestMode="Inherit" OnClick="GSD_btn_Click" />
    </div>
    </form>
</body>
</html>
