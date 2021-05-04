using System;
using MimicAPI.Helpers;
using MimicAPI.v1.Models;
using System.Collections.Generic;

namespace MimicAPI.Repositories.v1.Contracts
{
    public interface IPalavraRepository
    {
        PaginationList<Palavra> Obterpalavras(PalavraUrlQuery query);
        Palavra Obter(int id);
        void Cadastrar(Palavra palavra);
        void Atualizar(Palavra palavra);
        void Deletar(int id);

    }
}
