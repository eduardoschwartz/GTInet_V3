﻿using GTI_Dal;
using GTI_Models.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace GTI_Dal.Classes {
    public class Sistema_Data {

        private string _connection;
        public Sistema_Data(string sConnection) {
            _connection = sConnection;
        }

        public Contribuinte_Header_Struct Contribuinte_Header(int Codigo, dalCore.LocalEndereco Tipo) {
            Contribuinte_Header_Struct reg = new Contribuinte_Header_Struct {
                Codigo = Codigo
            };
            if (Tipo == dalCore.LocalEndereco.Imovel) {
                Imovel_Data imovel_Class = new Imovel_Data(_connection);
                List<ProprietarioStruct> ListaProp = imovel_Class.Lista_Proprietario(Codigo, true);
                reg.Nome = ListaProp[0].Nome;
                reg.cpf_cnpj = ListaProp[0].CPF;
                ImovelStruct RegImovel = imovel_Class.Dados_Imovel(Codigo);
                reg.Inscricao = RegImovel.Inscricao;
                reg.endereco = RegImovel.NomeLogradouro;
                reg.numero = (short)RegImovel.Numero;
                reg.complemento = RegImovel.Complemento;
                reg.nome_bairro = RegImovel.NomeBairro;
                reg.nome_cidade = "JABOTICABAL";
                reg.nome_uf = "SP";
                reg.cep = RegImovel.Cep;
            } else if (Tipo == dalCore.LocalEndereco.Empresa) {
                Empresa_Data clsEmpresa = new Empresa_Data(_connection);
                EmpresaStruct regEmpresa = clsEmpresa.Retorna_Empresa(Codigo);
                reg.Nome = regEmpresa.Razao_social;
                reg.cpf_cnpj = regEmpresa.Cpf_cnpj;
                reg.endereco = regEmpresa.Endereco_nome;
                reg.numero = (short)regEmpresa.Numero;
                reg.complemento = regEmpresa.Complemento;
                reg.nome_bairro = regEmpresa.Bairro_nome;
                reg.nome_cidade = regEmpresa.Cidade_nome;
                reg.nome_uf = regEmpresa.UF;
                reg.cep = regEmpresa.Cep;
            } else {
                Cidadao_Data clsCidadao = new Cidadao_Data(_connection);
                Cidadao regCidadao = clsCidadao.Retorna_Cidadao(Codigo);
                reg.Nome = regCidadao.Nomecidadao;
                reg.cpf_cnpj = regCidadao.Cpf;
                reg.endereco = regCidadao.Nomelogradouro;
                reg.numero = (short)regCidadao.Numimovel;
                reg.complemento = regCidadao.Complemento;
                reg.nome_bairro = regCidadao.Nomebairro;
                reg.nome_cidade = regCidadao.Nomecidade;
                reg.nome_uf = regCidadao.Siglauf;
                reg.cep = regCidadao.Cep.ToString();
            }

            return reg;
        }

        #region Security

        public Exception Alterar_Senha(Usuario reg) {
            using (var db = new GTI_Context(_connection)) {
                string sLogin = reg.Nomelogin;
                Usuario b = db.Usuario.First(i => i.Nomelogin == sLogin);
                b.Senha2 = reg.Senha2;
                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public int? Retorna_Ultimo_Codigo_Usuario() {
            using (var db = new GTI_Context(_connection)) {
                var Sql = (from c in db.Usuario orderby c.Id descending select c.Id).FirstOrDefault();
                return Sql;
            }
        }

        public string Retorna_User_FullName(string loginName) {
            using (var db = new GTI_Context(_connection)) {
                string Sql = (from u in db.Usuario where u.Nomelogin == loginName select u.Nomecompleto).FirstOrDefault();
                return Sql;
            }
        }

        public string Retorna_User_LoginName(string fullName) {
            using (var db = new GTI_Context(_connection)) {
                string Sql = (from u in db.Usuario where u.Nomecompleto == fullName select u.Nomelogin).FirstOrDefault();
                return Sql;
            }
        }

        public string Retorna_User_LoginName(int idUser) {
            using (var db = new GTI_Context(_connection)) {
                string Sql = (from u in db.Usuario where u.Id == idUser select u.Nomelogin).FirstOrDefault();
                return Sql;
            }
        }

        public int Retorna_User_LoginId(string loginName) {
            using (var db = new GTI_Context(_connection)) {
                int Sql = (from u in db.Usuario where u.Nomelogin == loginName select u.Id).FirstOrDefault();
                return Sql;
            }
        }

        public string Retorna_User_Password(string login) {
            using (var db = new GTI_Context(_connection)) {
                string Sql = (from u in db.Usuario where u.Nomelogin == login select u.Senha2).FirstOrDefault();
                return Sql;
            }
        }

        public List<security_event> Lista_Sec_Eventos() {
            using (var db = new GTI_Context(_connection)) {
                var reg = (from t in db.Security_event orderby t.Id select t).ToList();
                List<security_event> Lista = new List<security_event>();
                foreach (var item in reg) {
                    security_event Linha = new security_event { Id = item.Id, IdMaster = item.IdMaster, Descricao = item.Descricao };
                    Lista.Add(Linha);
                }
                return Lista;
            }
        }

        public int GetSizeofBinary (){
            using (var db = new GTI_Context(_connection)) {
                int nSize = (from t in db.Security_event orderby t.Id descending select t.Id).FirstOrDefault();
                return nSize;
            }
        }

        public string GetUserBinary(int id) {
            using (var db = new GTI_Context(_connection)) {
                string Sql = (from u in db.Usuario where u.Id == id select u.Userbinary).FirstOrDefault();
                if (Sql == null) {
                    Sql = "0";
                    int nSize = GetSizeofBinary();
                    int nDif = nSize - Sql.Length;
                    string sZero = new string('0', nDif);
                    Sql += sZero;
                    return dalCore.Encrypt(Sql);
                }
                    return Sql;
            }
        }

        public List<usuarioStruct> Lista_Usuarios() {
            using (var db = new GTI_Context(_connection)) {
                var reg = (from t in db.Usuario
                           join cc in db.Centrocusto on t.Setor_atual equals cc.Codigo into tcc from cc in tcc.DefaultIfEmpty()
                           where t.Ativo == 1
                           orderby t.Nomecompleto select new { t.Nomelogin, t.Nomecompleto, t.Ativo, t.Id, t.Senha, t.Setor_atual, cc.Descricao }).ToList();
                List<usuarioStruct> Lista = new List<usuarioStruct>();
                foreach (var item in reg) {
                    usuarioStruct Linha = new usuarioStruct {
                        Nome_login = item.Nomelogin,
                        Nome_completo = item.Nomecompleto,
                        Ativo = item.Ativo,
                        Id=item.Id,
                        Senha=item.Senha,
                        Setor_atual=item.Setor_atual,
                        Nome_setor=item.Descricao
                    };
                    Lista.Add(Linha);
                }
                return Lista;
            }
        }

        public Exception Incluir_Usuario(Usuario reg) {
            using (var db = new GTI_Context(_connection)) {
                try {
                    List<SqlParameter> parameters = new List<SqlParameter>();
                    parameters.Add(new SqlParameter("@id", reg.Id));
                    parameters.Add(new SqlParameter("@nomelogin", reg.Nomelogin));
                    parameters.Add(new SqlParameter("@nomecompleto", reg.Nomecompleto));
                    parameters.Add(new SqlParameter("@setor_atual", reg.Setor_atual));

                    db.Database.ExecuteSqlCommand("INSERT INTO usuario2(id,nomelogin,nomecompleto,ativo,setor_atual)" +
                                                  " VALUES(@id,@nomelogin,@nomecompleto,@ativo,@setor_atual)",parameters.ToArray());
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            } 
        }

        public Exception Alterar_Usuario(Usuario reg) {
            using (var db = new GTI_Context(_connection)) {
                Usuario b = db.Usuario.First(i => i.Id == reg.Id);
                b.Nomecompleto = reg.Nomecompleto;
                b.Nomelogin = reg.Nomelogin;
                b.Setor_atual = reg.Setor_atual;
                b.Ativo = reg.Ativo;
                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }

        public Exception Alterar_Usuario_Local(List<Usuariocc> reg) {
            using (var db = new GTI_Context(_connection)) {
                db.Database.ExecuteSqlCommand("DELETE FROM usuariocc WHERE userid=@id" ,new SqlParameter("@id", reg[0].Userid));

                List<SqlParameter> parameters = new List<SqlParameter>();
                foreach (Usuariocc item in reg) {
                    try {
                        parameters.Add(new SqlParameter("@Userid", item.Userid));
                        parameters.Add(new SqlParameter("@Codigocc", item.Codigocc));

                        db.Database.ExecuteSqlCommand("INSERT INTO usuariocc(userid,codigocc) VALUES(@Userid,@Codigocc)", parameters.ToArray());
                        parameters.Clear();
                    } catch (Exception ex) {
                        return ex;
                    }
                }
                return null;
            }
        }

        public usuarioStruct Retorna_Usuario(int Id) {
            using (var db = new GTI_Context(_connection)) {
                var reg = (from t in db.Usuario
                           join cc in db.Centrocusto on t.Setor_atual equals cc.Codigo into tcc from cc in tcc.DefaultIfEmpty()
                           where t.Id==Id
                           orderby t.Nomelogin select new usuarioStruct {Nome_login= t.Nomelogin,  Nome_completo=t.Nomecompleto,Ativo= t.Ativo,
                               Id=  t.Id, Senha= t.Senha, Setor_atual= t.Setor_atual, Nome_setor= cc.Descricao }).FirstOrDefault();
                usuarioStruct Sql = new usuarioStruct();
                Sql.Id = reg.Id;
                Sql.Nome_completo = reg.Nome_completo;
                Sql.Nome_login = reg.Nome_login;
                Sql.Senha = reg.Senha;
                Sql.Setor_atual = reg.Setor_atual;
                Sql.Nome_setor = reg.Nome_setor;
                Sql.Ativo = reg.Ativo;
                return Sql;
            }
        }

        public List<Usuariocc> Lista_Usuario_Local(int Id) {
            using (var db = new GTI_Context(_connection)) {
                var reg = (from cc in db.Usuariocc where cc.Userid == Id orderby cc.Codigocc select cc).ToList();
                return reg;
            }
        }

        public Exception SaveUserBinary(Usuario reg) {
            using (var db = new GTI_Context(_connection)) {
                int nId = reg.Id;
                Usuario b = db.Usuario.First(i => i.Id == nId);
                b.Userbinary = reg.Userbinary;
                try {
                    db.SaveChanges();
                } catch (Exception ex) {
                    return ex;
                }
                return null;
            }
        }
        #endregion


    }
}
