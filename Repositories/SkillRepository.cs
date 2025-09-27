using SkillSwapApp.Data;
using SkillSwapApp.Models;
using System.Collections.Generic;
using System.Linq;

namespace SkillSwapApp.Repositories
{
    public class SkillRepository : ISkillRepository
    {
        private readonly ApplicationDbContext _context;

        public SkillRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Skill> GetSkillsByUser(string userId)
        {
            return _context.Skills.Where(s => s.UserId == userId).ToList();
        }

        public Skill GetSkill(int id)
        {
            return _context.Skills.FirstOrDefault(s => s.Id == id);
        }

        public void AddSkill(Skill skill)
        {
            _context.Skills.Add(skill);
            _context.SaveChanges();
        }

        public void UpdateSkill(Skill skill)
        {
            _context.Skills.Update(skill);
            _context.SaveChanges();
        }

        public void DeleteSkill(int id)
        {
            var skill = _context.Skills.FirstOrDefault(s => s.Id == id);
            if (skill != null)
            {
                _context.Skills.Remove(skill);
                _context.SaveChanges();
            }
        }
    }
}