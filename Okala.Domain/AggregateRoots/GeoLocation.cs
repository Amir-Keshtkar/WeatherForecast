namespace Okala.Domain.AggregateRoots
{
    public class GeoLocation
    {
        public string Name { get; set; }
        public Dictionary<string, string> Local_names { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public string Country { get; set; }
    }

}