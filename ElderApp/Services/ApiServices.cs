﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ElderApp.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using SQLite;

namespace ElderApp.Services
{
    public class ApiServices
    {

        enum Result {success=1,decodeError=2,responseError=3};


        public string ApiHost
        { get; set; }

        public ApiServices()
        {
            ApiHost = "https://www.happybi.com.tw";
        }

        //--------------------------------------------------------------




        //會員申請續會
        public async Task<(int,string)> ExtendRequest()
        {
            var client = new RestClient($"{ApiHost}/api/extendMemberShip");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");
            request.AddParameter("token", App.CurrentUser.Token.ToString());
            request.AddParameter("user_id", App.CurrentUser.User_id);
            IRestResponse response = await client.ExecuteTaskAsync(request);
            if (response.Content != null)
            {
                try
                {
                    JObject res = JObject.Parse(response.Content);
                    if (res.ContainsKey("s"))
                    {
                        return ((int)Result.success, res["m"].ToString());
                    }

                }
                catch (Exception ex)
                {
                    return ((int)Result.decodeError, "系統錯誤。");
                    //傳送 app center ex.ToString()
                }
            }

            return ((int)Result.responseError, "伺服器無回應，網路連線錯誤。");

        }






        //使用者登出
        public async Task<int> LogoutRequest()
        {
            var client = new RestClient($"{ApiHost}/api/auth/logout");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");
            request.AddParameter("token", App.CurrentUser.Token);
            IRestResponse response = await client.ExecuteTaskAsync(request);
            if (response.Content != null)
            {
                try
                {
                    JObject res = JObject.Parse(response.Content);

                    if (res["message"].ToString() == "Successfully logged out")
                    {
                        return (int)Result.success;
                    }
                }
                catch (Exception ex)
                {
                    return (int)Result.decodeError;
                    //傳送 app center ex.ToString()
                }
            }

            return (int)Result.responseError;

        }





        //使用者登入
        public async Task<(int,JObject)> LoginRequest(string Email,string Password)
        {
            var client = new RestClient($"{ApiHost}/api/auth/login");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");
            request.AddParameter("email", Email);
            request.AddParameter("password", Password);
            IRestResponse response = await client.ExecuteTaskAsync(request);
            if(response.Content != null)
            {
                try
                {
                    JObject res = JObject.Parse(response.Content);
                    return ((int)Result.success,res);

                }
                catch (Exception ex)
                {
                    return ((int)Result.decodeError,null);
                    //傳送 app center ex.ToString()
                }
            }

            return ((int)Result.responseError,null);
        }




        //註冊
        public async Task<(int,string)> SignUpRequest(string Email,string Password,string Name,string Phone,string Tel,int GenderVal,string Birthdate,string Id_number,int DistrictId,string Address,int Pay_mathodVal,string Inviter_id_code)
        {
            var client = new RestClient($"{ApiHost}/api/member/join");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");
            request.AddParameter("email", Email);
            request.AddParameter("password", Password);
            request.AddParameter("name", Name);
            request.AddParameter("phone", Phone);
            request.AddParameter("tel", Tel);
            request.AddParameter("gender", GenderVal);
            request.AddParameter("birthdate", Birthdate);
            request.AddParameter("id_number", Id_number);
            request.AddParameter("district_id", DistrictId);
            request.AddParameter("address", Address);
            request.AddParameter("pay_method", Pay_mathodVal);
            request.AddParameter("inviter_id_code", Inviter_id_code);
            request.AddParameter("app", true);
            IRestResponse response = await client.ExecuteTaskAsync(request);
            if (response.Content != null)
            {
                try
                {
                    JObject res = JObject.Parse(response.Content);
                    if (res.ContainsKey("s"))
                    {
                        if (res["s"].ToString() == "1")
                        {
                            return ((int)Result.success, "您已成功註冊。");
                        }
                    }
                    else
                    {
                        return ((int)Result.responseError, res.ToString());
                    }
                }
                catch (Exception ex)
                {
                    return ((int)Result.decodeError, "系統錯誤。");
                }
            }

            return ((int)Result.responseError, "伺服器無回應，網路連線錯誤。");

        }


        //檢查 推薦人
        public async Task<(int, string)> CheckInviterRequest(string inviter_id_code)
        {
            var client = new RestClient($"{ApiHost}/api/inviterCheck?inviter_id_code={inviter_id_code}");
            var request = new RestRequest(Method.GET);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");
            IRestResponse response = await client.ExecuteTaskAsync(request);
            if (response.Content != null)
            {
                try
                {
                    JObject res = JObject.Parse(response.Content);
                    if (res["s"].ToString() == "1")
                    {
                        return ((int)Result.success, res["inviter"].ToString());
                    }
                    else
                    {
                        return ((int)Result.responseError, "此會員編號用戶並不存在，請檢查是否輸入錯誤或向推薦人確認。");
                    }
                    
                }
                catch (Exception ex)
                {
                    return ((int)Result.decodeError, "系統錯誤。");
                    //傳送 app center ex.ToString()
                }
            }

            return ((int)Result.responseError, "伺服器無回應，網路連線錯誤。");
        }



        //get 地區
        public async Task<(int, List<District>)> GetDistrict()
        {
            var client = new RestClient($"{ApiHost}/api/district");
            var request = new RestRequest(Method.GET);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");
            IRestResponse response = await client.ExecuteTaskAsync(request);
            if (response.Content != null)
            {
                try
                {
                    List<District> districtList = JsonConvert.DeserializeObject<List<District>>(response.Content);

                    return ((int)Result.success, districtList);

                }
                catch (Exception ex)
                {
                    return ((int)Result.decodeError, null);
                    //傳送 app center ex.ToString()
                }
            }

            return ((int)Result.responseError, null);
        }


        //get Category
        public async Task<(int, List<Category>)> GetCategory()
        {
            var client = new RestClient($"{ApiHost}/api/category");
            var request = new RestRequest(Method.GET);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");
            IRestResponse response = await client.ExecuteTaskAsync(request);
            if (response.Content != null)
            {
                try
                {
                    List<Category> _categories = JsonConvert.DeserializeObject<List<Category>>(response.Content);

                    return ((int)Result.success, _categories);

                }
                catch (Exception ex)
                {
                    return ((int)Result.decodeError, null);
                    //傳送 app center ex.ToString()
                }
            }

            return ((int)Result.responseError, null);
        }


        //get Event
        public async Task<(int, List<Event>)> GetEvents(string cat_id,string district_id)
        {
            
            var client = new RestClient($"{ApiHost}/api/getEvents?category={cat_id}&district={district_id}");
            var request = new RestRequest(Method.GET);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");
            IRestResponse response = await client.ExecuteTaskAsync(request);
            if (response.Content != null)
            {
                try
                {
                    List<Event> _events = JsonConvert.DeserializeObject<List<Event>>(response.Content);

                    return ((int)Result.success, _events);

                }
                catch (Exception ex)
                {
                    return ((int)Result.decodeError, null);
                    //傳送 app center ex.ToString()
                }
            }

            return ((int)Result.responseError, null);
        }




        //get 使用者參加的 Events
        public async Task<(int, List<Event>)> GetUserEvent()
        {

            var client = new RestClient($"{ApiHost}/api/myevent");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");
            request.AddParameter("id", App.CurrentUser.User_id);
            request.AddParameter("token", App.CurrentUser.Token);
            IRestResponse response = await client.ExecuteTaskAsync(request);
            if (response.Content != null)
            {
                try
                {
                    List<Event> _events = JsonConvert.DeserializeObject<List<Event>>(response.Content);

                    return ((int)Result.success, _events);

                }
                catch (Exception ex)
                {
                    return ((int)Result.decodeError, null);
                    //傳送 app center ex.ToString()
                }
            }

            return ((int)Result.responseError, null);
        }




        //上傳圖片
        public async Task<(int, string)> UploadImageRequest(string image)
        {
            var client = new RestClient($"{ApiHost}/api/auth/uploadImage");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");
            request.AddParameter("token", App.CurrentUser.Token);
            request.AddParameter("id", App.CurrentUser.User_id.ToString());
            request.AddParameter("name", App.CurrentUser.Name);
            request.AddParameter("image", image);
            IRestResponse response = await client.ExecuteTaskAsync(request);
            if (response.Content != null)
            {
                try
                {
                    JObject res = JObject.Parse(response.Content);
                    if (res.ContainsKey("image_name"))
                    {
                        return ((int)Result.success, res["image_name"].ToString());
                    }
                    else
                    {
                        return ((int)Result.responseError, null);
                    }

                }
                catch (Exception ex)
                {
                    return ((int)Result.decodeError, null);
                    //傳送 app center ex.ToString()
                }
            }

            return ((int)Result.responseError, null);
        }


        //get 使用者 資料
        public async Task<(int, JObject)> MyAccountRequest()
        {
            var client = new RestClient($"{ApiHost}/api/auth/myAccount");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");
            request.AddParameter("token", App.CurrentUser.Token);
            IRestResponse response = await client.ExecuteTaskAsync(request);
            if (response.Content != null)
            {
                try
                {
                    JObject res = JObject.Parse(response.Content);

                    return ((int)Result.success, res);

                }
                catch (Exception ex)
                {
                    return ((int)Result.decodeError, null);
                    //傳送 app center ex.ToString()
                }
            }

            return ((int)Result.responseError, null);
        }


        //更新使用者資料
        public async Task<(int, string)> UpdateAccountRequest(string Name,string Phone,string Tel,string Address,string Id_number)
        {
            var client = new RestClient($"{ApiHost}/api/auth/updateAccount");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");
            request.AddParameter("token", App.CurrentUser.Token);
            request.AddParameter("name", Name);
            request.AddParameter("phone", Phone);
            request.AddParameter("tel", Tel);
            request.AddParameter("address", Address);
            request.AddParameter("id_number", Id_number);
            IRestResponse response = await client.ExecuteTaskAsync(request);
            if (response.Content != null)
            {
                try
                {
                    JObject res = JObject.Parse(response.Content);

                    if (res["s"].ToString() == "1")
                    {
                        return ((int)Result.success, res["m"].ToString());
                    }
                    else
                    {
                        return ((int)Result.responseError, res["m"].ToString());
                    }

                }
                catch (Exception ex)
                {
                    return ((int)Result.decodeError, "系統錯誤。");
                    //傳送 app center ex.ToString()
                }
            }

            return ((int)Result.responseError, "伺服器無回應，網路連線錯誤。");
        }


        //參加活動
        public async Task<(int, string)> JoinEventRequest(string event_slug)
        {
            var client = new RestClient($"{ApiHost}/api/joinevent/{event_slug}");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");
            request.AddParameter("id", App.CurrentUser.User_id);
            request.AddParameter("token", App.CurrentUser.Token);
            IRestResponse response = await client.ExecuteTaskAsync(request);
            if (response.Content != null)
            {
                try
                {
                    JObject res = JObject.Parse(response.Content);

                    if (res["s"].ToString() == "1")
                    {
                        return ((int)Result.success, res["m"].ToString());
                    }
                    else
                    {
                        return ((int)Result.responseError, res["m"].ToString());
                    }

                }
                catch (Exception ex)
                {
                    return ((int)Result.decodeError, "系統錯誤。");
                    //傳送 app center ex.ToString()
                }
            }

            return ((int)Result.responseError, "伺服器無回應，網路連線錯誤。");
        }




        //取消參加活動
        public async Task<(int, string)> CancelEventRequest(string event_slug)
        {
            var client = new RestClient($"{ApiHost}/api/cancelevent/{event_slug}");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Accept", "application/json");
            request.AddParameter("id", App.CurrentUser.User_id);
            request.AddParameter("token", App.CurrentUser.Token);
            IRestResponse response = await client.ExecuteTaskAsync(request);
            if (response.Content != null)
            {
                try
                {
                    JObject res = JObject.Parse(response.Content);

                    if (res["s"].ToString() == "1")
                    {
                        return ((int)Result.success, res["m"].ToString());
                    }
                    else
                    {
                        return ((int)Result.responseError, res["m"].ToString());
                    }

                }
                catch (Exception ex)
                {
                    return ((int)Result.decodeError, "系統錯誤。");
                    //傳送 app center ex.ToString()
                }
            }

            return ((int)Result.responseError, "伺服器無回應，網路連線錯誤。");
        }


        









    }
}
