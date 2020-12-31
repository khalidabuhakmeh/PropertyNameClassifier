using System;
using System.Collections.Generic;

namespace Classifier
{
    public class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public List<Person> Friends { get; set; }
        public Guid ExternalId { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }
    }
}