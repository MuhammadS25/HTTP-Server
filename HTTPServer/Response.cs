using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{

    public enum StatusCode
    {
        OK = 200,
        InternalServerError = 500,
        NotFound = 404,
        BadRequest = 400,
        Redirect = 301
    }

    class Response
    {
        string responseString;
        public string ResponseString
        {
            get
            {
                return responseString;
            }
        }
        StatusCode code;
        List<string> headerLines = new List<string>();
        string Headers;
        public Response(StatusCode code, string contentType, string content, string redirectoinPath)
        {
            DateTime now = DateTime.Now;
            headerLines.Add("Date:" + now.ToString("F"));
            headerLines.Add("Content-Type:" + contentType);
            headerLines.Add("Content-Length:" + content.Length);
            if (redirectoinPath!=null)
            {           
                // TODO: Add headlines (Content-Type, Content-Length,Date, [location if there is redirection])
                headerLines.Add("Location:" + redirectoinPath);   
            }
           
            for (int i = 0; i < headerLines.Count; i++)
            {
                Headers += headerLines[i] + "\r\n";
            }
            // TODO: Create the request string
            responseString = GetStatusLine(code) + "\r\n" + Headers + "\r\n" + content;
        }

        private string GetStatusLine(StatusCode code)
        {
            string usermessage = string.Empty;
            switch (code)
            {
                case StatusCode.OK:
                    usermessage = "OK";
                    break;
                case StatusCode.BadRequest:
                    usermessage = "Bad Request";
                    break;
                case StatusCode.InternalServerError:
                    usermessage = "Internal ServerError";
                    break;
                case StatusCode.NotFound:
                    usermessage = "Not Found";
                    break;
                case StatusCode.Redirect:
                    usermessage = "Moved Permenantly";
                    break;
                default:
                    usermessage = "";
                    break;

            }
            // TODO: Create the response status line and return it
            
            string statusLine = Configuration.ServerHTTPVersion + " " +(int) code + " " + usermessage;

            return statusLine;
        }
    }
}
