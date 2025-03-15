using Core.Interfaces;
using Core.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectAPI.DTO;

namespace ProjectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MainTrackController : ControllerBase
    {
        private readonly IUnitOfWork<MainTrack> mainTrackUnitOfWork;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment hosting;

        public MainTrackController(IUnitOfWork<MainTrack> mainTrackUnitOfWork, Microsoft.AspNetCore.Hosting.IHostingEnvironment hosting)
        {
            this.mainTrackUnitOfWork = mainTrackUnitOfWork;
            this.hosting = hosting;
        }


        [HttpGet("GetMainTrack")]
        public async Task<IActionResult> GetMainTrack()
        {
            var Tracks = await mainTrackUnitOfWork.Entity.GetAllAsync();
            return Ok(Tracks);

        }

        [HttpPost("AddMainTrack")]
        [Authorize("AdminRole")]

        public async Task<IActionResult> AddMainTrack(AddMainTrackDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var UrlPhoto = "69c3c85f8aca980abdcfb79fe815dfbb.png";

            if (dto.Photo != null)
            {
                string uploads = Path.Combine(hosting.WebRootPath, @"TrackPhoto\");
                string fullPath = Path.Combine(uploads, dto.Photo.FileName);
                dto.Photo.CopyTo(new FileStream(fullPath, FileMode.Create));
                UrlPhoto = dto.Photo.FileName;
            }

            var track = new MainTrack
            {
                TrackId = Guid.NewGuid().ToString(),
                TarckName = dto.TarckName,
                Photo = UrlPhoto,
                description = dto.description,

            };

            await mainTrackUnitOfWork.Entity.AddAsync(track);

            mainTrackUnitOfWork.Save();

            return Ok(track);



        }
        [HttpPut("UpdateMaintrack/{TrackId}")]
        [Authorize("AdminRole")]
        public async Task<IActionResult> UpdateMaintrack(UpdateMainTrackDTO dto , string TrackId)
        {
            var track = await mainTrackUnitOfWork.Entity.GetAsync(TrackId);

            if (track == null)
                return NotFound("This Track Not Found");

            if (dto.TarckName == track.TarckName)
                return BadRequest("This Track Is already Exist");

            if (dto.Photo != null)
            {
                string uploads = Path.Combine(hosting.WebRootPath, @"TrackPhoto\");
                string fullPath = Path.Combine(uploads, dto.Photo.FileName);
                dto.Photo.CopyTo(new FileStream(fullPath, FileMode.Create));
                track.Photo = dto.Photo.FileName;
            }

            track.TarckName = dto.TarckName ?? track.TarckName;
            track.description = dto.description ?? track.description;

            await mainTrackUnitOfWork.Entity.UpdateAsync(track);

            mainTrackUnitOfWork.Save();

            return Ok(track);

        }
        [HttpDelete("DeleteTrack{trackID}")]
        [Authorize("AdminRole")]
        public async Task<IActionResult> DeleteTrack(string trackID)
        {
            var track = await mainTrackUnitOfWork.Entity.GetAsync(trackID);

            if (track == null)
                return NotFound("This Track Not Found");

            mainTrackUnitOfWork.Entity.Delete(track);
            return Ok(track);

        }

    }
}
