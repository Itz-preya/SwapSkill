using System.Collections.Generic;
using System.Threading.Tasks;
using SkillSwapApp.Models;

namespace SkillSwapApp.Repositories
{
    public interface ISkillRepository
    {
        IEnumerable<Skill> GetSkillsByUser(string userId);
        Skill GetSkill(int id);
        void AddSkill(Skill skill);
        void UpdateSkill(Skill skill);
        void DeleteSkill(int id);
        //Task<IEnumerable<Skill>> GetSkillsByUserIdAsync(string id);
    }
}