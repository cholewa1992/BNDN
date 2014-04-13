using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ArtShare.Models;
using ArtShare.Properties;
using ShareItServices.MediaItemService;

namespace ArtShare.Logic
{
    public class BookLogic : IBookLogic
    {


        public BookDetailsModel GetBookDetailsModel(int id, int? requestingUser)
        {
            MediaItemDTO dto;

            using (var ms = new MediaItemServiceClient())
            {
                dto = ms.GetMediaItemInformation(id, requestingUser, Resources.ClientToken);
            }

            return ExstractInformationFromDto(dto);
        }



        public bool DeleteBook(int id, int requestingUser)
        {
            throw new NotImplementedException();
        }



        public bool EditBook(BookDetailsModel model, int requestingUser)
        {
            throw new NotImplementedException();
        }



        public BookDetailsModel ExstractInformationFromDto(MediaItemDTO dto)
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
                        model.Genres.Add(v.Data);
                        break;

                    case InformationTypeDTO.KeywordTag:
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
    }
}