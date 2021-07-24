using AutoMapper;
using Innovativo.DTO;
using Innovativo.Models;
using System.Collections.Generic;
using System.Linq;

namespace Innovativo.Services
{
    public interface IEficaciaCanaisService
    {
        IList<EficaciaCanaisRelatorio> SelecionarPorUsuario(int usuarioID);

        int Inserir(EficaciaCanalDTO dto, out string mensagem);

        EficaciaCanalDTO ObterPorID(int id);

        bool PodeAcessarRelatorio(EficaciaCanalDTO ecr, int usuarioID);

        List<EficaciaCanalRelatorioDTO> Listar(int usuarioID);

        EficaciaCanalDTO ObterUltimoDoCliente(int usuarioID);
    }

    public class EficaciaCanaisService : IEficaciaCanaisService
    {
        private readonly InnovativoContext _context;
        private readonly IUsuarioService _usuarioService;
        private readonly IMapper _mapper;

        public EficaciaCanaisService(InnovativoContext context, IUsuarioService usuarioService, IMapper mapper)
        {
            _context = context;
            _usuarioService = usuarioService;
            _mapper = mapper;
        }

        public EficaciaCanalDTO ObterPorID(int id)
        {
            return _mapper.Map<EficaciaCanalDTO>(_context.EficaciaCanaisRelatorio.FirstOrDefault(x => x.ID == id));
        }

        public EficaciaCanalDTO ObterUltimoDoCliente(int usuarioID)
        {
            var usuario = _usuarioService.ObterPorID(usuarioID);
            
            if (usuario is null)
                return null;

            EficaciaCanaisRelatorio eficaciaCanaisRelatorio;

            if (usuario.ClienteID.HasValue)
                eficaciaCanaisRelatorio = usuario.Cliente.EficaciaCanalRelatorioLista.OrderByDescending(x => x.DataFinal).FirstOrDefault();
            else
                eficaciaCanaisRelatorio = _context.EficaciaCanaisRelatorio.OrderByDescending(x => x.DataFinal).FirstOrDefault();

            return _mapper.Map<EficaciaCanalDTO>(eficaciaCanaisRelatorio);
        }

        public IList<EficaciaCanaisRelatorio> SelecionarPorUsuario(int usuarioID)
        {
            var usuario = _usuarioService.ObterPorID(usuarioID);
            
            if (usuario is null)
                return null;

            if (usuario.ClienteID.HasValue)
                return usuario.Cliente.EficaciaCanalRelatorioLista;

            return _context.EficaciaCanaisRelatorio.ToList();
        }

        public bool PodeAcessarRelatorio(EficaciaCanalDTO eficaciaCanalDTO, int usuarioID)
        {
            var usuario = _usuarioService.ObterPorID(usuarioID);

            return !usuario.ClienteID.HasValue || eficaciaCanalDTO.Cliente == usuario.ClienteID.Value;
        }

        public int Inserir(EficaciaCanalDTO dto, out string mensagem)
        {
            if (!ValidarData(dto, out mensagem))
                return 0;

            if (!ValidarValoresCanais(dto, out mensagem))
                return 0;

            var eficaciaCanaisRelatorio = new EficaciaCanaisRelatorio
            {
                IdCliente = dto.Cliente,
                Descricao = dto.Descricao,
                DataInicial = dto.DataInicial,
                DataFinal = dto.DataFinal
            };

            _context.EficaciaCanaisRelatorio.Add(eficaciaCanaisRelatorio);

            var eficaciaCanalBuscaPaga = new EficaciaCanalBuscaPaga
            {
                EficaciaCanalID = eficaciaCanaisRelatorio.ID,
                Visitantes = dto.BuscaPagaVisitantes,
                Leads = dto.BuscaPagaLeads,
                Oportunidades = dto.BuscaPagaOportunidades,
                Vendas = dto.BuscaPagaVendas
            };

            _context.EficaciaCanalBuscaPaga.Add(eficaciaCanalBuscaPaga);

            var eficaciaCanalDireto = new EficaciaCanalDireto
            {
                EficaciaCanalID = eficaciaCanaisRelatorio.ID,
                Visitantes = dto.DiretoVisitantes,
                Leads = dto.DiretoLeads,
                Oportunidades = dto.DiretoOportunidades,
                Vendas = dto.DiretoVendas
            };
            _context.EficaciaCanalDireto.Add(eficaciaCanalDireto);

            var eficaciaCanalEmail = new EficaciaCanalEmail
            {
                EficaciaCanalID = eficaciaCanaisRelatorio.ID,
                Visitantes = dto.EmailVisitantes,
                Leads = dto.EmailLeads,
                Oportunidades = dto.EmailOportunidades,
                Vendas = dto.EmailVendas
            };
            _context.EficaciaCanalEmail.Add(eficaciaCanalEmail);

            var eficaciaCanalOrganico = new EficaciaCanalOrganico
            {
                EficaciaCanalID = eficaciaCanaisRelatorio.ID,
                Visitantes = dto.OrganicoVisitantes,
                Leads = dto.OrganicoLeads,
                Oportunidades = dto.OrganicoOportunidades,
                Vendas = dto.OrganicoVendas
            };
            _context.EficaciaCanalOrganico.Add(eficaciaCanalOrganico);

            var eficaciaCanalReferencia = new EficaciaCanalReferencia
            {
                EficaciaCanalID = eficaciaCanaisRelatorio.ID,
                Visitantes = dto.ReferenciaVisitantes,
                Leads = dto.ReferenciaLeads,
                Oportunidades = dto.ReferenciaOportunidades,
                Vendas = dto.ReferenciaVendas
            };
            _context.EficaciaCanalReferencia.Add(eficaciaCanalReferencia);

            _context.SaveChanges();
            return eficaciaCanaisRelatorio.ID;
        }

        private bool ValidarData(EficaciaCanalDTO dto, out string mensagem)
        {
            mensagem = string.Empty;

            if (dto.DataInicial > dto.DataFinal)
            {
                mensagem = "Data Inicial não pode ser maior que a Data Final";
                return false;
            }

            if (_context.EficaciaCanaisRelatorio.Any(x =>
                   ((dto.DataInicial >= x.DataInicial && dto.DataInicial <= x.DataFinal) || (dto.DataFinal >= x.DataInicial && dto.DataFinal <= x.DataFinal) || (dto.DataInicial < x.DataInicial && dto.DataFinal > x.DataFinal))
                 && dto.Cliente == x.Cliente.ID)
            )
            {
                mensagem = "Já existe um relatório para o período informado";
                return false;
            }

            return true;
        }

        private bool ValidarValoresCanais(EficaciaCanalDTO dto, out string mensagem)
        {
            mensagem = string.Empty;

            //Direto
            if (dto.DiretoVisitantes < dto.DiretoLeads || dto.DiretoVisitantes < dto.DiretoOportunidades || dto.DiretoVisitantes < dto.DiretoVendas)
            {
                mensagem = "Canal Direto: Os número de Visitantes tem que ser maior ou igual número de Leads, Oportunidade e Vendas";
                return false;
            }

            if (dto.DiretoLeads < dto.DiretoOportunidades || dto.DiretoLeads < dto.DiretoVendas)
            {
                mensagem = "Canal Direto: Os número de Leads tem que ser maior ou igual número de Oportunidade e Vendas";
                return false;
            }

            if (dto.DiretoOportunidades < dto.DiretoVendas)
            {
                mensagem = "Canal Direto: Os número de Oportunidades tem que ser maior ou igual número de Vendas";
                return false;
            }

            //Busca Paga
            if (dto.BuscaPagaVisitantes < dto.BuscaPagaLeads || dto.BuscaPagaVisitantes < dto.BuscaPagaOportunidades || dto.BuscaPagaVisitantes < dto.BuscaPagaVendas)
            {
                mensagem = "Canal Busca Paga: Os número de Visitantes tem que ser maior ou igual número de Leads, Oportunidade e Vendas";
                return false;
            }

            if (dto.BuscaPagaLeads < dto.BuscaPagaOportunidades || dto.BuscaPagaLeads < dto.BuscaPagaVendas)
            {
                mensagem = "Canal Busca Paga: Os número de Leads tem que ser maior ou igual número de Oportunidade e Vendas";
                return false;
            }

            if (dto.BuscaPagaOportunidades < dto.BuscaPagaVendas)
            {
                mensagem = "Canal Busca Paga: Os número de Oportunidades tem que ser maior ou igual número de Vendas";
                return false;
            }

            //Organico
            if (dto.OrganicoVisitantes < dto.OrganicoLeads || dto.OrganicoVisitantes < dto.OrganicoOportunidades || dto.OrganicoVisitantes < dto.OrganicoVendas)
            {
                mensagem = "Canal Orgânico: Os número de Visitantes tem que ser maior ou igual número de Leads, Oportunidade e Vendas";
                return false;
            }

            if (dto.OrganicoLeads < dto.OrganicoOportunidades || dto.OrganicoLeads < dto.OrganicoVendas)
            {
                mensagem = "Canal Orgânico: Os número de Leads tem que ser maior ou igual número de Oportunidade e Vendas";
                return false;
            }

            if (dto.OrganicoOportunidades < dto.OrganicoVendas)
            {
                mensagem = "Canal Orgânico: Os número de Oportunidades tem que ser maior ou igual número de Vendas";
                return false;
            }

            //Email
            if (dto.EmailVisitantes < dto.EmailLeads || dto.EmailVisitantes < dto.EmailOportunidades || dto.EmailVisitantes < dto.EmailVendas)
            {
                mensagem = "Canal E-mail: Os número de Visitantes tem que ser maior ou igual número de Leads, Oportunidade e Vendas";
                return false;
            }

            if (dto.EmailLeads < dto.EmailOportunidades || dto.EmailLeads < dto.EmailVendas)
            {
                mensagem = "Canal E-mail: Os número de Leads tem que ser maior ou igual número de Oportunidade e Vendas";
                return false;
            }

            if (dto.EmailOportunidades < dto.EmailVendas)
            {
                mensagem = "Canal E-mail: Os número de Oportunidades tem que ser maior ou igual número de Vendas";
                return false;
            }

            //Referência
            if (dto.ReferenciaVisitantes < dto.ReferenciaLeads || dto.ReferenciaVisitantes < dto.ReferenciaOportunidades || dto.ReferenciaVisitantes < dto.ReferenciaVendas)
            {
                mensagem = "Canal Referência: Os número de Visitantes tem que ser maior ou igual número de Leads, Oportunidade e Vendas";
                return false;
            }

            if (dto.ReferenciaLeads < dto.ReferenciaOportunidades || dto.ReferenciaLeads < dto.ReferenciaVendas)
            {
                mensagem = "Canal Referência: Os número de Leads tem que ser maior ou igual número de Oportunidade e Vendas";
                return false;
            }

            if (dto.ReferenciaOportunidades < dto.ReferenciaVendas)
            {
                mensagem = "Canal Referência: Os número de Oportunidades tem que ser maior ou igual número de Vendas";
                return false;
            }

            return true;
        }

        public List<EficaciaCanalRelatorioDTO> Listar(int usuarioID)
        {
            var eficaciaCanalRelatorioDTOs = new List<EficaciaCanalRelatorioDTO>();

            foreach (EficaciaCanaisRelatorio eficaciaCanaisRelatorio in this.SelecionarPorUsuario(usuarioID))
            {
                eficaciaCanalRelatorioDTOs.Add(new EficaciaCanalRelatorioDTO
                {
                    ID = eficaciaCanaisRelatorio.ID,
                    Descricao = eficaciaCanaisRelatorio.Descricao,
                    ClienteNomeFantasia = eficaciaCanaisRelatorio.Cliente.NomeFantasia,
                    DataInicial = eficaciaCanaisRelatorio.DataInicial,
                    DataFinal = eficaciaCanaisRelatorio.DataFinal
                });
            }
            return eficaciaCanalRelatorioDTOs;
        }
    }
}