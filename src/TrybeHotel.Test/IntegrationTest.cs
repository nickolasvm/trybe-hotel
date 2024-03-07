namespace TrybeHotel.Test;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using TrybeHotel.Models;
using TrybeHotel.Repository;
using TrybeHotel.Dto;
using TrybeHotel.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Diagnostics;
using System.Xml;
using System.IO;

public class LoginJson
{
    public string? token { get; set; }
}


public class IntegrationTest : IClassFixture<WebApplicationFactory<Program>>
{
    public HttpClient _clientTest;

    public IntegrationTest(WebApplicationFactory<Program> factory)
    {
        //_factory = factory;
        _clientTest = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<TrybeHotelContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<ContextTest>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryTestDatabase");
                });
                services.AddScoped<ITrybeHotelContext, ContextTest>();
                services.AddScoped<ICityRepository, CityRepository>();
                services.AddScoped<IHotelRepository, HotelRepository>();
                services.AddScoped<IRoomRepository, RoomRepository>();
                var sp = services.BuildServiceProvider();
                using (var scope = sp.CreateScope())
                using (var appContext = scope.ServiceProvider.GetRequiredService<ContextTest>())
                {
                    appContext.Database.EnsureCreated();
                    appContext.Database.EnsureDeleted();
                    appContext.Database.EnsureCreated();
                    appContext.Cities.Add(new City { CityId = 1, Name = "Manaus", State = "AM" });
                    appContext.Cities.Add(new City { CityId = 2, Name = "Palmas", State = "TO" });
                    appContext.SaveChanges();
                    appContext.Hotels.Add(new Hotel { HotelId = 1, Name = "Trybe Hotel Manaus", Address = "Address 1", CityId = 1 });
                    appContext.Hotels.Add(new Hotel { HotelId = 2, Name = "Trybe Hotel Palmas", Address = "Address 2", CityId = 2 });
                    appContext.Hotels.Add(new Hotel { HotelId = 3, Name = "Trybe Hotel Ponta Negra", Address = "Addres 3", CityId = 1 });
                    appContext.SaveChanges();
                    appContext.Rooms.Add(new Room { RoomId = 1, Name = "Room 1", Capacity = 2, Image = "Image 1", HotelId = 1 });
                    appContext.Rooms.Add(new Room { RoomId = 2, Name = "Room 2", Capacity = 3, Image = "Image 2", HotelId = 1 });
                    appContext.Rooms.Add(new Room { RoomId = 3, Name = "Room 3", Capacity = 4, Image = "Image 3", HotelId = 1 });
                    appContext.Rooms.Add(new Room { RoomId = 4, Name = "Room 4", Capacity = 2, Image = "Image 4", HotelId = 2 });
                    appContext.Rooms.Add(new Room { RoomId = 5, Name = "Room 5", Capacity = 3, Image = "Image 5", HotelId = 2 });
                    appContext.Rooms.Add(new Room { RoomId = 6, Name = "Room 6", Capacity = 4, Image = "Image 6", HotelId = 2 });
                    appContext.Rooms.Add(new Room { RoomId = 7, Name = "Room 7", Capacity = 2, Image = "Image 7", HotelId = 3 });
                    appContext.Rooms.Add(new Room { RoomId = 8, Name = "Room 8", Capacity = 3, Image = "Image 8", HotelId = 3 });
                    appContext.Rooms.Add(new Room { RoomId = 9, Name = "Room 9", Capacity = 4, Image = "Image 9", HotelId = 3 });
                    appContext.SaveChanges();
                    appContext.Users.Add(new User { UserId = 1, Name = "Ana", Email = "ana@trybehotel.com", Password = "Senha1", UserType = "admin" });
                    appContext.Users.Add(new User { UserId = 2, Name = "Beatriz", Email = "beatriz@trybehotel.com", Password = "Senha2", UserType = "client" });
                    appContext.Users.Add(new User { UserId = 3, Name = "Laura", Email = "laura@trybehotel.com", Password = "Senha3", UserType = "client" });
                    appContext.SaveChanges();
                    appContext.Bookings.Add(new Booking { BookingId = 1, CheckIn = new DateTime(2023, 07, 02), CheckOut = new DateTime(2023, 07, 03), GuestQuant = 1, UserId = 2, RoomId = 1 });
                    appContext.Bookings.Add(new Booking { BookingId = 2, CheckIn = new DateTime(2023, 07, 02), CheckOut = new DateTime(2023, 07, 03), GuestQuant = 1, UserId = 3, RoomId = 4 });
                    appContext.SaveChanges();
                }
            });
        }).CreateClient();
    }

    [Trait("Category", "Endpoint GET /city")]
    [Theory(DisplayName = "Deve retornar status 200 OK")]
    [InlineData("/city")]
    public async Task TestGetCityStatus(string url)
    {
        var response = await _clientTest.GetAsync(url);
        Assert.Equal(System.Net.HttpStatusCode.OK, response?.StatusCode);
    }

    [Trait("Category", "Endpoint GET /city")]
    [Theory(DisplayName = "Deve retornar uma lista com todas as cidades")]
    [InlineData("/city")]
    public async Task TestGetCityResponse(string url)
    {
        var response = await _clientTest.GetAsync(url);
        var responseString = await response.Content.ReadAsStringAsync();
        List<City>? jsonResponse = JsonConvert.DeserializeObject<List<City>>(responseString);
        Assert.Contains("Manaus", jsonResponse?[0].Name);
        Assert.Contains("Palmas", jsonResponse?[1].Name);
    }

    [Trait("Category", "Endpoint POST /city")]
    [Theory(DisplayName = "Deve ser possível criar uma nova cidade")]
    [InlineData("/city")]
    public async Task TestPostCity(string url)
    {
        var cityMoq = new { Name = "Cidade Moq" };

        var response = await _clientTest.PostAsync(url, new StringContent(JsonConvert.SerializeObject(cityMoq), System.Text.Encoding.UTF8, "application/json"));
        var responseString = await response.Content.ReadAsStringAsync();
        City? jsonResponse = JsonConvert.DeserializeObject<City>(responseString);

        Assert.Equal(System.Net.HttpStatusCode.Created, response?.StatusCode);
        Assert.Contains("Cidade Moq", jsonResponse?.Name);
    }

    [Trait("Category", "Endpoint GET /hotel")]
    [Theory(DisplayName = "Deve retornar status 200 OK")]
    [InlineData("/hotel")]
    public async Task TestGetHotelStatus(string url)
    {
        var response = await _clientTest.GetAsync(url);
        Assert.Equal(System.Net.HttpStatusCode.OK, response?.StatusCode);
    }

    [Trait("Category", "Endpoint GET /hotel")]
    [Theory(DisplayName = "Deve retornar uma lista com todas as cidades")]
    [InlineData("/hotel")]
    public async Task TestGetHotelResponse(string url)
    {
        var response = await _clientTest.GetAsync(url);
        var responseString = await response.Content.ReadAsStringAsync();
        List<Hotel>? jsonResponse = JsonConvert.DeserializeObject<List<Hotel>>(responseString);
        Assert.Contains("Trybe Hotel Palmas", jsonResponse?[0].Name);
        Assert.Contains("Trybe Hotel Manaus", jsonResponse?[1].Name);
    }

    [Trait("Category", "Endpoint GET /room/{hotelId}")]
    [Theory(DisplayName = "Deve retornar status 200 OK")]
    [InlineData("/room", 1)]
    public async Task TestGetRoomStatus(string url, int hotelId)
    {
        var response = await _clientTest.GetAsync(url + $"/{hotelId}");
        Assert.Equal(System.Net.HttpStatusCode.OK, response?.StatusCode);
    }

    [Trait("Category", "Endpoint GET /room/{hotelId}")]
    [Theory(DisplayName = "Deve retornar uma lista com todas os quartos")]
    [InlineData("/room", 1)]
    public async Task TestGetRoomResponse(string url, int hotelId)
    {
        var response = await _clientTest.GetAsync(url + $"/{hotelId}");
        var responseString = await response.Content.ReadAsStringAsync();
        List<Room>? jsonResponse = JsonConvert.DeserializeObject<List<Room>>(responseString);
        Assert.Contains("Room 3", jsonResponse?[0].Name);
        Assert.Contains("Room 2", jsonResponse?[1].Name);
    }

    [Trait("Category", "Endpoint POST /hotel")]
    [Theory(DisplayName = "Deve ser possível criar um novo hotel")]
    [InlineData("/hotel", "ana@trybehotel.com")]
    public async Task TestPostHotel(string url, string adminEmail)
    {
        var userResponse = await _clientTest.GetAsync($"/user/{adminEmail}");
        var userString = await userResponse.Content.ReadAsStringAsync();
        UserDto? userJson = JsonConvert.DeserializeObject<UserDto>(userString);

        var token = new TokenGenerator().Generate(userJson);
        _clientTest.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var hotelMoq = new { Name = "Hotel Moq", Address = "Rua Moq", CityId = 1 };
        var response = await _clientTest.PostAsync(url, new StringContent(JsonConvert.SerializeObject(hotelMoq), System.Text.Encoding.UTF8, "application/json"));
        var responseString = await response.Content.ReadAsStringAsync();
        Hotel? jsonResponse = JsonConvert.DeserializeObject<Hotel>(responseString);

        Assert.Equal(System.Net.HttpStatusCode.Created, response?.StatusCode);
        Assert.Contains("Hotel Moq", jsonResponse?.Name);
        Assert.Contains("Rua Moq", jsonResponse?.Address);
    }

    [Trait("Category", "Endpoint POST /room")]
    [Theory(DisplayName = "Deve ser possível criar um novo quarto")]
    [InlineData("/room", "ana@trybehotel.com")]
    public async Task TestPostRoom(string url, string adminEmail)
    {
        var userResponse = await _clientTest.GetAsync($"/user/{adminEmail}");
        var userString = await userResponse.Content.ReadAsStringAsync();
        UserDto? userJson = JsonConvert.DeserializeObject<UserDto>(userString);

        var token = new TokenGenerator().Generate(userJson);
        _clientTest.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var roomMoq = new { Name = "Quarto teste", Capacity = 5, Image = "test_image.jpg", HotelId = 1 };
        var response = await _clientTest.PostAsync(url, new StringContent(JsonConvert.SerializeObject(roomMoq), System.Text.Encoding.UTF8, "application/json"));
        var responseString = await response.Content.ReadAsStringAsync();
        Room? jsonResponse = JsonConvert.DeserializeObject<Room>(responseString);

        Assert.Equal(System.Net.HttpStatusCode.Created, response?.StatusCode);
        Assert.Contains("Quarto teste", jsonResponse?.Name);
    }

    [Trait("Category", "Endpoint DELETE /room/{roomId}")]
    [Theory(DisplayName = "Deve ser possível deletar um novo quarto")]
    [InlineData("/room", 1, "ana@trybehotel.com")]
    public async Task TestDeleteRoom(string url, int roomId, string adminEmail)
    {
        var userResponse = await _clientTest.GetAsync($"/user/{adminEmail}");
        var userString = await userResponse.Content.ReadAsStringAsync();
        UserDto? userJson = JsonConvert.DeserializeObject<UserDto>(userString);

        var token = new TokenGenerator().Generate(userJson);
        _clientTest.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _clientTest.DeleteAsync(url + $"/{roomId}");

        Assert.Equal(System.Net.HttpStatusCode.NoContent, response?.StatusCode);
    }

    [Trait("Category", "Endpoint POST /user")]
    [Theory(DisplayName = "Deve ser possível criar um novo usuario")]
    [InlineData("/user")]
    public async Task TestCreateUser(string url)
    {
        var userMoq = new { Name = "User test", Email = "test@gmail.com", Password = "test123" };
        var response = await _clientTest.PostAsync(url, new StringContent(JsonConvert.SerializeObject(userMoq), System.Text.Encoding.UTF8, "application/json"));
        var responseString = await response.Content.ReadAsStringAsync();
        var jsonResponse = JsonConvert.DeserializeObject<User>(responseString);

        Assert.Equal(System.Net.HttpStatusCode.Created, response?.StatusCode);
        Assert.Contains("User test", jsonResponse?.Name);
    }
}