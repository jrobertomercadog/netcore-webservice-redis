using System;
namespace PulseResponse
{
    public class VitalSigns
    {
        public int Id {get; set;}
        public int SpO2 { get; set; }
        public int Pulse { get; set; }
        public long Epoch { get; set; }
    }
}
