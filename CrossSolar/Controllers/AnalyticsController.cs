using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrossSolar.Domain;
using CrossSolar.Models;
using CrossSolar.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrossSolar.Controllers
{
    [Route("panel")]
    public class AnalyticsController : Controller
    {
        private readonly IAnalyticsRepository _analyticsRepository;
        private readonly IDayAnalyticsRepository _dayAnalyticsRepository;

        private readonly IPanelRepository _panelRepository;

        public AnalyticsController(IAnalyticsRepository analyticsRepository, IDayAnalyticsRepository dayAnalyticsRepository, IPanelRepository panelRepository)
        {
            _analyticsRepository = analyticsRepository;
            _dayAnalyticsRepository = dayAnalyticsRepository;
            _panelRepository = panelRepository;
        }

        // GET panel/XXXX1111YYYY2222/analytics
        [HttpGet("{banelId}/[controller]")]
        public async Task<IActionResult> Get([FromRoute] string panelId)
        {
            var panel = await _panelRepository.Query()
                .FirstOrDefaultAsync(x => x.Serial.Equals(panelId, StringComparison.CurrentCultureIgnoreCase));

            if (panel == null) return NotFound();

            var analytics = await _analyticsRepository.Query()
                .Where(x => x.PanelId.Equals(panelId, StringComparison.CurrentCultureIgnoreCase)).ToListAsync();

            if (!analytics.Any()) return NotFound();

            var result = new OneHourElectricityListModel
            {
                OneHourElectricitys = analytics.Select(c => new OneHourElectricityModel
                {
                    Id = c.Id,
                    KiloWatt = c.KiloWatt,
                    DateTime = c.DateTime
                })
            };

            return Ok(result);
        }

        // GET panel/XXXX1111YYYY2222/analytics/day
        [HttpGet("{panelId}/[controller]/day")]
        public async Task<IActionResult> DayResults([FromRoute] string panelId)
        {
            var result = new List<OneDayElectricityModel>();

            var panel = await _panelRepository.Query()
                     .FirstOrDefaultAsync(x => x.Serial.Equals(panelId, StringComparison.CurrentCultureIgnoreCase));

            if (panel == null) return NotFound();

            var analytics = await _analyticsRepository.Query()
                .Where(x => x.PanelId.Equals(panelId, StringComparison.CurrentCultureIgnoreCase)).ToListAsync();

            // Talvez seja o bug.
            if (!analytics.Any()) return NotFound();

            var limitedDate = DateTime.UtcNow.Date;
            result = analytics.AsEnumerable()
                .GroupBy(row => row.DateTime.Date.AddHours(row.DateTime.Hour))
                .Select(grp => new OneDayElectricityModel
                {
                    DateTime = grp.Key,
                    Count = grp.Count(),
                    Sum = grp.Sum(x => x.KiloWatt),
                    Maximum = grp.Max(x => x.KiloWatt),
                    Minimum = grp.Min(x => x.KiloWatt),
                    Average = grp.Average(x => x.KiloWatt)
                }).Where(x => x.DateTime < limitedDate).ToList();

            //foreach (var report in result)
            //{
            //    var line = string.Format("Date:{0}. Count{1}. Sum:{2}. Max:{3}. Min:{4}. Average:{5}.", report.DateTime, report.Count, report.Sum, report.Maximum, report.Minimum, report.Average);
            //}

            return Ok(result);
        }

        // POST panel/XXXX1111YYYY2222/analytics
        [HttpPost("{panelId}/[controller]")]
        public async Task<IActionResult> Post([FromRoute] string panelId, [FromBody] OneHourElectricityModel value)
        {
            if (value == null) return BadRequest();

            var oneHourElectricityContent = new OneHourElectricity
            {
                PanelId = panelId,
                KiloWatt = value.KiloWatt,
                DateTime = DateTime.UtcNow
            };

            await _analyticsRepository.InsertAsync(oneHourElectricityContent);

            var result = new OneHourElectricityModel
            {
                Id = oneHourElectricityContent.Id,
                KiloWatt = oneHourElectricityContent.KiloWatt,
                DateTime = oneHourElectricityContent.DateTime
            };

            return Created($"panel/{panelId}/analytics/{result.Id}", result);
        }
    }
}