using System;
using System.Collections.Generic;
using System.Linq;
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
        public bool PurchaseItem(int mediaId, int requestingUser)
        {

            var requestingUserDto = new ShareItServices.AccessRightService.UserDTO() {Id = requestingUser};
            var expiration = new DateTime(2200, 01, 01);

            bool result;

            using (var ars = new AccessRightServiceClient())
            {
                result = ars.Purchase(requestingUserDto, mediaId, expiration, Resources.ClientToken);
            }

            return result;
        }

        /// <summary>
        /// Retrieves details about a given book and returns it in a book model
        /// </summary>
        /// <param name="id">Id of book item</param>
        /// <param name="requestingUser">Id of the user requesting the details</param>
        /// <returns>Book model with requested information</returns>
        public BookDetailsModel GetBookDetailsModel(int id, int? requestingUser)
        {
            MediaItemDTO dto;

            using (var ms = new MediaItemServiceClient())
            {
                dto = ms.GetMediaItemInformation(id, requestingUser, Resources.ClientToken);
                
            }

            return ExstractBookInformation(dto);
        }

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

        /// <summary>
        /// Updates a book. All previous information will be deleted, and supplied information added
        /// </summary>
        /// <param name="model">New information to add</param>
        /// <param name="requestingUser">The user requesting an edit</param>
        /// <returns>a bool of whether the edit succeeded</returns>
        public bool EditBook(BookDetailsModel model, int requestingUser)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Retrieves details about a given Movie and returns it in a Movie model
        /// </summary>
        /// <param name="id">Id of Movie</param>
        /// <param name="requestingUser">Id of the user requesting the details</param>
        /// <returns>Movie model with requested information</returns>
        public MovieDetailsModel GetMovieDetailsModel(int id, int? requestingUser)
        {

            MediaItemDTO serviceDTO;

            using (var ms = new MediaItemServiceClient())
            {
                serviceDTO = ms.GetMediaItemInformation(id, requestingUser, Resources.ClientToken);
            }

            return ExstractMovieInformation(serviceDTO);
        }
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
        /// <summary>
        /// Updates a Movie. All previous information will be deleted, and supplied information added
        /// </summary>
        /// <param name="model">New information to add</param>
        /// <param name="requestingUser">The user requesting an edit</param>
        /// <returns>a bool of whether the edit succeeded</returns>
        public bool EditMovie(MovieDetailsModel model, int requestingUser)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Retrieves details about given Music and returns it in a Music model
        /// </summary>
        /// <param name="id">Id of Music item</param>
        /// <param name="requestingUser">Id of the user requesting the details</param>
        /// <returns>Music model with requested information</returns>
        public MusicDetailsModel GetMusicDetailsModel(int id, int? requestingUser)
        {

            MediaItemDTO dto;

            using (var ms = new MediaItemServiceClient())
            {
                dto = ms.GetMediaItemInformation(id, requestingUser, Resources.ClientToken);
            }

            return ExstractMusicInformation(dto);
        }
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
        /// Updates a Music item. All previous information will be deleted, and supplied information added
        /// </summary>
        /// <param name="model">New information to add</param>
        /// <param name="requestingUser">The user requesting an edit</param>
        /// <returns>a bool of whether the edit succeeded</returns>
        public bool EditMusic(MusicDetailsModel model, int requestingUser)
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


                    case InformationTypeDTO.ExpirationDate:
                        try
                        {
                            model.Expiration = DateTime.Parse(v.Data);
                        }
                        catch (Exception)
                        {
                            model.Expiration = null;
                        }
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

                    case InformationTypeDTO.ExpirationDate:
                        try
                        {
                            model.Expiration = DateTime.Parse(v.Data);
                        }
                        catch (Exception)
                        {
                            model.Expiration = null;
                        }
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

                    case InformationTypeDTO.ExpirationDate:
                        try
                        {
                            model.Expiration = DateTime.Parse(v.Data);
                        }
                        catch (Exception)
                        {
                            model.Expiration = null;
                        }
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
    }
}