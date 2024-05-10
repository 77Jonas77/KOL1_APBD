using System.Data.SqlClient;
using KOL1_APBD_S27523.DTOs;

namespace KOL1_APBD_S27523.Repositories;

public class BooksRepository(IConfiguration configuration) : IBooksRepository
{
    private IConfiguration _configuration = configuration;

    public async Task<string?> GetTitleOfBookByIdAsync(int bookId)
    {
        await using var con = new SqlConnection(_configuration["ConnectionStrings:Default"]);
        await con.OpenAsync();
        await using var cmd = new SqlCommand();
        cmd.CommandText = $"SELECT title FROM books WHERE PK=@1";
        cmd.Parameters.AddWithValue("@1", bookId);
        cmd.Connection = con;

        var bookTitle = (string?)await cmd.ExecuteScalarAsync();
        return bookTitle;
    }

    public async Task<IEnumerable<string>> GetGenresOfBookByIdAsync(int bookId)
    {
        await using var con = new SqlConnection(_configuration["ConnectionStrings:Default"]);
        await con.OpenAsync();
        await using var cmd = new SqlCommand();
        cmd.CommandText = $"SELECT name FROM genres " +
                          $"INNER JOIN dbo.books_genres bg on genres.PK = bg.FK_genre " +
                          $"WHERE  bg.FK_book=@1";
        cmd.Parameters.AddWithValue("@1", bookId);
        cmd.Connection = con;

        var genresOfBook = new List<string>();
        var reader = await cmd.ExecuteReaderAsync();
        while (reader.Read())
        {
            genresOfBook.Add(reader.GetString(0));
        }

        return genresOfBook;
    }

    public async Task<PostNewBookWithGenresResponse> AddNewBookWithAssignedGenres(
        PostBookWithGenresData postBookWithGenresData)
    {
        await using var con = new SqlConnection(_configuration["ConnectionStrings:Default"]);
        await con.OpenAsync();
        var transaction = await con.BeginTransactionAsync();

        try
        {
            var bookId = await AddBookAsync(con, (SqlTransaction)transaction, postBookWithGenresData.Title);
            var genresIdsOfBook = new List<string>();
            foreach (var genre in postBookWithGenresData.Genres)
            {
                await using var cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.Transaction = (SqlTransaction)transaction;
                cmd.CommandText =
                    $"SELECT name FROM genres WHERE PK=@1";
                cmd.Parameters.AddWithValue("@1", genre);
                var genreName = await cmd.ExecuteScalarAsync();
                if (genreName == null)
                    throw new Exception();
                genresIdsOfBook.Add((string)genreName);
            }

            await AssignBookToGenres(con, (SqlTransaction)transaction, bookId, postBookWithGenresData.Genres);
            await transaction.CommitAsync();
            return new PostNewBookWithGenresResponse(bookId, postBookWithGenresData.Title, genresIdsOfBook);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<string?> DoesGenreExist(int genreId)
    {
        await using var con = new SqlConnection(_configuration["ConnectionStrings:Default"]);
        await con.OpenAsync();
        await using var cmd = new SqlCommand();
        cmd.CommandText = $"SELECT name FROM genres WHERE PK=@1";
        cmd.Parameters.AddWithValue("@1", genreId);
        cmd.Connection = con;

        var genreName = (string?)await cmd.ExecuteScalarAsync();
        return genreName;
    }

    private static async Task<int> AddBookAsync(SqlConnection con, SqlTransaction tran, string title)
    {
        await using var bookInsertCmd = new SqlCommand();
        bookInsertCmd.CommandText = "INSERT INTO books " +
                                    "VALUES (@1); " +
                                    "SELECT CONVERT(INT, SCOPE_IDENTITY());";
        bookInsertCmd.Parameters.AddWithValue("@1", title);
        bookInsertCmd.Connection = con;
        bookInsertCmd.Transaction = tran;
        return (int)(await bookInsertCmd.ExecuteScalarAsync())!;
    }

    private static async Task AssignBookToGenres(SqlConnection con, SqlTransaction tran, int bookId,
        IEnumerable<int> genres)
    {
        foreach (var genre in genres)
        {
            await using var bookInsertCmd = new SqlCommand();
            bookInsertCmd.CommandText = "INSERT INTO books_genres " +
                                        "VALUES (@1,@2); " +
                                        "SELECT CONVERT(INT, SCOPE_IDENTITY());";
            bookInsertCmd.Parameters.AddWithValue("@1", bookId);
            bookInsertCmd.Parameters.AddWithValue("@2", genre);
            bookInsertCmd.Connection = con;
            bookInsertCmd.Transaction = tran;
            await bookInsertCmd.ExecuteNonQueryAsync();
        }
    }
}