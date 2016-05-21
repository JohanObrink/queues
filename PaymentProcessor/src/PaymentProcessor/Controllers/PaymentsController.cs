using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using PaymentProcessor.Services;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace PaymentProcessor.Controllers
{
    [Route("/")]
    public class PaymentsController : Controller
    {
        private readonly PaymentStore store;
        private readonly PaymentQueues queues;
        public PaymentsController(PaymentStore store, PaymentQueues queues)
        {
            this.store = store;
            this.queues = queues;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(store);
        }

        [HttpPost]
        public IActionResult SetStatus()
        {
            var id = Request.Form["id"];
            var success = Request.Form["result"] == "Success";
            queues.HandleResponse(id, success);
            return View("index", store);
        }
    }
}
