using BilakLk_API.Config;
using BilakLk_API.Models;
using BilakLk_API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TypeSafe_API.Models;
using TypeSafe_API.Services;
using static System.Net.Mime.MediaTypeNames;

namespace TypeSafe_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DesignController : ControllerBase
    {
        //Custom Authentication
        [HttpPost]
        [Route("Insert/Design")]
        public async Task<ActionResult<ResponseResult>> InsertDesign ([FromBody, Required] ModelDesignUpload modelDesignUpload)
        {
            AuthenticationService tokenService = new();
            if (Request.Headers.TryGetValue("AuthToken", out var token) && Request.Headers.TryGetValue("Email", out var email))
            {
                var ol = await tokenService.ValidateUser(token, email);
                if (ol.Status == ApiRespond.Success.ToString())
                {
                    if (string.IsNullOrWhiteSpace(modelDesignUpload.Name))
                    {
                        return new ResponseResult()
                        {
                            Status = ApiRespond.Fail.ToString(),
                            Content = null,
                            Message = "Got an Error When Save Your Design.\n Error : Got Empty value for Design Name, Please Recheck Your Inputs"
                        };
                    }
                    else if (modelDesignUpload.UserId == null || modelDesignUpload.Id <= 0)
                    {
                        return new ResponseResult()
                        {
                            Status = ApiRespond.Fail.ToString(),
                            Content = null,
                            Message = "Got an Error When Save Your Design.\n Error : Got Empty value for User ID, Please Recheck Your Inputs"
                        };
                    }
                    else
                    {

                        if (modelDesignUpload.Image == null || modelDesignUpload.Image.Count == 0)
                        {
                            return new ResponseResult()
                            {
                                Status = ApiRespond.Fail.ToString(),
                                Content = null,
                                Message = "Got an Error When Save Your Design.\n Error : At least one image must be provided , Please Recheck Your Inputs"
                            };
                        }

                        foreach (var image in modelDesignUpload.Image)
                        {
                            // Check if FileName is not empty
                            if (string.IsNullOrEmpty(image.FileName))
                            {
                                return new ResponseResult()
                                {
                                    Status = ApiRespond.Fail.ToString(),
                                    Content = null,
                                    Message = "Got an Error When Save Your Design.\n Error : All images must have a file name , Please Recheck Your Inputs"
                                };
                            }

                            // Check if ImageData is not empty
                            if (image.ImageData == null || image.ImageData.Length == 0)
                            {
                                return new ResponseResult()
                                {
                                    Status = ApiRespond.Fail.ToString(),
                                    Content = null,
                                    Message = "Got an Error When Save Your Design.\n Error : All images must have image data, Please Recheck Your Inputs"
                                };
                            }

                            // Example: Check allowed file extensions (you can modify as needed)
                            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                            if (!allowedExtensions.Contains(System.IO.Path.GetExtension(image.FileName).ToLower()))
                            {
                                return new ResponseResult()
                                {
                                    Status = ApiRespond.Fail.ToString(),
                                    Content = null,
                                    Message = "Got an Error When Save Your Design.\n $\"Image '{image.FileName}' has an unsupported file format., Please Recheck Your Inputs"
                                };
                            }
                        }

                        DesignService service = new();
                        return await service.ManageInsertDesign(modelDesignUpload);
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

        //Custom Authentication
        [HttpGet]
        [Route("Search")]
        public async Task<ActionResult<ResponseResult>> SearchDesignsByName([FromQuery, Required] int UserId, [FromQuery] string? searchString)
        {
            AuthenticationService tokenService = new();
            if (Request.Headers.TryGetValue("AuthToken", out var token) && Request.Headers.TryGetValue("Email", out var email))
            {
                var ol = await tokenService.ValidateUser(token, email);
                if (ol.Status == ApiRespond.Success.ToString())
                {
                    if (UserId <= 0)
                    {
                        return new ResponseResult()
                        {
                            Status = ApiRespond.Fail.ToString(),
                            Content = null,
                            Message = "Got an Error When getting Design Details.\n Invalid UserID, Please Recheck Your Inputs"
                        };
                    }
                    else
                    {
                        DesignService service = new();
                        return await service.ManageSearchByUserId(UserId,searchString);
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

        //Custom Authentication
        [HttpGet]
        [Route("Get/MainSubDesigns")]
        public async Task<ActionResult<ResponseResult>> GetMaiSubDesigns([FromQuery, Required] int DesignID)
        {
            AuthenticationService tokenService = new();
            if (Request.Headers.TryGetValue("AuthToken", out var token) && Request.Headers.TryGetValue("Email", out var email))
            {
                var ol = await tokenService.ValidateUser(token, email);
                if (ol.Status == ApiRespond.Success.ToString())
                {
                    if (DesignID <= 0)
                    {
                        return new ResponseResult()
                        {
                            Status = ApiRespond.Fail.ToString(),
                            Content = null,
                            Message = "Got an Error When getting Sub Designs Details.\n Design ID should be Valid, Please Recheck Your Inputs"
                        };
                    }
                    else
                    {
                        DesignService service = new();
                        return await service.GetSubDesigns(DesignID);
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

        //Custom Authentication
        [HttpGet]
        [Route("Get/SubDesignsUIs")]
        public async Task<ActionResult<ResponseResult>> GetSubDesignsUIs([FromQuery, Required] int DesignID)
        {
            AuthenticationService tokenService = new();
            if (Request.Headers.TryGetValue("AuthToken", out var token) && Request.Headers.TryGetValue("Email", out var email))
            {
                var ol = await tokenService.ValidateUser(token, email);
                if (ol.Status == ApiRespond.Success.ToString())
                {
                    if (DesignID <= 0)
                    {
                        return new ResponseResult()
                        {
                            Status = ApiRespond.Fail.ToString(),
                            Content = null,
                            Message = "Got an Error When getting Sub Designs Details.\n Design ID should be Valid, Please Recheck Your Inputs"
                        };
                    }
                    else
                    {
                        DesignService service = new();
                        return await service.DesignUISearchById(DesignID);
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
