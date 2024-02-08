using Microsoft.AspNetCore.Mvc;

namespace CanDataMonitor.Controllers 
{
    [Route("api/can")] 
    [ApiController]
    public class CanDataController : ControllerBase 
    {
        private static readonly List<CanFrame> _canData = new List<CanFrame>(); 

        [HttpPost]
        public void ReceiveCanData([FromBody] CanFrame canFrame)
        {   
            Console.WriteLine($"CAN ID Received: {canFrame.Id}"); 
            _canData.Add(canFrame); // Stores data in-memory for now
        }
    }
}
