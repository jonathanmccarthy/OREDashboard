using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JMCASPNET2.Controllers
{
    public class ExposureResult
    {
        public string TradeId;
        public string Date;
        public double Time;
        public double EPE;
        public double ENE;
        public double AllocatedEPE;
        public double AllocatedENE;
        public double PFE;
        public double BaselEE;
        public double BaselEEE;
    }

    public class ExposureResults
    {
        public List<ExposureResult> Results;

        public ExposureResults()
        {
            Results = new List<ExposureResult>();
        }
    }
}