namespace OKala.Application.Dto
{
    public class WeatherDataDto
    {
        public string Temperature { get; set; }
        public string Humidity { get; set; }
        public double WindSpeed { get; set; }
        public int AirQualityIndex { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public Pollutants Pollutants { get; set; }
    }
    public class Pollutants
    {
        public double CO { get; set; }
        public double NO { get; set; }
        public double NO2 { get; set; }
        public double O3 { get; set; }
        public double SO2 { get; set; }
        public double PM25 { get; set; }
        public double PM10 { get; set; }
        public double NH3 { get; set; }
    }
}
