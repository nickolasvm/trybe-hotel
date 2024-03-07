using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrybeHotel.Dto;
using TrybeHotel.Models;
using TrybeHotel.Repository;

namespace TrybeHotel.Controllers;

[ApiController]
[Route("room")]
public class RoomController : Controller
{
    private readonly IRoomRepository _repository;
    public RoomController(IRoomRepository repository)
    {
        _repository = repository;
    }

    [HttpGet("{HotelId}")]
    public IActionResult GetRoom(int HotelId)
    {
        var rooms = _repository.GetRooms(HotelId);
        return Ok(rooms);
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Policy = "Admin")]
    public IActionResult PostRoom([FromBody] Room room)
    {
        RoomDto newRoom = _repository.AddRoom(room);
        return CreatedAtAction("GetRoom", new { HotelId = newRoom.RoomId }, newRoom);
    }

    [HttpDelete("{RoomId}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Policy = "Admin")]
    public IActionResult Delete(int RoomId)
    {
        _repository.DeleteRoom(RoomId);
        return NoContent();
    }
}
