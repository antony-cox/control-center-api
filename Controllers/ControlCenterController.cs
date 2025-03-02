using Microsoft.AspNetCore.Mvc;
using DesktopAPI.Models;
using DesktopAPI.Services;

namespace DesktopAPI.Controllers;

[ApiController]
public class ControlCenterController : ControllerBase
{

    private readonly ILogger<ControlCenterController> _logger;
    private readonly IPowershellService _powershellService;

    public ControlCenterController(ILogger<ControlCenterController> logger, IPowershellService powershellService)
    {
        _logger = logger;
        _powershellService = powershellService;
    }

    [HttpGet]
    [Route("ControlCenter")]
    public async Task<ActionResult<IEnumerable<DiskInfo?>>> Get()
    {
        try
        {
            return Ok(await _powershellService.GetDiskInfo());
        } catch (Exception ex)
        {
            _logger.LogError($"Error in {ControllerContext.ActionDescriptor.ActionName}: {ex}");
            return StatusCode(500, "Internal server error");
        }
    }


    [HttpPost]
    [Route("ControlCenter/Shutdown")]
    public async Task<IActionResult> Shutdown()
    {
        try
        {
            await _powershellService.Shutdown();
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in {ControllerContext.ActionDescriptor.ActionName}: {ex}");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    [Route("ControlCenter/StartPlex")]
    public async Task<IActionResult> StartPlex()
    {
        try
        {
            await _powershellService.StartApp(Constants.PLEXPATH);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in {ControllerContext.ActionDescriptor.ActionName}: {ex}");
            return StatusCode(500, "Internal server error");
        }
    }
}
