using ArtShare.Models;
using ShareItServices.MediaItemService;

namespace ArtShare.Logic
{
    public interface IDetailsLogic
    {

        /// <summary>
        /// Purchases a mediaitem to a given user
        /// </summary>
        /// <param name="mediaId">media item to purchase</param>
        /// <param name="requestingUser">logged in user</param>
        /// <returns>bool of whether it succeeded</returns>
        bool PurchaseItem(int mediaId, int requestingUser);

        /// <summary>
        /// Retrieves details about a given book and returns it in a book model
        /// </summary>
        /// <param name="id">Id of book item</param>
        /// <param name="requestingUser">Id of the user requesting the details</param>
        /// <returns>Book model with requested information</returns>
        BookDetailsModel GetBookDetailsModel(int id, int? requestingUser);

        /// <summary>
        /// Deletes a book
        /// </summary>
        /// <param name="id">Book to delete</param>
        /// <param name="requestingUser">User requesting deletion</param>
        /// <returns>a bool of whether deletion succeeded</returns>
        bool DeleteBook(int id, int requestingUser);

        /// <summary>
        /// Updates a book. All previous information will be deleted, and supplied information added
        /// </summary>
        /// <param name="model">New information to add</param>
        /// <param name="requestingUser">The user requesting an edit</param>
        /// <returns>a bool of whether the edit succeeded</returns>
        bool EditBook(BookDetailsModel model, int requestingUser);

        /// <summary>
        /// Retrieves details about a given Movie and returns it in a Movie model
        /// </summary>
        /// <param name="id">Id of Movie</param>
        /// <param name="requestingUser">Id of the user requesting the details</param>
        /// <returns>Movie model with requested information</returns>
        MovieDetailsModel GetMovieDetailsModel(int id, int? requestingUser);

        /// <summary>
        /// Deletes a Movie
        /// </summary>
        /// <param name="id">Movie to delete</param>
        /// <param name="requestingUser">User requesting deletion</param>
        /// <returns>a bool of whether deletion succeeded</returns>
        bool DeleteMovie(int id, int requestingUser);

        /// <summary>
        /// Updates a Movie. All previous information will be deleted, and supplied information added
        /// </summary>
        /// <param name="model">New information to add</param>
        /// <param name="requestingUser">The user requesting an edit</param>
        /// <returns>a bool of whether the edit succeeded</returns>
        bool EditMovie(MovieDetailsModel model, int requestingUser);

        /// <summary>
        /// Retrieves details about given Music and returns it in a Music model
        /// </summary>
        /// <param name="id">Id of Music item</param>
        /// <param name="requestingUser">Id of the user requesting the details</param>
        /// <returns>Music model with requested information</returns>
        MusicDetailsModel GetMusicDetailsModel(int id, int? requestingUser);

        /// <summary>
        /// Deletes a Music Item
        /// </summary>
        /// <param name="id">Music to delete</param>
        /// <param name="requestingUser">User requesting deletion</param>
        /// <returns>a bool of whether deletion succeeded</returns>
        bool DeleteMusic(int id, int requestingUser);

        /// <summary>
        /// Updates a Music item. All previous information will be deleted, and supplied information added
        /// </summary>
        /// <param name="model">New information to add</param>
        /// <param name="requestingUser">The user requesting an edit</param>
        /// <returns>a bool of whether the edit succeeded</returns>
        bool EditMusic(MusicDetailsModel model, int requestingUser);

        /// <summary>
        /// Puts information from service dto into a model
        /// </summary>
        /// <param name="dto">service dto</param>
        /// <returns>model</returns>
        BookDetailsModel ExstractBookInformation(MediaItemDTO dto);

        /// <summary>
        /// Puts information from service dto into a model
        /// </summary>
        /// <param name="dto">service dto</param>
        /// <returns>model</returns>
        MovieDetailsModel ExstractMovieInformation(MediaItemDTO dto);

        /// <summary>
        /// Puts information from service dto into a model
        /// </summary>
        /// <param name="dto">service dto</param>
        /// <returns>model</returns>
        MusicDetailsModel ExstractMusicInformation(MediaItemDTO dto);
    }
}