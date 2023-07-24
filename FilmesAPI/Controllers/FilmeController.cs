using AutoMapper;
using FilmesAPI.Data;
using FilmesAPI.Data.Dtos;
using FilmesAPI.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace FilmesAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class FilmeController : ControllerBase
{
    private FilmeContext _context;
    private IMapper _mapper;

    public FilmeController(FilmeContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// Adiciona um filme ao banco de dados
    /// </summary>
    /// <param name="filmeDto">Objeto com os campos necessários para criação de um filme</param>
    /// <returns>IActionResult</returns>
    /// <response code="201">Caso inserção seja feita com sucesso</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public IActionResult AdicionarFilme(CreateFilmeDto filmeDto)
    {
        Filme filme = _mapper.Map<Filme>(filmeDto);
        _context.Filmes.Add(filme);
        _context.SaveChanges();
        return CreatedAtAction(nameof(RecuperaFilmePorId), new { id = filme.Id}, filme);
    }

    /// <summary>
    /// Retorna todos os filmes do banco de dados com paginação configurável
    /// </summary>
    /// <param name="skip">A partir de qual contagem de filmes os resultados devem ser coletados</param>
    /// <param name="take">Quantos filmes devem ser retornados</param>
    /// <param name="nomeCinema">Filtro opcional para mostrar apenas filmes que possuem sessoes no cinema de nome correspondente</param>
    /// <returns>IActionResult</returns>
    /// <response code="200">Caso a consulta tenha sido feita com sucesso</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IEnumerable<ReadFilmeDto> RecuperaFilmes( [FromQuery] int skip = 0, [FromQuery] int take = 50, [FromQuery] string? nomeCinema = null)
    {
        if(nomeCinema == null)
        {
            return _mapper.Map<List<ReadFilmeDto>>(_context.Filmes.Skip(skip).Take(take)).ToList();
        }
        return _mapper.Map<List<ReadFilmeDto>>(_context.Filmes.Skip(skip).Take(take)
            .Where(filme => filme.Sessoes
                .Any(sessao => sessao.Cinema.Nome == nomeCinema))).ToList();
    }

    /// <summary>
    /// Retorna o filme com o Id especificado
    /// </summary>
    /// <param name="id">O Id do filme requisitado</param>
    /// <returns>IActionResult</returns>
    /// <response code="200">Caso a consulta tenha sido feita com sucesso</response>
    [HttpGet("{id}")]
    public IActionResult RecuperaFilmePorId(int id)
    {
        var filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);
        if (filme == null) return NotFound(filme);
        var filmeDto = _mapper.Map<ReadFilmeDto>(filme);
        return Ok(filmeDto);
    }

    /// <summary>
    /// Atualiza o filme com o Id especificado
    /// </summary>
    /// <param name="id">O Id do filme que será atualizado</param>
    /// <param name="filmeDto">Objeto com os campos nescessários para a atualização do filme</param>
    /// <returns>IActionResult</returns>
    /// <response code="204">Caso a consulta tenha sido feita com sucesso</response>
    [HttpPut("{id}")]
    public IActionResult AtualizaFilme(int id, [FromBody] UpdateFilmeDto filmeDto)
    {
        var filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);
        if (filme == null) return NotFound();
        _mapper.Map(filmeDto, filme);
        _context.SaveChanges();
        return NoContent();

    }

    /// <summary>
    /// Atualiza parcialmente o filme com o Id especificado
    /// </summary>
    /// <param name="id">O Id do filme que será atualizado</param>
    /// <param name="patch">Objeto JsonPatchDocument com os campos nescessários para a atualização do filme</param>
    /// <returns>IActionResult</returns>
    /// <response code="204">Caso a consulta tenha sido feita com sucesso</response>
    [HttpPatch("{id}")]
    public IActionResult AtualizaFilmeParcial(int id, [FromBody] JsonPatchDocument<UpdateFilmeDto> patch)
    {
        var filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);
        if (filme == null) return NotFound();

        var filmeParaAtualizar = _mapper.Map<UpdateFilmeDto>(filme);

        patch.ApplyTo(filmeParaAtualizar, ModelState);

        if (!TryValidateModel(filmeParaAtualizar))
        {
            return ValidationProblem(ModelState);
        }
        _mapper.Map(filmeParaAtualizar, filme);
        _context.SaveChanges();
        return NoContent();

    }

    /// <summary>
    /// Delete o registro do filme especificado
    /// </summary>
    /// <param name="id">O Id do filme que será deletado</param>
    /// <returns>IActionResult</returns>
    /// <response code="204">Caso a consulta tenha sido feita com sucesso</response>
    [HttpDelete("{id}")]
    public IActionResult DeletaFilme(int id)
    {
        var filme = _context.Filmes.FirstOrDefault(filme => filme.Id == id);
        if (filme == null) return NotFound(filme);
        _context.Remove(filme);
        _context.SaveChanges();
        return NoContent(); 

    }

}
