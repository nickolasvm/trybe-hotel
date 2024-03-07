using Microsoft.AspNetCore.Mvc;
using TrybeHotel.Models;
using TrybeHotel.Repository;
using TrybeHotel.Dto;
using TrybeHotel.Services;


namespace TrybeHotel.Controllers
{
    [ApiController]
    [Route("geo")]
    public class GeoController : Controller
    {
        private readonly IHotelRepository _repository;
        private readonly IGeoService _geoService;


        public GeoController(IHotelRepository repository, IGeoService geoService)
        {
            _repository = repository;
            _geoService = geoService;
        }

        [HttpGet]
        [Route("status")]
        public async Task<IActionResult> GetStatus()
        {
            var geoStatus = await _geoService.GetGeoStatus();
            return Ok(geoStatus);
        }

        // 12. Desenvolva o endpoint GET /geo/address
        [HttpGet]
        [Route("address")]
        public async Task<IActionResult> GetHotelsByLocation([FromBody] GeoDto address)
        {
            var hotels = await _geoService.GetHotelsByGeo(address, _repository);
            return Ok(hotels);
        }
    }


}