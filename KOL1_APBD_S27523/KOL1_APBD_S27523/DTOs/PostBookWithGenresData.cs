using System.ComponentModel.DataAnnotations;

namespace KOL1_APBD_S27523.DTOs;

public record PostBookWithGenresData(
    [Required] string Title,
    [Required] IEnumerable<int> Genres
);