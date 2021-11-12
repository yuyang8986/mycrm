using System;
namespace MyCRM.Core.Contact
{
    public abstract class ContactBase:BaseEntity<ContactBase>,IContact
    {
        protected ContactBase(string name):base(name)
        {
        }

       
    }
}
