using System;
using System.Configuration;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;

namespace JMCASPNET2.Controllers
{
    public class OREManager
    {
        private string ShellCmdExe; //@"C:\Windows\System32\cmd.exe";
        private string ORERoot; //@"C:\work\ORE-1.8.2\";

        private string TradeInputFile; // @"Examples\Example_1\Input\portfolio_swap.xml";

        private string OREExe; //@"App\bin\x64\Release\ore.exe";
        private string OREConfigurationFile; //@"Examples\Example_1\Input\ore.xml";
        private string InputPath; //@"Examples\Example_1";

        private string NPVResultsFile; //@"Examples\Example_1\Output\npv.csv";
        private string ExposureResultsFile; //@"Examples\Example_1\Output\exposure_trade_Swap_20y.csv";

        private List<NPVResults> _results;
        private ExposureResults _exposureResults;

        public OREManager()
        {
            ShellCmdExe = ConfigurationManager.AppSettings.Get("ShellCmdExe");
            ORERoot = ConfigurationManager.AppSettings.Get("ORERoot");

            OREExe = ConfigurationManager.AppSettings.Get("OREExe");
            InputPath = ConfigurationManager.AppSettings.Get("InputPath");
            TradeInputFile = ConfigurationManager.AppSettings.Get("TradeInputFile");
            OREConfigurationFile = ConfigurationManager.AppSettings.Get("OREConfigurationFile");
            NPVResultsFile = ConfigurationManager.AppSettings.Get("NPVResultsFile");
            ExposureResultsFile = ConfigurationManager.AppSettings.Get("ExposureResultsFile");
        }

        public bool Run(string tradeSpecification)
        {
            SaveTradeDetails(tradeSpecification);
            return RunORE();
        }

        private string GetXmlValue(XmlDocument xml, string xmlPath)
        {
            XmlNode dataAttribute = xml.SelectSingleNode(xmlPath);
            return dataAttribute.InnerText;
        }

        private string GetXmlAttribute(XmlDocument xml, string xmlPath, string attr)
        {
            XmlNode dataAttribute = xml.SelectSingleNode(xmlPath + "/" + attr);
            return dataAttribute.InnerText;
        }

        private string GetLegTypeDetails(XmlDocument xml, string legPath, string legType)
        {
            string result;
            if (legType == "Fixed")
            {
                result =
                    "        <FixedLegData>" +
                    "          <Rates>" +
                    "            <Rate>" + GetXmlValue(xml, "/Details/" + legPath + "/Rate") + "</Rate>" +
                    "          </Rates>" +
                    "        </FixedLegData>";            }
            else
            {
                result =
                    "        <FloatingLegData>" +
                    "          <Index>" + GetXmlAttribute(xml, "/Details/" + legPath + "/Index", "id") + "</Index>" +
                    "          <Spreads>" +
                    "            <Spread>" + GetXmlValue(xml, "/Details/" + legPath + "/Spread") + "</Spread>" +
                    "          </Spreads>" +
                    "          <IsInArrears>" + GetXmlAttribute(xml, "/Details/" + legPath + "/IsInArrears", "id") + "</IsInArrears>" +
                    "          <FixingDays>" + GetXmlValue(xml, "/Details/" + legPath + "/FixingDays") + "</FixingDays>" +
                    "        </FloatingLegData>";
            }

            return result;
        }

        private bool SaveTradeDetails(string tradeSpecification)
        {
            dynamic trade = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(tradeSpecification);

            XmlDocument xml = Newtonsoft.Json.JsonConvert.DeserializeXmlNode(tradeSpecification);

            // first get the leg type details
            string payLegDetails = GetLegTypeDetails(xml, "PayLeg", GetXmlAttribute(xml, "/Details/PayLeg/LegType", "id"));
            string receiveLegDetails = GetLegTypeDetails(xml, "ReceiveLeg", GetXmlAttribute(xml, "/Details/ReceiveLeg/LegType", "id"));

            string xmlToSave = "<?xml version=\"1.0\"?>" +
"<Portfolio>" +
"  <Trade id=\"" + GetXmlValue(xml, "/Details/TradeId") + "\">" +
"    <TradeType>" + GetXmlValue(xml, "/Details/TradeType") + "</TradeType>" +
"    <Envelope>" +
"      <CounterParty>" + GetXmlValue(xml, "/Details/CounterParty") + "</CounterParty>" +
"      <NettingSetId>" + GetXmlValue(xml, "/Details/NettingSetId") + "</NettingSetId>" +
"      <AdditionalFields/>" +
"    </Envelope>" +
"    <SwapData>" +
"      <LegData>" +
"        <LegType>" + GetXmlAttribute(xml, "/Details/PayLeg/LegType", "id") + "</LegType>" +
"        <Payer>true</Payer>" +
"        <Currency>" + GetXmlAttribute(xml, "/Details/PayLeg/Currency", "id") + "</Currency>" +
"        <Notionals>" +
"          <Notional>" + GetXmlValue(xml, "/Details/PayLeg/Notional") + "</Notional>" +
"        </Notionals>" +
"        <DayCounter>" + GetXmlAttribute(xml, "/Details/PayLeg/DayCounter", "id") + "</DayCounter>" +
"        <PaymentConvention>" + GetXmlAttribute(xml, "/Details/PayLeg/PaymentConvention", "id") + "</PaymentConvention>" +

        payLegDetails +

"        <ScheduleData>" +
"          <Rules>" +
"            <StartDate>" + GetXmlValue(xml, "/Details/PayLeg/Schedule/StartDate") + "</StartDate>" +
"            <EndDate>" + GetXmlValue(xml, "/Details/PayLeg/Schedule/EndDate") + "</EndDate>" +
"            <Tenor>" + GetXmlValue(xml, "/Details/PayLeg/Schedule/Tenor") + GetXmlAttribute(xml, "/Details/PayLeg/Schedule/TenorPeriod", "id") + "</Tenor>" +
"            <Calendar>" + GetXmlAttribute(xml, "/Details/PayLeg/Schedule/Calendar", "id") + "</Calendar>" +
"            <Convention>" + GetXmlAttribute(xml, "/Details/PayLeg/Schedule/Convention", "id") + "</Convention>" +
"            <TermConvention>" + GetXmlAttribute(xml, "/Details/PayLeg/Schedule/TermConvention", "id") + "</TermConvention>" +
"            <Rule>" + GetXmlAttribute(xml, "/Details/PayLeg/Schedule/Rule", "id") + "</Rule>" +
"            <EndOfMonth>" + GetXmlValue(xml, "/Details/PayLeg/Schedule/EndOfMonth") + "</EndOfMonth>" +
"            <FirstDate>" + GetXmlValue(xml, "/Details/PayLeg/Schedule/FirstDate") + "</FirstDate>" +
"            <LastDate>" + GetXmlValue(xml, "/Details/PayLeg/Schedule/LastDate") + "</LastDate>" +
"          </Rules>" +
"        </ScheduleData>" +
"      </LegData>" +
"      <LegData>" +
"        <LegType>" + GetXmlAttribute(xml, "/Details/ReceiveLeg/LegType", "id") + "</LegType>" +
"        <Payer>false</Payer>" +
"        <Currency>" + GetXmlAttribute(xml, "/Details/ReceiveLeg/Currency", "id") + "</Currency>" +
"        <Notionals>" +
"          <Notional>" + GetXmlValue(xml, "/Details/ReceiveLeg/Notional") + "</Notional>" +
"        </Notionals>" +
"        <DayCounter>" + GetXmlAttribute(xml, "/Details/ReceiveLeg/DayCounter", "id") + "</DayCounter>" +
"        <PaymentConvention>" + GetXmlAttribute(xml, "/Details/ReceiveLeg/PaymentConvention", "id") + "</PaymentConvention>" +

        receiveLegDetails +

"        <ScheduleData>" +
"          <Rules>" +
"            <StartDate>" + GetXmlValue(xml, "/Details/ReceiveLeg/Schedule/StartDate") + "</StartDate>" +
"            <EndDate>" + GetXmlValue(xml, "/Details/ReceiveLeg/Schedule/EndDate") + "</EndDate>" +
"            <Tenor>" + GetXmlValue(xml, "/Details/ReceiveLeg/Schedule/Tenor") + GetXmlAttribute(xml, "/Details/ReceiveLeg/Schedule/TenorPeriod", "id") + "</Tenor>" +
"            <Calendar>" + GetXmlAttribute(xml, "/Details/ReceiveLeg/Schedule/Calendar", "id") + "</Calendar>" +
"            <Convention>" + GetXmlAttribute(xml, "/Details/ReceiveLeg/Schedule/Convention", "id") + "</Convention>" +
"            <TermConvention>" + GetXmlAttribute(xml, "/Details/ReceiveLeg/Schedule/TermConvention", "id") + "</TermConvention>" +
"            <Rule>" + GetXmlAttribute(xml, "/Details/ReceiveLeg/Schedule/Rule", "id") + "</Rule>" +
"            <EndOfMonth>" + GetXmlValue(xml, "/Details/ReceiveLeg/Schedule/EndOfMonth") + "</EndOfMonth>" +
"            <FirstDate>" + GetXmlValue(xml, "/Details/ReceiveLeg/Schedule/FirstDate") + "</FirstDate>" +
"            <LastDate>" + GetXmlValue(xml, "/Details/ReceiveLeg/Schedule/LastDate") + "</LastDate>" +
"          </Rules>" +
"        </ScheduleData>" +
"      </LegData>" +
"    </SwapData>" +
"  </Trade>" +
"</Portfolio>";

            string filePath = Path.Combine(ORERoot, @"Examples\Example_1\Input\portfolio_swap.xml");
            File.WriteAllText(filePath, xmlToSave);
            
            return true;
        }

        private bool RunORE()
        {
            string exePath = Path.Combine(ORERoot, OREExe); // @"App\bin\x64\Release\ore.exe");
            string otherPath = Path.Combine(ORERoot, OREConfigurationFile); // @"Examples\Example_1\Input\ore.xml");
            string inputPath = Path.Combine(ORERoot, InputPath); // @"Examples\Example_1");

            if (!File.Exists(exePath)) return false;
            if (!File.Exists(otherPath)) return false;

            bool status = ShellRun($"{ShellCmdExe}", $"/C cd {inputPath} & {exePath} {otherPath}");
            //bool status = true;

            // results are retrieved separately so just return success or failure
            return status;
        }

        public bool LoadNPV()
        {
            bool first = true;
            string filePath = Path.Combine(ORERoot, NPVResultsFile); // @"Examples\Example_1\Output\npv.csv");

            var fileLines = File.ReadAllLines(filePath);

            _results = new List<NPVResults>();
            foreach (string line in fileLines)
            {
                if (first)
                    first = false;
                else
                {
                    double maturityTime, npv, npvBase, notional, notionalBase;
                    var fields = line.Split(',');
                    Double.TryParse(fields[3], out maturityTime);
                    Double.TryParse(fields[4], out npv);
                    Double.TryParse(fields[6], out npvBase);
                    Double.TryParse(fields[8], out notional);
                    Double.TryParse(fields[9], out notionalBase);

                    NPVResults result = new NPVResults()
                    {
                        TradeId = fields[0],
                        TradeType = fields[1],
                        Maturity = fields[2],
                        MaturityTime = maturityTime,
                        NPV = npv,
                        NpvCurrency = fields[5],
                        NPVBase = npvBase,
                        BaseCurrency = fields[7],
                        Notional = notional,
                        NotionalBase = notionalBase,
                    };

                    _results.Add(result);
                }
            }

            return true;
        }

        public bool LoadExposures()
        {
            bool first = true;
            string filePath = Path.Combine(ORERoot, ExposureResultsFile); // @"Examples\Example_1\Output\exposure_trade_Swap_20y.csv");

            var fileLines = File.ReadAllLines(filePath);

            _exposureResults = new ExposureResults();
            foreach (string line in fileLines)
            {
                if (first)
                    first = false;
                else
                {
                    double time, epe, ene, allocatedEPE, allocatedENE, pfe, baselEE, baselEEE;

                    var fields = line.Split(',');
                    Double.TryParse(fields[2], out time);
                    Double.TryParse(fields[3], out epe);
                    Double.TryParse(fields[4], out ene);
                    Double.TryParse(fields[6], out allocatedEPE);
                    Double.TryParse(fields[8], out allocatedENE);
                    Double.TryParse(fields[9], out pfe);
                    Double.TryParse(fields[9], out baselEE);
                    Double.TryParse(fields[9], out baselEEE);

                    ExposureResult result = new ExposureResult()
                    {
                        TradeId = fields[0],
                        Date = fields[1],
                        Time = time,
                        EPE = epe,
                        ENE = ene,
                        AllocatedEPE = allocatedEPE,
                        AllocatedENE = allocatedENE,
                        PFE = pfe,
                        BaselEE = baselEE,
                        BaselEEE = baselEEE,
                    };

                    _exposureResults.Results.Add(result);
                }
            }

            return true;
        }

        public bool ShellRun(string command, string arguments)
        {
            try
            {
                Process process = Process.Start(command, arguments);
                int id = process.Id;
                Process tempProc = Process.GetProcessById(id);
                tempProc.WaitForExit();

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public string GetResults()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(_results);
        }

        public string GetExposureResults()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(_exposureResults);
        }
    }
}