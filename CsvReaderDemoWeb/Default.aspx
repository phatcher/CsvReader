<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="CsvReaderDemoWeb._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Untitled Page</title>
</head>
<body>
	<form id="form1" runat="server">
	<div>
		<asp:Button ID="btnBind" runat="server" Text="Bind" OnClick="btnBind_Click" />
		<table>
			<asp:Repeater ID="rptSampleData" runat="server" OnItemDataBound="rptSampleData_ItemDataBound">
				<ItemTemplate>
					<tr>
						<td><asp:Label ID="lblFirstName" runat="server" /></td>
						<td><asp:Label ID="lblLastName" runat="server" /></td>
						<td><asp:Label ID="lblAddress" runat="server" /></td>
						<td><asp:Label ID="lblCity" runat="server" /></td>
						<td><asp:Label ID="lblState" runat="server" /></td>
						<td><asp:Label ID="lblZipCode" runat="server" /></td>
					</tr>
				</ItemTemplate>
			</asp:Repeater>
		</table>
	</div>
	</form>
</body>
</html>