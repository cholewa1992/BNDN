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
        private Dictionary<MediaItemTypeDTO, MediaItemSearchResultDTO> _dictionary;

        #region Setup
        [TestInitialize]
        public void Setup()
        {
            _dictionary = SetupDictionary();
        }

        private static Dictionary<MediaItemTypeDTO, MediaItemSearchResultDTO> SetupDictionary()
        {
            var dictionary = new Dictionary<MediaItemTypeDTO, MediaItemSearchResultDTO>();
            var bookList = new List<MediaItemDTO>();
            for (int i = 0; i <= 3; i++)
            {
                bookList.Add(new MediaItemDTO
                {
                    Id = i,
                    Type = MediaItemTypeDTO.Book,
                    Information = new List<MediaItemInformationDTO>
                    {
                        new MediaItemInformationDTO
                        {
                            Type = InformationTypeDTO.Title,
                            Data = "BookTitle " + i
                        }
                    }
                });
            }

            var movieList = new List<MediaItemDTO>();
            for (int i = 0; i <= 3; i++)
            {
                movieList.Add(new MediaItemDTO
                {
                    Id = i,
                    Type = MediaItemTypeDTO.Movie,
                    Information = new List<MediaItemInformationDTO> 
                    {
                        new MediaItemInformationDTO
                        {
                            Type = InformationTypeDTO.Title,
                            Data = "MovieTitle " + i
                        }
                    }
                });
            }

            var musicList = new List<MediaItemDTO>();
            for (int i = 0; i <= 3; i++)
            {
                musicList.Add(new MediaItemDTO
                {
                    Id = i,
                    Type = MediaItemTypeDTO.Music,
                    Information = new List<MediaItemInformationDTO> 
                    {
                        new MediaItemInformationDTO
                        {
                            Type = InformationTypeDTO.Title,
                            Data = "MusicTitle " + i
                        }
                    }
                });
            }

            dictionary.Add(MediaItemTypeDTO.Book, new MediaItemSearchResultDTO
            {
                NumberOfSearchResults = 4,
                MediaItemList = bookList
            });
            dictionary.Add(MediaItemTypeDTO.Movie, new MediaItemSearchResultDTO
            {
                NumberOfSearchResults = 4,
                MediaItemList = movieList
            });
            dictionary.Add(MediaItemTypeDTO.Music, new MediaItemSearchResultDTO
            {
                NumberOfSearchResults = 4,
                MediaItemList = musicList
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
        public void PrepareSearchModel_BookListNull_ArgumentNullException()
        {
            var newDict = _dictionary;
            newDict[MediaItemTypeDTO.Book] = null;
            var actual = _searchLogic.PrepareSearchModel(newDict);
            Assert.IsNull(actual.Books);
        }

        
    }
}
