using System;
namespace PulseResponse
{
    public class Patient
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int Age { get; set; }
        public string SecurityID { get; set; }
        public string[] RiskGroups { get; set; }
    }
}
