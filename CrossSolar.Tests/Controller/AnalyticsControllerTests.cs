using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrossSolar.Controllers;
using CrossSolar.Domain;
using CrossSolar.Models;
using CrossSolar.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace CrossSolar.Tests.Controller
{
    public class AnalyticsControllerTests
    {
        private AnalyticsController _analyticsController;

        private Mock<IPanelRepository> _panelRepositoryMock = new Mock<IPanelRepository>();
        private Mock<IAnalyticsRepository> _analyticsRepositoryMock = new Mock<IAnalyticsRepository>();

        public AnalyticsControllerTests()
        {
            //Mock Data
            var panels = new List<Panel>()
            {
                new Panel
                {
                    Brand = "Areva",
                    Latitude = 12.345678,
                    Longitude = 98.7655432,
                    Serial = "AAAA1111BBBB2222"
                }
            }.AsQueryable();

            var electricityModels = new List<OneHourElectricity>()
            {
                new OneHourElectricity
                {
                    Id = 1,
                    KiloWatt = 1500,
                    PanelId = "AAAA1111BBBB2222",
                    DateTime = new System.DateTime(2018,6,8,13,0,0)
                },
                new OneHourElectricity
                {
                    Id = 2,
                    KiloWatt = 1200,
                    PanelId = "AAAA1111BBBB2222",
                    DateTime = new System.DateTime(2018,6,8,14,0,0)
                }
                ,
                new OneHourElectricity
                {
                    Id = 3,
                    KiloWatt = 1650,
                    PanelId = "AAAA1111BBBB2222",
                    DateTime = new System.DateTime(2018,6,8,15,0,0)
                }
            }.AsQueryable();

            //Mock Queries
            var mockSet = new Mock<DbSet<Panel>>();
            mockSet.As<IAsyncEnumerable<Panel>>()
                .Setup(m => m.GetEnumerator())
                .Returns(new TestIAsyncEnumerator<Panel>(panels.GetEnumerator()));

            mockSet.As<IQueryable<Panel>>()
                .Setup(m => m.Provider)
                .Returns(new TestIAsyncQueryProvider<Panel>(panels.Provider));

            mockSet.As<IQueryable<Panel>>().Setup(m => m.Expression).Returns(panels.Expression);
            mockSet.As<IQueryable<Panel>>().Setup(m => m.ElementType).Returns(panels.ElementType);
            mockSet.As<IQueryable<Panel>>().Setup(m => m.GetEnumerator()).Returns(panels.GetEnumerator());

            _panelRepositoryMock.Setup(c => c.Query()).Returns(mockSet.Object);


            var mockSet2 = new Mock<DbSet<OneHourElectricity>>();
            mockSet2.As<IAsyncEnumerable<OneHourElectricity>>()
                .Setup(m => m.GetEnumerator())
                .Returns(new TestIAsyncEnumerator<OneHourElectricity>(electricityModels.GetEnumerator()));

            mockSet2.As<IQueryable<OneHourElectricity>>()
                .Setup(m => m.Provider)
                .Returns(new TestIAsyncQueryProvider<OneHourElectricity>(electricityModels.Provider));

            mockSet2.As<IQueryable<OneHourElectricity>>().Setup(m => m.Expression).Returns(electricityModels.Expression);
            mockSet2.As<IQueryable<OneHourElectricity>>().Setup(m => m.ElementType).Returns(electricityModels.ElementType);
            mockSet2.As<IQueryable<OneHourElectricity>>().Setup(m => m.GetEnumerator()).Returns(electricityModels.GetEnumerator());

            _analyticsRepositoryMock.Setup(c => c.Query()).Returns(mockSet2.Object);

            _analyticsController = new AnalyticsController(_analyticsRepositoryMock.Object, _panelRepositoryMock.Object);
        }

        [Fact]
        public async Task Register_ShouldInsertOneHourElectricity()
        {

            var oneHourElectricityContent = new OneHourElectricityModel
            {
                KiloWatt = 1500,
            };

            // Act
            var result = await _analyticsController.Post("AAAA1111BBBB2222", oneHourElectricityContent);

            // Assert
            Assert.NotNull(result);

            var createdResult = result as CreatedResult;
            Assert.NotNull(createdResult);
            Assert.Equal(201, createdResult.StatusCode);
        }

        [Fact]
        public async Task Get_OneHourElectricityList()
        {
            // Act
            var result = await _analyticsController.Get("AAAA1111BBBB2222");

            // Assert
            Assert.NotNull(result);

            var createdResult = result as OkObjectResult;
            Assert.NotNull(createdResult);
            var results = ((OneHourElectricityListModel)createdResult.Value).OneHourElectricitys;
            Assert.Equal(3, results.Count<OneHourElectricityModel>());
        }

        [Fact]
        public async Task Get_DayResultsList()
        {
            // Act
            var result = await _analyticsController.DayResults("AAAA1111BBBB2222");

            // Assert
            Assert.NotNull(result);

            var createdResult = result as OkObjectResult;
            Assert.NotNull(createdResult);
            var results = createdResult.Value as List<OneDayElectricityModel>;
            Assert.Single(results);
        }
    }

}
