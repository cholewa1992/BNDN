using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using ArtShare.Models;
using ArtShare.Properties;
using Microsoft.Ajax.Utilities;
using ShareItServices.TransferService;

namespace ArtShare.Logic
{
    public class TransferLogic : ITransferLogic
    {
        private readonly Dictionary<Type, Func<IDetailsModel, MediaItemDTO>>  _mappingDictionary;

        public TransferLogic()
        {
            _mappingDictionary = new Dictionary<Type, Func<IDetailsModel, MediaItemDTO>>
            {
                {typeof (BookDetailsModel), MapBook},
                {typeof (MovieDetailsModel), MapMovie},
                {typeof (MusicDetailsModel), MapMusic}
            };
        }



        public int UploadFile(IUploadModel model, UserDTO user, IDetailsModel details)
        {
            Func<IDetailsModel, MediaItemDTO> mapper = MapDefault;
            var detailsType = details.GetType();
            if(_mappingDictionary.ContainsKey(detailsType))
                mapper = _mappingDictionary[details.GetType()];

            var metaData = mapper(details);
            int result = -1;
            using (var proxy = new TransferServiceClient())
            {
                var fileExtension = Path.GetExtension(model.File.FileName);
                metaData.FileExtension = fileExtension;
                result = proxy.UploadMedia(Resources.ClientToken, model.File.ContentLength, metaData, user,
                    model.File.InputStream);
                fileExtension = Path.GetExtension(model.Thumbnail.FileName);
                proxy.UploadThumbnail(Resources.ClientToken, model.Thumbnail.ContentLength, fileExtension, result, user,
                    model.Thumbnail.InputStream);
            }
            return result;
        }

        public DownloadModel DownloadFile(UserDTO user, int mediaItem)
        {
            string fileExtension;
            Stream fileStream;

            using (var ts = new TransferServiceClient())
            {
                ts.DownloadMedia(user, mediaItem, Resources.ClientToken, out fileExtension, out fileStream);
            }

            return new DownloadModel() {Stream = fileStream, FileExtension = fileExtension};
        }

        #region Information Mappers
        private void MapTags(MediaItemDTO result, List<string> tags)
        {
            if (tags == null)
                return;
            foreach (var tag in tags)
            {
                if(!string.IsNullOrWhiteSpace(tag))
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
                if(!string.IsNullOrWhiteSpace(genre))
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
            if(!string.IsNullOrWhiteSpace(title))
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
            if(!string.IsNullOrWhiteSpace(language))
                result.Information.Add(new MediaItemInformationDTO
                {
                    Data = language,
                    Type = InformationTypeDTO.Language
                });
        }

        private void MapDirector(MediaItemDTO result, string director)
        {
            if(!string.IsNullOrWhiteSpace(director))
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
                if(!string.IsNullOrWhiteSpace(castMember))
                    result.Information.Add(new MediaItemInformationDTO
                    {
                        Data = castMember,
                        Type = InformationTypeDTO.CastMember
                    });
            }
        }

        private void MapNumberOfPages(MediaItemDTO result, int? numberOfPages)
        {
            if(numberOfPages != null)
                result.Information.Add(new MediaItemInformationDTO
                {
                    Data = numberOfPages.ToString(),
                    Type = InformationTypeDTO.NumberOfPages
                });
        }

        private void MapAuthor(MediaItemDTO result, string author)
        {
            if(!string.IsNullOrWhiteSpace(author))
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
            var result = new MediaItemDTO { Information = new List<MediaItemInformationDTO>()};
            MapDescription(result, model.Description);
            MapTitle(result, model.Title);
            MapPrice(result, model.Price);
            MapGenres(result, model.Genres);
            MapTags(result, model.Tags);
            return result;
        }
        private MediaItemDTO MapMusic(IDetailsModel model)
        {
            var result = MapDefault(model);
            result.Type = MediaItemTypeDTO.Music;
            var music = (MusicDetailsModel) model;
            MapArtist(result, music.Artist);
            MapReleaseDate(result, music.ReleaseDate);
            MapTrackLength(result, music.TrackLength);
            return result;
        }

        private MediaItemDTO MapMovie(IDetailsModel model)
        {
            var result = MapDefault(model);
            result.Type = MediaItemTypeDTO.Movie;
            var movie = (MovieDetailsModel) model;
            MapRuntime(result, movie.Runtime);
            MapReleaseDate(result, movie.ReleaseDate);
            MapCastMembers(result, movie.CastMembers);
            MapDirector(result, movie.Director);
            MapLanguage(result, movie.Language);
            return result;
        }

        private MediaItemDTO MapBook(IDetailsModel model)
        {
            var result = MapDefault(model);
            result.Type = MediaItemTypeDTO.Book;
            var book = (BookDetailsModel) model;
            MapReleaseDate(result, book.ReleaseDate);
            MapLanguage(result, book.Language);
            MapAuthor(result, book.Author);
            MapNumberOfPages(result, book.NumberOfPages);
            return result;
        }
        #endregion

    }
}