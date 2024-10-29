using BilakLk_API.Config;
using BilakLk_API.Models;
using System.Data.SqlClient;
using System.Data;
using TypeSafe_API.Models;

namespace TypeSafe_API.Services
{
    public class DesignService
    {
        #region Manage Insert Design
        internal async Task<ResponseResult> ManageInsertDesign(ModelDesignUpload account)
        {
            ResponseResult r = new()
            {
                Status = ApiRespond.Fail.ToString(),
                Content = null,
                Message = "Something Went Wrong, Please Try Again"
            };

            try
            {
                r = await InsertDesign(account);
                if (r.Status == ApiRespond.Success.ToString() && r.Message == "success")
                {
                    int DesignId = Convert.ToInt32(r.Content);
                    foreach (var image in account.Image)
                    {
                        image.DesignId = DesignId;
                        r = await InsertDesignUI(image);
                    }
                }
                else
                {
                    r.Status = ApiRespond.Fail.ToString();
                    r.Content = null;
                    r.Message = "Main Design Insert Failed! ,Something Went Wrong, Please Try Again";
                }

            }
            catch (Exception ex)
            {
                r.Status = ApiRespond.Fail.ToString();
                r.Content = null;
                r.Message = ex.Message;
            }

            return r;
        } // Manage Exception Insert full process with MS Teams notification send
        #endregion

        #region Insert Design UI
        internal async Task<ResponseResult> InsertDesign(ModelDesignUpload account)
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
                            CommandText = "sp_Design_Insert"
                        };
                        command.Parameters.AddWithValue("@UserId", account.UserId);
                        command.Parameters.AddWithValue("@Ref_Design_Id", account.Ref_Design_Id);
                        command.Parameters.AddWithValue("@Name", account.Name);
                        command.Parameters.AddWithValue("@DesignArea", account.DesignArea);
                        command.Parameters.AddWithValue("@TriggerDesignArea", account.TriggerDesignArea);
                        command.Parameters.AddWithValue("@TriggerPercentage", account.TriggerPresentage);
                        command.Parameters.Add("RETURN_VALUE", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                        int num = await command.ExecuteNonQueryAsync();
                        int count = 0;
                        count = Convert.ToInt32(command.Parameters["RETURN_VALUE"].Value);

                        if (count > 0)
                        {
                            r.Status = ApiRespond.Success.ToString();
                            r.Content = count;
                            r.Message = "success";
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

        #region Manage Insert Design UI
        internal async Task<ResponseResult> InsertDesignUI(ModelDesignUploadImage account)
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
                            CommandText = "sp_DesignUI_Insert"
                        };
                        command.Parameters.AddWithValue("@FileName", account.FileName);
                        command.Parameters.AddWithValue("@DesignId", account.DesignId);
                        command.Parameters.AddWithValue("@ImageData", account.ImageData);
                        command.Parameters.AddWithValue("@TriggerImageData", account.TriggerImageData);
                        command.Parameters.AddWithValue("@ImageArea", account.ImageArea);
                        command.Parameters.AddWithValue("@TriggerImageArea", account.TriggerImageArea);
                        command.Parameters.AddWithValue("@TriggerImagePresentage", account.TriggerImagePresentage);
                        command.Parameters.AddWithValue("@TriggerImagePossibility", account.TriggerImagePossibilty);
                        command.Parameters.AddWithValue("@IsTriggerImagePossibility", account.IsTriggerImagePossibilty);
                        command.Parameters.Add("RETURN_VALUE", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                        int num = await command.ExecuteNonQueryAsync();
                        int count = 0;
                        count = Convert.ToInt32(command.Parameters["RETURN_VALUE"].Value);

                        if (count > 0)
                        {
                            r.Status = ApiRespond.Success.ToString();
                            r.Content = count;
                            r.Message = "success";
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

        #region Manage Design Search By Name

        internal async Task<ResponseResult> ManageSearchByUserId(int UserId, string? SearchString)
        {
            ResponseResult r;
            r = new()
            {
                Status = ApiRespond.Fail.ToString(),
                Content = null,
                Message = "Error in Manage Search Designs Process"
            };
            try
            {
                DesignService service = new();
                List<ModelDesignUpload> list1 = new();
                List<ModelDesignUploadImage> list3 = new();

                r = await service.DesignSearchByUserId(UserId, SearchString);
                if (r.Message == "success")
                {
                    list1 = r.Content as List<ModelDesignUpload>;

                    foreach (var list2 in list1)
                    {
                        r = await service.DesignUISearchById((int)list2.Id);

                        if (r.Message == "success")
                        {
                            if (r.Content is List<ModelDesignUploadImage> contentList1)
                            {
                                list2.Image = contentList1;
                                r.Content = list1;
                            }
                            else
                            {
                                return new ResponseResult()
                                {
                                    Status = ApiRespond.Fail.ToString(),
                                    Content = null,
                                    Message = "Error when Returning Design UIs"
                                };
                            }
                        }
                        else
                        {
                            return r;
                        }
                    }

                }
                else
                {
                    return r;
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
        internal async Task<ResponseResult> DesignSearchByUserId(int UserId, string? SearchString)
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
                List<ModelDesignUpload> list = new();
                ModelDesignUpload v = new();

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
                            CommandText = "sp_Designs_Full_Search"
                        };

                        command.Parameters.AddWithValue("@UserId", UserId);
                        command.Parameters.AddWithValue("@SearchString", SearchString);

                        using (SqlDataReader dr = await command.ExecuteReaderAsync())
                        {
                            if (dr.HasRows)
                            {
                                while (dr.Read())
                                {
                                    v = new ModelDesignUpload()
                                    {
                                        Id = Convert.ToInt32(dr["Id"]),
                                        UserId = Convert.ToInt32(dr["UserId"]),
                                        Ref_Design_Id = Convert.ToInt32(dr["Ref_Design_Id"]),
                                        Name = Convert.ToString(dr["Name"]),
                                        CreatedDate = Convert.ToString(dr["CreatedDate"]),
                                        DesignArea = Convert.ToInt32(dr["DesignArea"]),
                                        TriggerDesignArea = dr["TriggerDesignArea"] != DBNull.Value ? Convert.ToSingle(dr["TriggerDesignArea"]) : (float?)null,
                                        TriggerPresentage = dr["TriggerPresentage"] != DBNull.Value ? Convert.ToSingle(dr["TriggerPresentage"]) : (float?)null
                                    };
                                    list.Add(v);
                                }


                                r.Status = ApiRespond.Success.ToString();
                                r.Content = list;
                                r.Message = "success";
                            }
                            else
                            {
                                r.Status = ApiRespond.Success.ToString();
                                r.Content = null;
                                r.Message = "The Designs Not Found";
                            }
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
        internal async Task<ResponseResult> DesignUISearchById(int id)
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
                List<ModelDesignUploadImage> list = new();
                ModelDesignUploadImage v;

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
                            CommandText = "sp_Designs_UI_By_DesignId"
                        };

                        command.Parameters.AddWithValue("@id", id);
                        using (SqlDataReader dr = await command.ExecuteReaderAsync())
                        {
                            if (dr.HasRows)
                            {
                                while (dr.Read())
                                {
                                    v = new()
                                    {
                                        Id = Convert.ToInt32(dr["Id"]),
                                        DesignId = Convert.ToInt32(dr["DesignId"]),
                                        FileName = Convert.ToString(dr["FileName"]),
                                        ImageData = dr["ImageData"] is DBNull ? null : (byte[])dr["ImageData"],
                                        TriggerImageData = dr["TriggerImageData"] is DBNull ? null : (byte[])dr["TriggerImageData"],
                                        ImageArea = dr["ImageArea"] != DBNull.Value ? Convert.ToSingle(dr["ImageArea"]) : (float?)null,
                                        TriggerImageArea = dr["TriggerImageArea"] != DBNull.Value ? Convert.ToSingle(dr["TriggerImageArea"]) : (float?)null,
                                        TriggerImagePresentage = dr["TriggerImagePresentage"] != DBNull.Value ? Convert.ToSingle(dr["TriggerImagePresentage"]) : (float?)null,
                                        TriggerImagePossibilty = dr["TriggerImagePossibilty"] != DBNull.Value ? Convert.ToSingle(dr["TriggerImagePossibilty"]) : (float?)null,
                                        IsTriggerImagePossibilty = Convert.ToBoolean(dr["IsTriggerImagePossibilty"]),
                                    };
                                    list.Add(v);
                                }


                                r.Status = ApiRespond.Success.ToString();
                                r.Content = list;
                                r.Message = "success";
                            }
                            else
                            {
                                r.Status = ApiRespond.Success.ToString();
                                r.Content = null;
                                r.Message = "The Design UIs Not Found";
                            }
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

        #region Get Main Sub Designs

        internal async Task<ResponseResult> GetSubDesigns(int DesignID)
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
                List<ModelDesignUpload> list = new();
                ModelDesignUpload v = new();

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
                            CommandText = "sp_Get_Sub_Designs"
                        };

                        command.Parameters.AddWithValue("@DesignID", DesignID);

                        using (SqlDataReader dr = await command.ExecuteReaderAsync())
                        {
                            if (dr.HasRows)
                            {
                                while (dr.Read())
                                {
                                    v = new ModelDesignUpload()
                                    {
                                        Id = Convert.ToInt32(dr["Id"]),
                                        UserId = Convert.ToInt32(dr["UserId"]),
                                        Ref_Design_Id = Convert.ToInt32(dr["Ref_Design_Id"]),
                                        Name = Convert.ToString(dr["Name"]),
                                        CreatedDate = Convert.ToString(dr["CreatedDate"]),
                                        DesignArea = Convert.ToInt32(dr["DesignArea"]),
                                        TriggerDesignArea = dr["TriggerDesignArea"] != DBNull.Value ? Convert.ToSingle(dr["TriggerDesignArea"]) : (float?)null,
                                        TriggerPresentage = dr["TriggerPresentage"] != DBNull.Value ? Convert.ToSingle(dr["TriggerPresentage"]) : (float?)null
                                    };
                                    list.Add(v);
                                }


                                r.Status = ApiRespond.Success.ToString();
                                r.Content = list;
                                r.Message = "success";
                            }
                            else
                            {
                                r.Status = ApiRespond.Success.ToString();
                                r.Content = null;
                                r.Message = "The Sub Designs Not Found";
                            }
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
