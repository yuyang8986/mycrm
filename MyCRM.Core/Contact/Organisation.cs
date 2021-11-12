using System.Collections.Generic;

namespace MyCRM.Core.Contact
{
    public class Organisation:ContactBase
    {
        public Organisation(string name):base(name)
        {

        }



        public ICollection<People> Peoples { get; set; }
    }
}
