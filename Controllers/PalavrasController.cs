using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MimicAPI.Helpers;
using MimicAPI.Models;
using MimicAPI.Models.DTO;
using MimicAPI.Repositories.Contracts;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace MimicAPI.Controllers
{
    [Route("api/palavras")]

    public class PalavrasController : ControllerBase
    {
        private readonly IPalavraRepository _repository;
        private readonly IMapper _mapper;

        public PalavrasController(IPalavraRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }


        [Route("")]
        [HttpGet]
        public ActionResult ObterTodas([FromQuery] PalavraUrlQuery query)
        {
            var item = _repository.Obterpalavras(query);

            if (query.PagNumero > item.Paginacao.TotalPaginas)
            {
                return NotFound();
            }

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(item.Paginacao));
            return Ok(item.ToList());
        }



        [HttpGet("{id:int}", Name = "ObterPalavra")]
        public ActionResult Obter(int id)
        {
            var obj = _repository.Obter(id);

            if (obj == null)
            {
                return NotFound();
            }

            PalavraDTO palavraDTO = _mapper.Map<Palavra, PalavraDTO>(obj);

            palavraDTO.Links = new List<LinkDTO>();
            palavraDTO.Links.Add(new LinkDTO("self", Url.Link("ObterPalavra", new { id = palavraDTO.Id }), "GET"));
            palavraDTO.Links.Add(new LinkDTO("update", Url.Link("AtualizarPalavra", new { id = palavraDTO.Id }), "PUT"));
            palavraDTO.Links.Add(new LinkDTO("delete", Url.Link("DeletarPalavra", new { id = palavraDTO.Id }), "DELETE"));

            return Ok(palavraDTO);
    }


    [Route("")]
    [HttpPost]
    public ActionResult Cadastrar([FromBody] Palavra palavra)
    {
        _repository.Cadastrar(palavra);

        return Created($"/api/palavra/{palavra.Id}", palavra);
    }



    [HttpPut("{id:int}", Name = "AtualizarPalavra")]
    public ActionResult Atualizar(int id, [FromBody] Palavra palavra)
    {
        var obj = _repository.Obter(id);

        if (obj == null)
        {
            return NotFound();
        }

        palavra.Id = id;
        _repository.Atualizar(palavra);

        return Ok();
    }



    [HttpPut("{id:int}", Name = "DeletarPalavra")]
    public ActionResult Deletar(int id)
    {
        var palavra = _repository.Obter(id);

        if (palavra == null)
        {
            return NotFound();
        }

        _repository.Deletar(id);

        return NoContent();
    }
}
}
