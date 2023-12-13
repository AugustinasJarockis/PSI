using NoteaAPI.Models.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoteaApiUnitTest
{
    public class DomainTest
    {
        [Fact]
        public void CreatedUserConspectsModel_NoRightsProvided_ShouldBeA()
        {
            UserConspectsModel user = new UserConspectsModel(1, 2);
            Assert.Equal('a', user.Access_Type);
        }
    }
}
