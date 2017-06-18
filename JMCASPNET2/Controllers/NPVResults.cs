using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JMCASPNET2.Controllers
{
    public class NPVResults
    {
        public string TradeId;
        public string TradeType;
        public string Maturity;
        public double MaturityTime;
        public double NPV;
        public string NpvCurrency;
        public double NPVBase;
        public string BaseCurrency;
        public double Notional;
        public double NotionalBase;
    }
}