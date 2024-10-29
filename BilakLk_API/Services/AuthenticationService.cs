using BilakLk_API.Config;
using BilakLk_API.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace BilakLk_API.Services
{
    public class AuthenticationService
    {
        #region Validate Auth
        internal async Task<ResponseResult> ValidateUser(string token, string email)
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
                ModelUser v = new();
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
                            CommandText = "sp_User_Validate"
                        };
                        command.Parameters.AddWithValue("@token", token);
                        command.Parameters.AddWithValue("@email", email);
                        command.Parameters.Add("RETURN_VALUE", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                        int num = await command.ExecuteNonQueryAsync();

                        int count = 0;
                        count = Convert.ToInt32(command.Parameters["RETURN_VALUE"].Value);
                        if (count == 1)
                        {
                            r.Status = ApiRespond.Fail.ToString();
                            r.Content = null;
                            r.Message = "User Authentication Operation Failed";
                        }
                        else if (count == 2)
                        {
                            r.Status = ApiRespond.Success.ToString();
                            r.Content = null;
                            r.Message = "success";
                        }
                        else if (count == 3)
                        {
                            r.Status = ApiRespond.Fail.ToString();
                            r.Content = null;
                            r.Message = "No Account for this Email, Authentication Fail";
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

        #region Login
        internal async Task<ResponseResult> UserLogin(string email, string password)
        {
            SqlCommand command;
            ResponseResult r;
            r = new()
            {
                Status = ApiRespond.Fail.ToString(),
                Content = null,
                Message = "Didn't Connect to the SQL Connection"
            };

            string token = new Helper().GetToken(email);
            try
            {
                ModelUser v = new();
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
                            CommandText = "sp_User_Login"
                        };

                        command.Parameters.AddWithValue("@email", email);
                        command.Parameters.AddWithValue("@password", new Helper().EncryptPass(password));
                        command.Parameters.AddWithValue("@token", token);
                        command.Parameters.Add("RETURN_VALUE", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                        int num = await command.ExecuteNonQueryAsync();
                        int count = 0;
                        count = Convert.ToInt32(command.Parameters["RETURN_VALUE"].Value);
                        if (count == 1)
                        {
                            r.Status = ApiRespond.Fail.ToString();
                            r.Content = null;
                            r.Message = "User Authentication Operation Failed";
                        }
                        else if (count == 2)
                        {
                            command.Dispose();
                            using (SqlDataReader dr = await command.ExecuteReaderAsync())
                            {
                                if (dr.HasRows)
                                {
                                    while (dr.Read())
                                    {
                                        v = new()
                                        {
                                            Id = Convert.ToInt32(dr["Id"]),
                                            FirstName = Convert.ToString(dr["FirstName"]),
                                            LastName = Convert.ToString(dr["LastName"]),
                                            ContactNumber = Convert.ToString(dr["ContactNumber"]),
                                            Email = Convert.ToString(dr["Email"]),
                                            AuthToken = Convert.ToString(dr["AuthToken"]),
                                            UserStatus = Convert.ToString(dr["Status"])
                                        };
                                    }

                                    r.Status = ApiRespond.Success.ToString();
                                    r.Content = v;
                                    r.Message = "success";
                                }
                                else
                                {
                                    r.Status = ApiRespond.Success.ToString();
                                    r.Content = null;
                                    r.Message = "User Account Details Not Found. Login Fail!";
                                }
                            }

                        }
                        else if (count == 3)
                        {
                            r.Status = ApiRespond.Fail.ToString();
                            r.Content = null;
                            r.Message = "No Account for this Email or Account Deactivated, Authentication Fail";
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

        #region Reset Password
        internal async Task<ResponseResult> ResetPassword(string email, string currentPassword, string newPassword)
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
                            CommandText = "sp_User_Reset"
                        };

                        command.Parameters.AddWithValue("@email", email);
                        command.Parameters.AddWithValue("@currentPassword", new Helper().EncryptPass(currentPassword));
                        command.Parameters.AddWithValue("@newPassword", new Helper().EncryptPass(newPassword));
                        command.Parameters.Add("RETURN_VALUE", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                        int num = await command.ExecuteNonQueryAsync();
                        int count = 0;
                        count = Convert.ToInt32(command.Parameters["RETURN_VALUE"].Value);
                        if (count == 1)
                        {
                            r.Status = ApiRespond.Fail.ToString();
                            r.Content = null;
                            r.Message = "User Not Found, Password Reset Operation Failed";
                        }
                        else if (count == 2)
                        {
                            r.Status = ApiRespond.Success.ToString();
                            r.Content = count;
                            r.Message = "Success";

                        }
                        else if (count == 3)
                        {
                            r.Status = ApiRespond.Fail.ToString();
                            r.Content = null;
                            r.Message = "No Account for this Email, Password Reset Fail";
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

        internal async Task<ResponseResult> BadRequestRespond()
        {
            ResponseResult r;
            r = new()
            {
                Status = ApiRespond.Fail.ToString(),
                Content = null,
                Message = "Bad request: Required headers are missing or invalid."
            };
            return r;
        }

    }
}
