﻿using GTI_Bll.Classes;
using GTI_Desktop.Classes;
using GTI_Desktop.Datasets;
using GTI_Models.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using static GTI_Models.modelCore;

namespace GTI_Desktop.Forms {
    public partial class Carta_Cobranca : Form {
        private string _connection = gtiCore.Connection_Name();
        private bool _stop = false;
        short _remessa = 1;

        public Carta_Cobranca() {
            InitializeComponent();
        }

        private void PrintButton_Click(object sender, EventArgs e) {
            PrintReport();
        }

        private void CalcularButton_Click(object sender, EventArgs e) {
            if (MessageBox.Show("Efetuar o cálculo das cartas de cobrança?", "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Question,MessageBoxDefaultButton.Button2) == DialogResult.No) return;

            if (CodigoInicio.Text == "") CodigoInicio.Text = "0";
            if (CodigoFinal.Text == "") CodigoFinal.Text = "0";
            int _codigo_ini = Convert.ToInt32(CodigoInicio.Text);
            int _codigo_fim = Convert.ToInt32(CodigoFinal.Text);
            if (_codigo_ini == 0 || _codigo_fim == 0)
                MessageBox.Show("Digite o código inicial e o código final.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else {
                if (_codigo_ini > _codigo_fim)
                    MessageBox.Show("Código inicial não pode ser maior que o código final.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else {
                    if (!gtiCore.IsDate(DataVencto.Text))
                        MessageBox.Show("Data de vencimento inválida.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else {
                        if (Convert.ToDateTime(DataVencto.Text) > DateTime.Now)
                            MessageBox.Show("Data de vencimento tem que ser menor que a data atual.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        else {
                            _stop = false;
                            gtiCore.Ocupado(this);
                            Gera_Matriz(_codigo_ini, _codigo_fim, Convert.ToDateTime(DataVencto.Text));
                            gtiCore.Liberado(this);
                            PrintReport();
                        }
                    }
                }
            }
        }

        private void CodigoInicioText_KeyPress(object sender, KeyPressEventArgs e) {
            const char Delete = (char)8;
            e.Handled = !Char.IsDigit(e.KeyChar) && e.KeyChar != Delete;
        }

        private void CodigoFinalText_KeyPress(object sender, KeyPressEventArgs e) {
            const char Delete = (char)8;
            e.Handled = !Char.IsDigit(e.KeyChar) && e.KeyChar != Delete;
        }

        private void Gera_Matriz(int _codigo_ini, int _codigo_fim, DateTime _data_vencto) {
            int _total = _codigo_fim - _codigo_ini + 1, _pos = 1, _numero_documento = 5108165; //5.100.001 até 5.400.000
            DateTime _data_vencimento = Convert.ToDateTime("15/10/2018");

            Exception ex = null;
            List<SpExtrato_carta> Lista_Resumo = new List<SpExtrato_carta>();
            List<SpExtrato_carta> Lista_Final = new List<SpExtrato_carta>();

            //Exclui a remessa se já existir
            Tributario_bll tributario_Class = new Tributario_bll(_connection);
            Sistema_bll sistema_Class = new Sistema_bll(_connection);
            Imovel_bll imovel_Class = new Imovel_bll(_connection);
            Empresa_bll empresa_Class = new Empresa_bll(_connection);
            Cidadao_bll cidadao_Class = new Cidadao_bll(_connection);

            List<int> _lista_codigos = tributario_Class.Lista_Codigo_Carta(_codigo_ini, _codigo_fim, _data_vencto);
            

            //PBar.Value = 0;
            //ex = tributario_Class.Excluir_Carta_Cobranca(_remessa);
            //if (ex != null) {
            //    ErrorBox eBox = new ErrorBox("Atenção", ex.Message, ex);
            //    eBox.ShowDialog();
            //}

            for (int _codigo_atual = _codigo_ini; _codigo_atual < _codigo_fim+1; _codigo_atual++) {

                if (_stop) break;
                if (_pos % 10 == 0) {
                    PBar.Value = _pos * 100 / _total;
                    PBar.Update();
                    Refresh();
                    Application.DoEvents();
                }

                bool bFind = false;
                for (int i = 0; i < _lista_codigos.Count; i++) {
                    if (_codigo_atual == _lista_codigos[i]) {
                        bFind = true;
                        break;
                    }
                }
                if (!bFind) goto Proximo;
                
                
                if (_codigo_atual > 100000 && _codigo_atual < 300000) {
                    if (empresa_Class.EmpresaSuspensa(_codigo_atual))
                        goto Proximo;
                }

                List<SpExtrato_carta> Lista_Extrato_Tributo = tributario_Class.Lista_Extrato_Tributo_Carta(Codigo: _codigo_atual,Data_Atualizacao:Convert.ToDateTime("15/10/2018"));
                if (Lista_Extrato_Tributo.Count == 0)
                    goto Proximo;

                List<SpExtrato_carta> Lista_Extrato_Parcela = tributario_Class.Lista_Extrato_Parcela_Carta(Lista_Extrato_Tributo);
                Lista_Resumo.Clear();
                foreach (SpExtrato_carta item in Lista_Extrato_Parcela) {
                    if ( item.Codlancamento != 20 && item.Datavencimento <= _data_vencto) {
                        Lista_Resumo.Add(item);
                    }
                }

                if (Lista_Resumo.Count == 0)
                    goto Proximo;

                Lista_Final.Clear();
                foreach (SpExtrato_carta item in Lista_Resumo) {
                    bool _find = false;
                    int _index = 0;
                    foreach (SpExtrato_carta item2 in Lista_Final) {
                        if (item.Anoexercicio == item2.Anoexercicio) {
                            _find = true;
                            break;
                        }
                        _index++;
                    }
                    if (_find) {
                        Lista_Final[_index].Valortributo += item.Valortributo;
                        Lista_Final[_index].Valorjuros += item.Valorjuros;
                        Lista_Final[_index].Valormulta += item.Valormulta;
                        Lista_Final[_index].Valorcorrecao += item.Valorcorrecao;
                        Lista_Final[_index].Valortotal += item.Valortotal;
                    } else {
                        SpExtrato_carta reg = new SpExtrato_carta();
                        reg.Codreduzido = item.Codreduzido;
                        reg.Anoexercicio = item.Anoexercicio;
                        reg.Valortributo = item.Valortributo;
                        reg.Valorjuros = item.Valorjuros;
                        reg.Valormulta = item.Valormulta;
                        reg.Valorcorrecao = item.Valorcorrecao;
                        reg.Valortotal = item.Valortotal;
                        Lista_Final.Add(reg);
                    }
                }
                if (Lista_Final.Count == 0)
                    goto Proximo;
                
                //Soma o boleto
                decimal _valor_boleto = 0;
                foreach (SpExtrato_carta item in Lista_Final) {
                    _valor_boleto +=  Convert.ToDecimal( string.Format("{0:0.00}",item.Valortotal));
                }

                //Dados contribuinte
                string _nome = "", _cpfcnpj = "", _endereco = "", _bairro = "", _cidade = "", _cep = "", _inscricao = "", _lote = "", _quadra = "", _atividade = "";
                string _convenio = "2873532", _complemento = "", _complemento_entrega = "", _endereco_entrega = "", _bairro_entrega = "", _cidade_entrega = "", _cep_entrega = "";
                
                

                Contribuinte_Header_Struct dados = sistema_Class.Contribuinte_Header(_codigo_atual);
                if (dados == null)
                    goto Proximo;
                TipoCadastro _tipo = dados.Tipo;
                _nome = dados.Nome;
                _cpfcnpj = dados.Cpf_cnpj;
                _inscricao = dados.Inscricao;
                _complemento = dados.Complemento == "" ? "" : " " + dados.Complemento;
                _endereco = dados.Endereco + ", " + dados.Numero.ToString() +  _complemento;
                _bairro = dados.Nome_bairro;
                _cidade = dados.Nome_cidade + "/" + dados.Nome_uf;
                _cep = dados.Cep;
                _lote = dados.Lote_original;
                _quadra = dados.Quadra_original;
                _atividade = dados.Atividade;
                if (_tipo==TipoCadastro.Empresa &&  !dados.Ativo)
                    goto Proximo;


                //Endereço de Entrega
                if (_tipo == TipoCadastro.Imovel) {
                    EnderecoStruct endImovel = imovel_Class.Dados_Endereco(_codigo_atual, dados.TipoEndereco);
                    _complemento_entrega = endImovel.Complemento == "" ? "" : " " + endImovel.Complemento;
                    _endereco_entrega = endImovel.Endereco + ", " + endImovel.Numero.ToString() + _complemento;
                    _bairro_entrega = endImovel.NomeBairro;
                    _cidade_entrega = endImovel.NomeCidade + "/" + endImovel.UF;
                    _cep_entrega = endImovel.Cep;
                } else {
                    if (_tipo == TipoCadastro.Empresa) {
                        mobiliarioendentrega endEmpresa = empresa_Class.Empresa_Endereco_entrega(_codigo_atual);
                        _complemento_entrega = endEmpresa.Complemento == "" ? "" : " " + endEmpresa.Complemento;
                        _endereco_entrega = endEmpresa.Nomelogradouro + ", " + endEmpresa.Numimovel.ToString() + _complemento;
                        _bairro_entrega = endEmpresa.Descbairro;
                        _cidade_entrega = endEmpresa.Desccidade + "/" + endEmpresa.Uf;
                        _cep_entrega = endEmpresa.Cep;
                        if(String.IsNullOrWhiteSpace( endEmpresa.Nomelogradouro)) {
                            _endereco_entrega = _endereco;
                            _bairro_entrega = _bairro;
                            _cidade_entrega = _cidade;
                            _cep_entrega = _cep;
                        }
                    } else {
                        if (_tipo == TipoCadastro.Cidadao) {
                            CidadaoStruct endCidadao = cidadao_Class.LoadReg(_codigo_atual);
                            if (endCidadao.EtiquetaR == "S" || endCidadao.EtiquetaR==null) {
                                _complemento_entrega = endCidadao.ComplementoR == "" ? "" : " " + endCidadao.ComplementoR;
                                _endereco_entrega = endCidadao.EnderecoR + ", " + endCidadao.NumeroR.ToString() + _complemento;
                                _bairro_entrega = endCidadao.NomeBairroR;
                                _cidade_entrega = endCidadao.NomeCidadeR + "/" + endCidadao.UfR;
                                _cep_entrega = endCidadao.CepR.ToString();
                            } else {
                                _complemento_entrega = endCidadao.ComplementoC == "" ? "" : " " + endCidadao.ComplementoC;
                                _endereco_entrega = endCidadao.EnderecoC + ", " + endCidadao.NumeroC.ToString() + _complemento;
                                _bairro_entrega = endCidadao.NomeBairroC;
                                _cidade_entrega = endCidadao.NomeCidadeC + "/" + endCidadao.UfC;
                                _cep_entrega = endCidadao.CepR.ToString();
                            }
                            _endereco = _endereco_entrega;
                            _bairro = _bairro_entrega;
                            _cidade = _cidade_entrega;
                            _cep = _cep_entrega;

                        }
                    }
                }

                string _cep_str = gtiCore.RetornaNumero(_cep_entrega);
                int _cep_numero = Convert.ToInt32(_cep_str);
                _cep_entrega = _cep_numero.ToString("00000-000");

                //Se não tiver CEP ou CPF grava exclusão e passa para o próximo
                if(string.IsNullOrWhiteSpace( _cpfcnpj) || string.IsNullOrWhiteSpace(_cep_entrega) || _cep_entrega=="00000-000" || _cep_entrega == "14870-000" || string.IsNullOrWhiteSpace(_endereco_entrega)) {
                    Carta_cobranca_exclusao regE = new Carta_cobranca_exclusao();
                    regE.Remessa = _remessa;
                    regE.Codigo = _codigo_atual;
                    ex = tributario_Class.Insert_Carta_Cobranca_Exclusao(regE);
                    if (ex != null) {
                        ErrorBox eBox = new ErrorBox("Atenção", ex.Message, ex);
                        eBox.ShowDialog();
                    }
                    goto Proximo;
                }

                //***** GERA CÓDIGO DE BARRAS BOLETO REGISTRADO*****
                DateTime _data_base = Convert.ToDateTime("07/10/1997");
                TimeSpan ts = _data_vencimento - _data_base;
                int _fator_vencto = ts.Days;
                string _quinto_grupo = String.Format("{0:D4}", _fator_vencto);
                string _valor_boleto_str = string.Format("{0:0.00}", _valor_boleto);
                _quinto_grupo += string.Format("{0:D10}", Convert.ToInt64(gtiCore.RetornaNumero(_valor_boleto_str)));
                string _barra = "0019" + _quinto_grupo + String.Format("{0:D13}", Convert.ToInt32(_convenio));
                _barra += String.Format("{0:D10}", _numero_documento) + "17";
                string _campo1 = "0019" + _barra.Substring(19, 5);
                string _digitavel = _campo1 + gtiCore.Calculo_DV10(_campo1).ToString();
                string _campo2 = _barra.Substring(23, 10);
                _digitavel += _campo2 + gtiCore.Calculo_DV10(_campo2).ToString();
                string _campo3 = _barra.Substring(33, 10);
                _digitavel += _campo3 + gtiCore.Calculo_DV10(_campo3).ToString();
                string _campo5 = _quinto_grupo;
                string _campo4 = gtiCore.Calculo_DV11(_barra).ToString();
                _digitavel += _campo4 + _campo5;
                _barra = _barra.Substring(0, 4) + _campo4 + _barra.Substring(4,_barra.Length-4)  ;
                //**Resultado final**
                string _linha_digitavel = _digitavel.Substring(0, 5) + "." + _digitavel.Substring(5, 5) + " " + _digitavel.Substring(10, 5) + "." + _digitavel.Substring(15, 6) + " ";
                _linha_digitavel += _digitavel.Substring(21, 5) + "." + _digitavel.Substring(26, 6) + " " + _digitavel.Substring(32, 1) + " " + gtiCore.StringRight(_digitavel, 14);
                string _codigo_barra = gtiCore.Gera2of5Str(_barra);
                //**************************************************
                
                //****** GRAVA HEADER **************
                Carta_cobranca Reg = new Carta_cobranca();
                Reg.Remessa = _remessa;
                Reg.Codigo = _codigo_atual;
                Reg.Parcela = 1;
                Reg.Total_Parcela = 1;
                Reg.Parcela_Label = Reg.Parcela.ToString("00") + "/" + Reg.Total_Parcela.ToString("00");
                Reg.Nome = _nome.Length > 50 ? _nome.Substring(0, 50) : _nome;
                Reg.Cpf_cnpj = _cpfcnpj;
                Reg.Endereco = _endereco;
                Reg.Bairro = _bairro ?? "";
                Reg.Cidade = _cidade ?? "";
                Reg.Cep = _cep ?? "";
                Reg.Endereco_Entrega = _endereco_entrega;
                Reg.Bairro_Entrega = _bairro_entrega ?? "";
                Reg.Cidade_Entrega = _cidade_entrega ?? "";
                Reg.Cep_Entrega = _cep_entrega;
                Reg.Data_Vencimento = _data_vencimento;
                Reg.Data_Documento = DateTime.Now;
                Reg.Inscricao = _inscricao;
                Reg.Lote = _lote.Length > 15 ? _lote.Substring(0, 15) : _lote;
                Reg.Quadra = _quadra.Length > 15 ? _quadra.Substring(0, 15) : _quadra;
                Reg.Atividade = _atividade.Length > 50 ? _atividade.Substring(0, 50) : _atividade;
                Reg.Numero_Documento = _numero_documento;
                Reg.Nosso_Numero = _convenio + _numero_documento.ToString("0000000000");
                Reg.Valor_Boleto = _valor_boleto;
                Reg.Digitavel = _linha_digitavel;
                Reg.Codbarra = _codigo_barra;
                Reg.Cep_entrega_cod = _cep_numero;

                ex = tributario_Class.Insert_Carta_Cobranca(Reg);
                if (ex != null) {
                    ErrorBox eBox = new ErrorBox("Atenção", ex.Message, ex);
                    eBox.ShowDialog();
                }
                

                //****** GRAVA DETALHE**************

                foreach (SpExtrato_carta item in Lista_Final) {
                    Carta_cobranca_detalhe RegDet = new Carta_cobranca_detalhe();
                    RegDet.Codigo = _codigo_atual;
                    RegDet.Remessa = _remessa;
                    RegDet.Ano = item.Anoexercicio;
                    RegDet.Parcela = 1;
                    RegDet.Principal = item.Valortributo;
                    RegDet.Juros = item.Valorjuros;
                    RegDet.Multa = item.Valormulta;
                    RegDet.Correcao = item.Valorcorrecao;
                    RegDet.Total = item.Valortotal;
                    ex = tributario_Class.Insert_Carta_Cobranca_Detalhe(RegDet);
                    if (ex != null) {
                        ErrorBox eBox = new ErrorBox("Atenção", ex.Message, ex);
                        eBox.ShowDialog();
                    }
                }

                //****** GRAVA DOCUMENTO ****************
                Numdocumento RegDoc = new Numdocumento();
                RegDoc.numdocumento = _numero_documento;
                RegDoc.Datadocumento = DateTime.Now;
                RegDoc.Valorguia = _valor_boleto;
                RegDoc.Emissor = "GTI";
                RegDoc.Registrado = true;
                ex = tributario_Class.Insert_Documento_Existente(RegDoc);
                if (ex != null) {
                    ErrorBox eBox = new ErrorBox("Atenção", ex.Message, ex);
                    eBox.ShowDialog();
                }

                //****** GRAVA PARCELA x DOCUMENTO*******

                foreach (SpExtrato_carta item in Lista_Resumo) {
                    Parceladocumento RegParc = new Parceladocumento();
                    RegParc.Codreduzido = item.Codreduzido;
                    RegParc.Anoexercicio = item.Anoexercicio;
                    RegParc.Codlancamento = item.Codlancamento;
                    RegParc.Seqlancamento = item.Seqlancamento;
                    RegParc.Numparcela = Convert.ToByte(item.Numparcela);
                    RegParc.Codcomplemento = item.Codcomplemento;
                    RegParc.Numdocumento = _numero_documento;

                    ex = tributario_Class.Insert_Parcela_Documento(RegParc);
                    if (ex != null) {
                        ErrorBox eBox = new ErrorBox("Atenção", ex.Message, ex);
                        eBox.ShowDialog();
                    }
                }

                _numero_documento++;
                //***************************************
                Proximo:;
                _pos++;
            }

            PBar.Value = 100;
            return;
        }
        
        private void Carta_Cobranca_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Escape) {
                if (MessageBox.Show("Deseja cancelar a rotina?", "Confirmação", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
                    _stop=true;
                }
            }
        }

        private void PrintReportOLD(List<SpExtrato>Lista_Debitos,List<int>Lista_Codigos) {
            gtiCore.Ocupado(this);
            dsCartaCobranca Ds = new dsCartaCobranca();
            dsCartaCobranca.CartaCobrancaTableDataTable dTable = new dsCartaCobranca.CartaCobrancaTableDataTable();
            dsCartaCobranca.CartaCobrancaHeaderTableDataTable dTableHeader = new dsCartaCobranca.CartaCobrancaHeaderTableDataTable();

            for (int i = 0; i < Lista_Codigos.Count; i++) {
                DataRow dRow = dTableHeader.NewRow();
                dRow["Codigo"] = Lista_Codigos[i];
                dRow["Grupo"] = 1;
                dRow["Nome"] = "Kelly Debby";
                dTableHeader.Rows.Add(dRow);
            }

            foreach (SpExtrato item in Lista_Debitos) {
                DataRow dRow = dTable.NewRow();
                dRow["Codigo"] = item.Codreduzido;
                dRow["Ano"] = item.Anoexercicio;
                dRow["Valor_Tributo"] = item.Valortributo;
                dRow["Valor_Juros"] = item.Valorjuros;
                dRow["Valor_Multa"] = item.Valormulta;
                dRow["Valor_Correcao"] =item.Valorcorrecao;
                dTable.Rows.Add(dRow);
            }
            Ds.Tables.RemoveAt(0);
            Ds.Tables.RemoveAt(0);
            Ds.Tables.Add(dTable);
            Ds.Tables.Add(dTableHeader);

            gtiCore.Liberado(this);
            ReportCR fRpt = new ReportCR("Carta_Cobranca_Envelope", null,Ds);
            
            fRpt.ShowDialog();

        }

        private void PrintReport() {
            ReportCR fRpt = new ReportCR("Carta_Cobranca_Envelope", null, null,_remessa);
            fRpt.ShowDialog();
        }

    }
}
