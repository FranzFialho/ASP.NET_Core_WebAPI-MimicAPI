using Microsoft.AspNetCore.Mvc;
using MimicAPI.Helpers;
using MimicAPI.Models;
using MimicAPI.Repositories.Contracts;
using Newtonsoft.Json;
using System.Linq;

namespace MimicAPI.Controllers
{
    [Route("api/palavras")]

    public class PalavrasController : ControllerBase
    {
        private readonly IPalavraRepository _repository;

        public PalavrasController(IPalavraRepository repository)
        {
            _repository = repository;
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


        [Route("{id:int}")]
        [HttpGet]
        public ActionResult Obter(int id)
        {
            var obj = _repository.Obter(id);

            if (obj == null)
            {
                return NotFound();
            }

            return Ok(obj);
        }


        [Route("")]
        [HttpPost]
        public ActionResult Cadastrar([FromBody] Palavra palavra)
        {
            _repository.Cadastrar(palavra);

            return Created($"/api/palavra/{palavra.Id}", palavra);
        }


        [Route("{id:int}")]
        [HttpPut]
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


        [Route("{id:int}")]
        [HttpDelete]
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
