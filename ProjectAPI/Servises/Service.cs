using Core.Interfaces;
using Core.Model;
using ProjectAPI.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Servises
{
    public class Service
    {
        private readonly IUnitOfWork<Test> testUnitOfWork;
        private readonly IUnitOfWork<Frameworks> frameworksUnitOfWork;
        private readonly IUnitOfWork<MainTrack> mainTrackUnitOfWork;
        public Service(IUnitOfWork<Test> TestUnitOfWork, IUnitOfWork<Frameworks> FrameworksUnitOfWork, IUnitOfWork<MainTrack> mainTrackUnitOfWork)
        {
            testUnitOfWork = TestUnitOfWork;
            frameworksUnitOfWork = FrameworksUnitOfWork;
            this.mainTrackUnitOfWork = mainTrackUnitOfWork;


        }


        public async Task<List<object>> RendomQuestions(string framework)
        {
            var random = new Random();
            var Questions = await testUnitOfWork.Entity.FindAll(x=>x.FrameworkId == framework,x => x.Q_Id);

            if (Questions.Count() == 0)
                return null;

            var count = Questions.Count();
            var list = new List<object>();
            var list1 = new List<string>();
            for (var i = 0; i < count && i < 10; i++)
            {
                var index = random.Next(0, Questions.Count());
                var QuestionIndex = Questions[index];
                if (list1.Count() == 0 || !list1.Contains(QuestionIndex))
                {
                    list1.Add(QuestionIndex);
                    var question = await testUnitOfWork.Entity.GetAsync(QuestionIndex);
                    list.Add(question);
                }
                else
                {
                    --i;
                }

            }

            return list;

        }


        public async Task<bool> ComparerAnswer(string questionId, string selectedAnwser)
        {
            var question = await testUnitOfWork.Entity.GetAsync(questionId);

            if (question.CorrectAnswers != selectedAnwser)
                return false;
            return true;


        }

        public string DetermineLevel(int count)
        {
            if (count <= 4)
                return "beginner";
            else if (count > 4 && count <= 7)
                return "Intermediate";
            else
                return " Advanced";



        }


    }
}
