using Core.Interfaces;
using Core.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectAPI.DTO;

namespace ProjectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RequestController : ControllerBase
    {
        private readonly IUnitOfWork<MainTrack> mainTrackUnitOfWork;
        private readonly IUnitOfWork<RequestQuestions> requestnUitOfWork;
        private readonly IUnitOfWork<BeginnerLevel> beginnerUnitOfWork;
        private readonly UserManager<AppUser> userManager;
        private readonly IUnitOfWork<AdvancedLevel> advancedUnitOfWork;
        private readonly IUnitOfWork<IntermediateLevel> intermediateUnitOfWork;

        public RequestController(IUnitOfWork<MainTrack> MainTrackUnitOfWork, IUnitOfWork<RequestQuestions> RequestnUitOfWork, IUnitOfWork<BeginnerLevel> BeginnerUnitOfWork, UserManager<AppUser> userManager, IUnitOfWork<AdvancedLevel> AdvancedUnitOfWork, IUnitOfWork<IntermediateLevel> IntermediateUnitOfWork)
        {
            mainTrackUnitOfWork = MainTrackUnitOfWork;
            requestnUitOfWork = RequestnUitOfWork;
            beginnerUnitOfWork = BeginnerUnitOfWork;
            this.userManager = userManager;
            advancedUnitOfWork = AdvancedUnitOfWork;
            intermediateUnitOfWork = IntermediateUnitOfWork;
        }



        [HttpGet("GetRequests")]
        [Authorize("AdminRole")]
        public async Task<IActionResult> GetRequests()
        {
            var requests = await requestnUitOfWork.Entity.GetAllAsync(); 
            if (requests == null)
                return NotFound("Not Found Any Request");


            var request =  requests.Select( x => new GetRequestsDTO
            {

                RequestId = x.RequestId,
                Answers = x.Answers,
                DateRequest = x.DateRequest,
                Questions = x.Questions,
                Email = userManager.Users.Where(i=>i.Id == x.UserId).Select(x => x.Email).FirstOrDefault() ?? "",
                UserName = userManager.Users.Where(i => i.Id == x.UserId).Select(x => x.UserName).FirstOrDefault() ?? "",
                Photo = userManager.Users.Where(i => i.Id == x.UserId).Select(x => x.Photo).FirstOrDefault() ?? "",
                MainTrack = mainTrackUnitOfWork.Entity.Find(t => t.TrackId == x.MainTrackId).TarckName ?? ""
            });


            return Ok(request);


        }


        [HttpPost("AddQuestionRequest")]
        [Authorize("UserRole")]
        public async Task<IActionResult> AddQuestionRequest(AddQuestionsDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userid = userManager.GetUserId(HttpContext.User);

            var user = await userManager.FindByIdAsync(userid);

            if (user == null)
                return BadRequest("This User Not Found");
            var Main = mainTrackUnitOfWork.Entity.Find(x => x.TarckName==dto.MainTrackName);

            if (Main == null)
                return BadRequest("This Tarck Not Found");


            var Question = new RequestQuestions
            {
                RequestId = Guid.NewGuid().ToString(),
                UserId = userid,
                Answers = dto.Answers,
                FrameworkName = dto.FrameworkName,
                MainTrackId = Main.TrackId,
                Questions = dto.Questions,
                DateRequest = DateTime.Now,
            };
            await requestnUitOfWork.Entity.AddAsync(Question);
            requestnUitOfWork.Save();
            return Ok(Question);
        }


        [HttpPost("UpdateRequest/{requestId}")]

        public async Task<IActionResult> UpdateRequest(UpdateRequestDTO dto , string requestId)
        {
            var req = await requestnUitOfWork.Entity.GetAsync(requestId);
            if(req == null)
                return NotFound("This Request Is Not Found");

            req.Questions = dto.Questions ?? req.Questions;
            req.Answers = dto.Answers ?? req.Answers;
            await requestnUitOfWork.Entity.UpdateAsync(req);
            requestnUitOfWork.Save();

            return Ok(req);
            


        }


        [HttpDelete("DeleteRequest/{requestId}")]
        [Authorize("AdminRole")]
        public async Task<IActionResult> DeleteRequest(string requestId)
        {
            var req = await requestnUitOfWork.Entity.GetAsync(requestId);
            if (req == null)
                return NotFound("This Request Is Not Found");

            requestnUitOfWork.Entity.Delete(req);
            requestnUitOfWork.Save();
            
            return Ok("This Request Is Deleted");

        }


        
    }
}
