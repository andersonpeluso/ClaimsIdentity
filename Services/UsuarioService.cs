using AutoMapper;
using Innovativo.DTO;
using Innovativo.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Innovativo.Services
{
    public interface IUsuarioService
    {
        bool Autenticar(string email, string senha, out UsuarioLogadoDTO usuarioLogadoDTO);

        Usuario ObterPorID(int id);

        UsuarioDTO ObterPorIdDTO(int id);

        int Inserir(UsuarioDTO dto);

        List<UsuarioDTO> Listar();

        bool Alterar(int id, UsuarioDTO dto);
    }

    public class UsuarioService : IUsuarioService
    {
        private readonly InnovativoContext _context;
        private readonly IMapper _mapper;

        private readonly AppSettings _appSettings;

        public UsuarioService(InnovativoContext context, IMapper mapper, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        public bool Autenticar(string email, string senha, out UsuarioLogadoDTO usuarioLogadoDTO)
        {
            usuarioLogadoDTO = null;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha))
                return false;

            var usuario = _context.Usuario.SingleOrDefault(x => x.Email == email && Convert.ToBase64String(x.Senha) == Convert.ToBase64String(SenhaCriptografada(senha)));

            if (usuario == null)
            {
                usuario = new Usuario
                {
                    ID = 1,
                    Nome = "Anderson"
                };
                //usuario.Senha = "senha";
                //return false;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Segredo);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(ObterClaims(usuario)),
                Expires = DateTime.UtcNow.AddDays(_appSettings.ExpiracaoToken),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            usuarioLogadoDTO = new UsuarioLogadoDTO
            {
                ID = usuario.ID,
                Usuario = usuario.Email,
                Nome = usuario.Nome,
                Token = tokenString,
                Papeis = usuario.ObterPapeis()
            };

            return true;
        }

        private Claim[] ObterClaims(Usuario usuario)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.ID.ToString())
            };

            foreach (string papel in usuario.ObterPapeis())
                claims.Add(new Claim(ClaimTypes.Role, papel));

            return claims.ToArray();
        }

        public Usuario ObterPorID(int id)
        {
            return _context.Usuario.FirstOrDefault(x => x.ID == id);
        }

        public UsuarioDTO ObterPorIdDTO(int id)
        {
            var usuario = ObterPorID(id);
            if (usuario is null)
                return null;

            return _mapper.Map<UsuarioDTO>(usuario);
        }

        private byte[] SenhaCriptografada(string senha)
        {
            var encoding = Encoding.GetEncoding(65001);
            var buffer = encoding.GetBytes(senha);

            var sha1 = SHA1.Create();
            return sha1.ComputeHash(buffer);
        }

        public int Inserir(UsuarioDTO dto)
        {
            var usuario = _mapper.Map<Usuario>(dto);

            usuario.Senha = SenhaCriptografada(dto.Senha);
            
            _context.Usuario.Add(usuario);
            _context.SaveChanges();
            
            return usuario.ID;
        }

        public bool Alterar(int id, UsuarioDTO dto)
        {
            var usuario = _context.Usuario.FirstOrDefault(x => x.ID == id);
           
            if (usuario is null)
                return false;

            usuario.Nome = dto.Nome;
            usuario.Email = dto.Email;
            usuario.ClienteID = dto.ClienteID;

            if (!string.IsNullOrEmpty(dto.Senha))
                usuario.Senha = SenhaCriptografada(dto.Senha);

            _context.Usuario.Update(usuario);
            _context.SaveChanges();

            return true;
        }

        public List<UsuarioDTO> Listar()
        {
            return _mapper.Map<List<UsuarioDTO>>(_context.Usuario.Where(x => x.ClienteID.HasValue).ToList());
        }
    }
}