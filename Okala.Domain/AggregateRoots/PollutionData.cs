namespace Okala.Domain.AggregateRoots
{
    public class PollutionData
    {
        public Coord Coord { get; set; }
        public List<Polluts> List { get; set; }

        public class Polluts
        {
            public MainData Main { get; set; }
            public Component Components { get; set; }
            public long DT { get; set; }

            public class Component
            {
                public double CO { get; set; }
                public double NO { get; set; }
                public double NO2 { get; set; }
                public double O3 { get; set; }
                public double SO2 { get; set; }
                public double PM2_5 { get; set; }
                public double PM10 { get; set; }
                public double NH3 { get; set; }
            }
            public class MainData
            {
                public int AQI { get; set; }
            }
        }

    }

}