﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using TalkToAPI.V1.Models;
using TalkToAPI.V1.Repositories.Interfaces;

namespace TalkToAPI.V1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]

    public class MensagemController : ControllerBase
    {
        private readonly IMensagemRepository _mensagemRepository;

        public MensagemController(IMensagemRepository mensagemRepository)
        {
            _mensagemRepository = mensagemRepository;
        }

        [HttpPost("CadastrarMensagem")]
        [Authorize]
        public ActionResult CadastrarMensagem([FromBody] Mensagem mensagem)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _mensagemRepository.CadastrarMensagem(mensagem);
                }
                catch (Exception e)
                {
                    return UnprocessableEntity(e);
                }

                return Ok(mensagem);
            }
            else
            {
                return UnprocessableEntity(ModelState);
            }
        }

        [HttpPatch("AtualizarMensagem={id}")]
        [Authorize]
        public ActionResult AtualizarMensagem(int id, [FromBody] JsonPatchDocument<Mensagem> jsonPath)
        {
            if (jsonPath == null)
            {
                return BadRequest();
            }

            var mensagem = _mensagemRepository.ObterMensagem(id);

            jsonPath.ApplyTo(mensagem);
            mensagem.Atualizado = DateTime.UtcNow;

            _mensagemRepository.AtualizarMensagem(mensagem);

            return Ok(mensagem);
        }

        [HttpGet("ObterMensagens={idUsuarioDe}/{idUsuarioPara}")]
        [Authorize]
        public ActionResult ObterMensagens(string idUsuarioDe, string idUsuarioPara)
        {
            if (idUsuarioDe == idUsuarioPara)
            {
                return UnprocessableEntity();
            }

            return Ok(_mensagemRepository.ObterMensagens(idUsuarioDe, idUsuarioPara));
        }

        [HttpGet("ObterMensagem={id}")]
        [Authorize]
        public ActionResult ObterMensagem(int id)
        {
            var mensagem = _mensagemRepository.ObterMensagem(id);

            if (mensagem == null)
            {
                return NotFound();
            }

            return Ok(mensagem);
        }
    }
}