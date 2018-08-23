using CrossSolar.Controllers;
using CrossSolar.Domain;
using CrossSolar.Models;
using CrossSolar.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace CrossSolar.Tests.Controller
{
    public class AnalyticsControllerTests
    {
        private DbContextOptions<CrossSolarDbContext> options;
        private CrossSolarDbContext context;
        private readonly AnalyticsController _analyticsController;

        private IPanelRepository _panelRepository;
        private IAnalyticsRepository _analyticsRepository;
        private IDayAnalyticsRepository _dayAnalyticsRepository;

        /// <summary>
        /// Constructor of the class.
        /// </summary>
        public AnalyticsControllerTests()
        {
            // Using 'UseInMemoryDatabase' instead of throwing the repository, avoiding EF limitations and asynchronous returns. 
            options = new DbContextOptionsBuilder<CrossSolarDbContext>()
               .UseInMemoryDatabase(databaseName: "CrossSolarDbInMemory").Options;

            context = new CrossSolarDbContext(options);

            _panelRepository = new PanelRepository(context);
            _analyticsRepository = new AnalyticsRepository(context);
            _dayAnalyticsRepository = new DayAnalyticsRepository(context);

            _analyticsController = new AnalyticsController(_analyticsRepository, _dayAnalyticsRepository, _panelRepository);
        }


        /// <summary>
        /// Returns NotFound when can not find the panel.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Get_ShouldReturnNotFoundWhenCanNotFindPanel()
        {
            // Act
            var result = await _analyticsController.Get("123");

            // Assert
            Assert.NotNull(result);

            var statusResult = result as NotFoundResult;
            Assert.NotNull(statusResult);
            Assert.Equal(404, statusResult.StatusCode);
        }

        /// <summary>
        /// Returns NotFound when can not find HourElectricity.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Get_ShouldReturnNotFoundWhenCanNotFindHourElectricity()
        {

            var panel = new Panel
            {
                Brand = "Areva",
                Latitude = 12.345678,
                Longitude = 98.7655432,
                Serial = "AAAA1111BBBB-001"
            };


            context.Panels.Add(panel);
            context.SaveChanges();

            // Act
            var result = await _analyticsController.Get("AAAA1111BBBB-001");

            // Assert
            Assert.NotNull(result);

            var statusResult = result as NotFoundResult;
            Assert.NotNull(statusResult);
        }

        /// <summary>
        /// Returns OkObject when find HourElectricity.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Get_ShouldReturnOkObject()
        {
            var panel = new Panel
            {
                Brand = "Areva",
                Latitude = 12.345678,
                Longitude = 98.7655432,
                Serial = "AAAA1111BBBB-002"
            };

            var oneHourElectricity = new OneHourElectricity
            {
                PanelId = "AAAA1111BBBB-002",
                KiloWatt = 1000,
                DateTime = DateTime.UtcNow
            };

            context.Panels.Add(panel);
            context.SaveChanges();

            context.OneHourElectricitys.Add(oneHourElectricity);
            context.SaveChanges();

            // Act
            var result = await _analyticsController.Get("AAAA1111BBBB-002");

            // Assert
            Assert.NotNull(result);

            var statusResult = result as OkObjectResult;
            Assert.NotNull(statusResult);
            Assert.Equal(200, statusResult.StatusCode);
        }

        /// <summary>
        /// Returns NotFound when can not find the panel.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task DayResults_ShouldReturnNotFoundWhenCanNotFindPanel()
        {
            // Act
            var result = await _analyticsController.DayResults("123");

            // Assert
            Assert.NotNull(result);

            var statusResult = result as NotFoundResult;
            Assert.NotNull(statusResult);
            Assert.Equal(404, statusResult.StatusCode);
        }

        /// <summary>
        /// Returns NotFound when can not find HourElectricity.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task DayResults_ShouldReturnNotFoundWhenCanNotFindHourElectricity()
        {
            var panel = new Panel
            {
                Brand = "Areva",
                Latitude = 12.345678,
                Longitude = 98.7655432,
                Serial = "AAAA1111BBBB-003"
            };

            context.Panels.Add(panel);
            context.SaveChanges();

            // Act
            var result = await _analyticsController.DayResults("AAAA1111BBBB-003");

            // Assert
            Assert.NotNull(result);

            var statusResult = result as NotFoundResult;
            Assert.NotNull(statusResult);
            Assert.Equal(404, statusResult.StatusCode);
        }

        /// <summary>F
        /// Returns OkObject when find data in the report.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task DayResults_ShouldReturnOkObject()
        {
            var panel = new Panel
            {
                Brand = "Areva",
                Latitude = 12.345678,
                Longitude = 98.7655432,
                Serial = "AAAA1111BBBB-004"
            };

            var oneHourElectricity = new OneHourElectricity
            {
                //Id = 1,
                PanelId = "AAAA1111BBBB-004",
                KiloWatt = 10,
                DateTime = DateTime.UtcNow.Date.AddDays(-5).AddHours(1).AddMinutes(10)
            };

            var oneHourElectricity2 = new OneHourElectricity
            {
                //Id = 1,
                PanelId = "AAAA1111BBBB-004",
                KiloWatt = 20,
                DateTime = DateTime.UtcNow.Date.AddDays(-5).AddHours(2).AddMinutes(20)
            };

            var oneHourElectricity3 = new OneHourElectricity
            {
                //Id = 1,
                PanelId = "AAAA1111BBBB-004",
                KiloWatt = 30,
                DateTime = DateTime.UtcNow.Date.AddDays(-5).AddHours(2).AddMinutes(30)
            };

            var oneHourElectricity4 = new OneHourElectricity
            {
                //Id = 1,
                PanelId = "AAAA1111BBBB-004",
                KiloWatt = 40,
                DateTime = DateTime.UtcNow.Date.AddHours(3).AddMinutes(40)
            };

            var oneHourElectricity5 = new OneHourElectricity
            {
                //Id = 1,
                PanelId = "AAAA1111BBBB-004",
                KiloWatt = 50,
                DateTime = DateTime.UtcNow.Date.AddHours(4)
            };


            var oneHourElectricity6 = new OneHourElectricity
            {
                //Id = 1,
                PanelId = "AAAA1111BBBB-004",
                KiloWatt = 23,
                DateTime = DateTime.UtcNow.Date.AddDays(-4).AddHours(2).AddMinutes(58)
            };

            var oneHourElectricity7 = new OneHourElectricity
            {
                //Id = 1,
                PanelId = "AAAA1111BBBB-004",
                KiloWatt = 17,
                DateTime = DateTime.UtcNow.Date.AddDays(-4).AddHours(2).AddMinutes(59)
            };

            var oneHourElectricity8 = new OneHourElectricity
            {
                //Id = 1,
                PanelId = "AAAA1111BBBB-004",
                KiloWatt = 234,
                DateTime = DateTime.UtcNow.Date.AddDays(-4).AddHours(5).AddSeconds(1)
            };

            var oneHourElectricity9 = new OneHourElectricity
            {
                //Id = 1,
                PanelId = "AAAA1111BBBB-004",
                KiloWatt = 741,
                DateTime = DateTime.UtcNow.Date.AddDays(-4).AddHours(5).AddMinutes(2)
            };

            var oneHourElectricity10 = new OneHourElectricity
            {
                //Id = 1,
                PanelId = "AAAA1111BBBB-004",
                KiloWatt = 8,
                DateTime = DateTime.UtcNow.Date.AddDays(-4).AddHours(5)
            };


            context.Panels.Add(panel);
            context.SaveChanges();

            context.OneHourElectricitys.Add(oneHourElectricity);
            context.OneHourElectricitys.Add(oneHourElectricity2);
            context.OneHourElectricitys.Add(oneHourElectricity3);
            context.OneHourElectricitys.Add(oneHourElectricity4);
            context.OneHourElectricitys.Add(oneHourElectricity5);
            context.OneHourElectricitys.Add(oneHourElectricity6);
            context.OneHourElectricitys.Add(oneHourElectricity7);
            context.OneHourElectricitys.Add(oneHourElectricity8);
            context.OneHourElectricitys.Add(oneHourElectricity9);
            context.OneHourElectricitys.Add(oneHourElectricity10);
            context.SaveChanges();



            // Act
            var result = await _analyticsController.DayResults("AAAA1111BBBB-004");

            // Assert
            Assert.NotNull(result);

            var statusResult = result as OkObjectResult;
            Assert.NotNull(statusResult);
            Assert.Equal(200, statusResult.StatusCode);
        }



        /// <summary>
        /// Returns BadRequest when OneHourElectricityModel is null.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Post_ShouldReturnBadRequest()
        {
            // Act
            var result = await _analyticsController.Post("AAAA1111BBBB-005", null);

            // Assert
            Assert.NotNull(result);

            var statusResult = result as BadRequestResult;
            Assert.NotNull(statusResult);
            Assert.Equal(400, statusResult.StatusCode);
        }


        /// <summary>
        /// Returns Created when insert an HourElectricity.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task Post_ShouldReturnCreated()
        {

            var oneHourElectricity = new OneHourElectricityModel
            {
                KiloWatt = 1000,
                DateTime = DateTime.UtcNow
            };

            // Act
            var result = await _analyticsController.Post("AAAA1111BBBB-006", oneHourElectricity);

            // Assert
            Assert.NotNull(result);

            var statusResult = result as CreatedResult;
            Assert.NotNull(statusResult);
            Assert.Equal(201, statusResult.StatusCode);
        }
    }
}