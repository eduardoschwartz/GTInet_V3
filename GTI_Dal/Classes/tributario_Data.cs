﻿using GTI_Models.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace GTI_Dal.Classes {
    public class Tributario_Data {
        public static int Plano = 0;

        private string _connection;

        public Tributario_Data(string sConnection) {
            _connection = sConnection;
        }
        
        public List<Lancamento> Lista_Lancamento() {
            using (var db = new GTI_Context(_connection)) {
                var Sql = from l in db.Lancamento orderby l.Descfull select l;
                return Sql.ToList();
            }
        }

        public List<Situacaolancamento> Lista_Status() {
            using (var db = new GTI_Context(_connection)) {
                var Sql = from s in db.Situacaolancamento orderby s.Descsituacao select s;
                return Sql.ToList();
            }
        }

        public List<Tributo> Lista_Tributo() {
            using (var db = new GTI_Context(_connection)) {
                var Sql = from l in db.Tributo orderby l.Desctributo select l;
                return Sql.ToList();
            }
        }

        public List<Tipolivro> Lista_Tipo_Livro() {
            using (var db = new GTI_Context(_connection)) {
                var Sql = from t in db.Tipolivro orderby t.Desctipo select t;
                return Sql.ToList();
            }
        }

        public Exception Incluir_Lancamento(Lancamento reg) {
            using (var db = new GTI_Context(_connection)) {
                int cntCod = (from c in db.Lancamento select c).Count();
                int maxCod = 1;
                if (cntCod > 0)
                    maxCod = (from c in db.Lancamento select c.Codlancamento).Max() + 1;

                try {
                    db.Database.ExecuteSqlCommand("INSERT INTO lancamento(codlancamento,descfull,descreduz,tipolivro) VALUES(@codlancamento,@descfull,@descreduz,@tipolivro)",
                        new SqlParameter("@codlancamento", Convert.ToInt16(maxCod)),
                        new SqlParameter("@descfull", reg.Descfull),
                        new SqlParameter("@descreduz", reg.Descreduz),
                        new SqlParameter("@tipolivro", reg.Tipolivro));
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Exception Incluir_Tributo(Tributo reg) {
            using (var db = new GTI_Context(_connection)) {
                int cntCod = (from c in db.Tributo select c).Count();
                int maxCod = 1;
                if (cntCod > 0)
                    maxCod = (from c in db.Tributo select c.Codtributo).Max() + 1;

                reg.Codtributo = Convert.ToInt16(maxCod);
//                db.Tributo.Add(reg);
                try {
                    db.Database.ExecuteSqlCommand("INSERT INTO tributo(codtributo,desctributo,abrevtributo,da) VALUES(@codtributo,@desctributo,@abrevtributo,@da)",
                        new SqlParameter("@codtributo", Convert.ToInt16(maxCod)),
                        new SqlParameter("@desctributo", reg.Desctributo),
                        new SqlParameter("@abrevtributo", reg.Abrevtributo),
                        new SqlParameter("@da", reg.Da));
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Exception Alterar_Lancamento(Lancamento reg) {
            using (var db = new GTI_Context(_connection)) {
                int nCodLanc = reg.Codlancamento;
                Lancamento b = db.Lancamento.First(i => i.Codlancamento == nCodLanc);
                b.Descfull = reg.Descfull;
                b.Descreduz = reg.Descreduz;
                b.Tipolivro = reg.Tipolivro;
                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Exception Alterar_Tributo(Tributo reg) {
            using (var db = new GTI_Context(_connection)) {
                int nCodTrib = reg.Codtributo;
                Tributo b = db.Tributo.First(i => i.Codtributo == nCodTrib);
                b.Desctributo = reg.Desctributo;
                b.Abrevtributo = reg.Abrevtributo;
                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public bool Existe_Lancamento(Lancamento reg,bool novo=true) {
            bool bValido = false;
            using (var db = new GTI_Context(_connection)) {
                if (novo) {
                    var existingReg = db.Lancamento.Count(a => a.Descfull == reg.Descfull);
                    if (existingReg > 0)
                        bValido = true;
                } else {
                    var existingReg = db.Lancamento.Count(a => a.Descfull == reg.Descfull && a.Codlancamento!=reg.Codlancamento);
                    if (existingReg > 0)
                        bValido = true;
                }
            }
            return bValido;
        }

        public bool Existe_Tributo(Tributo reg, bool novo = true) {
            bool bValido = false;
            using (var db = new GTI_Context(_connection)) {
                if (novo) {
                    var existingReg = db.Tributo.Count(a => a.Desctributo == reg.Desctributo);
                    if (existingReg > 0)
                        bValido = true;
                } else {
                    var existingReg = db.Tributo.Count(a => a.Desctributo == reg.Desctributo && a.Codtributo != reg.Codtributo);
                    if (existingReg > 0)
                        bValido = true;
                }
            }
            return bValido;
        }

        public Exception Excluir_Lancamento(Lancamento reg) {
            using (var db = new GTI_Context(_connection)) {
                int nCodLanc = reg.Codlancamento;
                Lancamento b = db.Lancamento.First(i => i.Codlancamento == nCodLanc);
                try {
                    db.Lancamento.Remove(b);
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Exception Excluir_Tributo(Tributo reg) {
            using (var db = new GTI_Context(_connection)) {
                int nCodTrib = reg.Codtributo;
                Tributo b = db.Tributo.First(i => i.Codtributo == nCodTrib);
                try {
                    db.Tributo.Remove(b);
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public bool Lancamento_uso_debito(int codigo_lancamento) {
            using (var db = new GTI_Context(_connection)) {
                Debitoparcela reg = (from c in db.Debitoparcela where c.Codlancamento==codigo_lancamento select c).FirstOrDefault();
                return reg!=null;
            }
        }

        public bool Lancamento_uso_tributo(int codigo_lancamento) {
            using (var db = new GTI_Context(_connection)) {
                Tributolancamento reg = (from c in db.Tributolancamento where c.Codlancamento == codigo_lancamento select c).FirstOrDefault();
                return reg != null;
            }
        }

        public bool Tributo_uso_debito(int codigo_tributo) {
            using (var db = new GTI_Context(_connection)) {
                Debitoparcela reg = (from c in db.Debitoparcela where c.Codlancamento == codigo_tributo select c).FirstOrDefault();
                return reg != null;
            }
        }

        public bool Tributo_uso_lancamento(int codigo_tributo) {
            using (var db = new GTI_Context(_connection)) {
                Tributolancamento reg = (from c in db.Tributolancamento  where c.Codtributo == codigo_tributo select c).FirstOrDefault();
                return reg != null;
            }
        }

        public bool Tributo_uso_aliquota(int codigo_tributo) {
            using (var db = new GTI_Context(_connection)) {
                Tributoaliquota reg = (from c in db.Tributoaliquota where c.Codtributo == codigo_tributo select c).FirstOrDefault();
                return reg != null;
            }
        }

        public List<SpExtrato> Lista_Extrato_Tributo(int Codigo, short Ano1 = 1990, short Ano2 = 2050, short Lancamento1 = 1, short Lancamento2 = 99, short Sequencia1 = 0, short Sequencia2 = 9999,
            short Parcela1 = 0, short Parcela2 = 999, short Complemento1 = 0, short Complemento2 = 999, short Status1 = 0, short Status2 = 99, DateTime? Data_Atualizacao = null, string Usuario = "") {
            using (var db = new GTI_Context(_connection)) {

                var prmCod1 = new SqlParameter { ParameterName = "@CodReduz1", SqlDbType = SqlDbType.Int, SqlValue = Codigo };
                var prmCod2 = new SqlParameter { ParameterName = "@CodReduz2", SqlDbType = SqlDbType.Int, SqlValue = Codigo };
                var prmAno1 = new SqlParameter { ParameterName = "@AnoExercicio1", SqlDbType = SqlDbType.SmallInt, SqlValue = Ano1 };
                var prmAno2 = new SqlParameter { ParameterName = "@AnoExercicio2", SqlDbType = SqlDbType.SmallInt, SqlValue = Ano2 };
                var prmLanc1 = new SqlParameter { ParameterName = "@CodLancamento1", SqlDbType = SqlDbType.SmallInt, SqlValue = Lancamento1 };
                var prmLanc2 = new SqlParameter { ParameterName = "@CodLancamento2", SqlDbType = SqlDbType.SmallInt, SqlValue = Lancamento2 };
                var prmSeq1 = new SqlParameter { ParameterName = "@SeqLancamento1", SqlDbType = SqlDbType.SmallInt, SqlValue = Sequencia1 };
                var prmSeq2 = new SqlParameter { ParameterName = "@SeqLancamento2", SqlDbType = SqlDbType.SmallInt, SqlValue = Sequencia2 };
                var prmPc1 = new SqlParameter { ParameterName = "@NumParcela1", SqlDbType = SqlDbType.SmallInt, SqlValue = Parcela1 };
                var prmPc2 = new SqlParameter { ParameterName = "@NumParcela2", SqlDbType = SqlDbType.SmallInt, SqlValue = Parcela2 };
                var prmCp1 = new SqlParameter { ParameterName = "@CodComplemento1", SqlDbType = SqlDbType.SmallInt, SqlValue = Complemento1 };
                var prmCp2 = new SqlParameter { ParameterName = "@CodComplemento2", SqlDbType = SqlDbType.SmallInt, SqlValue = Complemento2 };
                var prmSta1 = new SqlParameter { ParameterName = "@Status1", SqlDbType = SqlDbType.SmallInt, SqlValue = Status1 };
                var prmSta2 = new SqlParameter { ParameterName = "@Status2", SqlDbType = SqlDbType.SmallInt, SqlValue = Status2 };
                var prmDtA = new SqlParameter { ParameterName = "@DataNow", SqlDbType = SqlDbType.SmallDateTime, SqlValue = Data_Atualizacao == null ? DateTime.Now : Data_Atualizacao };
                var prmUser = new SqlParameter { ParameterName = "@Usuario", SqlDbType = SqlDbType.VarChar, SqlValue = Usuario };

                var result = db.SpExtrato.SqlQuery("spEXTRATONEW @CodReduz1, @CodReduz2, @AnoExercicio1 ,@AnoExercicio2 ,@CodLancamento1 ,@CodLancamento2, @SeqLancamento1 ,@SeqLancamento2, @NumParcela1, @NumParcela2, @CodComplemento1, @CodComplemento2, @Status1, @Status2, @DataNow, @Usuario ",
                    prmCod1, prmCod2, prmAno1, prmAno2, prmLanc1, prmLanc2, prmSeq1, prmSeq2, prmPc1, prmPc2, prmCp1, prmCp2, prmSta1, prmSta2, prmDtA, prmUser).ToList();

                List<SpExtrato> ListaDebito = new List<SpExtrato>();

                foreach (SpExtrato item in result) {
                    SpExtrato reg = new SpExtrato {
                        Anoexercicio = item.Anoexercicio,
                        Codlancamento = item.Codlancamento,
                        Desclancamento = item.Desclancamento,
                        Seqlancamento = item.Seqlancamento,
                        Numparcela = item.Numparcela,
                        Codcomplemento = item.Codcomplemento,
                        Datavencimento = item.Datavencimento,
                        Datadebase = item.Datadebase,
                        Statuslanc = item.Statuslanc,
                        Situacao = item.Situacao,
                        Datainscricao = item.Datainscricao,
                        Certidao = item.Certidao,
                        Numlivro = item.Numlivro,
                        Pagina = item.Pagina,
                        Numdocumento=item.Numdocumento,
                        Dataajuiza = item.Dataajuiza,
                        Valortributo = item.Valortributo,
                        Valormulta = item.Valormulta,
                        Valorjuros = item.Valorjuros,
                        Valorcorrecao = item.Valorcorrecao,
                        Valortotal = item.Valortotal,
                        Valorpago = item.Valorpago,
                        Valorpagoreal = item.Valorpagoreal,
                        Abrevtributo = item.Abrevtributo,
                        Codtributo = item.Codtributo
                    };
                    reg.Valortributo = item.Valortributo;
                    reg.Anoexecfiscal = item.Anoexecfiscal;
                    reg.Numexecfiscal = item.Numexecfiscal;
                    reg.Processocnj = item.Processocnj;
                    reg.Prot_certidao = item.Prot_certidao;
                    reg.Prot_dtremessa = item.Prot_dtremessa;
                    ListaDebito.Add(reg);
                }
                return ListaDebito;
            }
        }

        public List<SpExtrato> Lista_Extrato_Parcela(List<SpExtrato> Lista_Tributo) {
            List<SpExtrato> ListaDebito = new List<SpExtrato>();

            foreach (SpExtrato item in Lista_Tributo) {
                bool bFind = false;
                int x;
                for (x = 0; x < ListaDebito.Count; x++) {
                    SpExtrato itemArray = ListaDebito[x];
                    if (item.Anoexercicio == itemArray.Anoexercicio && item.Codlancamento == itemArray.Codlancamento && item.Seqlancamento == itemArray.Seqlancamento &&
                        item.Numparcela == itemArray.Numparcela && item.Codcomplemento == itemArray.Codcomplemento) {
                        bFind = true;
                        break;
                    }
                }
                if (!bFind) {
                    SpExtrato reg = new SpExtrato {
                        Anoexercicio = item.Anoexercicio,
                        Codlancamento = item.Codlancamento,
                        Desclancamento = item.Desclancamento,
                        Seqlancamento = item.Seqlancamento,
                        Numparcela = item.Numparcela,
                        Codcomplemento = item.Codcomplemento,
                        Datadebase = item.Datadebase,
                        Datavencimento = item.Datavencimento,
                        Statuslanc = item.Statuslanc,
                        Situacao = item.Situacao,
                        Datainscricao = item.Datainscricao,
                        Certidao = item.Certidao,
                        Numlivro = item.Numlivro,
                        Pagina = item.Pagina,
                        Dataajuiza = item.Dataajuiza,
                        Valortributo = item.Valortributo,
                        Valormulta = item.Valormulta,
                        Valorjuros = item.Valorjuros,
                        Valorcorrecao = item.Valorcorrecao,
                        Valortotal = item.Valortotal,
                        Valorpago = item.Valorpago,
                        Numdocumento=item.Numdocumento,
                        Valorpagoreal = item.Valorpagoreal,
                        Anoexecfiscal = item.Anoexecfiscal,
                        Numexecfiscal = item.Numexecfiscal,
                        Processocnj = item.Processocnj,
                        Prot_certidao = item.Prot_certidao,
                        Prot_dtremessa = item.Prot_dtremessa
                    };
                    ListaDebito.Add(reg);
                } else {
                    ListaDebito[x].Valortributo += item.Valortributo;
                    ListaDebito[x].Valormulta += item.Valormulta;
                    ListaDebito[x].Valorjuros += item.Valorjuros;
                    ListaDebito[x].Valorcorrecao += item.Valorcorrecao;
                    ListaDebito[x].Valortotal += item.Valortotal;
                }
            }

            return ListaDebito;

        }

        public double Retorna_Taxa_Expediente(int codigo,short ano,short lancamento,short sequencia,short parcela,short complemento) {
            double nTaxa=0;
            using (var db = new GTI_Context(_connection)) {
                var reg = (from d in db.Debitotributo where d.Codreduzido == codigo && d.Anoexercicio==ano && d.Codlancamento==lancamento && d.Seqlancamento==sequencia &&
                           d.Numparcela==parcela && d.Codcomplemento==complemento && d.Codtributo==3  select d.Valortributo).FirstOrDefault();
                if (reg == null)
                    nTaxa = 0;
                else
                    nTaxa = Convert.ToDouble(reg);
            }
            return nTaxa;
        }

        public List<ObsparcelaStruct> Lista_Observacao_Parcela(int codigo) {
                using (var db = new GTI_Context(_connection)) {
                var reg = from d in db.Obsparcela
                          join u in db.Usuario on d.Userid equals u.Id into du from u in du.DefaultIfEmpty()
                          where d.Codreduzido == codigo
                          orderby d.Codreduzido,d.Anoexercicio,d.Codlancamento,d.Seqlancamento,d.Numparcela,d.Codcomplemento,d.Data
                          select new ObsparcelaStruct {Codreduzido= d.Codreduzido,Anoexercicio=d.Anoexercicio,Codcomplemento=d.Codcomplemento,Codlancamento=d.Codlancamento,
                          Data=d.Data,NomeCompleto=u.Nomecompleto,NomeLogin=u.Nomelogin,Numparcela=d.Numparcela,Obs=d.Obs,Seq=d.Seq,Seqlancamento=d.Seqlancamento,Userid=d.Userid};
                return reg.ToList();
            }
        }

        public Exception Incluir_Observacao_Parcela(Obsparcela reg) {
            using (var db = new GTI_Context(_connection)) {
                int maxSeq = Retorna_Ultima_Seq_Observacao_Parcela(reg.Codreduzido,reg.Anoexercicio,reg.Codlancamento,reg.Seqlancamento,reg.Numparcela,reg.Codcomplemento);

                try {
                    db.Database.ExecuteSqlCommand("INSERT INTO obsparcela(codreduzido,anoexercicio,codlancamento,seqlancamento,numparcela,codcomplemento,seq,obs,userid,data) " +
                        "VALUES(@codreduzido, @anoexercicio, @codlancamento, @seqlancamento, @numparcela, @codcomplemento, @seq, @obs, @userid, @data  )",
                        new SqlParameter("@codreduzido", reg.Codreduzido),
                        new SqlParameter("@anoexercicio", reg.Anoexercicio),
                        new SqlParameter("@codlancamento", reg.Codlancamento),
                        new SqlParameter("@seqlancamento", reg.Seqlancamento),
                        new SqlParameter("@numparcela", reg.Numparcela),
                        new SqlParameter("@codcomplemento", reg.Codcomplemento),
                        new SqlParameter("@seq", maxSeq+1),
                        new SqlParameter("@obs", reg.Obs),
                        new SqlParameter("@userid", reg.Userid),
                        new SqlParameter("@data", reg.Data));
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Exception Alterar_Observacao_Parcela(Obsparcela reg) {
            using (var db = new GTI_Context(_connection)) {
                Obsparcela b = db.Obsparcela.First(i => i.Codreduzido == reg.Codreduzido && i.Anoexercicio==reg.Anoexercicio && i.Codlancamento==reg.Codlancamento &&
                i.Seqlancamento==reg.Seqlancamento && i.Numparcela==reg.Numparcela && i.Codcomplemento==reg.Codcomplemento && i.Seq==reg.Seq);
                b.Data = reg.Data;
                b.Obs = reg.Obs;
                b.Userid = reg.Userid;
                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public short Retorna_Ultima_Seq_Observacao_Parcela(int Codigo,int Ano,short Lanc, short Sequencia,short Parcela,short Complemento) {
            using (var db = new GTI_Context(_connection)) {
                var cntCod = (from c in db.Obsparcela
                              where c.Codreduzido == Codigo && c.Anoexercicio == Ano && c.Codlancamento == Lanc && c.Seqlancamento == Sequencia && c.Numparcela == Parcela && c.Codcomplemento == Complemento
                              orderby c.Seq descending
                              select c.Seq).FirstOrDefault();
                return Convert.ToInt16(cntCod);
            }
        }

        public Exception Excluir_Observacao_Parcela(Obsparcela reg) {
            using (var db = new GTI_Context(_connection)) {
                Obsparcela b = db.Obsparcela.First(i => i.Codreduzido == reg.Codreduzido && i.Anoexercicio == reg.Anoexercicio && i.Codlancamento == reg.Codlancamento && i.Seqlancamento == reg.Seqlancamento && i.Numparcela == reg.Numparcela && i.Codcomplemento == reg.Codcomplemento && i.Seq == reg.Seq);
                try {
                    db.Obsparcela.Remove(b);
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public List<DebitoobservacaoStruct> Lista_Observacao_Codigo(int codigo) {
            using (var db = new GTI_Context(_connection)) {
                var reg = from d in db.Debitoobservacao
                          join u in db.Usuario on d.Userid equals u.Id into du from u in du.DefaultIfEmpty()
                          where d.Codreduzido == codigo
                          orderby  d.Dataobs select new DebitoobservacaoStruct {Codreduzido=  d.Codreduzido,Dataobs= d.Dataobs,
                              Obs=d.Obs,Seq=d.Seq,Userid=d.Userid,NomeLogin=u.Nomelogin,NomeCompleto=u.Nomecompleto };
                return reg.ToList();
            }
        }

        public Exception Incluir_Observacao_Codigo(Debitoobservacao reg) {
            using (var db = new GTI_Context(_connection)) {
                int maxSeq = Retorna_Ultima_Seq_Observacao_Codigo(reg.Codreduzido);

                try {
                    db.Database.ExecuteSqlCommand("INSERT INTO debitoobservacao(codreduzido,seq,usuario,dataobs,obs) " +
                        "VALUES(@codreduzido, @seq, @userid, @dataobs, @obs)",
                        new SqlParameter("@codreduzido", reg.Codreduzido),
                        new SqlParameter("@seq", maxSeq + 1),
                        new SqlParameter("@userid", reg.Userid),
                        new SqlParameter("@dataobs", reg.Dataobs),
                        new SqlParameter("@obs", reg.Obs));
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public short Retorna_Ultima_Seq_Observacao_Codigo(int Codigo) {
            using (var db = new GTI_Context(_connection)) {
                var cntCod = (from c in db.Debitoobservacao where c.Codreduzido == Codigo orderby c.Seq descending select c.Seq).FirstOrDefault();
                return Convert.ToInt16(cntCod);
            }
        }

        public Exception Alterar_Observacao_Codigo(Debitoobservacao reg) {
            using (var db = new GTI_Context(_connection)) {
                Debitoobservacao b = db.Debitoobservacao.First(i => i.Codreduzido == reg.Codreduzido && i.Seq==reg.Seq);
                b.Dataobs = reg.Dataobs;
                b.Obs = reg.Obs;
                b.Userid = reg.Userid;
                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Exception Excluir_Observacao_Codigo(int Codigo,short Seq) {
            using (var db = new GTI_Context(_connection)) {
                Debitoobservacao b = db.Debitoobservacao.First(i => i.Codreduzido == Codigo &&  i.Seq == Seq);
                try {
                    db.Debitoobservacao.Remove(b);
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public List<Numdocumento> Lista_Parcela_Documentos(Parceladocumento reg) {
            using (var db = new GTI_Context(_connection)) {
                var Sql = (from d in db.Numdocumento
                           join p in db.Parceladocumento on d.numdocumento equals p.Numdocumento into dp1 from p in dp1.DefaultIfEmpty()
                           where p.Codreduzido==reg.Codreduzido && p.Anoexercicio==reg.Anoexercicio && p.Codlancamento==reg.Codlancamento && 
                           p.Seqlancamento==reg.Seqlancamento && p.Numparcela==reg.Numparcela && p.Codcomplemento==reg.Codcomplemento
                           orderby d.numdocumento
                           select new { d.numdocumento, d.Datadocumento, d.Valorguia }).ToList();
                List<Numdocumento> Lista = new List<Numdocumento>();
                foreach (var item in Sql) {
                    Numdocumento Linha = new Numdocumento();
                    Linha.numdocumento = item.numdocumento;
                    Linha.Datadocumento = item.Datadocumento;
                    Linha.Valorguia = item.Valorguia;
                    Lista.Add(Linha);
                }
                return Lista;
            }
        }

        //public List<ParceladocumentoStruct> Lista_Lancamento_Documentos(Parceladocumento reg) {
        //    using (var db = new GTI_Context(_connection)) {
        //        var Sql = (from d in db.Numdocumento
        //                   join p in db.Parceladocumento on d.numdocumento equals p.Numdocumento into dp1 from p in dp1.DefaultIfEmpty()
        //                   where p.Codreduzido == reg.Codreduzido && p.Anoexercicio == reg.Anoexercicio && p.Codlancamento == reg.Codlancamento &&
        //                   p.Seqlancamento == reg.Seqlancamento && p.Numparcela == reg.Numparcela && p.Codcomplemento == reg.Codcomplemento
        //                   orderby d.numdocumento
        //                   select new ParceladocumentoStruct{Documento= d.numdocumento, d.Datadocumento, d.Valorguia }).ToList();
        //        List<Numdocumento> Lista = new List<Numdocumento>();
        //        foreach (var item in Sql) {
        //            Numdocumento Linha = new Numdocumento();
        //            Linha.numdocumento = item.numdocumento;
        //            Linha.Datadocumento = item.Datadocumento;
        //            Linha.Valorguia = item.Valorguia;
        //            Lista.Add(Linha);
        //        }
        //        return Lista;
        //    }
        //}
        
        public Exception InsertBoletoGuia(Boletoguia Reg) {
            using (var db = new GTI_Context(_connection)) {
                try {
                    db.Boletoguia.Add(Reg);
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Exception Insert_Numero_Segunda_Via(Segunda_via_web Reg) {
            using (var db = new GTI_Context(_connection)) {
                try {
                    db.Segunda_via_web.Add(Reg);
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public List<Boletoguia> Lista_Boleto_Guia(int nSid) {
            List<Boletoguia> reg;
            using (var db = new GTI_Context(_connection)) {
                reg = (from b in db.Boletoguia where b.Sid == nSid select b).ToList();
                return reg;
            }
        }

        public List<Boleto> Lista_Boleto_DAM(int nSid) {
            List<Boleto> reg;
            using (var db = new GTI_Context(_connection)) {
                reg = (from b in db.Boleto where b.Sid == nSid select b).ToList();
                return reg;
            }
        }

        public List<DebitoStructure> Lista_Parcelas_CIP(int nCodigo, int nAno) {
            using (var db = new GTI_Context(_connection)) {
                var reg = (from dp in db.Debitoparcela
                           join dt in db.Debitotributo on new { p1 = dp.Codreduzido, p2 = dp.Anoexercicio, p3 = dp.Codlancamento, p4 = dp.Seqlancamento, p5 = dp.Numparcela, p6 = dp.Codcomplemento }
                                                   equals new { p1 = dt.Codreduzido, p2 = dt.Anoexercicio, p3 = dt.Codlancamento, p4 = dt.Seqlancamento, p5 = dt.Numparcela, p6 = dt.Codcomplemento } into dpdt from dt in dpdt.DefaultIfEmpty()
                           join pd in db.Parceladocumento on new { p1 = dp.Codreduzido, p2 = dp.Anoexercicio, p3 = dp.Codlancamento, p4 = dp.Seqlancamento, p5 = dp.Numparcela, p6 = dp.Codcomplemento }
                                                      equals new { p1 = pd.Codreduzido, p2 = pd.Anoexercicio, p3 = pd.Codlancamento, p4 = pd.Seqlancamento, p5 = pd.Numparcela, p6 = pd.Codcomplemento } into dppd from pd in dppd.DefaultIfEmpty()
                           where dp.Codreduzido == nCodigo && dp.Anoexercicio == nAno && dp.Codlancamento == 79 && dp.Seqlancamento == 0 && dp.Statuslanc == 3
                           orderby new { dp.Numparcela, dp.Codcomplemento }
                           select new { dp.Codreduzido, dp.Anoexercicio, dp.Codlancamento, dp.Seqlancamento, dp.Numparcela, dp.Codcomplemento, dp.Datavencimento, dt.Valortributo, pd.Numdocumento });

                List<DebitoStructure> Lista = new List<DebitoStructure>();
                foreach (var query in reg) {
                    foreach (DebitoStructure item in Lista) {
                        if (item.Numero_Parcela == query.Numparcela && item.Complemento == query.Codcomplemento)
                            goto Proximo;
                    }
                    DebitoStructure Linha = new DebitoStructure();
                    Linha.Codigo_Reduzido = query.Codreduzido;
                    Linha.Ano_Exercicio = query.Anoexercicio;
                    Linha.Codigo_Lancamento = query.Codlancamento;
                    Linha.Sequencia_Lancamento = query.Seqlancamento;
                    Linha.Numero_Parcela = query.Numparcela;
                    Linha.Complemento = query.Codcomplemento;
                    Linha.Soma_Principal = Convert.ToDecimal(query.Valortributo);
                    Linha.Data_Vencimento = query.Datavencimento;
                    Linha.Numero_Documento = query.Numdocumento;
                    Lista.Add(Linha);
                    Proximo:;
                }
                return Lista;
            }
        }

        public List<DebitoStructure> Lista_Parcelas_IPTU(int nCodigo, int nAno) {
            using (var db = new GTI_Context(_connection)) {
                var reg = (from dp in db.Debitoparcela
                           join dt in db.Debitotributo on new { p1 = dp.Codreduzido, p2 = dp.Anoexercicio, p3 = dp.Codlancamento, p4 = dp.Seqlancamento, p5 = dp.Numparcela, p6 = dp.Codcomplemento }
                                                   equals new { p1 = dt.Codreduzido, p2 = dt.Anoexercicio, p3 = dt.Codlancamento, p4 = dt.Seqlancamento, p5 = dt.Numparcela, p6 = dt.Codcomplemento } into dpdt from dt in dpdt.DefaultIfEmpty()
                           join pd in db.Parceladocumento on new { p1 = dp.Codreduzido, p2 = dp.Anoexercicio, p3 = dp.Codlancamento, p4 = dp.Seqlancamento, p5 = dp.Numparcela, p6 = dp.Codcomplemento }
                                                      equals new { p1 = pd.Codreduzido, p2 = pd.Anoexercicio, p3 = pd.Codlancamento, p4 = pd.Seqlancamento, p5 = pd.Numparcela, p6 = pd.Codcomplemento } into dppd from pd in dppd.DefaultIfEmpty()
                           where dp.Codreduzido == nCodigo && dp.Anoexercicio == nAno && dp.Codlancamento == 1 && dp.Seqlancamento == 0 && dp.Statuslanc == 3
                           orderby new { dp.Numparcela, dp.Codcomplemento }
                           select new { dp.Codreduzido, dp.Anoexercicio, dp.Codlancamento, dp.Seqlancamento, dp.Numparcela, dp.Codcomplemento, dp.Datavencimento, dt.Valortributo, pd.Numdocumento });

                List<DebitoStructure> Lista = new List<DebitoStructure>();
                foreach (var query in reg) {
                    foreach (DebitoStructure item in Lista) {
                        if (item.Numero_Parcela == query.Numparcela && item.Complemento == query.Codcomplemento)
                            goto Proximo;
                    }
                    DebitoStructure Linha = new DebitoStructure();
                    Linha.Codigo_Reduzido = query.Codreduzido;
                    Linha.Ano_Exercicio = query.Anoexercicio;
                    Linha.Codigo_Lancamento = query.Codlancamento;
                    Linha.Sequencia_Lancamento = query.Seqlancamento;
                    Linha.Numero_Parcela = query.Numparcela;
                    Linha.Complemento = query.Codcomplemento;
                    Linha.Soma_Principal = Convert.ToDecimal(query.Valortributo);
                    Linha.Data_Vencimento = query.Datavencimento;
                    Linha.Numero_Documento = query.Numdocumento;
                    Lista.Add(Linha);
                    Proximo:;
                }
                return Lista;
            }
        }

        public Exception Insert_Carne_Web(int Codigo, int Ano) {
            using (var db = new GTI_Context(_connection)) {
                try {
                    Laseriptu b = db.Laser_iptu.First(i => i.Codreduzido == Codigo && i.Ano == Ano);
                    b.Carne_web = 1;
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Exception Excluir_Carne(int nSid) {
            using (var db = new GTI_Context(_connection)) {
                try {
                    db.Boletoguia.RemoveRange(db.Boletoguia.Where(i => i.Sid == nSid));
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Laseriptu Carrega_Dados_IPTU(int nCodigo, int nAno) {
            using (var db = new GTI_Context(_connection)) {
                Laseriptu reg = (from l in db.Laser_iptu where l.Ano == nAno && l.Codreduzido == nCodigo select l).FirstOrDefault();
                return reg;
            }
        }

        public bool Existe_Documento_CIP(int nNumDocumento) {
            bool bRet = false;
            using (var db = new GTI_Context(_connection)) {
                var existingReg = db.Parceladocumento.Where(a => a.Codlancamento == 79).Count(a => a.Numdocumento == nNumDocumento);
                if (existingReg != 0) {
                    bRet = true;
                }
            }
            return bRet;
        }

        public int Retorna_Codigo_por_Documento(int nNumDocumento) {
            int Sql = 0;
            using (var db = new GTI_Context(_connection)) {
                Sql = (from b in db.Parceladocumento where  b.Numdocumento == nNumDocumento select b.Codreduzido).FirstOrDefault();
            }
            return Sql;
        }

        public bool IsRefis() {
            return false;
        }

        public bool IsRefisDI() {
            return false;
        }

        public int Insert_Documento(Numdocumento Reg) {
            Int32 maxCod = 0;
            using (var db = new GTI_Context(_connection)) {
                try {
                    maxCod = db.Numdocumento.Max(u => u.numdocumento);
                    maxCod++;
                    db.Database.ExecuteSqlCommand("INSERT INTO numdocumento(numdocumento,datadocumento,valorguia,emissor,registrado) VALUES(@numdocumento,@datadocumento,@valorguia,@emissor,@registrado)",
                        new SqlParameter("@numdocumento", maxCod),
                        new SqlParameter("@datadocumento", Reg.Datadocumento),
                        new SqlParameter("@valorguia", Reg.Valorguia),
                        new SqlParameter("@emissor", Reg.Emissor),
                        new SqlParameter("@registrado", Reg.Registrado));
                } catch (Exception ex) {
                    throw (ex.InnerException);
                }
                return maxCod;

            }
        }

        public Exception Insert_Parcela_Documento(Parceladocumento Reg) {
            using (var db = new GTI_Context(_connection)) {
                try {
                    db.Parceladocumento.Add(Reg);
                    db.SaveChanges();
                    
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Int32 Insert_Boleto_DAM(List<DebitoStructure> lstDebito, Int32 nNumDoc, DateTime DataBoleto) {
            int nSid = dalCore.GetRandomNumber(), nCodigo = lstDebito[0].Codigo_Reduzido, nNumero = 0, Pos = 0;
            string sInscricao = "", sNome = "", sQuadra = "", sLote = "", sCPF = "", sEndereco = "", sComplemento = "", sBairro = "", sCidade = "", sUF = "";
            decimal SomaPrincipal = 0, SomaTotal = 0;

            string sNumDoc = nNumDoc.ToString() + "-" + dalCore.RetornaDVDocumento(nNumDoc).ToString();
            string sNumDoc2 = nNumDoc.ToString() + dalCore.RetornaDVDocumento(nNumDoc).ToString();
            string sNumDoc3 = nNumDoc.ToString() + dalCore.Modulo11(nNumDoc.ToString("0000000000000")).ToString();

            dalCore.TipoContribuinte tpContribuinte = nCodigo < 100000 ?
                dalCore.TipoContribuinte.Imovel : nCodigo >= 100000 && nCodigo < 400000 ? dalCore.TipoContribuinte.Empresa : dalCore.TipoContribuinte.Cidadao;
            dalCore.LocalEndereco Local = nCodigo < 100000 ?
                dalCore.LocalEndereco.Imovel : nCodigo >= 100000 && nCodigo < 400000 ? dalCore.LocalEndereco.Empresa : dalCore.LocalEndereco.Cidadao;


            Sistema_Data sistema_Class = new Sistema_Data("GTIconnection");
            Contribuinte_Header_Struct regDados = sistema_Class.Contribuinte_Header(nCodigo,Local);
            sNome = regDados.Nome;
            sCPF = regDados.cpf_cnpj;
            sInscricao = regDados.Inscricao;
            sQuadra = "";
            sLote = "";
            sEndereco = regDados.endereco == null ? "" : regDados.endereco;
            nNumero = regDados.numero;
            sComplemento = regDados.complemento == null ? "" : regDados.complemento;
            sBairro = regDados.nome_bairro == null ? "" : regDados.nome_bairro;
            sCidade = regDados.nome_cidade == null ? "" : regDados.nome_cidade;
            sUF = regDados.nome_uf;

            DeleteSid(nSid);

            foreach (DebitoStructure reg in lstDebito) {
                SomaPrincipal += Convert.ToDecimal(reg.Soma_Principal);
                SomaTotal += Convert.ToDecimal(reg.Soma_Total);
            }

            StringBuilder sFullLanc = new StringBuilder();

            foreach (DebitoStructure Lanc in lstDebito) {
                if (sFullLanc.ToString().IndexOf(Lanc.Descricao_Lancamento) == -1) {
                    String DescLanc = Lanc.Descricao_Lancamento;
                    sFullLanc.Append(DescLanc + "/");
                }
            }
            sFullLanc.Remove(sFullLanc.Length - 1, 1);

            decimal nValorguia = Math.Truncate(Convert.ToDecimal(SomaTotal * 100));
            string NumBarra = dalCore.Gera2of5Cod((nValorguia).ToString(), Convert.ToDateTime(DataBoleto), Convert.ToInt32(nNumDoc), Convert.ToInt32(nCodigo));
            string numbarra2a = NumBarra.Substring(0, 13);
            string numbarra2b = NumBarra.Substring(13, 13);
            string numbarra2c = NumBarra.Substring(26, 13);
            string numbarra2d = NumBarra.Substring(39, 13);
            string strBarra = dalCore.Gera2of5Str(numbarra2a.Substring(0, 11) + numbarra2b.Substring(0, 11) + numbarra2c.Substring(0, 11) + numbarra2d.Substring(0, 11));
            string sBarra = dalCore.Mask(strBarra);
            
            foreach (DebitoStructure reg in lstDebito) {
                try {
                    Boleto regBoleto = new Boleto();
                    regBoleto.Usuario = "GTI.Web.Dam";
                    regBoleto.Computer = "Internet";
                    regBoleto.Sid = nSid;
                    regBoleto.Seq = Convert.ToInt16(Pos);
                    regBoleto.Inscricao = sInscricao;
                    regBoleto.Codreduzido = reg.Codigo_Reduzido.ToString();
                    regBoleto.Nome = dalCore.Truncate(sNome.Trim(), 37, "...");
                    regBoleto.Cpf = sCPF;
                    regBoleto.Endereco = dalCore.Truncate(sEndereco, 37, "...");
                    regBoleto.Numimovel = Convert.ToInt16(nNumero);
                    regBoleto.Complemento = dalCore.Truncate(sComplemento, 27, "...");
                    regBoleto.Bairro = dalCore.Truncate(sBairro, 27, "...");
                    regBoleto.Cidade = sCidade;
                    regBoleto.Uf = sUF;
                    regBoleto.Quadra = dalCore.Truncate(sQuadra, 15, "");
                    regBoleto.Lote = dalCore.Truncate(sLote, 10, "");
                    regBoleto.Fulllanc = dalCore.Truncate(sFullLanc.ToString(), 1997, "...");
                    regBoleto.Fulltrib = dalCore.Truncate(reg.Descricao_Tributo.Trim(), 1997, "...");
                    regBoleto.Numdoc = sNumDoc;
                    regBoleto.Datadam = Convert.ToDateTime(DataBoleto);
                    regBoleto.Nomefunc = "GTI.Web";
                    regBoleto.Anoexercicio = reg.Ano_Exercicio;
                    regBoleto.Codlancamento = Convert.ToInt16(reg.Codigo_Lancamento);
                    regBoleto.Seqlancamento = Convert.ToInt16(reg.Sequencia_Lancamento);
                    regBoleto.Numparcela = Convert.ToInt16(reg.Numero_Parcela);
                    regBoleto.Codcomplemento = Convert.ToInt16(reg.Complemento);
                    regBoleto.Datavencto = Convert.ToDateTime(reg.Data_Vencimento);
                    regBoleto.Aj = reg.Data_Ajuizamento == null || reg.Data_Ajuizamento == DateTime.MinValue ? "N" : "S";
                    regBoleto.Da = reg.Ano_Exercicio == DateTime.Now.Year ? "N" : "S";
                    regBoleto.Principal = Convert.ToDecimal(reg.Soma_Principal);
                    regBoleto.Juros = Convert.ToDecimal(reg.Soma_Juros);
                    regBoleto.Multa = Convert.ToDecimal(reg.Soma_Multa);
                    regBoleto.Correcao = Convert.ToDecimal(reg.Soma_Correcao);
                    regBoleto.Total = Convert.ToDecimal(reg.Soma_Total);
                    regBoleto.Numdoc2 = sNumDoc2;
                    regBoleto.Digitavel = "";
                    regBoleto.Codbarra = sBarra;
                    regBoleto.Valordam = SomaTotal;
                    regBoleto.Valorprincdam = SomaPrincipal;
                    regBoleto.Numbarra2a = numbarra2a;
                    regBoleto.Numbarra2b = numbarra2b;
                    regBoleto.Numbarra2c = numbarra2c;
                    regBoleto.Numbarra2d = numbarra2d;
                    GravaDam(regBoleto);
                    Pos++;
                } catch (SqlException ex) {
                    throw new Exception(ex.Message);
                } catch (Exception ex) {
                    throw new Exception(ex.Message);
                }
            }
            return nSid;
        }

        public void DeleteSid(int nSid) {
            using (var db = new GTI_Context(_connection)) {
                try {
                    db.Boletoguia.RemoveRange(db.Boletoguia.Where(i => i.Sid == nSid));
                    db.SaveChanges();
                    db.Boleto.RemoveRange(db.Boleto.Where(i => i.Sid == nSid));
                    db.SaveChanges();
                } catch (Exception ex) {
                    throw (ex.InnerException);
                }
            }
        }

        public void GravaDam(Boleto Reg) {
            using (var db = new GTI_Context(_connection)) {
                try {
                    db.Boleto.Add(Reg);
                    db.SaveChanges();
                    return;
                } catch (Exception ex) {
                    throw (ex.InnerException);
                }
            }
        }

        public bool Existe_Comercio_Eletronico(int nNumDocumento) {
            bool bRet = false;
            using (var db = new GTI_Context(_connection)) {
                var existingReg = db.Comercio_eletronico.Where(a => a.Numdoc > 1).Count(a => a.Numdoc == nNumDocumento);
                if (existingReg != 0) {
                    bRet = true;
                }
            }
            return bRet;
        }

        public Exception Insert_Boleto_Comercio_Eletronico(comercio_eletronico Reg) {
            using (var db = new GTI_Context(_connection)) {
                try {
                    db.Comercio_eletronico.Add(Reg);
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }


    }//end class
}
