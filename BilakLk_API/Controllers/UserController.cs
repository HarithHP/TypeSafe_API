using BilakLk_API.Config;
using BilakLk_API.Models;
using BilakLk_API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;
using System.Text.RegularExpressions;

namespace BilakLk_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly string Authtoken = "EyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ";

        //Fixed custom authentication
        [HttpPost]
        [Route("Insert")]
        public async Task<ResponseResult> InsertAccountDetails([FromBody, Required] ModelUser account)
        {


            if (Request.Headers.TryGetValue("AuthToken", out var token))
            {
                if (string.IsNullOrWhiteSpace(token) || token != Authtoken)
                {
                    return new ResponseResult()
                    {
                        Status = ApiRespond.Fail.ToString(),
                        Content = null,
                        Message = " Unauthorized! | Error : Authentication token invalid, Please Recheck Your Inputs"
                    };
                }
                if (string.IsNullOrWhiteSpace(account.Email) || string.IsNullOrWhiteSpace(account.FirstName) || string.IsNullOrWhiteSpace(account.LastName))
                {
                    return new ResponseResult()
                    {
                        Status = ApiRespond.Fail.ToString(),
                        Content = null,
                        Message = "Got an Error When Inserting Account Details.\n Error : Got a Null or Empty value for One or More Required Fields, Please Recheck Your Inputs"
                    };
                }
                else
                {
                    UserService service = new();
                    return await service.UserInsert(account);
                }
            }
            else
            {
                return new ResponseResult()
                {
                    Status = ApiRespond.Fail.ToString(),
                    Content = null,
                    Message = " Bad Request! | Error : Authentication token is unavailable, Please Recheck Your Inputs"
                };
            }

        }

        //Fixed custom authentication
        [HttpGet]
        [Route("Login")]
        public async Task<ResponseResult> Select([FromQuery, Required] string email, [FromQuery, Required] string password)
        {
            if (Request.Headers.TryGetValue("AuthToken", out var token))
            {
                if (string.IsNullOrWhiteSpace(token) || token != Authtoken)
                {
                    return new ResponseResult()
                    {
                        Status = ApiRespond.Fail.ToString(),
                        Content = null,
                        Message = " Unauthorized! | Error : Authentication token invalid, Please Recheck Your Inputs"
                    };
                }
                if (string.IsNullOrWhiteSpace(email) || (string.IsNullOrWhiteSpace(password)))
                {
                    return new ResponseResult()
                    {
                        Status = ApiRespond.Fail.ToString(),
                        Content = null,
                        Message = "Mandatory cannot be empty , Please Recheck your Inputs"
                    };
                }
                else
                {
                    AuthenticationService service = new();
                    return await service.UserLogin(email, password);
                }
            }
            else
            {
                return new ResponseResult()
                {
                    Status = ApiRespond.Fail.ToString(),
                    Content = null,
                    Message = " Bad Request! | Error : Authentication token is unavailable, Please Recheck Your Inputs"
                };
            }

        }

        //Reset own password,Admin can change users password by using "Update" method.
        //Custom Authentication
        [HttpPut]
        [Route("ResetPassword")]
        public async Task<ActionResult<ResponseResult>> ResetPassword([FromQuery, Required] string currentPassword, [FromQuery, Required] string newPassword)
        {
            AuthenticationService tokenService = new();
            if (Request.Headers.TryGetValue("AuthToken", out var token) && Request.Headers.TryGetValue("Email", out var email))
            {
                var ol = await tokenService.ValidateUser(token, email);
                if (ol.Status == ApiRespond.Success.ToString())
                {
                    if (string.IsNullOrWhiteSpace(currentPassword) || string.IsNullOrWhiteSpace(currentPassword))
                    {
                        return new ResponseResult()
                        {
                            Status = ApiRespond.Fail.ToString(),
                            Content = null,
                            Message = "Got an Error When Updating Your Account Password.\n Error : Got a Null or Empty value for One or More Required Fields, Please Recheck Your Inputs"
                        };
                    }
                    else if (currentPassword == newPassword)
                    {
                        return new ResponseResult()
                        {
                            Status = ApiRespond.Fail.ToString(),
                            Content = null,
                            Message = "Got an Error When Reseting Your User Account Password.\n Error : Your current password and new password are same, Please Recheck Your Inputs"
                        };
                    }
                    else
                    {
                        AuthenticationService service = new();
                        return await service.ResetPassword(email, currentPassword, newPassword);
                    }
                }
                else
                {
                    return Unauthorized(ol);
                }
            }
            else
            {
                return BadRequest(tokenService.BadRequestRespond());
            }


        }

        //Reset own password,Admin can change users password by using "Update" method.
        //Custom Authentication
        [HttpPut]
        [Route("UserUpdate")]
        public async Task<ActionResult<ResponseResult>> UserUpdate([FromBody, Required] ModelUser modelUser)
        {
            AuthenticationService tokenService = new();
            if (Request.Headers.TryGetValue("AuthToken", out var token) && Request.Headers.TryGetValue("Email", out var email))
            {
                var ol = await tokenService.ValidateUser(token, email);
                if (ol.Status == ApiRespond.Success.ToString())
                {
                    if (string.IsNullOrWhiteSpace(modelUser.FirstName) || string.IsNullOrWhiteSpace(modelUser.LastName))
                    {
                        return new ResponseResult()
                        {
                            Status = ApiRespond.Fail.ToString(),
                            Content = null,
                            Message = "Got an Error When Updating User Account.\n Error : Got Empty value for One or More Required Fields, Please Recheck Your Inputs"
                        };
                    }
                    else if (modelUser.Id == null || modelUser.Id <= 0)
                    {
                        return new ResponseResult()
                        {
                            Status = ApiRespond.Fail.ToString(),
                            Content = null,
                            Message = "Got an Error When Updating User Account.\n Error : Got Empty value for User ID, Please Recheck Your Inputs"
                        };
                    }
                    else
                    {
                        UserService service = new();
                        return await service.UserUpdate(modelUser);
                    }
                }
                else
                {
                    return Unauthorized(ol);
                }
            }
            else
            {
                return BadRequest(tokenService.BadRequestRespond());
            }


        }


    }
}
