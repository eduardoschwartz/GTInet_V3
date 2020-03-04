﻿using CrystalDecisions.CrystalReports.Engine;
using GTI_Bll.Classes;
using GTI_Models;
using GTI_Models.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;

namespace GTI_Web.Pages {
    public partial class certidaovalorvenal : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {
            String s = Request.QueryString["d"];
            if (s != "gti")
                Response.Redirect("~/Pages/gtiMenu.aspx");

        }

        protected void btPrint_Click(object sender, EventArgs e) {
            if (txtIM.Text == "")
                lblMsg.Text = "Digite o código do imóvel.";
            else {
                lblMsg.Text = "";
                int Codigo = Convert.ToInt32(txtIM.Text);
                Imovel_bll imovel_Class = new Imovel_bll("GTIconnection");
                bool ExisteImovel = imovel_Class.Existe_Imovel(Codigo);
                if (!ExisteImovel)
                    lblMsg.Text = "Imóvel não cadastrado.";
                else {
                    if (txtimgcode.Text != Session["randomStr"].ToString())
                        lblMsg.Text = "Código da imagem inválido";
                    else {
                        ImovelStruct _imovel = imovel_Class.Dados_Imovel(Codigo);
                        if (_imovel.Inativo == true)
                            lblMsg.Text = "O imóvel esta inativo!";
                        else
                            PrintReport(Codigo);

                    }
                }
            }
        }

        protected void ValidarButton_Click(object sender, EventArgs e) {
            string sCod = Codigo.Text;
            string sTipo = "";
            lblMsg.Text = "";
            int nPos = 0, nPos2 = 0, nCodigo = 0, nAno = 0, nNumero = 0;
            if (sCod.Trim().Length < 8)
                lblMsg.Text = "Código de validação inválido.";
            else {
                nPos = sCod.IndexOf("-");
                if (nPos < 6)
                    lblMsg.Text = "Código de validação inválido.";
                else {
                    nPos2 = sCod.IndexOf("/");
                    if (nPos2 < 5 || nPos - nPos2 < 2)
                        lblMsg.Text = "Código de validação inválido.";
                    else {
                        nCodigo = Convert.ToInt32(sCod.Substring(nPos2 + 1, nPos - nPos2 - 1));
                        nAno = Convert.ToInt32(sCod.Substring(nPos2 - 4, 4));
                        nNumero = Convert.ToInt32(sCod.Substring(0, 5));
                        if (nAno < 2010 || nAno > DateTime.Now.Year + 1)
                            lblMsg.Text = "Código de validação inválido.";
                        else {
                            sTipo = sCod.Substring(sCod.Length - 2, 2);
                            if (sTipo == "VV") {
                                Certidao_valor_venal dados = Valida_Dados(nNumero, nAno, nCodigo);
                                if (dados != null)
                                    Exibe_Certidao_ValorVenal(dados);
                                else
                                    lblMsg.Text = "Certidão não cadastrada.";
                            } else {
                                lblMsg.Text = "Código de validação inválido.";
                            }
                        }
                    }
                }
            }
        }

        private void PrintReport(int Codigo) {
            Imovel_bll imovel_Class = new Imovel_bll("GTIconnection");
            ImovelStruct Reg = imovel_Class.Dados_Imovel(Codigo);
            Laseriptu RegIPTU = imovel_Class.Dados_IPTU(Codigo,DateTime.Now.Year);

            Tributario_bll tributario_Class = new Tributario_bll("GTIconnection");
            SpCalculo RegCalculo = tributario_Class.Calculo_IPTU(Codigo, DateTime.Now.Year);
            if (Reg == null){
                lblMsg.Text = "Não foi possível emitir certidão para este código";
                return;
            }
            string sComplemento = string.IsNullOrWhiteSpace(Reg.Complemento) ? "" : " " + Reg.Complemento.ToString().Trim();
            string sQuadras = string.IsNullOrWhiteSpace(Reg.QuadraOriginal) ? "" : " Quadra: " + Reg.QuadraOriginal.ToString().Trim();
            string sLotes = string.IsNullOrWhiteSpace(Reg.LoteOriginal) ? "" : " Lote: " + Reg.LoteOriginal.ToString().Trim();
            sComplemento += sQuadras + sLotes;
            string sEndereco = Reg.NomeLogradouro + ", " + Reg.Numero.ToString() + sComplemento;
            string sBairro = Reg.NomeBairro;
            string sInscricao = Reg.Distrito.ToString() + "." + Reg.Setor.ToString("00") + "." + Reg.Quadra.ToString("0000") + "." + Reg.Lote.ToString("00000") + "." +
                Reg.Seq.ToString("00") + "." + Reg.Unidade.ToString("00") + "." + Reg.SubUnidade.ToString("000");
            List<ProprietarioStruct> Lista = imovel_Class.Lista_Proprietario(Codigo, true);
            string sNome = Lista[0].Nome;

            ReportDocument crystalReport = new ReportDocument();
            crystalReport.Load(Server.MapPath("~/Report/CertidaoValorVenal.rpt"));

            
            int _numero_certidao = tributario_Class.Retorna_Codigo_Certidao(modelCore.TipoCertidao.ValorVenal);
            int _ano_certidao = DateTime.Now.Year;

            Certidao_valor_venal cert = new Certidao_valor_venal();
            cert.Codigo = Codigo;
            cert.Ano = _ano_certidao;
            cert.Numero = _numero_certidao;
            cert.Data = DateTime.Now;
            cert.Inscricao = sInscricao;
            cert.Nomecidadao = sNome;
            cert.Logradouro = Reg.NomeLogradouro;
            cert.Li_num = Convert.ToInt32(Reg.Numero);
            cert.Li_compl = Reg.Complemento;
            cert.Descbairro = sBairro;
            cert.Li_quadras = Reg.QuadraOriginal;
            cert.Li_lotes = Reg.LoteOriginal;
            cert.Vvt = (decimal)RegCalculo.Vvt;
            cert.Vvp = (decimal)RegCalculo.Vvp;
            cert.Vvi = (decimal)RegCalculo.Vvi;
            cert.Areaterreno =(decimal) RegCalculo.Areaterreno;
            string _valor_metro = string.Format(CultureInfo.GetCultureInfo("pt-BR"), "R${0:#,###.##}", (cert.Vvt / cert.Areaterreno))+ " R$/m²";

            Exception ex = tributario_Class.Insert_Certidao_ValorVenal(cert);
            if (ex != null) {
                throw ex;
            } else {
                crystalReport.SetParameterValue("NUMCERTIDAO", _numero_certidao.ToString("00000") + "/" + _ano_certidao.ToString("0000"));
                crystalReport.SetParameterValue("DATAEMISSAO", DateTime.Now.ToString("dd/MM/yyyy") + " às " + DateTime.Now.ToString("HH:mm:ss"));
                crystalReport.SetParameterValue("CONTROLE", _numero_certidao.ToString("00000") + _ano_certidao.ToString("0000") + "/" + Codigo.ToString() + "-VV");
                crystalReport.SetParameterValue("ENDERECO", sEndereco);
                crystalReport.SetParameterValue("CADASTRO", Codigo.ToString("000000"));
                crystalReport.SetParameterValue("NOME", sNome);
                crystalReport.SetParameterValue("INSCRICAO", sInscricao);
                crystalReport.SetParameterValue("BAIRRO", sBairro);
                crystalReport.SetParameterValue("VVT", string.Format(CultureInfo.GetCultureInfo("pt-BR"), "R${0:0,000.00}", cert.Vvt) + " (" + _valor_metro +")");
                crystalReport.SetParameterValue("VVP", string.Format(CultureInfo.GetCultureInfo("pt-BR"), "R${0:0,000.00}", cert.Vvp));
                crystalReport.SetParameterValue("VVI", string.Format(CultureInfo.GetCultureInfo("pt-BR"), "R${0:0,000.00}", cert.Vvi));

                HttpContext.Current.Response.Buffer = false;
                HttpContext.Current.Response.ClearContent();
                HttpContext.Current.Response.ClearHeaders();

                try {
                    crystalReport.ExportToHttpResponse(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, HttpContext.Current.Response, true, "certidao" + _numero_certidao.ToString() + _ano_certidao.ToString());
                } catch {
                } finally {
                    crystalReport.Close();
                    crystalReport.Dispose();
                }
            }
        }

        private Certidao_valor_venal Valida_Dados(int Numero, int Ano, int Codigo) {
            Tributario_bll tributario_Class = new Tributario_bll("GTIconnection");
            Certidao_valor_venal dados = tributario_Class.Retorna_Certidao_ValorVenal(Ano, Numero, Codigo);
            return dados;
        }

        private void Exibe_Certidao_ValorVenal(Certidao_valor_venal dados) {
            lblMsg.Text = "";
            string sComplemento = dados.Li_compl;
            string sQuadras = string.IsNullOrWhiteSpace(dados.Li_quadras) ? "" : " Quadra: " + dados.Li_quadras.ToString().Trim();
            string sLotes = string.IsNullOrWhiteSpace(dados.Li_lotes) ? "" : " Lote: " + dados.Li_lotes.ToString().Trim();
            sComplemento += sQuadras + sLotes;
            string sEndereco = dados.Logradouro + ", " + dados.Numero.ToString() + sComplemento;

            Imovel_bll imovel_Class = new Imovel_bll("GTIconnection");
            Laseriptu RegIPTU = imovel_Class.Dados_IPTU(dados.Codigo, DateTime.Now.Year);
            if (RegIPTU == null) {
                lblMsg.Text = "Não foi possível emitir certidão para este código";
                return;
            }
            decimal Vvt = (decimal)RegIPTU.Vvt;
            decimal Vvp = (decimal)RegIPTU.Vvc;
            decimal Vvi = (decimal)RegIPTU.Vvi;
            decimal Areaterreno = (decimal)RegIPTU.Areaterreno;
            string _valor_metro = string.Format(CultureInfo.GetCultureInfo("pt-BR"), "R${0:#,###.##}", (Vvt / Areaterreno)) + " R$/m²";


            ReportDocument crystalReport = new ReportDocument();
            crystalReport.Load(Server.MapPath("~/Report/CertidaoValorVenalValida.rpt"));
            crystalReport.SetParameterValue("NUMCERTIDAO", dados.Numero.ToString("00000") + "/" + dados.Ano.ToString("0000"));
            crystalReport.SetParameterValue("DATAEMISSAO", dados.Data.ToString("dd/MM/yyyy") + " às " + dados.Data.ToString("HH:mm:ss"));
            crystalReport.SetParameterValue("CONTROLE", dados.Numero.ToString("00000") + dados.Ano.ToString("0000") + "/" + dados.Codigo.ToString() + "-VV");
            crystalReport.SetParameterValue("ENDERECO", sEndereco);
            crystalReport.SetParameterValue("CADASTRO", dados.Codigo.ToString("000000"));
            crystalReport.SetParameterValue("NOME", dados.Nomecidadao);
            crystalReport.SetParameterValue("INSCRICAO", dados.Inscricao);
            crystalReport.SetParameterValue("BAIRRO", dados.Descbairro);
            crystalReport.SetParameterValue("VVT", string.Format(CultureInfo.GetCultureInfo("pt-BR"), "R${0:#,###.##}", Vvt) + " (" + _valor_metro + ")");
            crystalReport.SetParameterValue("VVP", string.Format(CultureInfo.GetCultureInfo("pt-BR"), "R${0:#,###.##}", Vvp));
            crystalReport.SetParameterValue("VVI", string.Format(CultureInfo.GetCultureInfo("pt-BR"), "R${0:#,###.##}", Vvi));

            HttpContext.Current.Response.Buffer = false;
            HttpContext.Current.Response.ClearContent();
            HttpContext.Current.Response.ClearHeaders();

            try {
                crystalReport.ExportToHttpResponse(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, HttpContext.Current.Response, true, "comp" + dados.Numero.ToString() + dados.Ano.ToString() );
            } catch {
            } finally {
                crystalReport.Close();
                crystalReport.Dispose();
            }

        }
    }
}