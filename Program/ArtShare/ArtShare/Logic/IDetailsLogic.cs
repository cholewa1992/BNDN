using ArtShare.Models;
using ShareItServices.MediaItemService;

namespace ArtShare.Logic
{
    public interface IDetailsLogic
    {

        int CheckAccessRights(ShareItServices.AccessRightService.UserDTO requestingUser, int id);

        /// <summary>
        /// Purchases a mediaitem to a given user
        /// </summary>
        /// <param name="mediaId">media item to purchase</param>
        /// <param name="requestingUser">logged in user</param>
        /// <returns>bool of whether it succeeded</returns>
        bool PurchaseItem(int mediaId, ShareItServices.AccessRightService.UserDTO requestingUser);

        /// <summary>
        /// Retrieves a mediaitem
        /// </summary>
        /// <param name="id">Id of item to retrieve</param>
        /// <param name="requestingUser"></param>
        /// <returns>Retrieved media item</returns>
        MediaItemDTO GetMediaItem(int id, UserDTO requestingUser);

        /// <summary>
        /// Retrieves details about a given book and returns it in a book model
        /// </summary>
        /// <param name="id">Id of book item</param>
        /// <param name="requestingUser">Id of the user requesting the details</param>
        /// <returns>Book model with requested information</returns>
        //BookDetailsModel GetBookDetailsModel(int id, int? requestingUser);

        /// <summary>
        /// Deletes a book
        /// </summary>
        /// <param name="id">Book to delete</param>
        /// <param name="requestingUser">User requesting deletion</param>
        /// <returns>a bool of whether deletion succeeded</returns>
        bool DeleteBook(int id, int requestingUser);

        /// <summary>
        /// Updates a book's information. All previous information will be deleted, and supplied information added
        /// </summary>
        /// <param name="bookModel">The model containing all the information the book will have after the update</param>
        /// <param name="user">The Username and Password of the User requesting the operation</param>
        void EditBook(BookDetailsModel bookModel, ShareItServices.MediaItemService.UserDTO user);

        /// <summary>
        /// Retrieves details about a given Movie and returns it in a Movie model
        /// </summary>
        /// <param name="id">Id of Movie</param>
        /// <param name="requestingUser">Id of the user requesting the details</param>
        /// <returns>Movie model with requested information</returns>
        //MovieDetailsModel GetMovieDetailsModel(int id, int? requestingUser);

        /// <summary>
        /// Deletes a Movie
        /// </summary>
        /// <param name="id">Movie to delete</param>
        /// <param name="requestingUser">User requesting deletion</param>
        /// <returns>a bool of whether deletion succeeded</returns>
        bool DeleteMovie(int id, int requestingUser);

        /// <summary>
        /// Updates a movie's information. All previous information will be deleted, and supplied information added
        /// </summary>
        /// <param name="movieModel">The model containing all the information the movie will have after the update</param>
        /// <param name="user">The Username and Password of the User requesting the operation</param>
        void EditMovie(MovieDetailsModel movieModel, ShareItServices.MediaItemService.UserDTO user);

        /// <summary>
        /// Retrieves details about given Music and returns it in a Music model
        /// </summary>
        /// <param name="id">Id of Music item</param>
        /// <param name="requestingUser">Id of the user requesting the details</param>
        /// <returns>Music model with requested information</returns>
        //MusicDetailsModel GetMusicDetailsModel(int id, int? requestingUser);

        /// <summary>
        /// Deletes a Music Item
        /// </summary>
        /// <param name="id">Music to delete</param>
        /// <param name="requestingUser">User requesting deletion</param>
        /// <returns>a bool of whether deletion succeeded</returns>
        bool DeleteMusic(int id, int requestingUser);

        /// <summary>
        /// Updates a music's information. All previous information will be deleted, and supplied information added
        /// </summary>
        /// <param name="musicModel">The model containing all the information the music will have after the update</param>
        /// <param name="user">The Username and Password of the User requesting the operation</param>
        void EditMusic(MusicDetailsModel musicModel, ShareItServices.MediaItemService.UserDTO user);

        /// <summary>
        /// Puts information from service dto into a model
        /// </summary>
        /// <param name="dto">service dto</param>
        /// <returns>model</returns>
        BookDetailsModel ExtractBookInformation(MediaItemDTO dto);

        /// <summary>
        /// Puts information from service dto into a model
        /// </summary>
        /// <param name="dto">service dto</param>
        /// <returns>model</returns>
        MovieDetailsModel ExtractMovieInformation(MediaItemDTO dto);

        /// <summary>
        /// Puts information from service dto into a model
        /// </summary>
        /// <param name="dto">service dto</param>
        /// <returns>model</returns>
        MusicDetailsModel ExtractMusicInformation(MediaItemDTO dto);

        /// <summary>
        /// Rates a media item. If the user has already rated the media item, the rating is updated.
        /// </summary>
        /// <param name="userDto">The user who wishes to rate the media item</param>
        /// <param name="mediaId">The id of the media item to be rated</param>
        /// <param name="rating">The rating (1-10)</param> 
        /// <returns></returns>
        bool RateMediaItem(UserDTO userDto, int mediaId, int rating);
    }
}