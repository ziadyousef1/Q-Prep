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
    public class FrameworksController : ControllerBase
    {
        private readonly IUnitOfWork<Frameworks> frameworksUnitOfWork;
        private readonly IUnitOfWork<MainTrack> mainTrackUnitOfWork;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment hosting;

        public FrameworksController(IUnitOfWork<Frameworks> FrameworksUnitOfWork ,IUnitOfWork<MainTrack> mainTrackUnitOfWork , Microsoft.AspNetCore.Hosting.IHostingEnvironment hosting)
        {
            frameworksUnitOfWork = FrameworksUnitOfWork;
            this.mainTrackUnitOfWork = mainTrackUnitOfWork;
            this.hosting = hosting;
        }


        [HttpGet("GetFramework/{mainTrackId}")]
        public async Task<IActionResult> GetFramework(string mainTrackId)
        {
            var track = await mainTrackUnitOfWork.Entity.GetAsync(mainTrackId);
            if (track == null) 
                return NotFound("This track Not Found");

            var frameworks = await frameworksUnitOfWork.Entity.FindAll(x=>x.MainTrackId == mainTrackId);

            if (frameworks.Count == 0)
                return NotFound("This Track Hasn't Any Framwork!");

            return Ok(frameworks);

        }

        [HttpPost ("AddFramework")]
        [Authorize("AdminRole")]
        public async Task<IActionResult> AddFramework(AddFramewordDTO dto)
        {
            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            var track = await mainTrackUnitOfWork.Entity.GetAsync(dto.MainTrackId);
            if (track == null) 
                return NotFound("This track Not Found");


            string photo = "69c3c85f8aca980abdcfb79fe815dfbb.png";
            if (dto.Photo != null)
            {
                string uploads = Path.Combine(hosting.WebRootPath, @"FrameworkPhoto");
                string fullPath = Path.Combine(uploads, dto.Photo.FileName);
                dto.Photo.CopyTo(new FileStream(fullPath, FileMode.Create));
                photo = dto.Photo.FileName;
            }

            var framwork = new Frameworks
            {
                FrameworkId = Guid.NewGuid().ToString(),
                FrameworkName = dto.FramworkName,
                description = dto.description,
                MainTrackId = dto.MainTrackId,
                Photo = photo,
            };

            await frameworksUnitOfWork.Entity.AddAsync(framwork);
            frameworksUnitOfWork.Save();

            return Ok(framwork);



        }

        [HttpPut("UpdateFramewrok/{frameworkId}")]
        [Authorize("AdminRole")]
        public async Task<IActionResult> UpdateFramewrok(string frameworkId , UpdateFramewordDTO dto)
        {
            var framework = await frameworksUnitOfWork.Entity.GetAsync(frameworkId);

            if (framework == null)
                return NotFound("This Framwork Not Found");

            if (dto.Photo != null)
            {
                string uploads = Path.Combine(hosting.WebRootPath, @"FrameworkPhoto");
                string fullPath = Path.Combine(uploads, dto.Photo.FileName);
                dto.Photo.CopyTo(new FileStream(fullPath, FileMode.Create));
                framework.Photo = dto.Photo.FileName;
            }
            framework.description = dto.description ?? framework.description;
            framework.FrameworkName = dto.FrameworkName ?? framework.FrameworkName;

            await frameworksUnitOfWork.Entity.UpdateAsync(framework);
            frameworksUnitOfWork.Save();
            return Ok(framework);


        }


        [HttpDelete("DeleteFramework/{frameworkId}")]
        [Authorize("AdminRole")]

        public async Task<IActionResult> DeleteFramework(string frameworkId)
        {
            var framework = await frameworksUnitOfWork.Entity.GetAsync(frameworkId);

            if (framework == null)
                return NotFound("This Framework Not Found");

             frameworksUnitOfWork.Entity.Delete(framework);
            frameworksUnitOfWork.Save();

            return Ok("The Framework Is Deleted");


        }









    }
}
