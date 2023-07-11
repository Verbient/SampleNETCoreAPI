using MyApp.Common;
using MyApp.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.Services
{
    public class MiscellaneousService : IMiscellaneousService
    {
        private readonly IAdhocRepository adhocRepository;

        public MiscellaneousService(IAdhocRepository adhocRepository)
        {
            this.adhocRepository = adhocRepository;
        }
        public void ResetDatabase()
        {
            adhocRepository.ExecuteCommand("exec sp_DatabaseSeed");
        }

        public void CustomExceptionExample()
        {
            throw new CustomException("This is a Custom Exception");
        }

    }
}
