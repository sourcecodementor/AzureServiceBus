using System;
using System.Collections.Generic;
using System.Text;

namespace AzureServiceBusPublisher.Model
{
    public class SOVMessage
    {
        public int SOVId { get; set; }
        public int FacilityNumber { get; set; }
        public string FacilityName { get; set; }
        public double? RiskScore { get; set; }
        public float? Longitude { get; set; }
        public float? Latitude { get; set; }
        public double TotalInsuredValue { get; set; }
    }
}
