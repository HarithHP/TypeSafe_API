using BilakLk_API.Config;
using BilakLk_API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Security.Principal;
using System.Text.Json;

namespace BilakLk_API.Services
{
    public class UserService
    {
        #region user Insert
        internal async Task<ResponseResult> UserInsert(ModelUser account)
        {
            SqlCommand command;
            ResponseResult r;
            r = new()
            {
                Status = ApiRespond.Fail.ToString(),
                Content = null,
                Message = "Didn't Connect to the SQL Connection"
            };
            try
            {   
                using (SqlConnection con = new(ApiManager.Instance.GetConnectionString().ConnectionString))
                {
                    try
                    {
                        con.Open();
                        command = new()
                        {
                            Connection = con,
                            CommandType = CommandType.StoredProcedure,
                            CommandTimeout = 0,
                            CommandText = "sp_User_Insert"
                        };
                        command.Parameters.AddWithValue("@firstName", account.FirstName);
                        command.Parameters.AddWithValue("@lastName", account.LastName);
                        command.Parameters.AddWithValue("@contactNumber", account.ContactNumber);
                        command.Parameters.AddWithValue("@email", account.Email);
                        command.Parameters.AddWithValue("password", new Helper().EncryptPass(account.Password));
                        command.Parameters.AddWithValue("@userStatus", account.UserStatus);
                        command.Parameters.Add("RETURN_VALUE", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                        int num = await command.ExecuteNonQueryAsync();
                        int count = 0;
                        count = Convert.ToInt32(command.Parameters["RETURN_VALUE"].Value);

                        if (count == 1)
                        {
                            r.Status = ApiRespond.Success.ToString();
                            r.Content = null;
                            r.Message = "There is a user account for this email";
                        }
                        else if (count == 2)
                        {
                            if (num > 0)
                            {
                                r.Status = ApiRespond.Success.ToString();
                                r.Content = count;
                                r.Message = "success";
                            }
                            else
                            {
                                r.Status = ApiRespond.Fail.ToString();
                                r.Content = null;
                                r.Message = "Insert Operation Failed";
                            }
                        }
                        else
                        {
                            r.Status = ApiRespond.Fail.ToString();
                            r.Content = null;
                            r.Message = "Something Went Wrong, Got an Unknown Error";
                        }
                        command.Dispose();
                    }
                    catch (Exception ex)
                    {
                        r.Status = ApiRespond.Fail.ToString();
                        r.Content = null;
                        r.Message = ex.Message;
                    }
                    finally
                    { con.Close(); }
                }
                
            }
            catch (Exception ex)
            {

                r.Status = ApiRespond.Fail.ToString();
                r.Content = null;
                r.Message = ex.Message;
            }
            return r;
        }
        #endregion

        #region user Update
        internal async Task<ResponseResult> UserUpdate(ModelUser modelUser)
        {
            SqlCommand command;
            ResponseResult r;
            r = new()
            {
                Status = ApiRespond.Fail.ToString(),
                Content = null,
                Message = "Didn't Connect to the SQL Connection"
            };
            try
            {
                using (SqlConnection con = new(ApiManager.Instance.GetConnectionString().ConnectionString))
                {
                    try
                    {
                        con.Open();
                        command = new()
                        {
                            Connection = con,
                            CommandType = CommandType.StoredProcedure,
                            CommandTimeout = 0,
                            CommandText = "sp_User_Update"
                        };

                        command.Parameters.AddWithValue("@Id", modelUser.Id);
                        command.Parameters.AddWithValue("@FirstName", modelUser.FirstName);
                        command.Parameters.AddWithValue("@LastName", modelUser.LastName);
                        command.Parameters.Add("RETURN_VALUE", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                        int num = await command.ExecuteNonQueryAsync();
                        int count = 0;
                        count = Convert.ToInt32(command.Parameters["RETURN_VALUE"].Value);
                        if (count == 1)
                        {
                            r.Status = ApiRespond.Fail.ToString();
                            r.Content = null;
                            r.Message = "User ID is Unavailable, User Update Operation Failed";
                        }
                        else if (count == 2)
                        {
                            r.Status = ApiRespond.Success.ToString();
                            r.Content = count;
                            r.Message = "Success";

                        }
                        else
                        {
                            r.Status = ApiRespond.Fail.ToString();
                            r.Content = null;
                            r.Message = "Got an Unhandle Error, Please Retry";
                        }
                        command.Dispose();
                    }
                    catch (Exception ex)
                    {
                        r.Status = ApiRespond.Fail.ToString();
                        r.Content = null;
                        r.Message = ex.Message;

                    }
                    finally
                    { con.Close(); }
                }

            }
            catch (Exception ex)
            {

                r.Status = ApiRespond.Fail.ToString();
                r.Content = null;
                r.Message = ex.Message;
            }
            return r;
        }
        #endregion
    }
}
