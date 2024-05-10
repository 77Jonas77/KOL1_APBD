using KOL1_APBD_S27523.DTOs;

namespace KOL1_APBD_S27523.Repositories;

public interface IBooksRepository
{
    Task<string?> GetTitleOfBookByIdAsync(int bookId);
    Task<IEnumerable<string>> GetGenresOfBookByIdAsync(int bookId);

    Task<PostNewBookWithGenresResponse> AddNewBookWithAssignedGenres(PostBookWithGenresData postBookWithGenresData);
    Task<string?> DoesGenreExist(int genre);
}