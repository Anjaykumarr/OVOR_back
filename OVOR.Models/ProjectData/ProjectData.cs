namespace OVOR.Models.ProjectData
{
    public class ProjectData
    {
        // MapMyIndia 
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
    }

    public class LocationData
    {
        // lat and lon data 
        public double? Lat { get; set; }
        public double? Lon { get; set; }
    }

    public class StateName
    {
        public string Name { get; set; }
    }

    public class DistrictDashboardData
    {
        public string Data { get; set; }
    }

    public class GraphData
    {
        public string district { get; set; }
        public string year { get; set; }
        public string metric { get; set; }
    }

    public class WageOverTime
    {
        public String district { get; set; }
    }

    public class GetAutoDistrict : LocationData
    {
        public String? district { get; set; }
        public String? state { get; set; }
    }
}
