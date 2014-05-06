using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using ArtShare.Models;
using ArtShare.Properties;
using ShareItServices.AccessRightService;
using ShareItServices.MediaItemService;
using UserDTO = ShareItServices.MediaItemService.UserDTO;

namespace ArtShare.Logic
{
    /// <summary>
    /// Logic for media item details
    /// </summary>
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

        /// <summary>
        /// Checkes which access right a user has to a media item. 0 means no acessright, 1 means buyer and 2 is owner or admin
        /// </summary>
        /// <param name="requestingUser">The user DTO of whom to check</param>
        /// <param name="mediaId">The media item id</param>
        /// <returns>0 if no accessright, 1 if buyer, 2 if owner and 3 if admin</returns>
        public int CheckAccessRights(ShareItServices.AccessRightService.UserDTO requestingUser, int mediaId)
        {
            using (var authl = new ShareItServices.AuthService.AuthServiceClient())
            {
                if (authl.IsUserAdminOnClient(
                    new ShareItServices.AuthService.UserDTO
                    {
                        Id = requestingUser.Id,
                        Username = requestingUser.Username,
                        Password = requestingUser.Password
                    },
                        Resources.ClientToken)) { return 3; }
            }

            using (var arsc = new AccessRightServiceClient())
            {
                if (arsc.GetUploadHistory(requestingUser, requestingUser.Id, Resources.ClientToken).
                    Any(t => t.MediaItemId == mediaId)) return 2;
                if (arsc.GetPurchaseHistory(requestingUser, requestingUser.Id, Resources.ClientToken).
                    Any(t => t.MediaItemId == mediaId)) return 1;

                return 0;
            }
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
        /// Puts information from service dto into a model
        /// </summary>
        /// <param name="dto">service dto</param>
        /// <returns>model</returns>
        public BookDetailsModel ExtractBookInformation(MediaItemDTO dto)
        {
            var model = new BookDetailsModel();

            model.ProductId = dto.Id;
            model.FileExtension = dto.FileExtension;
            model.AvgRating = dto.AverageRating;
            model.RatingsGiven = dto.NumberOfRatings;
            if (dto.Owner != null)
            {
                model.UploaderName = dto.Owner.Username;
                model.UploaderId = dto.Owner.Id;
            }

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
                        model.GenresString += v.Data + ",";
                        model.Genres.Add(v.Data);
                        break;

                    case InformationTypeDTO.KeywordTag:
                        if (model.Tags == null)
                        {
                            model.Tags = new List<string>();
                        }
                        model.TagsString += v.Data + ",";
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
                    case InformationTypeDTO.Language:
                        model.Language = v.Data;
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
        public MovieDetailsModel ExtractMovieInformation(MediaItemDTO dto)
        {
            var model = new MovieDetailsModel();

            model.ProductId = dto.Id;
            model.FileExtension = dto.FileExtension;
            model.AvgRating = dto.AverageRating;
            model.RatingsGiven = dto.NumberOfRatings;
            if (dto.Owner != null)
            {
                model.UploaderName = dto.Owner.Username;
                model.UploaderId = dto.Owner.Id; 
            }

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
                        model.CastMembersString += v.Data + ",";
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
                        model.GenresString += v.Data + ",";
                        model.Genres.Add(v.Data);
                        break;

                    case InformationTypeDTO.KeywordTag:
                        if (model.Tags == null)
                        {
                            model.Tags = new List<string>();
                        }
                        model.TagsString += v.Data + ",";
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
        public MusicDetailsModel ExtractMusicInformation(MediaItemDTO dto)
        {

            var model = new MusicDetailsModel();

            model.ProductId = dto.Id;
            model.FileExtension = dto.FileExtension;
            model.AvgRating = dto.AverageRating;
            model.RatingsGiven = dto.NumberOfRatings;
            if (dto.Owner != null)
            {
                model.UploaderName = dto.Owner.Username;
                model.UploaderId = dto.Owner.Id;
            }

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
                        model.GenresString += v.Data + ",";
                        model.Genres.Add(v.Data);
                        break;

                    case InformationTypeDTO.KeywordTag:
                        if (model.Tags == null)
                        {
                            model.Tags = new List<string>();
                        }
                        model.TagsString += v.Data + ",";
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

        public bool DeleteDetails(UserDTO user, int mediaToDeleteId)
        {
            var accessUser = new ShareItServices.AccessRightService.UserDTO
            {
                Username = user.Username,
                Password = user.Password
            };
            if (CheckAccessRights(accessUser, mediaToDeleteId) != 3)
                return false;
            using (var proxy = new MediaItemServiceClient())
            {
                proxy.DeleteMediaItem(user, mediaToDeleteId, Resources.ClientToken);
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
        public void MapTags(MediaItemDTO result, List<string> tags)
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

        public void MapGenres(MediaItemDTO result, List<string> genres)
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

        public void MapPrice(MediaItemDTO result, float? price)
        {
            if (price != null)
                result.Information.Add(new MediaItemInformationDTO
                {
                    Data = price.ToString(),
                    Type = InformationTypeDTO.Price
                });
        }

        public void MapTitle(MediaItemDTO result, string title)
        {
            if (!string.IsNullOrWhiteSpace(title))
                result.Information.Add(new MediaItemInformationDTO
                {
                    Data = title,
                    Type = InformationTypeDTO.Title
                });
        }

        public void MapDescription(MediaItemDTO dto, string desc)
        {
            if (!string.IsNullOrWhiteSpace(desc))
                dto.Information.Add(new MediaItemInformationDTO
                {
                    Data = desc, 
                    Type = InformationTypeDTO.Description
                });
        }

        public void MapTrackLength(MediaItemDTO result, string trackLength)
        {
            if (!string.IsNullOrWhiteSpace(trackLength))
                result.Information.Add(new MediaItemInformationDTO
                {
                    Data = trackLength,
                    Type = InformationTypeDTO.TrackLength
                });
        }

        public void MapReleaseDate(MediaItemDTO result, DateTime? releaseDate)
        {
            if (releaseDate != null)
                result.Information.Add(new MediaItemInformationDTO
                {
                    Data = releaseDate.ToString(),
                    Type = InformationTypeDTO.ReleaseDate
                });
        }

        public void MapArtist(MediaItemDTO result, string artist)
        {
            if (!string.IsNullOrWhiteSpace(artist))
                result.Information.Add(new MediaItemInformationDTO
                {
                    Data = artist,
                    Type = InformationTypeDTO.Artist
                });
        }

        public void MapLanguage(MediaItemDTO result, string language)
        {
            if (!string.IsNullOrWhiteSpace(language))
                result.Information.Add(new MediaItemInformationDTO
                {
                    Data = language,
                    Type = InformationTypeDTO.Language
                });
        }

        public void MapDirector(MediaItemDTO result, string director)
        {
            if (!string.IsNullOrWhiteSpace(director))
                result.Information.Add(new MediaItemInformationDTO
                {
                    Data = director,
                    Type = InformationTypeDTO.Director
                });
        }

        public void MapCastMembers(MediaItemDTO result, List<string> castMembers)
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

        public void MapNumberOfPages(MediaItemDTO result, int? numberOfPages)
        {
            if (numberOfPages != null)
                result.Information.Add(new MediaItemInformationDTO
                {
                    Data = numberOfPages.ToString(),
                    Type = InformationTypeDTO.NumberOfPages
                });
        }

        public void MapAuthor(MediaItemDTO result, string author)
        {
            if (!string.IsNullOrWhiteSpace(author))
                result.Information.Add(new MediaItemInformationDTO
                {
                    Data = author,
                    Type = InformationTypeDTO.Author
                });
        }

        public void MapRuntime(MediaItemDTO result, string runtime)
        {
            if (!string.IsNullOrWhiteSpace(runtime))
                result.Information.Add(new MediaItemInformationDTO
                {
                    Data = runtime,
                    Type = InformationTypeDTO.Runtime
                });
        }

        public void MapThumbnail(MediaItemDTO result, string thumbnail)
        {
            if (!string.IsNullOrWhiteSpace(thumbnail))
                result.Information.Add(new MediaItemInformationDTO
                {
                    Data = thumbnail,
                    Type = InformationTypeDTO.Thumbnail
                });
        }

        #endregion
        #region DTO Mappers
        public MediaItemDTO MapDefault(IDetailsModel model)
        {
            var result = new MediaItemDTO { Information = new List<MediaItemInformationDTO>() };
            if (model.ProductId <= 0) { throw new ArgumentException("Invalid product ID"); }
            result.Id = model.ProductId;
            MapDescription(result, model.Description);
            MapTitle(result, model.Title);
            MapPrice(result, model.Price);
            MapGenres(result, model.Genres);
            MapTags(result, model.Tags);
            MapThumbnail(result, model.Thumbnail);
            return result;
        }
        public MediaItemDTO MapMusic(MusicDetailsModel music)
        {
            var result = MapDefault(music);
            result.Type = MediaItemTypeDTO.Music;
            MapArtist(result, music.Artist);
            MapReleaseDate(result, music.ReleaseDate);
            MapTrackLength(result, music.TrackLength);
            return result;
        }

        public MediaItemDTO MapMovie(MovieDetailsModel movie)
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

        public MediaItemDTO MapBook(BookDetailsModel book)
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