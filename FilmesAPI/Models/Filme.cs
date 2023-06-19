using System.ComponentModel.DataAnnotations;

namespace FilmesAPI.Models;

public class Filme
{
    [Key]
    [Required]
    public int Id { get; set; } 

    [Required(ErrorMessage = "O Título é obrigatório.")]
    public string Titulo { get; set; }

    [Required(ErrorMessage = "O Gênero é obrigatório.")]
    [MaxLength(50, ErrorMessage = "O Gênero não pode ter mais do que 50 caractéres.")]
    public string Genero { get; set; }

    [Required(ErrorMessage = "A Duração é obrigatório.")]
    [Range(70,600,ErrorMessage = "O filme deve ter entre 70 e 600 minutos de duração.")]
    public int Duracao { get; set; }
}
