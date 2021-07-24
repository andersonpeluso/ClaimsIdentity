using AutoMapper;
using Innovativo.DTO;
using Innovativo.Models;
using System.Collections.Generic;
using System.Linq;

namespace Innovativo.Services
{
    public interface IClienteService
    {
        List<ClienteDTO> Listar();

        ClienteDTO ObterPorID(int id);

        bool Alterar(int id, ClienteDTO dto);

        ClienteDTO Inserir(ClienteDTO dto);
    }

    public class ClienteService : IClienteService
    {
        private readonly InnovativoContext _context;
        private readonly IMapper _mapper;

        public ClienteService(InnovativoContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public List<ClienteDTO> Listar()
        {
            return _mapper.Map<List<ClienteDTO>>(_context.Cliente.ToList());
        }

        public ClienteDTO ObterPorID(int id)
        {
            return _mapper.Map<ClienteDTO>(_context.Cliente.FirstOrDefault(x => x.ID == id));
        }

        public bool Alterar(int id, ClienteDTO dto)
        {
            var cliente = _context.Cliente.FirstOrDefault(x => x.ID == id);
            
            if (cliente is null)
                return false;

            cliente.NomeFantasia = dto.NomeFantasia;
            _context.Cliente.Update(cliente);
            _context.SaveChanges();
            return true;
        }

        public ClienteDTO Inserir(ClienteDTO dto)
        {
            var cliente = new Cliente
            {
                NomeFantasia = dto.NomeFantasia
            };

            _context.Cliente.Add(cliente);
            _context.SaveChanges();
            dto.ID = cliente.ID;
            
            return dto;
        }
    }
}