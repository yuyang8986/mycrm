using System;
namespace MyCRM.Core.Contact
{
    public class People: ContactBase, IPeople
    {
        public People(string name):base(name)
        {
        }

        public People(string name, bool isCustomer) : base(name)
        {
            IsCustomer = isCustomer;
        }

        public bool IsCustomer { get; set; }




        //organisation - people, 1 to many, people can be nullable 
    }
}
