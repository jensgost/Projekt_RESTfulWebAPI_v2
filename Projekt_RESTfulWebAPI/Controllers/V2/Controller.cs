using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Projekt_RESTfulWebAPI.Data;
using Projekt_RESTfulWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Projekt_RESTfulWebAPI.DTO.V2;

namespace Projekt_RESTfulWebAPI.Controllers.V2
{
    [Route("api/v{version:apiVersion}/geo-comments")]
    [ApiController]
    [ApiVersion("2.0")]
    public class Controller : ControllerBase
    {
        private readonly OurDbContext _context;
        private readonly UserManager<User> _userManager;
        public Controller(OurDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Hämtar specifikt Geo-Message via unikt ID
        /// </summary>
        /// <param name="id"></param>
        /// <response code="200">Geo-Message found!</response>
        /// <response code="404">Failed to find Geo-Message</response>
        /// <returns>This returns a Geo-Message</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id}")]
        public async Task<ActionResult<GetGeoMessageDTO>> GetGeoMessage(int id)
        {
            var geoMessage = await _context.GeoMessages.FirstOrDefaultAsync(g => g.Id == id);

            if (geoMessage == null)
                return NotFound();

            var geoMessageDTO = new GetGeoMessageDTO
            {
                Message = new GetMessageDTO
                {
                    Title = geoMessage.Title,
                    Body = geoMessage.Body,
                    Author = geoMessage.Author
                },
                Latitude = geoMessage.Latitude,
                Longitude = geoMessage.Longitude
            };

            return Ok(geoMessageDTO);
        }

        /// <summary>
        /// Parametrarna möjliggör sökning av Geo-Message av särskilt område via uppgivna koordinater 
        /// </summary>
        /// <param name="minLon">Minimum Longitude</param>
        /// <param name="minLat">Minimum Latitude</param>
        /// <param name="maxLon">Maximum Longitude</param>
        /// <param name="maxLat">Maximum Latitude</param>
        /// <response code="200">Geo-Messages found!</response>
        /// <response code="404">Failed to find Geo-Messages</response>
        /// <returns>This returns all Geo-Messages within a certain area</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetGeoMessageDTO>>> GetGeoMessagesQuery([FromQuery] double minLon, [FromQuery] double minLat, [FromQuery] double maxLon, [FromQuery] double maxLat)
        {
            var geoMessages = await _context.GeoMessages
                .Select(g =>
                    new GetGeoMessageDTO
                    {
                        Message = new GetMessageDTO
                        {
                            Title = g.Title,
                            Body = g.Body,
                            Author = g.Author
                        },
                        Latitude = g.Latitude,
                        Longitude = g.Longitude
                    }
                )
                .ToListAsync();

            if (Request.Query.ContainsKey("minLon") && Request.Query.ContainsKey("minLat") && Request.Query.ContainsKey("maxLon") && Request.Query.ContainsKey("maxLat"))
                geoMessages = geoMessages.Where(g =>
                g.Longitude > minLon && g.Longitude < maxLon && g.Latitude > minLat && g.Latitude < maxLat).ToList();

            return Ok(geoMessages);
        }

        /// <summary>
        /// Lägger till ett nytt Geo-Message om användaren är behörig
        /// </summary>
        /// <param name="addGeoMessage"></param>
        /// <param name="ApiKey"></param>
        /// <response code="201">Geo-Message created</response>
        /// <response code="400">Failed to Create Geo-Message</response>
        /// <response code="401">Failed to authorize</response>
        /// <returns>A newly created Geo-Message</returns>
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<GetGeoMessageDTO>> CreateGeoMessage([FromQuery] Guid ApiKey, AddGeoMessageDTO addGeoMessage)
        {
            if (addGeoMessage == null)
            {
                return BadRequest();
            }

            var user = await _userManager.GetUserAsync(this.User);
            var newGeoMessage = new GeoMessage
            {
                Title = addGeoMessage.Message.Title,
                Body = addGeoMessage.Message.Body,
                Author = $"{user.FirstName} {user.LastName}",
                Longitude = addGeoMessage.Longitude,
                Latitude = addGeoMessage.Latitude
            };

            await _context.AddAsync(newGeoMessage);
            await _context.SaveChangesAsync();

            var getGeoMessage = new GetGeoMessageDTO
            {
                Message = new GetMessageDTO
                {
                    Title = newGeoMessage.Title,
                    Body = newGeoMessage.Body,
                    Author = newGeoMessage.Author
                },
                Longitude = newGeoMessage.Longitude,
                Latitude = newGeoMessage.Latitude
            };

            return CreatedAtAction(nameof(GetGeoMessage), new { id = newGeoMessage.Id }, getGeoMessage);
        }
    }
}
