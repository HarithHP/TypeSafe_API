namespace BilakLk_API.Models
{
    public class ResponseResult
    {
        public string Status { get; set; }//Success or Fail
        public string? Message { get; set; }//Msg of response
        public object? Content { get; set; }//Result 

        
    }
}
