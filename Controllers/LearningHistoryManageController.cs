using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using AlgorithmEasy.Server.CourseCenter.Services;
using AlgorithmEasy.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlgorithmEasy.Server.CourseCenter.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class LearningHistoryManageController : ControllerBase
    {
        private readonly LearningHistoryManageService _learningHistoryManager;

        private string UserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public LearningHistoryManageController(LearningHistoryManageService learningHistoryManager) =>
            _learningHistoryManager = learningHistoryManager;

        [Authorize(Roles = "Teacher")]
        [HttpGet]
        public async Task<ActionResult<GetStudentLearningHistoriesResponse>> GetStudentLearningHistories()
        {
            return Ok(new GetStudentLearningHistoriesResponse
            {
                Users = await _learningHistoryManager.GetStudentLearningHistories()
            });
        }

        [Authorize(Roles = "Student")]
        [HttpGet]
        public async Task<ActionResult<GetPersonalLearningHistoriesResponse>> GetPersonalLearningHistories()
        {
            return Ok(new GetPersonalLearningHistoriesResponse
            {
                LearningHistories = await _learningHistoryManager.GetPersonalLearningHistory(UserId)
            });
        }

        [Authorize(Roles = "Student")]
        [HttpPut]
        public async Task<ActionResult<string>> UpdateLearningProgress([Required] int courseId, [Required] int progress)
        {
            if (await _learningHistoryManager.UpdateLearningProgress(UserId, courseId, progress))
                return Ok("保存学习记录成功。");
            return BadRequest("保存学习记录失败，请稍后再试。");
        }
    }
}