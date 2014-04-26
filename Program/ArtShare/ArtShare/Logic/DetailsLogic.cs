using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using ArtShare.Models;
using ArtShare.Properties;
using ShareItServices.AccessRightService;
using ShareItServices.MediaItemService;

namespace ArtShare.Logic
{
    public class DetailsLogic : IDetailsLogic
    {

        /// <summary>
        /// Purchases a mediaitem to a given user
        /// </summary>
        /// <param name="mediaId">media item to purchase</param>
        /// <param name="requestingUser">logged in user</param>
        /// <returns>bool of whether it succeeded</returns>
        public bool PurchaseItem(int mediaId, ShareItServices.AccessRightService.UserDTO requestingUser)
        {

            bool result;

            using (var ars = new AccessRightServiceClient())
            {
                result = ars.Purchase(requestingUser, mediaId, null, Resources.ClientToken);
            }

            return result;
        }


        /// <summary>
        /// Retrieves a mediaitem
        /// </summary>
        /// <param name="id">Id of item to retrieve</param>
        /// <param name="requestingUser"></param>
        /// <returns>Retrieved media item</returns>
        public MediaItemDTO GetMediaItem(int id, ShareItServices.MediaItemService.UserDTO requestingUser)
        {

            MediaItemDTO dto;
            

            using (var ms = new MediaItemServiceClient())
            {
                dto = ms.GetMediaItemInformation(id, requestingUser, Resources.ClientToken);

            }


            return dto;
        }

        public int CheckAccessRights(ShareItServices.AccessRightService.UserDTO requestingUser, int id)
        {
            using (var arsc = new AccessRightServiceClient())
            {
                var r = arsc.GetPurchaseHistory(requestingUser, requestingUser.Id, Resources.ClientToken);
                if (r.Any(t => t.Id == id)) return 1;
                if (arsc.GetUploadHistory(requestingUser, requestingUser.Id, Resources.ClientToken).Any(t => t.Id == id)) return 2;

                using (var authl = new ShareItServices.AuthService.AuthServiceClient())
                {
                    if (authl.IsUserAdminOnClient(
                        new ShareItServices.AuthService.UserDTO
                        {
                            Id = requestingUser.Id,
                            Username = requestingUser.Username,
                            Password = requestingUser.Password
                        },
                            Resources.ClientToken)) { return 2; }
                }

                return 0;
            }
        }

        ///// <summary>
        ///// Retrieves details about a given book and returns it in a book model
        ///// </summary>
        ///// <param name="id">Id of book item</param>
        ///// <param name="requestingUser">Id of the user requesting the details</param>
        ///// <returns>Book model with requested information</returns>
        //public BookDetailsModel GetBookDetailsModel(int id, int? requestingUser)
        //{
        //    MediaItemDTO dto;

        //    using (var ms = new MediaItemServiceClient())
        //    {
        //        dto = ms.GetMediaItemInformation(id, requestingUser, Resources.ClientToken);
                
        //    }

        //    return ExstractBookInformation(dto);
        //}

        /// <summary>
        /// Deletes a book
        /// </summary>
        /// <param name="id">Book to delete</param>
        /// <param name="requestingUser">User requesting deletion</param>
        /// <returns>a bool of whether deletion succeeded</returns>
        public bool DeleteBook(int id, int requestingUser)
        {
            throw new NotImplementedException();
        }


        ///// <summary>
        ///// Retrieves details about a given Movie and returns it in a Movie model
        ///// </summary>
        ///// <param name="id">Id of Movie</param>
        ///// <param name="requestingUser">Id of the user requesting the details</param>
        ///// <returns>Movie model with requested information</returns>
        //public MovieDetailsModel GetMovieDetailsModel(int id, int? requestingUser)
        //{

        //    MediaItemDTO serviceDTO;

        //    using (var ms = new MediaItemServiceClient())
        //    {
        //        serviceDTO = ms.GetMediaItemInformation(id, requestingUser, Resources.ClientToken);
        //    }

        //    return ExstractMovieInformation(serviceDTO);
        //}
        /// <summary>
        /// Deletes a Movie
        /// </summary>
        /// <param name="id">Movie to delete</param>
        /// <param name="requestingUser">User requesting deletion</param>
        /// <returns>a bool of whether deletion succeeded</returns>
        public bool DeleteMovie(int id, int requestingUser)
        {
            throw new NotImplementedException();
        }


        ///// <summary>
        ///// Retrieves details about given Music and returns it in a Music model
        ///// </summary>
        ///// <param name="id">Id of Music item</param>
        ///// <param name="requestingUser">Id of the user requesting the details</param>
        ///// <returns>Music model with requested information</returns>
        //public MusicDetailsModel GetMusicDetailsModel(int id, int? requestingUser)
        //{

        //    MediaItemDTO dto;

        //    using (var ms = new MediaItemServiceClient())
        //    {
        //        dto = ms.GetMediaItemInformation(id, requestingUser, Resources.ClientToken);
        //    }

        //    return ExstractMusicInformation(dto);
        //}
        /// <summary>
        /// Deletes a Music Item
        /// </summary>
        /// <param name="id">Music to delete</param>
        /// <param name="requestingUser">User requesting deletion</param>
        /// <returns>a bool of whether deletion succeeded</returns>
        public bool DeleteMusic(int id, int requestingUser)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Puts information from service dto into a model
        /// </summary>
        /// <param name="dto">service dto</param>
        /// <returns>model</returns>
        public BookDetailsModel ExstractBookInformation(MediaItemDTO dto)
        {
            var model = new BookDetailsModel();

            model.ProductId = dto.Id;
            model.FileExtension = dto.FileExtension;
            model.AvgRating = dto.AverageRating;
            model.RatingsGiven = dto.NumberOfRatings;

            // Pass data from service DTO to model
            foreach (var v in dto.Information)
            {
                switch (v.Type)
                {

                    case InformationTypeDTO.Author:
                        model.Author = v.Data;
                        break;

                    case InformationTypeDTO.Description:
                        model.Description = v.Data;
                        break;

                    case InformationTypeDTO.Genre:
                        if (model.Genres == null)
                        {
                            model.Genres = new List<string>();
                        }
                        model.Genres.Add(v.Data);
                        break;

                    case InformationTypeDTO.KeywordTag:
                        if (model.Tags == null)
                        {
                            model.Tags = new List<string>();
                        }
                        model.Tags.Add(v.Data);
                        break;

                    case InformationTypeDTO.Price:
                        try
                        {
                            model.Price = float.Parse(v.Data);
                        }
                        catch (Exception)
                        {
                            model.Price = null;
                        }
                        break;

                    case InformationTypeDTO.ReleaseDate:
                        try
                        {
                            model.ReleaseDate = DateTime.Parse(v.Data);
                        }
                        catch (Exception)
                        {
                            model.ReleaseDate = null;
                        }
                        break;


                    case InformationTypeDTO.Thumbnail:
                        model.Thumbnail = v.Data;
                        break;

                    case InformationTypeDTO.Title:
                        model.Title = v.Data;
                        break;

                    case InformationTypeDTO.NumberOfPages:
                        try
                        {
                            model.NumberOfPages = int.Parse(v.Data);
                        }
                        catch (Exception)
                        {
                            model.NumberOfPages = null;
                        }
                        break;

                }
            }


            return model;
        }


        /// <summary>
        /// Puts information from service dto into a model
        /// </summary>
        /// <param name="dto">service dto</param>
        /// <returns>model</returns>
        public MovieDetailsModel ExstractMovieInformation(MediaItemDTO dto)
        {
            var model = new MovieDetailsModel();

            model.ProductId = dto.Id;
            model.FileExtension = dto.FileExtension;
            model.AvgRating = dto.AverageRating;
            model.RatingsGiven = dto.NumberOfRatings;

            // Pass data from service DTO to model
            foreach (var v in dto.Information)
            {
                switch (v.Type)
                {
                    case InformationTypeDTO.CastMember:
                        if (model.CastMembers == null)
                        {
                            model.CastMembers = new List<string>();
                        }
                        model.CastMembers.Add(v.Data);
                        break;

                    case InformationTypeDTO.Description:
                        model.Description = v.Data;
                        break;

                    case InformationTypeDTO.Director:
                        model.Director = v.Data;
                        break;

                    case InformationTypeDTO.Genre:
                        if (model.Genres == null)
                        {
                            model.Genres = new List<string>();
                        }
                        model.Genres.Add(v.Data);
                        break;

                    case InformationTypeDTO.KeywordTag:
                        if (model.Tags == null)
                        {
                            model.Tags = new List<string>();
                        }
                        model.Tags.Add(v.Data);
                        break;

                    case InformationTypeDTO.Language:
                        model.Language = v.Data;
                        break;

                    case InformationTypeDTO.Price:
                        try
                        {
                            model.Price = float.Parse(v.Data);
                        }
                        catch (Exception)
                        {
                            model.Price = null;
                        }
                        break;

                    case InformationTypeDTO.ReleaseDate:
                        try
                        {
                            model.ReleaseDate = DateTime.Parse(v.Data);
                        }
                        catch (Exception)
                        {
                            model.ReleaseDate = null;
                        }
                        break;

                    case InformationTypeDTO.Runtime:
                        model.Runtime = v.Data;
                        break;

                    case InformationTypeDTO.Thumbnail:
                        model.Thumbnail = v.Data;
                        break;

                    case InformationTypeDTO.Title:
                        model.Title = v.Data;
                        break;

                }
            }


            return model;
        }


        /// <summary>
        /// Puts information from service dto into a model
        /// </summary>
        /// <param name="dto">service dto</param>
        /// <returns>model</returns>
        public MusicDetailsModel ExstractMusicInformation(MediaItemDTO dto)
        {

            var model = new MusicDetailsModel();

            model.ProductId = dto.Id;
            model.FileExtension = dto.FileExtension;
            model.AvgRating = dto.AverageRating;
            model.RatingsGiven = dto.NumberOfRatings;

            // Pass data from service DTO to model
            foreach (var v in dto.Information)
            {
                switch (v.Type)
                {

                    case InformationTypeDTO.Description:
                        model.Description = v.Data;
                        break;

                    case InformationTypeDTO.Artist:
                        model.Artist = v.Data;
                        break;

                    case InformationTypeDTO.Genre:
                        if (model.Genres == null)
                        {
                            model.Genres = new List<string>();
                        }
                        model.Genres.Add(v.Data);
                        break;

                    case InformationTypeDTO.KeywordTag:
                        if (model.Tags == null)
                        {
                            model.Tags = new List<string>();
                        }
                        model.Tags.Add(v.Data);
                        break;

                    case InformationTypeDTO.Price:
                        try
                        {
                            model.Price = float.Parse(v.Data);
                        }
                        catch (Exception)
                        {
                            model.Price = null;
                        }
                        break;

                    case InformationTypeDTO.ReleaseDate:
                        try
                        {
                            model.ReleaseDate = DateTime.Parse(v.Data);
                        }
                        catch (Exception)
                        {
                            model.ReleaseDate = null;
                        }
                        break;


                    case InformationTypeDTO.Thumbnail:
                        model.Thumbnail = v.Data;
                        break;

                    case InformationTypeDTO.Title:
                        model.Title = v.Data;
                        break;

                    case InformationTypeDTO.TrackLength:
                        model.TrackLength = v.Data;
                        break;
                    
                }
                
            }

            return model;

        }
        
        public bool RateMediaItem(ShareItServices.MediaItemService.UserDTO user, int mediaItemId, int rating)
        {
            if (user == null) { throw new ArgumentNullException("user"); }
            using (var client = new MediaItemServiceClient())
            {
                client.RateMediaItem(user, mediaItemId, rating, Resources.ClientToken);
            }
            return true;
        }

        /// <summary>
        /// Updates a book's information. All previous information will be deleted, and supplied information added
        /// </summary>
        /// <param name="bookModel">The model containing all the information the book will have after the update</param>
        /// <param name="user">The Username and Password of the User requesting the operation</param>
        public void EditBook(BookDetailsModel bookModel, ShareItServices.MediaItemService.UserDTO user)
        {
            using (var mis = new MediaItemServiceClient())
            {
                var updatedBookDTO = MapBook(bookModel);

                mis.UpdateMediaItemInformation(user, updatedBookDTO, Resources.ClientToken);
            }
        }

        /// <summary>
        /// Updates a movie's information. All previous information will be deleted, and supplied information added
        /// </summary>
        /// <param name="movieModel">The model containing all the information the movie will have after the update</param>
        /// <param name="user">The Username and Password of the User requesting the operation</param>
        public void EditMovie(MovieDetailsModel movieModel, ShareItServices.MediaItemService.UserDTO user)
        {
            using (var mis = new MediaItemServiceClient())
            {
                var updatedMovieDTO = MapMovie(movieModel);

                mis.UpdateMediaItemInformation(user, updatedMovieDTO, Resources.ClientToken);
            }
        }

        /// <summary>
        /// Updates a music's information. All previous information will be deleted, and supplied information added
        /// </summary>
        /// <param name="musicModel">The model containing all the information the music will have after the update</param>
        /// <param name="user">The Username and Password of the User requesting the operation</param>
        public void EditMusic(MusicDetailsModel musicModel, ShareItServices.MediaItemService.UserDTO user)
        {
            using (var mis = new MediaItemServiceClient())
            {
                var updatedMusicDTO = MapMusic(musicModel);

                mis.UpdateMediaItemInformation(user, updatedMusicDTO, Resources.ClientToken);
            }
        }

        #region Information Mappers
        private void MapTags(MediaItemDTO result, List<string> tags)
        {
            if (tags == null)
                return;
            foreach (var tag in tags)
            {
                if (!string.IsNullOrWhiteSpace(tag))
                    result.Information.Add(new MediaItemInformationDTO
                    {
                        Data = tag,
                        Type = InformationTypeDTO.KeywordTag
                    });
            }
        }

        private void MapGenres(MediaItemDTO result, List<string> genres)
        {
            if (genres == null)
                return;
            foreach (var genre in genres)
            {
                if (!string.IsNullOrWhiteSpace(genre))
                    result.Information.Add(new MediaItemInformationDTO
                    {
                        Data = genre,
                        Type = InformationTypeDTO.Genre
                    });
            }
        }

        private void MapPrice(MediaItemDTO result, float? price)
        {
            if (price != null)
                result.Information.Add(new MediaItemInformationDTO
                {
                    Data = price.ToString(),
                    Type = InformationTypeDTO.Price
                });
        }

        private void MapTitle(MediaItemDTO result, string title)
        {
            if (!string.IsNullOrWhiteSpace(title))
                result.Information.Add(new MediaItemInformationDTO
                {
                    Data = title,
                    Type = InformationTypeDTO.Title
                });
        }

        private void MapDescription(MediaItemDTO dto, string desc)
        {
            if (!string.IsNullOrWhiteSpace(desc))
                dto.Information.Add(new MediaItemInformationDTO { Data = desc, Type = InformationTypeDTO.Description });
        }

        private void MapTrackLength(MediaItemDTO result, string trackLength)
        {
            if (!string.IsNullOrWhiteSpace(trackLength))
                result.Information.Add(new MediaItemInformationDTO
                {
                    Data = trackLength,
                    Type = InformationTypeDTO.TrackLength
                });
        }

        private void MapReleaseDate(MediaItemDTO result, DateTime? releaseDate)
        {
            if (releaseDate != null)
                result.Information.Add(new MediaItemInformationDTO
                {
                    Data = releaseDate.ToString(),
                    Type = InformationTypeDTO.ReleaseDate
                });
        }

        private void MapArtist(MediaItemDTO result, string artist)
        {
            if (!string.IsNullOrWhiteSpace(artist))
                result.Information.Add(new MediaItemInformationDTO
                {
                    Data = artist,
                    Type = InformationTypeDTO.Artist
                });
        }

        private void MapLanguage(MediaItemDTO result, string language)
        {
            if (!string.IsNullOrWhiteSpace(language))
                result.Information.Add(new MediaItemInformationDTO
                {
                    Data = language,
                    Type = InformationTypeDTO.Language
                });
        }

        private void MapDirector(MediaItemDTO result, string director)
        {
            if (!string.IsNullOrWhiteSpace(director))
                result.Information.Add(new MediaItemInformationDTO
                {
                    Data = director,
                    Type = InformationTypeDTO.Director
                });
        }

        private void MapCastMembers(MediaItemDTO result, List<string> castMembers)
        {
            if (castMembers == null)
                return;
            foreach (var castMember in castMembers)
            {
                if (!string.IsNullOrWhiteSpace(castMember))
                    result.Information.Add(new MediaItemInformationDTO
                    {
                        Data = castMember,
                        Type = InformationTypeDTO.CastMember
                    });
            }
        }

        private void MapNumberOfPages(MediaItemDTO result, int? numberOfPages)
        {
            if (numberOfPages != null)
                result.Information.Add(new MediaItemInformationDTO
                {
                    Data = numberOfPages.ToString(),
                    Type = InformationTypeDTO.NumberOfPages
                });
        }

        private void MapAuthor(MediaItemDTO result, string author)
        {
            if (!string.IsNullOrWhiteSpace(author))
                result.Information.Add(new MediaItemInformationDTO
                {
                    Data = author,
                    Type = InformationTypeDTO.Author
                });
        }

        private void MapRuntime(MediaItemDTO result, string runtime)
        {
            if (!string.IsNullOrWhiteSpace(runtime))
                result.Information.Add(new MediaItemInformationDTO
                {
                    Data = runtime,
                    Type = InformationTypeDTO.Runtime
                });
        }

        #endregion
        #region DTO Mappers
        private MediaItemDTO MapDefault(IDetailsModel model)
        {
            var result = new MediaItemDTO { Information = new List<MediaItemInformationDTO>() };
            MapDescription(result, model.Description);
            MapTitle(result, model.Title);
            MapPrice(result, model.Price);
            MapGenres(result, model.Genres);
            MapTags(result, model.Tags);
            return result;
        }
        private MediaItemDTO MapMusic(MusicDetailsModel music)
        {
            var result = MapDefault(music);
            result.Type = MediaItemTypeDTO.Music;
            MapArtist(result, music.Artist);
            MapReleaseDate(result, music.ReleaseDate);
            MapTrackLength(result, music.TrackLength);
            return result;
        }

        private MediaItemDTO MapMovie(MovieDetailsModel movie)
        {
            var result = MapDefault(movie);
            result.Type = MediaItemTypeDTO.Movie;
            MapRuntime(result, movie.Runtime);
            MapReleaseDate(result, movie.ReleaseDate);
            MapCastMembers(result, movie.CastMembers);
            MapDirector(result, movie.Director);
            MapLanguage(result, movie.Language);
            return result;
        }

        private MediaItemDTO MapBook(BookDetailsModel book)
        {
            var result = MapDefault(book);
            result.Type = MediaItemTypeDTO.Book;
            MapReleaseDate(result, book.ReleaseDate);
            MapLanguage(result, book.Language);
            MapAuthor(result, book.Author);
            MapNumberOfPages(result, book.NumberOfPages);
            return result;
        }
        #endregion

    }
}