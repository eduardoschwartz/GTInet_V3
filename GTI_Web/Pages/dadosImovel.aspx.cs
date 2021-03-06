﻿using CrystalDecisions.CrystalReports.Engine;
using GTI_Bll.Classes;
using GTI_Models;
using GTI_Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using UIWeb;

namespace GTI_Web.Pages {
    public partial class dadosImovel : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {
            String s = Request.QueryString["d"];
            if (s != "gti")
                Response.Redirect("~/Pages/gtiMenu.aspx");

            if (!IsPostBack) {
                txtCNPJ.Text = "";
                txtIM.Text = "";
                lblMsg.Text = "";
            }
        }

        protected void optCNPJ_CheckedChanged(object sender, EventArgs e) {
            if (optCNPJ.Checked) {
                txtCPF.Visible = false;
                txtCNPJ.Visible = true;
                txtCPF.Text = "";
                txtCNPJ.Text = "";
                txtIM.Text = "";
            }
        }

        protected void optCPF_CheckedChanged(object sender, EventArgs e) {
            if (optCPF.Checked) {
                txtCPF.Visible = true;
                txtCNPJ.Visible = false;
                txtCPF.Text = "";
                txtCNPJ.Text = "";
                txtIM.Text = "";
            }
        }

        protected void txtCPF_TextChanged(object sender, EventArgs e) {
            txtCNPJ.Text = "";
        
        }

        protected void txtCNPJ_TextChanged(object sender, EventArgs e) {
            txtCPF.Text = "";
          
        }

        protected void btPrint_Click(object sender, EventArgs e) {
            lblMsg.Text = "";
            if (txtIM.Text =="") txtIM.Text = "0";
            int Codigo = Convert.ToInt32(txtIM.Text);

            Imovel_bll imovel_class = new Imovel_bll("GTIconnection");
            if (Convert.ToInt32(txtIM.Text)==0 && string.IsNullOrWhiteSpace(txtCNPJ.Text) && string.IsNullOrWhiteSpace(txtCPF.Text))
                lblMsg.Text = "Digite IM ou CPF ou CNPJ.";
            else {

                if (Convert.ToInt32(txtIM.Text) > 0 && (!string.IsNullOrWhiteSpace(txtCNPJ.Text) || !string.IsNullOrWhiteSpace(txtCPF.Text))) {

                } else {
                    lblMsg.Text = "Erro: Digite a inscrição municipal e o CPF ou o CNPJ do proprietário.";
                    return;
                }

                if (!imovel_class.Existe_Imovel(Codigo)) {
                    lblMsg.Text = "Erro: Cadastro inexistente.";
                    return;
                }

                if (optCPF.Checked && txtCPF.Text.Length < 14) {
                    lblMsg.Text = "CPF inválido!";
                    return;
                }
                if (optCNPJ.Checked && txtCNPJ.Text.Length < 18) {
                    lblMsg.Text = "CNPJ inválido!";
                    return;
                }

                string num_cpf_cnpj = "";
                Imovel_bll imovel_Class = new Imovel_bll("GTIconnection");
                List<ProprietarioStruct> _prop = imovel_Class.Lista_Proprietario(Codigo, true);
                string _cpfcnpj = _prop[0].CPF;

                if (optCPF.Checked) {
                    num_cpf_cnpj = gtiCore.RetornaNumero(txtCPF.Text);
                    if (!gtiCore.ValidaCpf(num_cpf_cnpj)) {
                        lblMsg.Text = "CPF inválido!";
                        return;
                    } else {
                        if (num_cpf_cnpj != _cpfcnpj) {
                            lblMsg.Text = "CPF informado não pertence a este imóvel!";
                            return;
                        }
                    }
                } else {
                    num_cpf_cnpj = gtiCore.RetornaNumero(txtCNPJ.Text);
                    if (!gtiCore.ValidaCNPJ(num_cpf_cnpj)) {
                        lblMsg.Text = "CNPJ inválido!";
                        return;
                    } else {
                        if (num_cpf_cnpj != _cpfcnpj) {
                            lblMsg.Text = "CNPJ informado não pertence a este imóvel!";
                            return;
                        }
                    }
                }
                Imprimir_Ficha(Convert.ToInt32(txtIM.Text));
            }
        }

        private void Imprimir_Ficha(int Codigo) {
            ReportDocument crystalReport = new ReportDocument();
            crystalReport.Load(Server.MapPath("~/Report/Dados_Imovel.rpt"));

            Imovel_bll imovel_Class = new Imovel_bll("GTIconnection");
            Tributario_bll tributario_Class = new Tributario_bll("GTIconnection");
            int _numero_certidao = tributario_Class.Retorna_Codigo_Certidao(modelCore.TipoCertidao.Ficha_Imovel);
            int _ano_certidao = DateTime.Now.Year;
            string _controle = _numero_certidao.ToString("00000") + _ano_certidao.ToString("0000") + "/" + Codigo.ToString() + "-FI";

            ImovelStruct _dados = imovel_Class.Dados_Imovel(Codigo);

            dados_imovel_web cert = new dados_imovel_web {
                Ano_Certidao=_ano_certidao,
                Numero_Certidao=_numero_certidao,
                Agrupamento = 0,
                Areaterreno = (decimal)_dados.Area_Terreno,
                Ativo = _dados.Inativo == true ? "INATIVO" : "ATIVO",
                Bairro = _dados.NomeBairro,
                Benfeitoria = _dados.Benfeitoria_Nome,
                Categoria = _dados.Categoria_Nome,
                Cep = _dados.Cep,
                Codigo = Codigo,
                Complemento = _dados.Complemento,
                Condominio = _dados.NomeCondominio=="NÃO CADASTRADO"?"":_dados.NomeCondominio,
                Controle = _controle,
                Endereco = _dados.NomeLogradouro,
                Imunidade = _dados.Imunidade == true ? "Sim" : "Não",
                Inscricao = _dados.Inscricao,
                Isentocip = _dados.Cip == true ? "Sim" : "Não",
                Lote = _dados.LoteOriginal,
                Mt = _dados.NumMatricula.ToString(),
                Numero = (int)_dados.Numero,
                Pedologia = _dados.Pedologia_Nome,
                Proprietario2 = "",
                Qtdeedif=0,
                Quadra=_dados.QuadraOriginal,
                Reside=(bool)_dados.ResideImovel?"Sim":"Não",
                Situacao=_dados.Situacao_Nome,
                Topografia=_dados.Topografia_Nome,
                Usoterreno=_dados.Uso_terreno_Nome
            };

            List<ProprietarioStruct> _prop = imovel_Class.Lista_Proprietario(Codigo,false);
            foreach (ProprietarioStruct item in _prop) {
                if(item.Tipo=="P" && item.Principal)
                    cert.Proprietario = item.Nome;
                else {
                    cert.Proprietario2 += item.Nome + "; ";
                }
            }

            List<AreaStruct> _areas = imovel_Class.Lista_Area(Codigo);
            cert.Qtdeedif = _areas.Count;

            
            SpCalculo _calculo = tributario_Class.Calculo_IPTU(Codigo, DateTime.Now.Year);
            cert.Vvc = _calculo.Vvp;
            cert.Vvt = _calculo.Vvt;
            cert.Vvi = _calculo.Vvi;
            cert.Iptu = _calculo.Valoriptu==0?_calculo.Valoritu:_calculo.Valoriptu;
            cert.Testada = _calculo.Testadaprinc;
            cert.Agrupamento = _calculo.Valoragrupamento;
            cert.Areapredial = _calculo.Areapredial;
            cert.Fracaoideal = _calculo.Fracao;
            cert.Somafator =  _calculo.Fgle * _calculo.Fped * _calculo.Fpro * _calculo.Fsit * _calculo.Ftop;

            Exception ex = imovel_Class.Insert_Dados_Imovel(cert);
            if (ex != null) {
                throw ex;
            } else {
                crystalReport.SetParameterValue("CODIGO", cert.Codigo.ToString("000000"));
                crystalReport.SetParameterValue("INSCRICAO", cert.Inscricao);
                crystalReport.SetParameterValue("SITUACAO", cert.Ativo);
                crystalReport.SetParameterValue("MT", cert.Mt);
                crystalReport.SetParameterValue("PROPRIETARIO", cert.Proprietario);
                crystalReport.SetParameterValue("CONTROLE", cert.Controle);
                crystalReport.SetParameterValue("PROPRIETARIO2", cert.Proprietario2);
                crystalReport.SetParameterValue("ENDERECO", cert.Endereco);
                crystalReport.SetParameterValue("NUMERO", cert.Numero);
                crystalReport.SetParameterValue("COMPLEMENTO", cert.Complemento);
                crystalReport.SetParameterValue("BAIRRO", cert.Bairro);
                crystalReport.SetParameterValue("CEP", cert.Cep);
                crystalReport.SetParameterValue("QUADRA", cert.Quadra);
                crystalReport.SetParameterValue("LOTE", cert.Lote);
                crystalReport.SetParameterValue("AREATERRENO", cert.Areaterreno);
                crystalReport.SetParameterValue("FRACAO", cert.Fracaoideal);
                crystalReport.SetParameterValue("TESTADA", cert.Testada);
                crystalReport.SetParameterValue("AGRUPAMENTO", cert.Agrupamento);
                crystalReport.SetParameterValue("FATORES", cert.Somafator);
                crystalReport.SetParameterValue("AREAPREDIAL", cert.Areapredial);
                crystalReport.SetParameterValue("IMUNIDADE", cert.Imunidade);
                crystalReport.SetParameterValue("RESIDE", cert.Reside);
                crystalReport.SetParameterValue("QTDEEDIF", cert.Qtdeedif);
                crystalReport.SetParameterValue("ISENTOCIP", cert.Isentocip);
                crystalReport.SetParameterValue("SITUACAO2", cert.Situacao);
                crystalReport.SetParameterValue("PEDOLOGIA", cert.Pedologia);
                crystalReport.SetParameterValue("TOPOGRAFIA", cert.Topografia);
                crystalReport.SetParameterValue("CATEGORIA", cert.Categoria);
                crystalReport.SetParameterValue("BENFEITORIA", cert.Benfeitoria);
                crystalReport.SetParameterValue("USOTERRENO", cert.Usoterreno);
                crystalReport.SetParameterValue("CONDOMINIO", cert.Condominio);
                crystalReport.SetParameterValue("VVT", cert.Vvt);
                crystalReport.SetParameterValue("VVI", cert.Vvi);
                crystalReport.SetParameterValue("VVP", cert.Vvc);
                crystalReport.SetParameterValue("IPTU", cert.Iptu);

                HttpContext.Current.Response.Buffer = false;
                HttpContext.Current.Response.ClearContent();
                HttpContext.Current.Response.ClearHeaders();

                try {
                    crystalReport.ExportToHttpResponse(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, HttpContext.Current.Response, true, "Ficha_Cadastral");
                } catch {
                } finally {
                    crystalReport.Close();
                    crystalReport.Dispose();
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
                            if (sTipo == "FI") {
                                dados_imovel_web dados = Valida_Certidao(nNumero, nAno, nCodigo);
                                if (dados != null)
                                    Exibe_Certidao(dados);
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

        private dados_imovel_web Valida_Certidao(int Numero, int Ano, int Codigo) {
            Tributario_bll tributario_Class = new Tributario_bll("GTIconnection");
            dados_imovel_web dados = tributario_Class.Retorna_Ficha_Imovel_Web(Ano, Numero, Codigo);
            return dados;
        }

        private void Exibe_Certidao(dados_imovel_web cert) {
            lblMsg.Text = "";

            ReportDocument crystalReport = new ReportDocument();
            crystalReport.Load(Server.MapPath("~/Report/Dados_Imovel.rpt"));
            crystalReport.SetParameterValue("CODIGO", cert.Codigo.ToString("000000"));
            crystalReport.SetParameterValue("INSCRICAO", cert.Inscricao);
            crystalReport.SetParameterValue("SITUACAO", cert.Ativo);
            crystalReport.SetParameterValue("MT", cert.Mt);
            crystalReport.SetParameterValue("PROPRIETARIO", cert.Proprietario);
            crystalReport.SetParameterValue("CONTROLE", cert.Controle);
            crystalReport.SetParameterValue("PROPRIETARIO2", cert.Proprietario2);
            crystalReport.SetParameterValue("ENDERECO", cert.Endereco);
            crystalReport.SetParameterValue("NUMERO", cert.Numero);
            crystalReport.SetParameterValue("COMPLEMENTO", cert.Complemento);
            crystalReport.SetParameterValue("BAIRRO", cert.Bairro);
            crystalReport.SetParameterValue("CEP", cert.Cep);
            crystalReport.SetParameterValue("QUADRA", cert.Quadra);
            crystalReport.SetParameterValue("LOTE", cert.Lote);
            crystalReport.SetParameterValue("AREATERRENO", cert.Areaterreno);
            crystalReport.SetParameterValue("FRACAO", cert.Fracaoideal);
            crystalReport.SetParameterValue("TESTADA", cert.Testada);
            crystalReport.SetParameterValue("AGRUPAMENTO", cert.Agrupamento);
            crystalReport.SetParameterValue("FATORES", cert.Somafator);
            crystalReport.SetParameterValue("AREAPREDIAL", cert.Areapredial);
            crystalReport.SetParameterValue("IMUNIDADE", cert.Imunidade);
            crystalReport.SetParameterValue("RESIDE", cert.Reside);
            crystalReport.SetParameterValue("QTDEEDIF", cert.Qtdeedif);
            crystalReport.SetParameterValue("ISENTOCIP", cert.Isentocip);
            crystalReport.SetParameterValue("SITUACAO2", cert.Situacao);
            crystalReport.SetParameterValue("PEDOLOGIA", cert.Pedologia);
            crystalReport.SetParameterValue("TOPOGRAFIA", cert.Topografia);
            crystalReport.SetParameterValue("CATEGORIA", cert.Categoria);
            crystalReport.SetParameterValue("BENFEITORIA", cert.Benfeitoria);
            crystalReport.SetParameterValue("USOTERRENO", cert.Usoterreno);
            crystalReport.SetParameterValue("CONDOMINIO", cert.Condominio);
            crystalReport.SetParameterValue("VVT", cert.Vvt);
            crystalReport.SetParameterValue("VVI", cert.Vvi);
            crystalReport.SetParameterValue("VVP", cert.Vvc);
            crystalReport.SetParameterValue("IPTU", cert.Iptu);


            HttpContext.Current.Response.Buffer = false;
            HttpContext.Current.Response.ClearContent();
            HttpContext.Current.Response.ClearHeaders();

            try {
                crystalReport.ExportToHttpResponse(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, HttpContext.Current.Response, true, "comp" + cert.Numero_Certidao.ToString() + cert.Ano_Certidao.ToString() + "_FI");
            } catch {
            } finally {
                crystalReport.Close();
                crystalReport.Dispose();
            }
        }
               
    }
}