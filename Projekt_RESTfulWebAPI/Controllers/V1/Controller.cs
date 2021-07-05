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
using Projekt_RESTfulWebAPI.DTO.V1;

namespace Projekt_RESTfulWebAPI.Controllers.V1
{
    [Route("api/v{version:apiVersion}/geo-comments")]
    [ApiController]
    [ApiVersion("1.0")]
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
        public async Task<ActionResult<GeoMessageDTO>> GeoMessageDTO (int id)
        {
            var geoMessage = await _context.GeoMessages.Include(g => g.Message).FirstOrDefaultAsync(g => g.Id == id);

            if (geoMessage == null)
                return NotFound();

            var geoMessageDTO = new GeoMessageDTO
            {
                Message = geoMessage.Message,
                Latitude = geoMessage.Latitude,
                Longitude = geoMessage.Longitude
            };

            return Ok(geoMessageDTO);
        }

        /// <summary>
        /// Hämtar alla Geo-Message
        /// </summary>
        /// <response code="200">Geo-Messages found!</response>
        /// <response code="404">Failed to find Geo-Messages</response>
        /// <returns>This returns all Geo-Messages</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GeoMessageDTO>>> GetGeoMessages()
        {
            return await _context.GeoMessages
                .Select(g => 
                    new GeoMessageDTO
                    {
                        Message = g.Message,
                        Latitude = g.Latitude,
                        Longitude = g.Longitude
                    }
                )
                .ToListAsync();
        }

        /// <summary>
        /// Lägger till ett nytt Geo-Message om användaren är behörig
        /// </summary>
        /// <param name="geoMessageDTO"></param>
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
        public async Task<ActionResult<GeoMessageDTO>> CreateGeoMessage([FromQuery] Guid ApiKey, GeoMessageDTO geoMessageDTO)
        {
            if(geoMessageDTO == null)
            {
                return BadRequest();
            }

            var user = await _userManager.GetUserAsync(this.User);
            var newGeoMessage = new GeoMessage
            {
                Body = geoMessageDTO.Message,
                Author = $"{user.FirstName} {user.LastName}",
                Longitude = geoMessageDTO.Longitude,
                Latitude = geoMessageDTO.Latitude
            };

            await _context.AddAsync(newGeoMessage);
            await _context.SaveChangesAsync();

            var getGeoMessage = new GeoMessageDTO
            {
                Message = newGeoMessage.Message,
                Longitude = newGeoMessage.Longitude,
                Latitude = newGeoMessage.Latitude
            };

            return CreatedAtAction(nameof(GeoMessageDTO), new { id = newGeoMessage.Id }, getGeoMessage);
        }
    }
}
