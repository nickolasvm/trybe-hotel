using System.Net;
using System.Net.Http;
using TrybeHotel.Dto;
using TrybeHotel.Repository;

namespace TrybeHotel.Services
{
    public class GeoService : IGeoService
    {
        private readonly HttpClient _client;
        public GeoService(HttpClient client)
        {
            _client = client;
        }

        public async Task<object> GetGeoStatus()
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Get, "https://nominatim.openstreetmap.org/status.php?format=json");
            requestMessage.Headers.Add("Accept", "application/json");
            requestMessage.Headers.Add("User-Agent", "aspnet-user-agent");
            var response = await _client.SendAsync(requestMessage);
            if (!response.IsSuccessStatusCode)
            {
                return default(Object)!;
            }
            var result = await response.Content.ReadFromJsonAsync<object>();
            return result!;
        }

        // 12. Desenvolva o endpoint GET /geo/address
        public async Task<GeoDtoResponse> GetGeoLocation(GeoDto geoDto)
        {
            var requestMessage = new HttpRequestMessage(
                HttpMethod.Get,
                $"https://nominatim.openstreetmap.org/search?street={geoDto.Address}&city={geoDto.City}&country=Brazil&state={geoDto.State}&format=json&limit=1");
            requestMessage.Headers.Add("Accept", "application/json");
            requestMessage.Headers.Add("User-Agent", "aspnet-user-agent");

            var response = await _client.SendAsync(requestMessage);
            if (!response.IsSuccessStatusCode & response != null)
            {
                return default(GeoDtoResponse)!;
            }

            var result = await response!.Content.ReadFromJsonAsync<GeoDtoResponse[]>();
            return new GeoDtoResponse { lat = result![0].lat, lon = result![0].lon };
        }

        // 12. Desenvolva o endpoint GET /geo/address
        public async Task<List<GeoDtoHotelResponse>> GetHotelsByGeo(GeoDto geoDto, IHotelRepository repository)
        {
            var hotels = repository.GetHotels();
            var addresGeo = await GetGeoLocation(geoDto);
            List<GeoDtoHotelResponse> hotelsByGeo = new List<GeoDtoHotelResponse>();

            foreach (var hotel in hotels)
            {
                var hotelGeo = await GetGeoLocation(new GeoDto { Address = hotel.Address, City = hotel.CityName, State = hotel.State });
                var distance = CalculateDistance(addresGeo.lat!, addresGeo.lon!, hotelGeo.lat!, hotelGeo.lon!);
                hotelsByGeo.Add(new GeoDtoHotelResponse
                {
                    Distance = distance,
                    HotelId = hotel.HotelId,
                    Name = hotel.Name,
                    Address = hotel.Address,
                    CityName = hotel.CityName,
                    State = hotel.State,
                });
            }

            return hotelsByGeo.OrderBy(h => h.Distance).ToList();
        }



        public int CalculateDistance(string latitudeOrigin, string longitudeOrigin, string latitudeDestiny, string longitudeDestiny)
        {
            double latOrigin = double.Parse(latitudeOrigin.Replace('.', ','));
            double lonOrigin = double.Parse(longitudeOrigin.Replace('.', ','));
            double latDestiny = double.Parse(latitudeDestiny.Replace('.', ','));
            double lonDestiny = double.Parse(longitudeDestiny.Replace('.', ','));
            double R = 6371;
            double dLat = radiano(latDestiny - latOrigin);
            double dLon = radiano(lonDestiny - lonOrigin);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) + Math.Cos(radiano(latOrigin)) * Math.Cos(radiano(latDestiny)) * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double distance = R * c;
            return int.Parse(Math.Round(distance, 0).ToString());
        }

        public double radiano(double degree)
        {
            return degree * Math.PI / 180;
        }

    }
}