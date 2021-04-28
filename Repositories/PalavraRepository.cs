using Microsoft.EntityFrameworkCore;
using MimicAPI.Database;
using MimicAPI.Helpers;
using MimicAPI.Models;
using MimicAPI.Repositories.Contracts;
using System;
using System.Linq;

namespace MimicAPI.Repositories
{
    public class PalavraRepository : IPalavraRepository
    {
        private readonly MimicContext _banco;

        public PalavraRepository(MimicContext banco)
        {
            _banco = banco;
        }

        public PaginationList<Palavra> Obterpalavras(PalavraUrlQuery query)
        {
            var lista = new PaginationList<Palavra>();
            var item = _banco.Palavras.AsNoTracking().AsQueryable();

            if (query.Data.HasValue)
            {
                item = item.Where(a => a.Criado > query.Data.Value || a.Atualizado > query.Data.Value);
            }

            if (query.PagNumero.HasValue)
            {
                var quantidadeTotalRegistros = item.Count();
                item = item.Skip((query.PagNumero.Value - 1) * query.PagRegistro.Value).Take(query.PagRegistro.Value);

                var paginacao = new Paginacao()
                {
                    NumeroPagina = query.PagNumero.Value,
                    ResgistroPorPagina = query.PagRegistro.Value,
                    TotalRegistro = quantidadeTotalRegistros,
                    TotalPaginas = (int)Math.Ceiling((double)quantidadeTotalRegistros / query.PagRegistro.Value)
                };

                lista.Paginacao = paginacao;
            }

            lista.AddRange(item.ToList());
            return lista;
        }

        public Palavra Obter(int id)
        {
            return _banco.Palavras.AsNoTracking().FirstOrDefault(a => a.Id == id);
        }

        public void Cadastrar(Palavra palavra)
        {
            _banco.Palavras.Add(palavra);
            _banco.SaveChanges();
        }

        public void Atualizar(Palavra palavra)
        {
            _banco.Palavras.Update(palavra);
            _banco.SaveChanges();
        }

        public void Deletar(int id)
        {
            var palavra = Obter(id);
            palavra.Ativo = false;

            _banco.Palavras.Find(palavra);
            _banco.SaveChanges();
        }



    }
}
