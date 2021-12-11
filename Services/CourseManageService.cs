using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlgorithmEasy.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace AlgorithmEasy.Server.CourseCenter.Services
{
    public class CourseManageService
    {
        private readonly AlgorithmEasyDbContext _dbContext;

        public CourseManageService(AlgorithmEasyDbContext dbContext) => _dbContext = dbContext;

        public async Task<IEnumerable<Course>> GetCourses()
        {
            return await _dbContext.Courses
                .Include(c => c.CourseDetail)
                .OrderBy(c => c.CourseId)
                .ToListAsync();
        }

        public async Task<Course> GetCourseDetail(int courseId)
        {
            return await _dbContext.Courses
                .Where(c => c.CourseId == courseId)
                .Include(c => c.CourseDetail)
                .SingleOrDefaultAsync();
        }

        public async Task<bool> CreateCourse(string title, string content)
        {
            if (await _dbContext.CourseDetails.AnyAsync(d => d.Title == title))
                return false;
            await _dbContext.Courses
                .AddAsync(
                    new Course
                    {
                        CourseDetail = new CourseDetail
                        {
                            Title = title,
                            Content = content
                        }
                    });
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateCourseContent(int courseId, string content)
        {
            var course = await _dbContext.Courses
                .Include(c => c.CourseDetail)
                .SingleOrDefaultAsync(c => c.CourseId == courseId);
            if (course == null)
                return false;

            course.CourseDetail.Content = content;
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateCourseWorkspace(int courseId, string workspace)
        {
            var course = await _dbContext.Courses
                .Include(c => c.CourseDetail)
                .SingleOrDefaultAsync(c => c.CourseId == courseId);
            if (course == null)
                return false;

            course.CourseDetail.Workspace = workspace;
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}