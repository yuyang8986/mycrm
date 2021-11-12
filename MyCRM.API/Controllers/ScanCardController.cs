using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyCRM.Shared.Communications.Requests.FileUpload;
using MyCRM.Shared.Communications.Responses.UploadFile;
using Newtonsoft.Json.Linq;

namespace MyCRM.API.Controllers
{
    [Route("api/businesscard")]
    [ApiController]
    public class ScanCardController : ControllerBase
    {
        private int _tryTime = 0;

        public ScanCardController()
        {
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public static String Md5(string s)
        {
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(s);
            bytes = md5.ComputeHash(bytes);
            md5.Clear();
            string ret = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                ret += Convert.ToString(bytes[i], 16).PadLeft(2, '0');
            }
            return ret.PadLeft(32, '0');
        }

        [HttpPost]
        public async Task<IActionResult> Scan([FromForm] ScanCardRequest file)
        {
            CardReaderViewModel cardReaderViewModel = new CardReaderViewModel();
            try
            {
                cardReaderViewModel = StartUpload(file.File);
                return Ok(cardReaderViewModel);
            }
            catch (Exception e)
            {
                return BadRequest();
            }

            //return Ok(cardReaderViewModel);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        private CardReaderViewModel StartUpload(IFormFile file)
        {
            CardReaderViewModel card = new CardReaderViewModel();
            try
            {
                _tryTime++;

                string x_appid = "5db8cba9";

                string api_key = "3e8907e58d6b6a191b2a7389f0375375";

                string ENGINE_TYPE = "business_card";
                string param = "{\"engine_type\":\"" + ENGINE_TYPE + "\"}";
                System.Text.Encoding encode = System.Text.Encoding.ASCII;
                byte[] bytedata = encode.GetBytes(param);
                string x_param = Convert.ToBase64String(bytedata);

                TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                string curTime = Convert.ToInt64(ts.TotalSeconds).ToString();

                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                string result = string.Format("{0}{1}{2}", api_key, curTime, x_param);
                string x_checksum = Md5(result);
                Stream fs = file.OpenReadStream();

                BinaryReader br = new BinaryReader(fs);
                byte[] arr = br.ReadBytes(Convert.ToInt32(fs.Length));
                string cc = Convert.ToBase64String(arr);
                string data = "image=" + cc;

                String Url = "https://webapi.xfyun.cn/v1/service/v1/ocr/business_card";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Headers["X-Appid"] = x_appid;
                request.Headers["X-CurTime"] = curTime;
                request.Headers["X-Param"] = x_param;
                request.Headers["X-Checksum"] = x_checksum;
                request.ContentLength = Encoding.UTF8.GetByteCount(data);
                Stream requestStream = request.GetRequestStream();
                StreamWriter streamWriter = new StreamWriter(requestStream, Encoding.GetEncoding("gb2312"));
                streamWriter.Write(data);
                streamWriter.Close();
                string htmlStr = string.Empty;
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                Stream responseStream = response.GetResponseStream();
                using (StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("UTF-8")))
                {
                    htmlStr = reader.ReadToEnd();
                }
                responseStream.Close();

                JObject jsonObj = (JObject)JsonConvert.DeserializeObject(htmlStr);

                CardReaderResponse responseCard = new CardReaderResponse();

                responseCard = jsonObj.ToObject<CardReaderResponse>();
                Console.WriteLine(responseCard);
                if (responseCard.Data.Name != null)
                {
                    card.FirstName = responseCard.Data.Name[0].Item.GivenName;
                    card.LastName = responseCard.Data.Name[0].Item.FamilyName;
                }
                if (responseCard.Data.Email != null) card.Email = responseCard.Data.Email[0].Item;
                if (responseCard.Data.Telephone != null)
                {
                    card.Phone = responseCard.Data.Telephone[0].Item.Number;
                    if (responseCard.Data.Telephone.Length > 1) card.WorkPhone = responseCard.Data.Telephone[1].Item.Number;
                }

                if (responseCard.Data.Organization != null)
                {
                    if (responseCard.Data.Organization[0].Item.Name != null)
                    {
                        card.Company = responseCard.Data.Organization[0].Item.Name;
                    }
                    else if (responseCard.Data.Organization[0].Item.Unit != null)
                    {
                        card.Company = responseCard.Data.Organization[0].Item.Unit;
                    }
                    if (responseCard.Data.Organization.Length > 1)
                    {
                        if (responseCard.Data.Organization[0].Item.Name != null)
                        {
                            card.Company = responseCard.Data.Organization[0].Item.Name;
                        }
                        else if (responseCard.Data.Organization[0].Item.Unit != null)
                        {
                            card.Company = responseCard.Data.Organization[0].Item.Unit;
                        }
                    }
                }

                if (responseCard.Data.OriginAddress != null) card.Address = responseCard.Data.OriginAddress[0].Item;

                return card;
            }
            catch (Exception e)
            {
                if (_tryTime == 2) throw e;
                StartUpload(file);
            }

            return card;
        }
    }
}