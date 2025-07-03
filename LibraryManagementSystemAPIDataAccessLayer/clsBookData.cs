using System;
using System.Data;
using Microsoft.Data.SqlClient;
using LibraryManagementSystemAPIDataAccessLayer;
using SharedClasses;
using static SharedClasses.BookDTOs;
using System.Net;
using static SharedClasses.BookCopyDTOs;
using static System.Reflection.Metadata.BlobBuilder;
using System.Text.RegularExpressions;

namespace LibraryManagementSystemAPIDataAccessLayer
{
    
    public class clsBookData
    {
        static public bool IsISBN(string input)
        {
            return Regex.IsMatch(input, @"^(?:\d{9}[\dX]|\d{3}[-\s]?\d{1,5}[-\s]?\d{1,7}[-\s]?\d{1,7}[-\s]?\d{1})$");
        }
        public static async Task<Result<FullBookDTO>> GetBookInfoByIDAsync(int BookID)
        {
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT * FROM Books WHERE BookID = @BookID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@BookID", BookID);
                    try
                    {
                        await connection.OpenAsync();
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                                int authorID = reader.GetInt32(reader.GetOrdinal("AuthorID"));
                                int publisherID = reader.GetInt32(reader.GetOrdinal("PublisherID"));
                                int categoryID = reader.GetInt32(reader.GetOrdinal("CategoryID"));


                                Result<string> authorResult = await clsAuthorData.GetAuthorNameAsync(authorID);
                                if (!authorResult.Success)
                                {
                                    return new Result<FullBookDTO>(false, authorResult.Message, null, authorResult.ErrorCode);
                                }
                                Result<string> publisherResult = await clsPublisherData.GetPublisherNameAsync(publisherID);
                                if (!publisherResult.Success)
                                {
                                    return new Result<FullBookDTO>(false, publisherResult.Message, null, publisherResult.ErrorCode);
                                }
                                Result<string> categoryResult = await clsBookCategoryData.GetCategoryNameByIDAsync(categoryID);
                                if (!categoryResult.Success)
                                {
                                    return new Result<FullBookDTO>(false, categoryResult.Message, null, categoryResult.ErrorCode);
                                }
                                FullBookDTO book = new FullBookDTO(
                                    reader.GetInt32(reader.GetOrdinal("BookID")),
                                    reader.GetString(reader.GetOrdinal("Title")),
                                    authorID,
                                    reader.GetString(reader.GetOrdinal("ISBN")),
                                    publisherID,
                                    categoryID,
                                    reader.GetDateTime(reader.GetOrdinal("Year")),
                                    reader.GetString(reader.GetOrdinal("Location")),
                                    Convert.ToSingle(reader.GetDecimal(reader.GetOrdinal("BorrowFees"))),
                                    authorResult.Data,
                                    publisherResult.Data,
                                    categoryResult.Data
                                );
                                return new Result<FullBookDTO>(true, "Book retrieved successfully.", book);
                            }
                            else
                            {
                                return new Result<FullBookDTO>(false, "Book not found.", null, 404);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return new Result<FullBookDTO>(false, "An unexpected error occurred on the server.", null, 500);
                    }
                }
            }
        }

        public static async Task<Result<FullBookDTO>> GetBookInfoByISBNOrTitleAsync(string bookIdentifier)
        {
            string query;

            if (IsISBN(bookIdentifier)) 
            {
                query = "SELECT * FROM Books WHERE ISBN = @bookIdentifier;";
            }
            else 
            {
                query = "SELECT * FROM Books WHERE Title = @bookIdentifier;";
            }

            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@bookIdentifier", bookIdentifier);
                    try
                    {
                        await connection.OpenAsync();
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                int authorID = reader.GetInt32(reader.GetOrdinal("AuthorID"));
                                int publisherID = reader.GetInt32(reader.GetOrdinal("PublisherID"));
                                int categoryID = reader.GetInt32(reader.GetOrdinal("CategoryID"));

                                Result<string> authorResult = await clsAuthorData.GetAuthorNameAsync(authorID);
                                if (!authorResult.Success)
                                {
                                    return new  Result<FullBookDTO>(false,authorResult.Message,null,authorResult.ErrorCode);
                                }
                                Result<string> publisherResult = await clsPublisherData.GetPublisherNameAsync(publisherID);
                                if (!publisherResult.Success)
                                {
                                    return new Result<FullBookDTO>(false, publisherResult.Message, null, publisherResult.ErrorCode);
                                }
                                Result<string> categoryResult = await clsBookCategoryData.GetCategoryNameByIDAsync(categoryID);
                                if (!categoryResult.Success)
                                {
                                    return new Result<FullBookDTO>(false, categoryResult.Message, null, categoryResult.ErrorCode);
                                }
                                FullBookDTO book = new FullBookDTO(
                                    reader.GetInt32(reader.GetOrdinal("BookID")),
                                    reader.GetString(reader.GetOrdinal("Title")),
                                    authorID,
                                    reader.GetString(reader.GetOrdinal("ISBN")),
                                    publisherID,
                                    categoryID,
                                    reader.GetDateTime(reader.GetOrdinal("Year")),
                                    reader.GetString(reader.GetOrdinal("Location")),
                                    Convert.ToSingle(reader.GetDecimal(reader.GetOrdinal("BorrowFees"))),
                                    authorResult.Data,
                                    authorResult.Data,
                                    categoryResult.Data
                                    );
                                return new Result<FullBookDTO>(true, "Books retrieved successfully", book);
                            }
                            return new Result<FullBookDTO>(false, "No books found.", null, 404);
                        }
                    }
                    catch (Exception ex)
                    {
                        return new Result<FullBookDTO>(false, "An unexpected error occurred on the server.", null, 500);
                    }
                }
            }
        }
        public static async Task<Result<bool>> IsBookExistByISBN(string ISBN)
        {
            bool isFound = false;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT BookID FROM Books WHERE ISBN = @ISBN";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ISBN", ISBN);
                    try
                    {
                        await connection.OpenAsync();
                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            isFound = reader.HasRows;
                        }
                        return new Result<bool>(true, "Book existence check completed.", isFound);
                    }
                    catch (Exception ex)
                    {
                        return new Result<bool>(false, "An unexpected error occurred on the server.", false, 500);
                    }
                }
            }
        }
        public static async Task<Result<int>> AddNewBookAsync(AddNewBookDTO addNewBookDTO)
        {
            int BookID = -1;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand command = new SqlCommand("sp_AddNewBook", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@Title", addNewBookDTO.bookDTO.Title);
                command.Parameters.AddWithValue("@AuthorID", addNewBookDTO.bookDTO.AuthorID);
                command.Parameters.AddWithValue("@ISBN", addNewBookDTO.bookDTO.ISBN);
                command.Parameters.AddWithValue("@PublisherID", addNewBookDTO.bookDTO.PublisherID);
                command.Parameters.AddWithValue("@CategoryID", addNewBookDTO.bookDTO.CategoryID);
                command.Parameters.AddWithValue("@Year", addNewBookDTO.bookDTO.Year);
                command.Parameters.AddWithValue("@Location", addNewBookDTO.bookDTO.Location);
                command.Parameters.AddWithValue("@BorrowFees", addNewBookDTO.bookDTO.BorrowFees);
                command.Parameters.AddWithValue("@TheNumberOfCopies", addNewBookDTO.numberOfCopies);
                SqlParameter returnedValue = new SqlParameter("@ReturnValue", SqlDbType.Int)
                {
                    Direction = ParameterDirection.ReturnValue
                };
                command.Parameters.Add(returnedValue);
                SqlParameter RowAffected = new SqlParameter("@TheRowAffected", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(RowAffected);

                try
                {
                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                    BookID = (int)returnedValue.Value;
                    if (BookID > 0)
                    {
                        return new Result<int>(true, "Book added successfully.", BookID);
                    }
                    else
                    {
                        return new Result<int>(false, "An unexpected error occurred on the server.", -1, 500);
                    }
                }
                catch (Exception ex)
                {
                    return new Result<int>(false, "An unexpected error occurred on the server.", -1, 500);
                }
            }
        }

        public static async Task<Result<int>> UpdateBookAsync(BookDTO bookDTO)
        {
            int rowsAffected = 0;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"UPDATE Books 
                      SET Title = @Title, AuthorID = @AuthorID, ISBN = @ISBN, PublisherID = @PublisherID, 
                          CategoryID = @CategoryID, Year = @Year, Location = @Location, BorrowFees = @BorrowFees
                      WHERE BookID = @BookID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@BookID", bookDTO.BookID);
                    command.Parameters.AddWithValue("@Title", bookDTO.Title);
                    command.Parameters.AddWithValue("@AuthorID", bookDTO.AuthorID);
                    command.Parameters.AddWithValue("@ISBN", bookDTO.ISBN);
                    command.Parameters.AddWithValue("@PublisherID", bookDTO.PublisherID);
                    command.Parameters.AddWithValue("@CategoryID", bookDTO.CategoryID);
                    command.Parameters.AddWithValue("@Year", bookDTO.Year);
                    command.Parameters.AddWithValue("@Location", bookDTO.Location);
                    command.Parameters.AddWithValue("@BorrowFees", bookDTO.BorrowFees);

                    try
                    {
                        await connection.OpenAsync();
                        rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return new Result<int>(true, "Book updated successfully.", rowsAffected);
                        }
                        else
                        {
                            return new Result<int>(false, "An unexpected error occurred on the server.", -1, 500);
                        }
                    }
                    catch (Exception ex)
                    {
                        return new Result<int>(false, "An unexpected error occurred on the server.", -1, 500);
                    }
                }
            }
        }

        public static async Task<Result<bool>> DeleteBookAsync(int BookID)
        {
            int rowsAffected = 0;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = @"DELETE FROM Books WHERE BookID = @BookID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@BookID", BookID);
                    try
                    {
                        await connection.OpenAsync();
                        rowsAffected = await command.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return new Result<bool>(true, "Book deleted successfully.", true);
                        }
                        else
                        {
                            return new Result<bool>(false, "An unexpected error occurred on the server.", false, 500);
                        }
                    }
                    catch (Exception ex)
                    {
                        return new Result<bool>(false, "An unexpected error occurred on the server.", false, 500);
                    }
                }
            }
        }

        public static async Task<Result<bool>> IsBookExistAsync(int BookID)
        {
            bool isFound = false;
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            {
                string query = "SELECT BookID FROM Books WHERE BookID = @BookID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@BookID", BookID);
                    try
                    {
                        await connection.OpenAsync();
                        var result = await command.ExecuteScalarAsync();
                        isFound = result != null;
                        return new Result<bool>(true, "Book existence checked.", isFound);
                    }
                    catch (Exception ex)
                    {
                        return new Result<bool>(false, "An unexpected error occurred on the server.", false, 500);
                    }
                }
            }
        }
        public static async Task<Result<List<BookViewDTO>>> GetAllBooksAsync()
        {
            string query = "SELECT * FROM Book_View";
            List<BookViewDTO> allBooks = new List<BookViewDTO>();
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                try
                {
                    await connection.OpenAsync();
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            allBooks.Add(new BookViewDTO
                            (
                                reader.GetInt32(reader.GetOrdinal("BookID")),
                                reader.GetString(reader.GetOrdinal("Title")),
                                reader.GetString(reader.GetOrdinal("Author")),
                                reader.GetString(reader.GetOrdinal("ISBN")),
                                reader.GetString(reader.GetOrdinal("Publisher")),
                                reader.GetString(reader.GetOrdinal("CategoryName")),
                                reader.GetString(reader.GetOrdinal("Location")),
                                (float)reader.GetDecimal(reader.GetOrdinal("BorrowFees")),
                                reader.GetInt32(reader.GetOrdinal("TotalCopies")),
                                reader.GetInt32(reader.GetOrdinal("AvailableCopies"))
                            ));
                        }
                    }
                    if (allBooks.Count == 0)
                    {
                        return new Result<List<BookViewDTO>>(false, "No books found.", allBooks, 404);
                    }
                    return new Result<List<BookViewDTO>>(true, "Books retrieved successfully.", allBooks);
                }
                catch (Exception ex)
                {
                    return new Result<List<BookViewDTO>>(false, "An unexpected error occurred on the server.", allBooks, 500);
                }
            }
        }

        public static async Task<Result<List<BookViewDTO>>> GetAllBooksForAuthorIDAsync(int authorID)
        {
            string query = @"SELECT Book_View.*
                     FROM Authors 
                     INNER JOIN Books ON Authors.AuthorID = Books.AuthorID 
                     INNER JOIN Book_View ON Books.BookID = Book_View.BookID
                     WHERE Authors.AuthorID = @authorID";
            List<BookViewDTO> allBooks = new List<BookViewDTO>();
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@authorID", authorID);
                try
                {
                    await connection.OpenAsync();
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            allBooks.Add(new BookViewDTO
                            (
                                reader.GetInt32(reader.GetOrdinal("BookID")),
                                reader.GetString(reader.GetOrdinal("Title")),
                                reader.GetString(reader.GetOrdinal("Author")),
                                reader.GetString(reader.GetOrdinal("ISBN")),
                                reader.GetString(reader.GetOrdinal("Publisher")),
                                reader.GetString(reader.GetOrdinal("CategoryName")),
                                reader.GetString(reader.GetOrdinal("Location")),
                                (float)reader.GetDecimal(reader.GetOrdinal("BorrowFees")),
                                reader.GetInt32(reader.GetOrdinal("TotalCopies")),
                                reader.GetInt32(reader.GetOrdinal("AvailableCopies"))
                            ));
                        }
                    }
                    if (allBooks.Count == 0)
                    {
                        return new Result<List<BookViewDTO>>(false, "No books found.", allBooks, 404);
                    }
                    return new Result<List<BookViewDTO>>(true, "Books retrieved successfully.", allBooks);
                }
                catch (Exception ex)
                {
                    return new Result<List<BookViewDTO>>(false, "An unexpected error occurred on the server.", allBooks, 500);
                }
            }
        }

        public static async Task<Result<List<BookViewDTO>>> GetAllBooksForPublisherIDAsync(int publisherID)
        {
            string query = @"SELECT Book_View.*
                     FROM Books 
                     INNER JOIN Publishers ON Books.PublisherID = Publishers.PublisherID 
                     INNER JOIN Book_View ON Books.BookID = Book_View.BookID
                     WHERE Publishers.PublisherID = @publisherID";
            List<BookViewDTO> allBooks = new List<BookViewDTO>();
            using (SqlConnection connection = new SqlConnection(clsDataAccessSettings.ConnectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@publisherID", publisherID);
                try
                {
                    await connection.OpenAsync();
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            allBooks.Add(new BookViewDTO
                            (
                                reader.GetInt32(reader.GetOrdinal("BookID")),
                                reader.GetString(reader.GetOrdinal("Title")),
                                reader.GetString(reader.GetOrdinal("Author")),
                                reader.GetString(reader.GetOrdinal("ISBN")),
                                reader.GetString(reader.GetOrdinal("Publisher")),
                                reader.GetString(reader.GetOrdinal("CategoryName")),
                                reader.GetString(reader.GetOrdinal("Location")),
                                (float)reader.GetDecimal(reader.GetOrdinal("BorrowFees")),
                                reader.GetInt32(reader.GetOrdinal("TotalCopies")),
                                reader.GetInt32(reader.GetOrdinal("AvailableCopies"))
                            ));
                        }
                    }
                    if (allBooks.Count == 0)
                    {
                        return new Result<List<BookViewDTO>>(false, "No books found.", allBooks, 404);
                    }
                    return new Result<List<BookViewDTO>>(true, "Books retrieved successfully.", allBooks);
                }
                catch (Exception ex)
                {
                    return new Result<List<BookViewDTO>>(false, "An unexpected error occurred on the server.", allBooks, 500);
                }
            }
        }
    }

}
