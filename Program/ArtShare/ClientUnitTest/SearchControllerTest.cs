using System;
using System.Text;
using System.Collections.Generic;
using System.Web.Mvc;
using ArtShare.Controllers;
using ArtShare.Logic;
using ArtShare.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClientUnitTest
{
    [TestClass]
    public class SearchControllerTest
    {
        private SearchController _searchController = new SearchController(new SearchLogicStub());

/*        [TestMethod]
        public void SearchMediaItems_ValidInput_SearchModel()
        {
            var actionResult = _searchController.SearchMediaItems(1, 10, "Book") as ViewResult;
            var searchModel = actionResult.ViewData.Model as SearchModel;
            Assert.AreEqual(4, searchModel.NumberOfMatchingBooks); //I have 4 books in the stub. Ignoring range here
           
        }*/
    }
}
