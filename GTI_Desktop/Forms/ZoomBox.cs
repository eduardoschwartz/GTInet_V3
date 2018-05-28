﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GTI_Desktop.Forms {
    public partial class ZoomBox : Form {
        private Form FormCalled { get; set; }
        public string ReturnText;

        public ZoomBox(String Titulo, Form FormCalling, string Texto, bool ReadOnly) {
            InitializeComponent();
            this.Text = Titulo;
            this.Location = new Point(FormCalling.Location.X + (FormCalling.Width - this.Width) / 2, FormCalling.Location.Y + (FormCalling.Height - this.Height) + 55 / 2);
            FormCalled = FormCalling;
            txtZoom.Text = Texto;
            txtZoom.ReadOnly = ReadOnly;
            if (ReadOnly)
                tBar.Focus();
        }

        private void BtSair_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void ZoomBox_FormClosing(object sender, FormClosingEventArgs e) {
            ReturnText = txtZoom.Text;
        }
    }
}