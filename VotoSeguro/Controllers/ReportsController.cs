


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VotoSeguro.Services;

namespace VotoSeguro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "admin")] // Estrictamente limitado al rol admin
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        // GET: api/Reports/statistics
        // Genera el reporte de estadísticas generales (Gráficos)
        [HttpGet("statistics")]
        public async Task<IActionResult> GetVoteStatistics()
        {
            try
            {
                var statistics = await _reportService.GetVoteStatistics();
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        
        // GET: api/Reports/audit
        // Obtiene el registro de auditoría de todos los votos emitidos
        [HttpGet("audit")]
        public async Task<IActionResult> GetVoteAuditLog()
        {
            try
            {
                var auditLog = await _reportService.GetVoteAuditLog();
                return Ok(auditLog);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}