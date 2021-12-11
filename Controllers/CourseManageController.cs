using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using AlgorithmEasy.Server.CourseCenter.Services;
using AlgorithmEasy.Shared.Models;
using AlgorithmEasy.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlgorithmEasy.Server.CourseCenter.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class CourseManageController : ControllerBase
    {
        private readonly CourseManageService _courseManager;

        private string UserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public CourseManageController(CourseManageService courseManager) => _courseManager = courseManager;

        [Authorize(Roles = "Teacher, Student")]
        [HttpGet]
        public async Task<ActionResult<GetCoursesResponse>> GetCourses()
        {
            return Ok(new GetCoursesResponse
            {
                Courses = await _courseManager.GetCourses()
            });
        }

        [Authorize(Roles = "Teacher, Student")]
        [HttpGet]
        public async Task<ActionResult<Course>> GetCourseDetail([Required] int courseId)
        {
            var course = await _courseManager.GetCourseDetail(courseId);
            if (course == null)
                return BadRequest();
            return Ok(course);
        }

        [Authorize(Roles = "Teacher")]
        [HttpPost]
        public async Task<ActionResult<string>> CreateCourse([Required] string title, [FromBody] string content)
        {
            if (await _courseManager.CreateCourse(title, content))
                return Ok($"课程{title}创建成功。");
            return BadRequest($"课程{title}创建失败。");
        }

        [Authorize(Roles = "Teacher")]
        [HttpPut]
        public async Task<ActionResult<string>> UpdateCourseContent([Required] int courseId, [FromBody] string content)
        {
            if (await _courseManager.UpdateCourseContent(courseId, content))
                return Ok($"课程内容更新成功。");
            return BadRequest("课程内容更新失败，请稍后再试。");
        }

        [Authorize(Roles = "Teacher")]
        [HttpPut]
        public async Task<ActionResult<string>> UpdateCourseWorkspace([Required] int courseId, [FromBody] string workspace)
        {
            if (await _courseManager.UpdateCourseWorkspace(courseId, workspace))
                return Ok("课程案例更新成功。");
            return BadRequest("课程案例更新失败。");
        }
    }
}