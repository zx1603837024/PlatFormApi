using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace F2.Application.Parkade.Dtos
{
    public class ParkadeResponse
    {
        public Response_AlarmInfoPlate Response_AlarmInfoPlate { get; set; }
    }
    public class Response_AlarmInfoPlate
    {
        public string info { get; set; }
        //public List<serialData> serialData { get; set; }

    }
    public class serialData {
        public int serialChannel { get; set; }
        public string data { get; set; }
        public int dataLen { get; set; }
    }
}
