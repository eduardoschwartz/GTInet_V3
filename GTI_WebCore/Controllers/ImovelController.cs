﻿using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using GTI_WebCore.Interfaces;
using GTI_WebCore.Models;
using GTI_WebCore.Models.ReportModels;
using GTI_WebCore.Repository;
using GTI_WebCore.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;

namespace GTI_WebCore.Controllers {

    [Route("Imovel/Certidao_Endereco")]
    public class ImovelController : Controller{
        private readonly IImovelRepository _imovelRepository;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly ITributarioRepository tributarioRepository;

        public ImovelController(IImovelRepository imovelRepository, IHostingEnvironment hostingEnvironment,ITributarioRepository tributarioRepository) {
            _imovelRepository = imovelRepository;
            this.hostingEnvironment = hostingEnvironment;
            this.tributarioRepository = tributarioRepository;
        }

        [HttpGet]
        public ViewResult Certidao_Endereco() {
            return View();
        }

        [HttpPost]
        public IActionResult Certidao_Endereco(CertidaoViewModel model) {
            int _codigo = 0;
            int _numero = tributarioRepository.Retorna_Codigo_Certidao(Functions.TipoCertidao.Endereco);
            bool _existeCod = false;
            CertidaoViewModel certidaoViewModel = new CertidaoViewModel();
            ViewBag.Result = "";

            if (!string.IsNullOrWhiteSpace(model.Chave)) {
                if (!Valida_Certidao(model.Chave)) {
                    ViewBag.Result = "Chave de autenticação da certidão inválida.";
                    return View(certidaoViewModel);
                }
            }

            if (model.Inscricao != null) {
                _codigo = Convert.ToInt32(model.Inscricao);
                if (_codigo < 100000) 
                    _existeCod = _imovelRepository.Existe_Imovel(_codigo);
            }

            if (!Captcha.ValidateCaptchaCode(model.CaptchaCode, HttpContext)) {
                ViewBag.Result = "Código de verificação inválido.";
                return View(certidaoViewModel);
            }

            if (!_existeCod) {
                ViewBag.Result = "Imóvel não cadastrado.";
                return View(certidaoViewModel);
            }

            ReportDocument rd = new ReportDocument();
            rd.Load(hostingEnvironment.ContentRootPath + "\\Reports\\Certidao_Endereco.rpt");
            List<Certidao> certidao = new List<Certidao>();
            List<ProprietarioStruct> listaProp = _imovelRepository.Lista_Proprietario(_codigo, true);
            ImovelStruct _dados = _imovelRepository.Dados_Imovel(_codigo);
            Certidao reg = new Certidao() {
                Codigo = _dados.Codigo,
                Inscricao = _dados.Inscricao,
                Endereco = _dados.NomeLogradouro + ", " + _dados.Numero + " " + _dados.Complemento,
                Bairro = _dados.NomeBairro ?? "",
                Nome_Requerente = listaProp[0].Nome,
                Ano = DateTime.Now.Year,
                Numero = _numero,
                Numero_Ano=_numero.ToString("00000")+"/"+ DateTime.Now.Year.ToString(),
                Quadra_Original = _dados.QuadraOriginal ?? "",
                Lote_Original = _dados.LoteOriginal ?? "",
                Controle = _numero.ToString("00000") + DateTime.Now.Year.ToString("0000") + "/" + _codigo.ToString() + "-EA"
            };
            certidao.Add(reg);
            try {
                rd.SetDataSource(certidao);
                Stream stream = rd.ExportToStream(ExportFormatType.PortableDocFormat);
                return File(stream, "application/pdf", "Certidao_Endereco.pdf");
            } catch {

                throw;
            }

        }

        [Route("get-captcha-image")]
        public IActionResult GetCaptchaImage() {
            int width = 100;
            int height = 36;
            var captchaCode = Captcha.GenerateCaptchaCode();
            var result = Captcha.GenerateCaptchaImage(width, height, captchaCode);
            HttpContext.Session.SetString("CaptchaCode", result.CaptchaCode);
            Stream s = new MemoryStream(result.CaptchaByteData);
            return new FileStreamResult(s, "image/png");
        }

        public bool Valida_Certidao(string Chave) {
            string sTipo = "";
            int nPos = 0, nPos2 = 0, nCodigo = 0, nAno = 0, nNumero = 0;
            if (Chave.Trim().Length < 8)
                return false;
            else {
                nPos = Chave.IndexOf("-");
                if (nPos < 6)
                    return false;
                else {
                    nPos2 = Chave.IndexOf("/");
                    if (nPos2 < 5 || nPos - nPos2 < 2)
                        return false;
                    else {
                        nCodigo = Convert.ToInt32(Chave.Substring(nPos2 + 1, nPos - nPos2 - 1));
                        nAno = Convert.ToInt32(Chave.Substring(nPos2 - 4, 4));
                        nNumero = Convert.ToInt32(Chave.Substring(0, 5));
                        if (nAno < 2010 || nAno > DateTime.Now.Year + 1)
                            return false;
                        else {
                            sTipo = Chave.Substring(Chave.Length - 2, 2);
                            if (sTipo == "EA") {
                                Certidao_endereco dados = tributarioRepository.Retorna_Certidao_Endereco(nNumero, nAno, nCodigo);
                                if (dados != null)
                                    return true;
                                else
                                    return false;
                            } else {
                                return false;
                            }
                        }
                    }
                }
            }
        }



    }
}