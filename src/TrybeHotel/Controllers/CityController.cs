using Microsoft.AspNetCore.Mvc;
using TrybeHotel.Models;
using TrybeHotel.Repository;
using TrybeHotel.Dto;

namespace TrybeHotel.Controllers
{
    [ApiController]
    [Route("city")]
    public class CityController : Controller
    {
        private readonly ICityRepository _repository;
        public CityController(ICityRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IActionResult GetCities()
        {
            var cities = _repository.GetCities();
            return Ok(cities);
        }

        [HttpPost]
        public IActionResult PostCity([FromBody] City city)
        {
            CityDto newCity = _repository.AddCity(city);
            return CreatedAtAction("GetCities", new { id = newCity.CityId, name = newCity.Name, state = newCity.State }, newCity);
        }

        [HttpPut]
        public IActionResult PutCity([FromBody] City city)
        {
            CityDto updatedCity = _repository.UpdateCity(city);
            return Ok(updatedCity);
        }
    }
}