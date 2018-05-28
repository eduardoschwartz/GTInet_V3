﻿using GTI_Models.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace GTI_Dal {
    public class GTI_Context :DbContext{
        public GTI_Context(string Connection_Name) : base(Connection_Name) { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            Database.SetInitializer<GTI_Context>(null);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Conventions.Remove<ManyToManyCascadeDeleteConvention>();
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Uf> Uf { get; set; }
        public DbSet<Cidade> Cidade { get; set; }
        public DbSet<Bairro> Bairro { get; set; }
        public DbSet<Cidadao> Cidadao { get; set; }
        public DbSet<Profissao> Profissao { get; set; }
        public DbSet<Pais> Pais { get; set; }
        public DbSet<Logradouro> Logradouro { get; set; }
        public DbSet<Cep> Cep { get; set; }
        public DbSet<Processogti> Processogti { get; set; }
        public DbSet<Anexo> Anexo { get; set; }
        public DbSet<Processo_historico> Processo_historico { get; set; }
        public DbSet<Processoend> Processoend { get; set; }
        public DbSet<Processodoc> Processodoc { get; set; }
        public DbSet<Processocidadao> Processocidadao { get; set; }
        public DbSet<Documento> Documento { get; set; }
        public DbSet<Despacho> Despacho { get; set; }
        public DbSet<Assunto> Assunto { get; set; }
        public DbSet<Centrocusto> Centrocusto { get; set; }
        public DbSet<Assuntocc> Assuntocc { get; set; }
        public DbSet<Assuntodoc> Assuntodoc { get; set; }
        public DbSet<Benfeitoria> Benfeitoria { get; set; }
        public DbSet<Categprop> Categprop { get; set; }
        public DbSet<Categconstr> Categconstr { get; set; }
        public DbSet<Pedologia> Pedologia { get; set; }
        public DbSet<Situacao> Situacao { get; set; }
        public DbSet<Topografia> Topografia { get; set; }
        public DbSet<Tipoconstr> Tipoconstr { get; set; }
        public DbSet<Usoterreno> Usoterreno { get; set; }
        public DbSet<Usoconstr> Usoconstr { get; set; }
        public DbSet<Mobiliario> Mobiliario { get; set; }
        public DbSet<Mobiliarioevento> Mobiliarioevento { get; set; }
        public DbSet<Horariofunc> Horariofunc { get; set; }
        public DbSet<Mobiliarioatividadevs2> Mobiliarioatividadevs2 { get; set; }
        public DbSet<Mobiliarioatividadeiss> Mobiliarioatividadeiss { get; set; }
        public DbSet<Mobiliarioproprietario> Mobiliarioproprietario { get; set; }
        public DbSet<Mei> Mei { get; set; }
        public DbSet<Usuariocc> Usuariocc { get; set; }
        public DbSet<Usuariofunc> Usuariofunc { get; set; }
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Tipousuario> Tipousuario { get; set; }
        public DbSet<Tramitacao> Tramitacao { get; set; }
        public DbSet<Tramitacaocc> Tramitacaocc { get; set; }
        public DbSet<Cadimob> Cadimob { get; set; }
        public DbSet<Proprietario> Proprietario { get; set; }
        public DbSet<Condominio> Condominio { get; set; }
        public DbSet<Condominioarea> CondominioArea { get; set; }
        public DbSet<Condominiounidade> CondominioUnidade { get; set; }
        public DbSet<Testadacondominio> Testadacondominio { get; set; }
        public DbSet<Facequadra> Facequadra { get; set; }
        public DbSet<Endentrega> Endentrega { get; set; }
        public DbSet<Testada> Testada { get; set; }
        public DbSet<Areas> Areas { get; set; }
        public DbSet<Lancamento> Lancamento { get; set; }
        public DbSet<Tipolivro> Tipolivro { get; set; }
        public DbSet<Debitoparcela> Debitoparcela { get; set; }
        public DbSet<Tributo> Tributo { get; set; }
        public DbSet<Tributolancamento> Tributolancamento { get; set; }
        public DbSet<Tributoaliquota> Tributoaliquota { get; set; }
        public DbSet<SpExtrato> SpExtrato { get; set; }
        public DbSet<Situacaolancamento> Situacaolancamento { get; set; }
        public DbSet<Debitotributo> Debitotributo { get; set; }
        public DbSet<Obsparcela> Obsparcela { get; set; }
        public DbSet<Debitoobservacao> Debitoobservacao { get; set; }
        public DbSet<Numdocumento> Numdocumento { get; set; }
        public DbSet<Parceladocumento> Parceladocumento { get; set; }
        public DbSet<security_event> Security_event { get; set; }
        public DbSet<mobiliarioplaca> Mobiliarioplaca { get; set; }
        public DbSet<sil> Sil { get; set; }
        public DbSet<mobiliarioendentrega> Mobiliarioendentrega { get; set; }
        public DbSet<Escritoriocontabil> Escritoriocontabil { get; set; }

    }
}