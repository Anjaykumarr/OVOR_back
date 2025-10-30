using System.Data;
using Microsoft.AspNetCore.Mvc;
using OVOR.Models.ProjectData;
using OVOR.Services.ProjectServices;

namespace OVOR.Controller.Controllers.ProjectController
{
    [ApiController]
    [Route("api/mgnrega")]
    public class MgnregaController : ControllerBase
    {
        private readonly IMgnregaServices _mgnregaServices;

        public MgnregaController(IMgnregaServices mgnregaServices)
        {
            _mgnregaServices = mgnregaServices;
        }

        [HttpPost("GetStateList")]
        public IActionResult GetStateList()
        {
            var dt = _mgnregaServices.GetStateList();

            return Ok(dt);
        }

        [HttpPost("GetDistrictList")]
        public IActionResult GetDistrictList(StateName data)
        {
            var dt = _mgnregaServices.GetDistrictList(data);

            return Ok(dt);
        }

        [HttpPost("GetDistrictDashboardData")]
        public IActionResult GetDistrictDashboardData(DistrictDashboardData data)
        {
            var dt = _mgnregaServices.GetDistrictDashboardData(data);

            return Ok(dt);
        }

        [HttpPost("GetMonth")]
        public IActionResult GetMonth()
        {
            var dt = _mgnregaServices.GetMonth();

            return Ok(dt);
        }

        [HttpPost("GetYear")]
        public IActionResult GetYear()
        {
            var dt = _mgnregaServices.GetYear();

            return Ok(dt);
        }

        [HttpPost("GetMetric")]
        public IActionResult GetMetric()
        {
            var dt = _mgnregaServices.GetMetric();

            return Ok(dt);
        }

        [HttpPost("GetChartData")]
        public IActionResult GetChartData(GraphData data)
        {
            var dt = _mgnregaServices.GetChartData(data);

            return Ok(dt);
        }

        [HttpPost("WageOverTime")]
        public IActionResult WageOverTime(WageOverTime data)
        {
            var dt = _mgnregaServices.WageOverTime(data);

            return Ok(dt);
        }

        [HttpPost("GetAutoLocation")]
        public async Task<IActionResult> GetAutoLocation(GetAutoDistrict data)
        {
            var dt = await _mgnregaServices.GetAutoLocation(data);

            return Ok(dt);
        }
    }
}
