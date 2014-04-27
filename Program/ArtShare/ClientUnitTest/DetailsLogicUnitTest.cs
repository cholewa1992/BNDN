using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Web.Mvc;
using ArtShare.Logic;
using ArtShare.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShareItServices.MediaItemService;

namespace ClientUnitTest
{
    
    [TestClass]
    public class DetailsLogicUnitTest
    {


        private DetailsLogic _logic = new DetailsLogic();


        [TestMethod]
        public void ExtractionBookDto_Title_BookModelWithTitle()
        {

            var dto = new MediaItemDTO()
            {
                Information = new List<MediaItemInformationDTO>()
                {
                    new MediaItemInformationDTO()
                    {
                        Type = InformationTypeDTO.Title,
                        Data = "BookTitle"
                    }, 
                }
            };

            var result = _logic.ExtractBookInformation(dto);

            Assert.AreEqual("BookTitle", result.Title);
        }


        [TestMethod]
        public void ExtractionMovieDto_TwoCasts_ListOfTwoCasts()
        {

            var dto = new MediaItemDTO()
            {
                Information = new List<MediaItemInformationDTO>()
                {
                    new MediaItemInformationDTO()
                    {
                        Type = InformationTypeDTO.CastMember,
                        Data = "Member1"
                    },
                    new MediaItemInformationDTO()
                    {
                        Type = InformationTypeDTO.CastMember,
                        Data = "Member2"
                    }
                }
            };

            var result = _logic.ExtractMovieInformation(dto);

            Assert.AreEqual(2, result.CastMembers.Count);
        }


        [TestMethod]
        public void ExtractionBookDto__ReleaseDateString_CorrectParse()
        {

            var dto = new MediaItemDTO()
            {
                Information = new List<MediaItemInformationDTO>()
                {
                    new MediaItemInformationDTO()
                    {
                        Type = InformationTypeDTO.ReleaseDate,
                        Data = "1/1/2014"
                    }
                }
            };

            var result = _logic.ExtractMovieInformation(dto);

            Assert.AreEqual("01-01-2014 00:00:00", result.ReleaseDate.ToString());
        }


        [TestMethod]
        public void ExtractionBookDto__InvalidReleaseDateString_PropertyIsNull()
        {
            
            var dto = new MediaItemDTO()
            {
                Information = new List<MediaItemInformationDTO>()
                {
                    new MediaItemInformationDTO()
                    {
                        Type = InformationTypeDTO.ReleaseDate,
                        Data = "1/1//2000019"
                    }
                }
            };

            var result = _logic.ExtractMovieInformation(dto);

            Assert.AreEqual(null, result.ReleaseDate);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RateMediaItem_UserIsNull_ArgumentNullException()
        {
            _logic.RateMediaItem(null, 1, 10);
        }


        [TestMethod]
        public void AvgRatingExtraction__ValidProperty_CorrectExtraction()
        {

            var dto = new MediaItemDTO()
            {
                AverageRating = 5.5,
                Information = new List<MediaItemInformationDTO>()
            };

            var result = _logic.ExtractMovieInformation(dto);

            Assert.AreEqual(5.5, result.AvgRating);
        }

        [TestMethod]
        public void MapTags_AddOneTag()
        {
            var dto = new MediaItemDTO()
            {
                Information = new List<MediaItemInformationDTO>()
            };

            var tags = new List<string>
            {
                "TestTag"
            };

            _logic.MapTags(dto, tags);

            Assert.AreEqual("TestTag", dto.Information.Where(x => x.Type == InformationTypeDTO.KeywordTag)
                .Select(x => x.Data).FirstOrDefault());
        }

        [TestMethod]
        public void MapTags_AddNoTags()
        {
            var dto = new MediaItemDTO()
            {
                Information = new List<MediaItemInformationDTO>()
            };

            var tags = new List<string>();

            _logic.MapTags(dto, tags);

            Assert.AreEqual(0, dto.Information.Where(x => x.Type == InformationTypeDTO.KeywordTag)
                .Select(x => x).Count());
        }

        [TestMethod]
        public void MapGenres_AddOneGenre()
        {
            var dto = new MediaItemDTO()
            {
                Information = new List<MediaItemInformationDTO>()
            };

            var tags = new List<string>
            {
                "TestGenre"
            };

            _logic.MapGenres(dto, tags);

            Assert.AreEqual("TestGenre", dto.Information.Where(x => x.Type == InformationTypeDTO.Genre)
                .Select(x => x.Data).FirstOrDefault());
        }

        [TestMethod]
        public void MapGenres_AddNoGenres()
        {
            var dto = new MediaItemDTO()
            {
                Information = new List<MediaItemInformationDTO>()
            };

            var tags = new List<string>();

            _logic.MapGenres(dto, tags);

            Assert.AreEqual(0, dto.Information.Where(x => x.Type == InformationTypeDTO.Genre)
                .Select(x => x).Count());
        }

        [TestMethod]
        public void MapPrice_AddPrice()
        {
            var dto = new MediaItemDTO()
            {
                Information = new List<MediaItemInformationDTO>()
            };

            var tags = 10;

            _logic.MapPrice(dto, tags);

            Assert.AreEqual("10", dto.Information.Where(x => x.Type == InformationTypeDTO.Price)
                .Select(x => x.Data).FirstOrDefault());
        }

        [TestMethod]
        public void MapPrice_AddNullPrice()
        {
            var dto = new MediaItemDTO()
            {
                Information = new List<MediaItemInformationDTO>()
            };

            float? tags = null;

            _logic.MapPrice(dto, tags);

            Assert.AreEqual(0, dto.Information.Where(x => x.Type == InformationTypeDTO.Price)
                .Select(x => x).Count());
        }

        [TestMethod]
        public void MapTitle_AddTitle()
        {
            var dto = new MediaItemDTO()
            {
                Information = new List<MediaItemInformationDTO>()
            };

            var tags = "TestTitle";

            _logic.MapTitle(dto, tags);

            Assert.AreEqual("TestTitle", dto.Information.Where(x => x.Type == InformationTypeDTO.Title)
                .Select(x => x.Data).FirstOrDefault());
        }

        [TestMethod]
        public void MapTitle_AddEmptyTitle()
        {
            var dto = new MediaItemDTO()
            {
                Information = new List<MediaItemInformationDTO>()
            };

            var tags = "";

            _logic.MapTitle(dto, tags);

            Assert.AreEqual(0, dto.Information.Where(x => x.Type == InformationTypeDTO.Title)
                .Select(x => x).Count());
        }

        [TestMethod]
        public void MapDescription_AddDescription()
        {
            var dto = new MediaItemDTO()
            {
                Information = new List<MediaItemInformationDTO>()
            };

            var tags = "TestDescription";

            _logic.MapDescription(dto, tags);

            Assert.AreEqual("TestDescription", dto.Information.Where(x => x.Type == InformationTypeDTO.Description)
                .Select(x => x.Data).FirstOrDefault());
        }

        [TestMethod]
        public void MapDescription_AddEmptyDescription()
        {
            var dto = new MediaItemDTO()
            {
                Information = new List<MediaItemInformationDTO>()
            };

            var tags = "";

            _logic.MapDescription(dto, tags);

            Assert.AreEqual(0, dto.Information.Where(x => x.Type == InformationTypeDTO.Description)
                .Select(x => x).Count());
        }

        [TestMethod]
        public void MapTrackLength_AddTrackLength()
        {
            var dto = new MediaItemDTO()
            {
                Information = new List<MediaItemInformationDTO>()
            };

            var tags = "TestTrackLength";

            _logic.MapTrackLength(dto, tags);

            Assert.AreEqual("TestTrackLength", dto.Information.Where(x => x.Type == InformationTypeDTO.TrackLength)
                .Select(x => x.Data).FirstOrDefault());
        }

        [TestMethod]
        public void MapTrackLength_AddEmptyTrackLength()
        {
            var dto = new MediaItemDTO()
            {
                Information = new List<MediaItemInformationDTO>()
            };

            var tags = "";

            _logic.MapTrackLength(dto, tags);

            Assert.AreEqual(0, dto.Information.Where(x => x.Type == InformationTypeDTO.TrackLength)
                .Select(x => x).Count());
        }

        [TestMethod]
        public void MapReleaseDate_AddReleaseDate()
        {
            var dto = new MediaItemDTO()
            {
                Information = new List<MediaItemInformationDTO>()
            };

            var tags = new DateTime(2000, 01, 01);

            _logic.MapReleaseDate(dto, tags);

            Assert.AreEqual(tags.ToString(), dto.Information.Where(x => x.Type == InformationTypeDTO.ReleaseDate)
                .Select(x => x.Data).FirstOrDefault());
        }

        [TestMethod]
        public void MapReleaseDate_AddNullReleaseDate()
        {
            var dto = new MediaItemDTO()
            {
                Information = new List<MediaItemInformationDTO>()
            };

            DateTime? tags = null;

            _logic.MapReleaseDate(dto, tags);

            Assert.AreEqual(0, dto.Information.Where(x => x.Type == InformationTypeDTO.ReleaseDate)
                .Select(x => x).Count());
        }

        [TestMethod]
        public void MapArtist_AddArtist()
        {
            var dto = new MediaItemDTO()
            {
                Information = new List<MediaItemInformationDTO>()
            };

            var tags = "TestArtist";

            _logic.MapArtist(dto, tags);

            Assert.AreEqual("TestArtist", dto.Information.Where(x => x.Type == InformationTypeDTO.Artist)
                .Select(x => x.Data).FirstOrDefault());
        }

        [TestMethod]
        public void MapArtist_AddEmptyArtist()
        {
            var dto = new MediaItemDTO()
            {
                Information = new List<MediaItemInformationDTO>()
            };

            var tags = "";

            _logic.MapArtist(dto, tags);

            Assert.AreEqual(0, dto.Information.Where(x => x.Type == InformationTypeDTO.Artist)
                .Select(x => x).Count());
        }

        [TestMethod]
        public void MapLanguage_AddLanguage()
        {
            var dto = new MediaItemDTO()
            {
                Information = new List<MediaItemInformationDTO>()
            };

            var tags = "TestLanguage";

            _logic.MapLanguage(dto, tags);

            Assert.AreEqual("TestLanguage", dto.Information.Where(x => x.Type == InformationTypeDTO.Language)
                .Select(x => x.Data).FirstOrDefault());
        }

        [TestMethod]
        public void MapLanguage_AddEmptyLanguage()
        {
            var dto = new MediaItemDTO()
            {
                Information = new List<MediaItemInformationDTO>()
            };

            var tags = "";

            _logic.MapLanguage(dto, tags);

            Assert.AreEqual(0, dto.Information.Where(x => x.Type == InformationTypeDTO.Language)
                .Select(x => x).Count());
        }

        [TestMethod]
        public void MapDirector_AddDirector()
        {
            var dto = new MediaItemDTO()
            {
                Information = new List<MediaItemInformationDTO>()
            };

            var tags = "TestDirector";

            _logic.MapDirector(dto, tags);

            Assert.AreEqual("TestDirector", dto.Information.Where(x => x.Type == InformationTypeDTO.Director)
                .Select(x => x.Data).FirstOrDefault());
        }

        [TestMethod]
        public void MapDirector_AddEmptyDirector()
        {
            var dto = new MediaItemDTO()
            {
                Information = new List<MediaItemInformationDTO>()
            };

            var tags = "";

            _logic.MapDirector(dto, tags);

            Assert.AreEqual(0, dto.Information.Where(x => x.Type == InformationTypeDTO.Director)
                .Select(x => x).Count());
        }

        [TestMethod]
        public void MapCastMembers_AddOneCastMember()
        {
            var dto = new MediaItemDTO()
            {
                Information = new List<MediaItemInformationDTO>()
            };

            var tags = new List<string>
            {
                "TestCastMember"
            };

            _logic.MapCastMembers(dto, tags);

            Assert.AreEqual("TestCastMember", dto.Information.Where(x => x.Type == InformationTypeDTO.CastMember)
                .Select(x => x.Data).FirstOrDefault());
        }

        [TestMethod]
        public void MapCastMembers_AddNoCastMembers()
        {
            var dto = new MediaItemDTO()
            {
                Information = new List<MediaItemInformationDTO>()
            };

            var tags = new List<string>();

            _logic.MapCastMembers(dto, tags);

            Assert.AreEqual(0, dto.Information.Where(x => x.Type == InformationTypeDTO.CastMember)
                .Select(x => x).Count());
        }

        [TestMethod]
        public void MapNumberOfPages_AddNumberOfPages()
        {
            var dto = new MediaItemDTO()
            {
                Information = new List<MediaItemInformationDTO>()
            };

            var tags = 100;

            _logic.MapNumberOfPages(dto, tags);

            Assert.AreEqual("100", dto.Information.Where(x => x.Type == InformationTypeDTO.NumberOfPages)
                .Select(x => x.Data).FirstOrDefault());
        }

        [TestMethod]
        public void MapNumberOfPages_AddNullNumberOfPages()
        {
            var dto = new MediaItemDTO()
            {
                Information = new List<MediaItemInformationDTO>()
            };

            int? tags = null;

            _logic.MapNumberOfPages(dto, tags);

            Assert.AreEqual(0, dto.Information.Where(x => x.Type == InformationTypeDTO.NumberOfPages)
                .Select(x => x).Count());
        }

        [TestMethod]
        public void MapAuthor_AddAuthor()
        {
            var dto = new MediaItemDTO()
            {
                Information = new List<MediaItemInformationDTO>()
            };

            var tags = "TestAuthor";

            _logic.MapAuthor(dto, tags);

            Assert.AreEqual("TestAuthor", dto.Information.Where(x => x.Type == InformationTypeDTO.Author)
                .Select(x => x.Data).FirstOrDefault());
        }

        [TestMethod]
        public void MapAuthor_AddEmptyAuthor()
        {
            var dto = new MediaItemDTO()
            {
                Information = new List<MediaItemInformationDTO>()
            };

            var tags = "";

            _logic.MapAuthor(dto, tags);

            Assert.AreEqual(0, dto.Information.Where(x => x.Type == InformationTypeDTO.Author)
                .Select(x => x).Count());
        }

        [TestMethod]
        public void MapRuntime_AddRuntime()
        {
            var dto = new MediaItemDTO()
            {
                Information = new List<MediaItemInformationDTO>()
            };

            var tags = "TestRuntime";

            _logic.MapRuntime(dto, tags);

            Assert.AreEqual("TestRuntime", dto.Information.Where(x => x.Type == InformationTypeDTO.Runtime)
                .Select(x => x.Data).FirstOrDefault());
        }

        [TestMethod]
        public void MapRuntime_AddEmptyRuntime()
        {
            var dto = new MediaItemDTO()
            {
                Information = new List<MediaItemInformationDTO>()
            };

            var tags = "";

            _logic.MapRuntime(dto, tags);

            Assert.AreEqual(0, dto.Information.Where(x => x.Type == InformationTypeDTO.Runtime)
                .Select(x => x).Count());
        }

        [TestMethod]
        public void MapDefault_AllDefaultFieldsFilled()
        {
            var model = new BookDetailsModel()
            {
                ProductId = 1,
                Description = "TestDescription",
                Title = "TestTitle",
                Genres = new List<string>()
                {
                    "TestGenre"
                },
                Price = 10,
                Tags = new List<string>()
                {
                    "TestTags"
                }
            };

            var result = _logic.MapDefault(model);

            Assert.AreEqual(model.ProductId, result.Id);
            Assert.AreEqual(model.Description, result.Information
                .Where(x => x.Type == InformationTypeDTO.Description)
                .Select(x => x.Data).FirstOrDefault());
            Assert.AreEqual(model.Title, result.Information
                .Where(x => x.Type == InformationTypeDTO.Title)
                .Select(x => x.Data).FirstOrDefault());
            Assert.AreEqual(model.Genres.FirstOrDefault(), result.Information
                .Where(x => x.Type == InformationTypeDTO.Genre)
                .Select(x => x.Data).FirstOrDefault());
            Assert.AreEqual(model.Price.ToString(), result.Information
                .Where(x => x.Type == InformationTypeDTO.Price)
                .Select(x => x.Data).FirstOrDefault());
            Assert.AreEqual(model.Tags.FirstOrDefault(), result.Information
                .Where(x => x.Type == InformationTypeDTO.KeywordTag)
                .Select(x => x.Data).FirstOrDefault());
        }

        [TestMethod]
        public void MapMusic_AllMusicFieldsFilled()
        {
            var model = new MusicDetailsModel()
            {
                ProductId = 2,
                Artist = "TestArtist",
                TrackLength = "3:30",
                ReleaseDate = new DateTime(2000, 01, 01)
            };

            var result = _logic.MapMusic(model);

            Assert.AreEqual(model.ProductId, result.Id);
            Assert.AreEqual(model.Artist, result.Information.Where(x => x.Type == InformationTypeDTO.Artist)
                .Select(x => x.Data).FirstOrDefault());
            Assert.AreEqual(model.TrackLength, result.Information
                .Where(x => x.Type == InformationTypeDTO.TrackLength)
                .Select(x => x.Data).FirstOrDefault());
            Assert.AreEqual(model.ReleaseDate.ToString(), result.Information
                .Where(x => x.Type == InformationTypeDTO.ReleaseDate)
                .Select(x => x.Data).FirstOrDefault());
        }

        [TestMethod]
        public void MapMovie_AllMovieFieldsFilled()
        {
            var model = new MovieDetailsModel()
            {
                ProductId = 2,
                Runtime = "124",
                CastMembers = new List<string>()
                {
                    "TestCastMember"
                },
                ReleaseDate = new DateTime(2000, 01, 01),
                Director = "TestDirector",
                Language = "TestLanguage"
            };

            var result = _logic.MapMovie(model);

            Assert.AreEqual(model.ProductId, result.Id);
            Assert.AreEqual(model.Runtime, result.Information.Where(x => x.Type == InformationTypeDTO.Runtime)
                .Select(x => x.Data).FirstOrDefault());
            Assert.AreEqual(model.CastMembers.FirstOrDefault(), result.Information
                .Where(x => x.Type == InformationTypeDTO.CastMember)
                .Select(x => x.Data).FirstOrDefault());
            Assert.AreEqual(model.ReleaseDate.ToString(), result.Information
                .Where(x => x.Type == InformationTypeDTO.ReleaseDate)
                .Select(x => x.Data).FirstOrDefault());
            Assert.AreEqual(model.Director, result.Information.Where(x => x.Type == InformationTypeDTO.Director)
                .Select(x => x.Data).FirstOrDefault());
            Assert.AreEqual(model.Language, result.Information.Where(x => x.Type == InformationTypeDTO.Language)
                .Select(x => x.Data).FirstOrDefault());
        }

        [TestMethod]
        public void MapBook_AllBookFieldsFilled()
        {
            var model = new BookDetailsModel()
            {
                ProductId = 2,
                ReleaseDate = new DateTime(2000, 01, 01),
                Language = "TestLanguage",
                Author = "TestAuthor",
                NumberOfPages = 100
            };

            var result = _logic.MapBook(model);

            Assert.AreEqual(model.ProductId, result.Id);
            Assert.AreEqual(model.Author, result.Information.Where(x => x.Type == InformationTypeDTO.Author)
                .Select(x => x.Data).FirstOrDefault());
            Assert.AreEqual(model.ReleaseDate.ToString(), result.Information
                .Where(x => x.Type == InformationTypeDTO.ReleaseDate)
                .Select(x => x.Data).FirstOrDefault());
            Assert.AreEqual(model.NumberOfPages.ToString(), result.Information
                .Where(x => x.Type == InformationTypeDTO.NumberOfPages)
                .Select(x => x.Data).FirstOrDefault());
            Assert.AreEqual(model.Language, result.Information.Where(x => x.Type == InformationTypeDTO.Language)
                .Select(x => x.Data).FirstOrDefault());
        }
    }
}
