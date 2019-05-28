﻿using GTI_Bll.Classes;
using GTI_Desktop.Classes;
using GTI_Models.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using static GTI_Desktop.Classes.GtiTypes;

namespace GTI_Desktop.Forms {
    public partial class Divida_Ativa_Manual : Form {
        public int ReturnValue { get; set; }
        int _codigo, _UserId;
        string _connection = gtiCore.Connection_Name();
        string _connection_integrativa = gtiCore.Connection_Name("GTI_Integrativa");

        public Divida_Ativa_Manual(List<SpExtrato> Lista) {
            InitializeComponent();
            Sistema_bll sistema_Class = new Sistema_bll(_connection);
            Tributario_bll tributario_Class = new Tributario_bll(_connection);

            _UserId = sistema_Class.Retorna_User_LoginId(gtiCore.Retorna_Last_User());

            _codigo = Lista[0].Codreduzido;
            int _lanc = Convert.ToInt32(Lista[0].Desclancamento.Substring(0, 2));
            Tipolivro _tipo_livro = tributario_Class.Retorna_Tipo_Livro_Divida_Ativa(_lanc);
            if (_tipo_livro == null)
                LivroText.Text = "0 - LIVRO NÃO CADASTRADO";
            else
                LivroText.Text = _tipo_livro.Codtipo.ToString() + " - " + _tipo_livro.Desctipo;

            foreach (SpExtrato item in Lista) {
                _lanc = Convert.ToInt32(item.Desclancamento.Substring(0, 2));
                _tipo_livro = tributario_Class.Retorna_Tipo_Livro_Divida_Ativa(_lanc);

                ListViewItem lv = new ListViewItem(item.Anoexercicio.ToString());
                lv.SubItems.Add(item.Desclancamento);
                lv.SubItems.Add(item.Seqlancamento.ToString());
                lv.SubItems.Add(item.Numparcela.ToString());
                lv.SubItems.Add(item.Codcomplemento.ToString());
                lv.SubItems.Add(item.Datavencimento.ToString("dd/MM/yyyy"));
                lv.SubItems.Add(item.Valortributo.ToString("#0.00"));
                lv.SubItems.Add(_tipo_livro==null?"0":_tipo_livro.Codtipo.ToString());
                MainListView.Items.Add(lv);
            }
        }

        private void SairButton_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void OKButton_Click(object sender, EventArgs e) {
            List<int> _lista = new List<int>();
            foreach (ListViewItem linha in MainListView.Items) {
                int _tipo = Convert.ToInt32(linha.SubItems[7].Text);
                if (_lista.Count == 0)
                    _lista.Add(_tipo);
                else {
                    foreach (int item in _lista) {
                        if (item != _tipo) {
                            _lista.Add(item);
                            break;
                        }
                    }
                }
            }
            if (_lista.Count > 1)
                MessageBox.Show("Não é possível escrever em dívida ativa lançamentos que pertencem a livros diferemtes.", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

    }
}
