using BeerSender.Domain;
using BeerSender.Domain.Boxes.Commands;
using Microsoft.AspNetCore.Mvc;

namespace BeerSender.Web.Controllers;

[ApiController]
[Route("api/command/[controller]")]
public class BoxController(CommandRouter router) : ControllerBase
{
    [HttpPost]
    [Route("create")]
    public IActionResult CreateBox([FromBody] CreateBox command)
    {
        router.HandleCommand(command);

        return Accepted();
    }
}
