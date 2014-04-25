using System;
using System.Text;
using System.Collections.Generic;
using ArtShare.Logic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShareItServices.MediaItemService;

namespace ClientUnitTest
{
    
    [TestClass]
    public class DetailsLogicUnitTest
    {


        private DetailsLogic _logic = new DetailsLogic();


        [TestMethod]
        public void ExstractionBookDto_Title_BookModelWithTitle()
        {

            var dto = new MediaItemDTO()
            {
                Information = new[]
                {
                    new MediaItemInformationDTO()
                    {
                        Type = InformationTypeDTO.Title,
                        Data = "BookTitle"
                    }, 
                }
            };

            var result = _logic.ExstractBookInformation(dto);

            Assert.AreEqual("BookTitle", result.Title);
        }


        [TestMethod]
        public void ExstractionMovieDto_TwoCasts_ListOfTwoCasts()
        {

            var dto = new MediaItemDTO()
            {
                Information = new[]
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

            var result = _logic.ExstractMovieInformation(dto);

            Assert.AreEqual(2, result.CastMembers.Count);
        }


        [TestMethod]
        public void ExstractionBookDto__ReleaseDateString_CorrectParse()
        {

            var dto = new MediaItemDTO()
            {
                Information = new[]
                {
                    new MediaItemInformationDTO()
                    {
                        Type = InformationTypeDTO.ReleaseDate,
                        Data = "1/1/2014"
                    }
                }
            };

            var result = _logic.ExstractMovieInformation(dto);

            Assert.AreEqual("01-01-2014 00:00:00", result.ReleaseDate.ToString());
        }


        [TestMethod]
        public void ExstractionBookDto__InvalidReleaseDateString_PropertyIsNull()
        {
            
            var dto = new MediaItemDTO()
            {
                Information = new[]
                {
                    new MediaItemInformationDTO()
                    {
                        Type = InformationTypeDTO.ReleaseDate,
                        Data = "1/1//2000019"
                    }
                }
            };

            var result = _logic.ExstractMovieInformation(dto);

            Assert.AreEqual(null, result.ReleaseDate);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RateMediaItem_UserIsNull_ArgumentNullException()
        {
            _logic.RateMediaItem(null, 1, 10);
        }

    }
}
