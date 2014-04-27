using System;
using System.Collections.Generic;
using System.Linq;
using BusinessLogicLayer.DataMappers;
using BusinessLogicLayer.DTO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BusinessLogicTests
{
    [TestClass]
    public class MediaMapperTest
    {
        [TestMethod]
        public void TestMapToEntityMapsCorrectNumberOfInformation()
        {
            var target = new MediaItemMapper();
            var input = new MediaItemDTO()
            {
                Information = new MediaItemInformationDTO[]
                {
                    new MediaItemInformationDTO(), new MediaItemInformationDTO(), new MediaItemInformationDTO()
                }
            };

            var output = target.MapToEntity(input);
            Assert.AreEqual(3, output.EntityInfo.Count);
        }

        [TestMethod]
        public void TestMapToEntityMapsMediaItemTypeCorrectly()
        {
            var target = new MediaItemMapper();
            var input = new MediaItemDTO()
            {
                Type = MediaItemTypeDTO.Book
            };
            var output = target.MapToEntity(input);
            Assert.AreEqual(2, output.TypeId);
        }

        [TestMethod]
        public void TestMapToEntityMapsDataInInformationCorrectly()
        {
            var target = new MediaItemMapper();
            var input = new MediaItemDTO()
            {
                Information = new List<MediaItemInformationDTO>()
                {
                    new MediaItemInformationDTO()
                    {
                        Data = "data",
                        Type = InformationTypeDTO.Title
                    }
                }
            };
            var output = target.MapToEntity(input);
            Assert.AreEqual(1, output.EntityInfo.First().EntityInfoTypeId);
            Assert.AreEqual("data", output.EntityInfo.First().Data);
        }
    }
}
