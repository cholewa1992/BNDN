using System;
using System.Web.Mvc;
using ArtShare.Controllers;
using ArtShare.Logic;
using ArtShare.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClientUnitTest
{
    [TestClass]
    public class AccountControllerTest
    {

        AccountController ac = new AccountController(new AccountLogicStub());

        [TestMethod]
        public void Register_ExceptionThrown_ErrorSet()
        {
            var model = new RegisterModel() {Username = "throw"};
            var result = ac.Register(model) as ViewResult;
            var resultModel = result.Model as RegisterModel;
            Assert.AreEqual("error", resultModel.Error);

        }

        [TestMethod]
        public void Register_ValidInput_ViewChanged()
        {
            var model = new RegisterModel() { Username = "nothrow" };
            var result = ac.Register(model) as RedirectToRouteResult;
            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.IsNull(result.RouteValues["controller"]);
        }
    }
}
