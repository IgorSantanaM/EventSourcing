using BeerSender.Domain;
using BeerSender.Domain.Boxes;
using BeerSender.Domain.Projections;
using Marten;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BeerSender.QueryAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class BoxController(IDocumentStore store) : ControllerBase
{
    [HttpGet]
    [Route("{id:guid}")]
    public async Task<Box> GetById([FromRoute]Guid id)
    {
        await using var session = store.QuerySession();
        var box = await session.Events.AggregateStreamAsync<Box>(id);
        return box!;
    }

    [HttpGet]
    [Route("{id:guid}/by-sequence/{sequence:int}")]
    public async Task<Box> GetById([FromRoute] Guid id, [FromRoute]int sequence)
    {
        await using var session = store.QuerySession();
        var box = await session.Events.AggregateStreamAsync<Box>(id, version: sequence);
        return box!;
    }

    [HttpGet]
    [Route("all-open")]
    public async Task<IEnumerable<OpenBox>> GetOpenBoxes()
    {
        await using var session = store.QuerySession();

        var boxes = await session.Query<OpenBox>().ToListAsync();

        return boxes;
    }

    [HttpGet]
    [Route("all-unsent")]
    public async Task<IEnumerable<UnsentBox>> GetUnsentBoxes()
    {
        await using var session = store.QuerySession();

        var boxes = await session.Query<UnsentBox>().ToListAsync();

        return boxes;
    }
}