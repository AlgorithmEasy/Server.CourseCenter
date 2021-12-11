using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlgorithmEasy.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace AlgorithmEasy.Server.CourseCenter.Services
{
    public class LearningHistoryManageService
    {
        private readonly AlgorithmEasyDbContext _dbContext;

        public LearningHistoryManageService(AlgorithmEasyDbContext dbContext) => _dbContext = dbContext;

        public async Task<IEnumerable<User>> GetStudentLearningHistories()
        {
            return await _dbContext.Users
                .Include(u => u.Role)
                .Where(u => u.Role.RoleName == "Student")
                .Include(u => u.LearningHistories)
                .ToListAsync();
        }

        public async Task<IEnumerable<LearningHistory>> GetPersonalLearningHistory(string userId)
        {
            return await _dbContext.LearningHistories
                .Where(h => h.UserId == userId)
                .OrderBy(h => h.CourseId)
                .ToListAsync();
        }

        public async Task<bool> UpdateLearningProgress(string userId, int courseId, int progress)
        {
            if (await _dbContext.Courses.AllAsync(c => c.CourseId != courseId))
                return false;
            progress = Math.Min(progress, 100);
            var history = await _dbContext.LearningHistories
                .Where(h => h.UserId == userId && h.CourseId == courseId)
                .SingleOrDefaultAsync();
            if (history == null)
            {
                await _dbContext.AddAsync(new LearningHistory
                {
                    UserId = userId,
                    CourseId = courseId,
                    Progress = progress
                });
            }
            else
            {
                history.Progress = progress;
            }

            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}