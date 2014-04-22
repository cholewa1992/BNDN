using System;
using System.Collections.Generic;
using ArtShare.Logic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ShareItServices.MediaItemService;

namespace ClientUnitTest
{
    [TestClass]
    public class SearchLogicTest
    {
        private SearchLogic _searchLogic = new SearchLogic();
        private static Dictionary<MediaItemTypeDTO, MediaItemSearchResultDTO> _dictionary;

        #region Setup
        [ClassInitialize]
        public static void SetupClass(TestContext context)
        {
            _dictionary = SetupDictionary();
        }

        private static Dictionary<MediaItemTypeDTO, MediaItemSearchResultDTO> SetupDictionary()
        {
            var dictionary = new Dictionary<MediaItemTypeDTO, MediaItemSearchResultDTO>();
            var bookArray = new MediaItemDTO[4];
            for (int i = 0; i <= 3; i++)
            {
                bookArray[i] = new MediaItemDTO
                {
                    Id = i,
                    Type = MediaItemTypeDTO.Book,
                    Information = new MediaItemInformationDTO[] 
                    {
                        new MediaItemInformationDTO
                        {
                            Type = InformationTypeDTO.Title,
                            Data = "BookTitle " + i
                        }
                    }
                };
            }

            var movieArray = new MediaItemDTO[4];
            for (int i = 0; i <= 3; i++)
            {
                movieArray[i] = new MediaItemDTO
                {
                    Id = i,
                    Type = MediaItemTypeDTO.Movie,
                    Information = new MediaItemInformationDTO[] 
                    {
                        new MediaItemInformationDTO
                        {
                            Type = InformationTypeDTO.Title,
                            Data = "MovieTitle " + i
                        }
                    }
                };
            }

            var musicArray = new MediaItemDTO[4];
            for (int i = 0; i <= 3; i++)
            {
                musicArray[i] = new MediaItemDTO
                {
                    Id = i,
                    Type = MediaItemTypeDTO.Music,
                    Information = new MediaItemInformationDTO[] 
                    {
                        new MediaItemInformationDTO
                        {
                            Type = InformationTypeDTO.Title,
                            Data = "MusicTitle " + i
                        }
                    }
                };
            }

            dictionary.Add(MediaItemTypeDTO.Book, new MediaItemSearchResultDTO
            {
                NumberOfSearchResults = 4,
                MediaItemList = bookArray
            });
            dictionary.Add(MediaItemTypeDTO.Movie, new MediaItemSearchResultDTO
            {
                NumberOfSearchResults = 4,
                MediaItemList = movieArray
            });
            dictionary.Add(MediaItemTypeDTO.Music, new MediaItemSearchResultDTO
            {
                NumberOfSearchResults = 4,
                MediaItemList = musicArray
            });

            return dictionary;
        }
        #endregion

        [TestMethod]
        public void PrepareSearchModel_ValidInput_NumberOfMatchingBooks()
        {
            var result = _searchLogic.PrepareSearchModel(_dictionary);
            Assert.AreEqual(4, result.NumberOfMatchingBooks);
        }

        [TestMethod]
        public void PrepareSearchModel_ValidInput_CorrectBookTitle()
        {
            var result = _searchLogic.PrepareSearchModel(_dictionary);
            var actual = result.Books[0];
            Assert.AreEqual("BookTitle 0", actual.Title);
        }

        [TestMethod]
        public void PrepareSearchModel_ValidInput_TotalNumberOfItems()
        {
            var result = _searchLogic.PrepareSearchModel(_dictionary);
            var actual = result.Movies.Count + result.Books.Count + result.Music.Count;
            Assert.AreEqual(12, actual);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void PrepareSearchModel_SearchResultIsNull_ArgumentNullException()
        {
            _searchLogic.PrepareSearchModel(null);
        }

        [TestMethod]
        public void PrepareSearchModel_BookArrayNull_ArgumentNullException()
        {
            var newDict = _dictionary;
            newDict[MediaItemTypeDTO.Book] = null;
            var actual = _searchLogic.PrepareSearchModel(newDict);
            Assert.IsNull(actual.Books);
        }

        
    }
}
