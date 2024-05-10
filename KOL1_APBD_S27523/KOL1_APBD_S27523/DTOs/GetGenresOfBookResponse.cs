using System.ComponentModel.DataAnnotations;

namespace KOL1_APBD_S27523.DTOs;

public record GetGenresOfBookResponse(
    [Required] int Id,
    [Required][MaxLength(100)] string Title,
    [Required] ICollection<IEnumerable<string>> Genres
);