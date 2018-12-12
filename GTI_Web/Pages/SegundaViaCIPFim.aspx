﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SegundaViaCIPFim.aspx.cs" Inherits="UIWeb.Pages.SegundaViaCIPFim" MasterPageFile="~/Pages/default.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MenuContentPlaceHolder" runat="server">
    <link href="../css/gti.css" rel="stylesheet" />


 




        <div style="color: #3a8dcc;">
            <br />
            &nbsp;<br />
            
            Emissão de segunda via da CIP - 2018<br />
            <br />
            <br />
            <asp:Label ID="Label6" runat="server" Font-Size="Small" Text="Código do imóvel:"></asp:Label>
&nbsp;<asp:Label ID="lblCod" runat="server" Font-Size="Small" ForeColor="#990000" Text="000000"></asp:Label>
            <br />
            <asp:Label ID="Label7" runat="server" Font-Size="Small" Text="Proprietário: "></asp:Label>
&nbsp;<asp:Label ID="lblNome" runat="server" Font-Size="Small" ForeColor="Maroon" Text="Label"></asp:Label>
            <br />
            <br />
            <asp:Button ID="btPrint" class="button1" runat="server" OnClick="btPrint_Click" Text="Imprimir" />
            <br />
            <br />
            <br />
            <asp:Label ID="lblMsg" runat="server" ForeColor="Red"></asp:Label>
            <br />
            <br />
            <asp:HyperLink ID="HyperLink1" runat="server" Font-Size="Small" NavigateUrl="~/Pages/SegundaViaIPTU.aspx">Emitir segunda via de outro imóvel</asp:HyperLink>
            <br />
        </div>
        <br />
    
</asp:Content>