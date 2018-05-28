﻿using GTI_Dal.Classes;
using GTI_Models.Models;
using System;
using System.Collections.Generic;

namespace GTI_Bll.Classes {
    public class Empresa_bll {
        private string _connection;
        public Empresa_bll(string sConnection) {
            _connection = sConnection;
        }

        /// <summary>
        /// Verifica se a empresa esta cadastrada
        /// </summary>
        /// <param name="nCodigo"></param>
        /// <returns></returns>
        public bool Existe_Empresa(int nCodigo) {
            Empresa_Data obj = new Empresa_Data(_connection);
            return obj.Existe_Empresa(nCodigo);

        }

        /// <summary>
        /// Retorna o cadastro da empresa
        /// </summary>
        /// <param name="Codigo"></param>
        /// <returns></returns>
        public EmpresaStruct Retorna_Empresa(int Codigo) {
            Empresa_Data obj = new Empresa_Data(_connection);
            return obj.Retorna_Empresa(Codigo);
        }

        /// <summary>
        /// Verifica se a empresa esta suspensa
        /// </summary>
        /// <param name="nCodigo"></param>
        /// <returns></returns>
        public bool EmpresaSuspensa(int nCodigo) {
            Empresa_Data obj = new Empresa_Data(_connection);
            return obj.Empresa_Suspensa(nCodigo);
        }

        /// <summary>
        /// Verifica se tem alguma empresa com o CNPJ informado
        /// </summary>
        /// <param name="sCNPJ"></param>
        /// <returns></returns>
        public int ExisteEmpresaCnpj(string sCNPJ) {
            Empresa_Data obj = new Empresa_Data(_connection);
            return obj.Existe_EmpresaCnpj(sCNPJ);
        }

        /// <summary>
        /// Verifica se tem alguma empresa com o CPF informado
        /// </summary>
        /// <param name="sCPF"></param>
        /// <returns></returns>
        public int ExisteEmpresaCpf(string sCPF) {
            Empresa_Data obj = new Empresa_Data(_connection);
            return obj.Existe_EmpresaCpf(sCPF);
        }

        /// <summary>
        /// Verifica se a empresa possui vigilância sanitária
        /// </summary>
        /// <param name="nCodigo"></param>
        /// <returns></returns>
        public bool Empresa_tem_VS(int nCodigo) {
            Empresa_Data obj = new Empresa_Data(_connection);
            return obj.Empresa_tem_VS(nCodigo);
        }

        /// <summary>
        /// Verifica se a empresa trem taxa de licença
        /// </summary>
        /// <param name="nCodigo"></param>
        /// <returns></returns>
        public bool Empresa_tem_TL(int nCodigo) {
            Empresa_Data obj = new Empresa_Data(_connection);
            return obj.Empresa_tem_TL(nCodigo);
        }

        /// <summary>
        /// Retorna o regime da empresa
        /// </summary>
        /// <param name="nCodigo"></param>
        /// <returns></returns>
        public string RegimeEmpresa(int nCodigo) {
            Empresa_Data obj = new Empresa_Data(_connection);
            return obj.Regime_Empresa(nCodigo);
        }

        /// <summary>
        /// Verifica se a empresa esta no Mei
        /// </summary>
        /// <param name="nCodigo"></param>
        /// <returns></returns>
        public bool Empresa_Mei(int nCodigo) {
            Empresa_Data obj = new Empresa_Data(_connection);
            return obj.Empresa_Mei(nCodigo);
        }

        /// <summary>
        /// Lista dos sócios de uma empresa
        /// </summary>
        /// <param name="nCodigo"></param>
        /// <returns></returns>
        public List<CidadaoStruct> ListaSocio(int nCodigo) {
            Empresa_Data obj = new Empresa_Data(_connection);
            return obj.Lista_Socio(nCodigo);
        }

        /// <summary>
        /// Lista dos horários de funcionamento
        /// </summary>
        /// <returns></returns>
        public List<Horariofunc> Lista_Horario() {
            Empresa_Data obj = new Empresa_Data(_connection);
            return obj.Lista_Horario();
        }

        /// <summary>
        /// Lista as placas dos veículos de uma empresa
        /// </summary>
        /// <param name="Codigo"></param>
        /// <returns></returns>
        public List<string> Lista_Placas(int Codigo) {
            Empresa_Data obj = new Empresa_Data(_connection);
            return obj.Lista_Placas(Codigo);
        }

        /// <summary>
        /// Retorna a lista de protocolos do VRE
        /// </summary>
        /// <param name="Codigo"></param>
        /// <returns></returns>
        public List<sil> Lista_Sil(int Codigo) {
            Empresa_Data obj = new Empresa_Data(_connection);
            return obj.Lista_Sil(Codigo);
        }

        /// <summary>
        /// Retorna True se a empresa for do simpels e false se nãop for.
        /// </summary>
        /// <param name="Codigo"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public bool Empresa_Simples(int Codigo, DateTime Data) {
            Empresa_Data obj = new Empresa_Data(_connection);
            return obj.Empresa_Simples(Codigo,Data);
        }

        /// <summary>
        /// Retorna o endereço de entrega da empresa
        /// </summary>
        /// <param name="Codigo"></param>
        /// <returns></returns>
        public mobiliarioendentrega Empresa_Endereco_entrega(int Codigo) {
            Empresa_Data obj = new Empresa_Data(_connection);
            return obj.Empresa_Endereco_entrega(Codigo);
        }

        /// <summary>
        /// Lista dos proprietários da empresa
        /// </summary>
        /// <param name="Codigo"></param>
        /// <returns></returns>
        public List<MobiliarioproprietarioStruct> Lista_Empresa_Proprietario(int Codigo) {
            Empresa_Data obj = new Empresa_Data(_connection);
            return obj.Lista_Empresa_Proprietario(Codigo);
        }

        /// <summary>
        /// Lista dos escritórios de contabilidade
        /// </summary>
        /// <returns></returns>
        public List<Escritoriocontabil> Lista_Escritorio_Contabil() {
            Empresa_Data obj = new Empresa_Data(_connection);
            return obj.Lista_Escritorio_Contabil();
        }

        /// <summary>
        /// Retorna os dados do escritório contabil
        /// </summary>
        /// <param name="Codigo"></param>
        /// <returns></returns>
        public EscritoriocontabilStruct Dados_Escritorio_Contabil(int Codigo) {
            Empresa_Data obj = new Empresa_Data(_connection);
            return obj.Dados_Escritorio_Contabil(Codigo);
        }

        /// <summary>
        /// Validação para gravação do escritório de contabilidade
        /// </summary>
        /// <param name="reg"></param>
        /// <returns></returns>
        public Exception Valida_Escritorio_Contabil(Escritoriocontabil reg) {
            Exception AppEx;
            if (String.IsNullOrWhiteSpace(reg.Nomeesc)) {
                AppEx = new Exception("Digite o nome do escritório");
                return AppEx;
            }

            if (String.IsNullOrWhiteSpace(reg.Cpf) && String.IsNullOrEmpty(reg.Cnpj)) {
                AppEx = new Exception("Digite o CPF ou CNPJ.");
                return AppEx;
            }

            if (!string.IsNullOrWhiteSpace(reg.Cpf) && !bllCore.ValidaCpf(reg.Cpf)) {
                AppEx = new Exception("CPF inválido.");
                return AppEx;
            }

            if (!string.IsNullOrWhiteSpace(reg.Cnpj) && !bllCore.ValidaCNPJ(reg.Cnpj)) {
                AppEx = new Exception("CNPJ inválido.");
                return AppEx;
            }

            if (String.IsNullOrWhiteSpace(reg.Nomelogradouro) && reg.Codlogradouro==0) {
                AppEx = new Exception("Digite o endereço do escritório.");
                return AppEx;
            }

            if (reg.Codcidade==0) {
                AppEx = new Exception("Selecione a cidade.");
                return AppEx;
            }

            if (String.IsNullOrWhiteSpace( reg.UF) ) {
                AppEx = new Exception("Selecione a UF.");
                return AppEx;
            }

            return null;
        }

        /// <summary>
        /// Incluir um novo escritório contábil
        /// </summary>
        /// <param name="reg"></param>
        /// <returns></returns>
        public Exception Incluir_escritorio(Escritoriocontabil reg) {
            Exception AppEx = Valida_Escritorio_Contabil(reg);
            if (AppEx != null) return AppEx;
            Empresa_Data obj = new Empresa_Data(_connection);
            Exception ex = obj.Incluir_escritorio(reg);
            return ex;
        }

        /// <summary>
        /// Alterar o cadastro de um escritório contábil
        /// </summary>
        /// <param name="reg"></param>
        /// <returns></returns>
        public Exception Alterar_escritorio(Escritoriocontabil reg) {
            Exception AppEx = Valida_Escritorio_Contabil(reg);
            if (AppEx != null) return AppEx;
            Empresa_Data obj = new Empresa_Data(_connection);
            Exception ex = obj.Alterar_escritorio(reg);
            return ex;
        }

        /// <summary>
        /// Último código de escritório cadastrado
        /// </summary>
        /// <returns></returns>
        public int Retorna_Ultimo_Codigo_Escritorio() {
            Empresa_Data obj = new Empresa_Data(_connection);
            return obj.Retorna_Ultimo_Codigo_Escritorio();
        }

        /// <summary>
        /// Verifica se alguma empresa esta usando o código de escritório
        /// </summary>
        /// <param name="id_cidadao"></param>
        /// <returns></returns>
        private Exception Escritorio_em_uso(int id_escritorio) {
            Exception AppEx = null;
            Empresa_Data obj = new Empresa_Data(_connection);
            bool bUso = obj.Empresa_Escritorio(id_escritorio);
            if (bUso)
                AppEx = new Exception("Exclusão não permitida. Escritório em uso nas empresas.");
            return AppEx;
        }

        /// <summary>
        /// Verifica se o escritório esta sendo utilizado em alguma empresa
        /// </summary>
        /// <param name="id_escritorio"></param>
        /// <returns></returns>
        public bool Empresa_Escritorio(int id_escritorio) {
            Empresa_Data obj = new Empresa_Data(_connection);
            return obj.Empresa_Escritorio(id_escritorio);
        }

        /// <summary>
        /// Excluir um escritório cadastrado
        /// </summary>
        /// <param name="Codigo"></param>
        /// <returns></returns>
        public Exception Excluir_Escritorio(int Codigo) {
            Empresa_Data obj = new Empresa_Data(_connection);
            Exception ex = Escritorio_em_uso(Codigo);
            if (ex == null)
                ex = obj.Excluir_Escritorio(Codigo);
            return ex;

        }


    }
}