using System.ComponentModel.DataAnnotations;

namespace KOL1_APBD_S27523.DTOs;

public record PostNewBookWithGenresResponse(
    [Required] int Id,
    [Required] string Title,
    [Required] IEnumerable<string> Genres
);