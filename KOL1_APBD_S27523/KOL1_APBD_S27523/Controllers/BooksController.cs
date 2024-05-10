using KOL1_APBD_S27523.DTOs;
using KOL1_APBD_S27523.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace KOL1_APBD_S27523.Controllers;

[Route("api/books")]
[ApiController]
public class BookController(IBooksRepository booksRepository) : ControllerBase
{
    private IBooksRepository _booksRepository = booksRepository;

    [HttpGet("{bookId:int}/genres")]
    public async Task<IActionResult> GetGenresOfBookByIdAsync(int bookId)
    {
        var bookTitle = await _booksRepository.GetTitleOfBookByIdAsync(bookId);
        if (bookTitle == null) return NotFound($"Book with id {bookId} does not exist!");

        var genresOfBook = await _booksRepository.GetGenresOfBookByIdAsync(bookId);
        var response = new GetGenresOfBookResponse(bookId, bookTitle, new[] { genresOfBook });
        return Ok(response);
    }

    [HttpPost]
    public async Task<IActionResult> AddNewBookWithAssignedAuthors(
        [FromBody] PostBookWithGenresData postBookWithGenresData)
    {
        foreach (var genreId in postBookWithGenresData.Genres)
        {
            var genreName = await _booksRepository.DoesGenreExist(genreId);
            if (genreName == null)
            {
                return NotFound($"Genre doesnt exist with id{genreId}!");
            }
        }

        PostNewBookWithGenresResponse response;
        try
        {
            response = await _booksRepository.AddNewBookWithAssignedGenres(postBookWithGenresData);
        }
        catch (Exception e)
        {
            return NotFound(e.Message);
        }

        return StatusCode(201, response);
    }
}