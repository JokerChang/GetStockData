<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Source.aspx.cs" Inherits="Source" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:FileUpload ID="Source_FileUpload" runat="server" />
        <br />
        <br />
        <asp:TextBox ID="DataTime_Source_txt" runat="server"></asp:TextBox>
        <br />
        <br />
        <asp:Button ID="Source_btn" runat="server" Text="上傳" OnClick="Source_btn_Click" />
        <br />
        <br />
    
    </div>
    </form>
</body>
</html>
