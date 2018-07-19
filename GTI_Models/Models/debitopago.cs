﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GTI_Models.Models {
    public class Debitopago {
        [Key]
        [Column(Order = 1)]
        public int Codreduzido { get; set; }
        [Key]
        [Column(Order = 2)]
        public short Anoexercicio { get; set; }
        [Key]
        [Column(Order = 3)]
        public short Codlancamento { get; set; }
        [Key]
        [Column(Order = 4)]
        public short Seqlancamento { get; set; }
        [Key]
        [Column(Order = 5)]
        public byte Numparcela { get; set; }
        [Key]
        [Column(Order = 6)]
        public byte Codcomplemento { get; set; }
        [Key]
        [Column(Order = 7)]
        public byte Seqpag { get; set; }
        public DateTime Datapagamento { get; set; }
        public DateTime Datarecebimento { get; set; }
        public decimal Valorpago { get; set; }
        public short? Codbanco { get; set; }
        public int? Codagencia { get; set; }
        public DateTime? Restituido { get; set; }
        public int? Numdocumento { get; set; }
        public decimal? Valorpagoreal { get; set; }
        public bool? Intacto { get; set; }
        public decimal? Valortarifa { get; set; }
        public string Arquivobanco { get; set; }
        public decimal? Valordif { get; set; }
        public DateTime? Datapagamentocalc { get; set; }
        public DateTime? Dataintegracao { get; set; }
        public string Contacorrente { get; set; }
    }

    public class Extrato_Pagamento {
        [Key]
        [Column(Order = 1)]
        public int Codigo { get; set; }
        [Key]
        [Column(Order = 2)]
        public short Ano { get; set; }
        [Key]
        [Column(Order = 3)]
        public short Lancamento_Codigo { get; set; }
        [Key]
        [Column(Order = 4)]
        public short Sequencia_Lancamento { get; set; }
        [Key]
        [Column(Order = 5)]
        public byte Parcela { get; set; }
        [Key]
        [Column(Order = 6)]
        public byte Complemento { get; set; }
        [Key]
        [Column(Order = 7)]
        public byte Sequencia_pagamento { get; set; }
        public string Lancamento_Descricao { get; set; }
        public short Tributo_Codigo { get; set; }
        public string Tributo_Descricao { get; set; }
        public DateTime Data_Vencimento { get; set; }
        public DateTime Data_Pagamento { get; set; }
        public DateTime Data_Recebimento { get; set; }
        public decimal Valor_Pago { get; set; }
        public short? Codigo_Banco { get; set; }
        public int? Codigo_Agencia { get; set; }
        public string Nome_Banco { get; set; }
        public int? Numero_Documento { get; set; }
    }

}
