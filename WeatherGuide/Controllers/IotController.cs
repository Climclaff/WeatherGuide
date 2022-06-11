using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using WeatherGuide.Data;
using WeatherGuide.Models;

namespace WeatherGuide.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IotController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public IotController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        [Route("SendMeasurement")]
        public async Task<IActionResult> SendMeasurement([FromBody] IotMeasurement model)
        {
            var state = await _context.Set<State>().Where(x => x.Id == model.StateId).FirstOrDefaultAsync();
            if (state == null)
            {
                return BadRequest();
            }
            Measurement measurement = new Measurement();
            measurement.Temperature = model.Temperature;
            measurement.Humidity = model.Humidity;
            measurement.WindSpeed = model.WindSpeed;
            measurement.DateTime = model.DateTime;
            measurement.CountryId = model.CountryId;
            measurement.StateId = model.StateId;
            _context.Add(measurement);
            await _context.SaveChangesAsync();
            return Ok();

        }
    }
    public class IotMeasurement
    {
        public int Temperature { get; set; }

        public int Humidity { get; set; }

        public int WindSpeed { get; set; }

        public DateTime DateTime { get; set; }

        public int CountryId { get; set; }

        public int StateId { get; set; }

    }
}
