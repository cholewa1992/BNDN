using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ArtShare.Models;
using ArtShare.Properties;
using ShareItServices.MediaItemService;

namespace ArtShare.Logic
{
    public class MusicLogic : IMusicLogic
    {
        public MusicDetailsModel GetMusicDetailsModel(int id, int? requestingUser)
        {

            MediaItemDTO dto;

            using (var ms = new MediaItemServiceClient())
            {
                dto = ms.GetMediaItemInformation(id, requestingUser, Resources.ClientToken);
            }

            return ExstractInformationFromDto(dto);
        }

        public bool DeleteMusic(int id, int requestingUser)
        {
            throw new NotImplementedException();
        }

        public bool EditMusic(MusicDetailsModel model, int requestingUser)
        {
            throw new NotImplementedException();
        }

        public MusicDetailsModel ExstractInformationFromDto(MediaItemDTO dto)
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

                    case InformationTypeDTO.TrackLength:
                        model.TrackLength = v.Data;
                        break;


                }
            }


            return model;



        }
    }
}