using Microsoft.AspNetCore.Mvc;

namespace MimicAPI.v2
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("2.0")]

    public class PalavrasController : ControllerBase
    {
        /// <summary>
        /// Operação que pega do banco de dados todas as palvras existentes
        /// </summary>
        /// <param name="query">Filtros de pesquisa</param>
        /// <returns>Listagem de palavras</returns>
        [HttpGet("", Name = "ObterTodas")]
        public string ObterTodas()
        {
            return "Versão 2.0";

        }

    }
}
