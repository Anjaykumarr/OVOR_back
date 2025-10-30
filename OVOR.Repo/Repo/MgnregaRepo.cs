using System.Data;
using MySql.Data.MySqlClient;
using OVOR.Models.ProjectData;
using OVOR.Repo.DataTools;

namespace OVOR.Repo.Repo
{
    public interface IMgnregaRepo
    {
        public DataTable GetStateList();
        public DataTable GetDistrictList(StateName data);
        public DataTable GetDistrictDashboardData(DistrictDashboardData data);
        public DataTable GetMonth();
        public DataTable GetYear();
        public DataTable GetMetric();
        public DataTable GetChartData(GraphData data);
        public DataTable WageOverTime(WageOverTime data);
        public Task<DataTable> GetAutoLocation(GetAutoDistrict data);
    }
    public class MgnregaRepo : IMgnregaRepo
    {

        public DataTable GetStateList()
        {
            return DataAccessor.GetDataTable("sp_GetStateList", null);
        }

        public DataTable GetDistrictList(StateName data)
        {
            var Parameter = new List<MySqlParameter>
            {
                new MySqlParameter("statename", data.Name)
            };
            var dt = DataAccessor.GetDataTable("sp_GetDistrictList", Parameter);
            return dt;
        }

        public DataTable GetDistrictDashboardData(DistrictDashboardData data)
        {
            var Parameter = new List<MySqlParameter>
            {
                new MySqlParameter("district", data.Data)
            };
            var dt = DataAccessor.GetDataTable("sp_GetDistrictDashboardData", Parameter);
            return dt;
        }

        public DataTable GetMonth()
        {
            return DataAccessor.GetDataTable("sp_GetMonth", null);
        }

        public DataTable GetYear()
        {
            return DataAccessor.GetDataTable("sp_GetYear", null);
        }

        public DataTable GetMetric()
        {
            return DataAccessor.GetDataTable("sp_GetMetric", null);
        }

        public DataTable GetChartData(GraphData data)
        {
            var Parameter = new List<MySqlParameter>
            {
                new MySqlParameter("district", data.district),
                new MySqlParameter("metric", data.metric),
                new MySqlParameter("finyear", data.year)
            };
            var dt = DataAccessor.GetDataTable("sp_GetChartData", Parameter);
            return dt;
        }

        public DataTable WageOverTime(WageOverTime data)
        {
            var Parameter = new List<MySqlParameter>
            {
                new MySqlParameter("district", data.district)
            };
            var dt = DataAccessor.GetDataTable("sp_WageOverTime", Parameter);
            return dt;
        }

        public async Task<DataTable> GetAutoLocation(GetAutoDistrict data)
        {
            var Parameter = new List<MySqlParameter>
            {
                new MySqlParameter("state", data.state),
                new MySqlParameter("district", data.district)
            };

            var dt = await DataAccessor.GetDataTableAsync("sp_GetStateDisc", Parameter);

            return dt;
        }
    }
}
