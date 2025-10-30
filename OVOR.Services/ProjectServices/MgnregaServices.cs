using System.Data;
using OVOR.Models.ProjectData;
using OVOR.Repo.Repo;
using OVOR.Services.ProjectServices;

namespace OVOR.Services.ProjectServices
{
    public interface IMgnregaServices
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
    public class MgnregaServices : IMgnregaServices
    {
        public readonly IMgnregaRepo _mgnregaRepo;
        public readonly ProjectServices _projectServices;

        public MgnregaServices(IMgnregaRepo mgnregaRepo, ProjectServices projectServices)
        {
            _mgnregaRepo = mgnregaRepo;
            _projectServices = projectServices;
        }



        public DataTable GetStateList()
        {
            return _mgnregaRepo.GetStateList();
        }

        public DataTable GetDistrictList(StateName data)
        {
            return _mgnregaRepo.GetDistrictList(data);
        }

        public DataTable GetDistrictDashboardData(DistrictDashboardData data)
        {
            return _mgnregaRepo.GetDistrictDashboardData(data);
        }

        public DataTable GetMonth()
        {
            return _mgnregaRepo.GetMonth();
        }

        public DataTable GetYear()
        {
            return _mgnregaRepo.GetYear();
        }

        public DataTable GetMetric()
        {
            return _mgnregaRepo.GetMetric();
        }

        public DataTable GetChartData(GraphData data)
        {
            return _mgnregaRepo.GetChartData(data);
        }

        public DataTable WageOverTime(WageOverTime data)
        {
            return _mgnregaRepo.WageOverTime(data);
        }

        public async Task<DataTable> GetAutoLocation(GetAutoDistrict data)
        {
            var pdt = await _projectServices.GetDistrictFromCoordinatesAsync(data);

            var dt = await _mgnregaRepo.GetAutoLocation(pdt);
            
            return dt;
        }
    }
}
