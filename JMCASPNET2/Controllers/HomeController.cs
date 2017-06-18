using System;
using System.Web.Mvc;

namespace JMCASPNET2.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            int hour = DateTime.Now.Hour;

            if (hour < 12)
                ViewBag.Greeting = "Good Morning. Time is " + DateTime.Now.ToShortTimeString();
            else
                ViewBag.Greeting = "Good Afternoon. Time is " + DateTime.Now.ToShortTimeString();

            return View();
        }

        public ActionResult IRSwap()
        {
            ViewBag.Message = "Enter IRSwap details.";

            return View();
        }

        public ActionResult NPVResults()
        {
            ViewBag.Message = "IR Swap results.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public JsonResult RunORE(string tradeSpecification)
        {
            OREManager myORE = new OREManager();

            // call ORE to perform the calculation
            bool success = myORE.Run(tradeSpecification);

            return Json(success);
        }
    }
}