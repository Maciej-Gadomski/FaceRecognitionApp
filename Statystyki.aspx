<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Statystyki.aspx.cs" Inherits="FaceRecognitonApp.Statystyki" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <link href="/Style/StyleGadomski.css" rel="stylesheet" type="text/css"/>
    <title>Statystyki</title>

</head>
<body>
    <form id="form1" runat="server">
                <div >
            <div class="menu">
                <asp:Button ID="log" runat="server" Text="Wyloguj" Visible="True"  CssClass="buttonred" Font-Size="Large" OnClick="log_Click" Font-Bold="True" />
            </div>
        </div>
        <div class="logs">
            <asp:Panel ID="Panel1" runat="server" style="height:60vh; width:50vw; overflow:auto;" ScrollBars="Vertical"  >
            <asp:GridView ID="gridView" runat="server"  AutoGenerateColumns="false" AllowSorting="true" OnSelectedIndexChanged="gridView_SelectedIndexChanged" OnSorting="gridView_Sorting" OnRowDataBound="gridView_RowDataBound" PageSize="8">
                <Columns>
<asp:BoundField DataField="status" HeaderText="Status logowania" SortExpression="status" />
<asp:BoundField DataField="data" HeaderText="Data" SortExpression="data"/>
<asp:BoundField DataField="przyczyna" HeaderText="Przyczyna" SortExpression="przyczyna" />
                </Columns>
                <HeaderStyle BackColor="#6b696b" Font-Bold="true" Font-Size="XX-Large" ForeColor="White" />
            <RowStyle BackColor="#f5f5f5" />
            </asp:GridView>
                </asp:Panel>
        </div>
    </form>
</body>
</html>
