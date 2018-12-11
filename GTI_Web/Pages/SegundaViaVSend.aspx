﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Pages/default.Master" AutoEventWireup="true" CodeBehind="SegundaViaVSend.aspx.cs" Inherits="GTI_Web.Pages.SegundaViaVSend" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MenuContentPlaceHolder" runat="server">
        <link href="../css/gti.css" rel="stylesheet" />


        <div style="color: #3a8dcc;">
            <br />
            &nbsp;<br />
            
            Emissão de segunda via do carnê de Vigilância Sanitária - 2019<br />
            <br />
            <br />
            <asp:Label ID="Label6" runat="server" Font-Size="Small" Text="Inscrição Municipal: "></asp:Label>
&nbsp;<asp:Label ID="lblCod" runat="server" Font-Size="Small" ForeColor="#990000" Text="000000"></asp:Label>
            <br />
            <asp:Label ID="Label7" runat="server" Font-Size="Small" Text="Razão Social: "></asp:Label>
&nbsp;<asp:Label ID="lblNome" runat="server" Font-Size="Small" ForeColor="Maroon" Text="Label"></asp:Label>
            <br />
            <br />
            <asp:Button ID="btPrint" class="button1" runat="server" OnClick="btPrint_Click" Text="Imprimir" Width="93px" />
            <br />
            <br />
            <br />
            <asp:Label ID="lblMsg" runat="server" ForeColor="Red"></asp:Label>
            <br />
            <br />
            <asp:HyperLink ID="HyperLink1" runat="server" Font-Size="Small" NavigateUrl="~/Pages/SegundaViaISS.aspx">Emitir segunda via de outra empresa</asp:HyperLink>
            <br />
            <br />
            <asp:HyperLink ID="HyperLink2" runat="server" Font-Size="Small" NavigateUrl="~/Pages/gtiMenu.aspx">Voltar ao menu principal</asp:HyperLink>
            <br />
        </div>
        <br />
</asp:Content>