using System;
using System.Collections.Generic;
using System.Linq;

namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
    }

    class Request
    {
        string[] requestLines;
        RequestMethod method;
        public string relativeURI;
        Dictionary<string, string> headerLines;

        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        HTTPVersion httpVersion;
        string requestString;
        string[] contentLines;

        public Request(string requestString)
        {
            headerLines = new Dictionary<string, string>();
            this.requestString = requestString;
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        public bool ParseRequest()
        {
            //throw new NotImplementedException();

            //TODO: parse the receivedRequest using the \r\n delimeter
            //bakr
            string[] delims = new String[] { "\r\n" };
            requestLines = requestString.Split(delims, StringSplitOptions.None);
            string req = requestLines[0];
            string[] separatedReq = req.Split(' ');
            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)
            //Console.WriteLine(requestLines.Length);
            if (requestLines.Length < 3)
                return false;
            // Parse Request line
            bool validReqline = ParseRequestLine();
            bool validUri = ValidateIsURI(separatedReq[1]);
            // Validate blank line exists
            bool IsblankLine = ValidateBlankLine();
            if ((!validReqline) || (!validUri) || (!IsblankLine))
            {
                return false;
            }

            this.relativeURI = separatedReq[1];
            // Load header lines into HeaderLines dictionary
            bool headerLoaded = LoadHeaderLines();
            if (!headerLoaded)
            {
                return false;
            }
            return true;
        }

        private bool ParseRequestLine()
        {
            string[] reqParts = requestLines[0].Split(' ');
            if (reqParts[0].Equals("GET"))
            {
                if (reqParts.Length == 2)
                {
                    this.httpVersion = HTTPVersion.HTTP09;
                }
                else if (reqParts[2].Equals("HTTP/0.9"))
                {
                    this.httpVersion = HTTPVersion.HTTP09;

                }
                else if (reqParts[2].Equals("HTTP/1.0"))
                {
                    this.httpVersion = HTTPVersion.HTTP10;

                }
                else if (reqParts[2].Equals("HTTP/1.1"))
                {
                    this.httpVersion = HTTPVersion.HTTP11;
                }
                this.method = RequestMethod.GET;
            }
            else if (reqParts[0].Equals("POST"))
            {
                contentLines[0] = requestLines[requestLines.Length - 1];
                if (reqParts.Length == 2)
                {
                    this.httpVersion = HTTPVersion.HTTP09;
                }
                else if (reqParts[2].Equals("HTTP/0.9"))
                {
                    this.httpVersion = HTTPVersion.HTTP09;
                }
                else if (reqParts[2].Equals("HTTP/1.0"))
                {
                    this.httpVersion = HTTPVersion.HTTP10;
                }
                else if (reqParts[2].Equals("HTTP/1.1"))
                {
                    this.httpVersion = HTTPVersion.HTTP11;
                }

                this.method = RequestMethod.POST;
            }
            else if (reqParts[0].Equals("HEAD"))
            {
                if (reqParts.Length == 2)
                {
                    this.httpVersion = HTTPVersion.HTTP09;
                }
                else if (reqParts[2].Equals("HTTP/0.9"))
                {
                    this.httpVersion = HTTPVersion.HTTP09;
                }
                else if (reqParts[2].Equals("HTTP/1.0"))
                {
                    this.httpVersion = HTTPVersion.HTTP10;
                }
                else if (reqParts[2].Equals("HTTP/1.1"))
                {
                    this.httpVersion = HTTPVersion.HTTP11;
                }
                this.method = RequestMethod.HEAD;
            }
            else
            {
                return false;
            }

            return true;

        }

        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

        private bool LoadHeaderLines()
        {
            //throw new NotImplementedException();
            for (int i = 1; i < requestLines.Length - 2; i++)
            {
                string[] header = requestLines[i].Split(':');
                headerLines.Add(header[0], header[1]);
            }
            if (this.httpVersion == HTTPVersion.HTTP10)
            {
                if (headerLines.Count != 0)
                    return false;
            }
            else if (this.httpVersion == HTTPVersion.HTTP11)
            {
                if (!headerLines.ContainsKey("Host"))
                    return false;
            }

            return true;
        }

        private bool ValidateBlankLine()
        {
            return requestLines.Contains(string.Empty);
        }

    }
}
