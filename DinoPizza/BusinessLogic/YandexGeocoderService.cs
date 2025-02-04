using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DinoPizza.BusinessLogic
{
    public class YandexGeocoderService
    {
        private readonly string apiKey = "e6598c17-363a-4fdb-bd29-e0a48d1f04ca";
        private readonly HttpClient _httpClient;

        public YandexGeocoderService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<Coordinates> GetCoordinatesAsync(string address)
        {
            try
            {
                var url = $"https://geocode-maps.yandex.ru/1.x/?geocode={Uri.EscapeDataString(address)}&apikey={apiKey}&format=json";

                var response = await _httpClient.GetStringAsync(url);

                var result = Newtonsoft.Json.JsonConvert.DeserializeObject<YandexGeocodeResponse>(response);

                if (result?.Response?.GeoObjectCollection?.FeatureMember != null &&
                    result.Response.GeoObjectCollection.FeatureMember.Length > 0)
                {
                    var coords = result.Response.GeoObjectCollection.FeatureMember[0].GeoObject.Point.Pos;
                    var coordinates = coords.Split(' ');

                    return new Coordinates
                    {
                        Latitude = double.Parse(coordinates[1]),
                        Longitude = double.Parse(coordinates[0])
                    };
                }

                return null; // Адрес не найден
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при вызове API: {ex.Message}");
                throw; // Или верните null в зависимости от вашей логики
            }
        }

    }

    public class YandexGeocodeResponse
    {
        public Response Response { get; set; }
    }

    public class Response
    {
        public GeoObjectCollection GeoObjectCollection { get; set; }
    }

    public class GeoObjectCollection
    {
        public FeatureMember[] FeatureMember { get; set; }
    }

    public class FeatureMember
    {
        public GeoObject GeoObject { get; set; }
    }

    public class GeoObject
    {
        public Point Point { get; set; }
    }

    public class Point
    {
        public string Pos { get; set; }
    }
}
