using BeerSender.Domain;
using BeerSender.Domain.Boxes;
using BeerSender.QueryAPI.Database;
using Microsoft.AspNetCore.Mvc;

namespace BeerSender.QueryAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class BoxController(IEventStore eventStore,BoxQueryRepository repository) : ControllerBase
{
    [HttpGet]
    [Route("{id}")]
    public Box GetById([FromRoute]Guid id)
    {
        var boxStream = new EventStream<Box>(eventStore, id);
        var box = boxStream.GetEntity();
        return box;
    }

    [HttpGet]
    [Route("{id}/version/{version}")]
    public Box GetById([FromRoute] Guid id, [FromRoute]int version)
    {
        throw new NotImplementedException();
    }

    [HttpGet]
    [Route("all-open")]
    public IEnumerable<OpenBox> GetOpenBoxes()
    {
        return repository.GetAllOpen();
    }

    [HttpGet]
    [Route("all-unsent")]
    public IEnumerable<UnsentBox> GetUnsentBoxes()
    {
        return repository.GetAllUnsent();
    }
}