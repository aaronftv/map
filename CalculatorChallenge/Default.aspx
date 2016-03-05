<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CalculatorChallenge.Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:TextBox ID="inputTbx" runat="server"></asp:TextBox>&nbsp;
        <asp:Button ID="calculateBtn" runat="server" Text="Calculate" OnClick="calculateBtn_Click" />   
    </div>
    </form>
</body>
</html>
