using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using ArtShare.Models;
using ArtShare.Properties;
using ShareItServices.MediaItemService;

namespace ArtShare.Logic
{
    public class MovieLogic : IMovieLogic
    {

        public MovieDetailsModel GetMovieDetailsModel(int id, int? requestingUser)
        {

            MediaItemDTO serviceDTO;

            using (var ms = new MediaItemServiceClient())
            {
                serviceDTO = ms.GetMediaItemInformation(id, requestingUser, Resources.ClientToken);
            }

            return ConvertFromServiceDto(serviceDTO);
        }


        public bool DeleteMovie(int id, int requestingUser)
        {
            throw new NotImplementedException();
        }

        public bool EditMovie(MovieDetailsModel model, int requestingUser)
        {
            throw new NotImplementedException();
        }


        public MovieDetailsModel ConvertFromServiceDto(MediaItemDTO dto)
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
                        model.Genres.Add(v.Data);
                        break;

                    case InformationTypeDTO.KeywordTag:
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
    }
}