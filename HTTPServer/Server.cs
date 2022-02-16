using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using HTTPServer;

namespace HTTPServer
{
    class Server
    {
        Socket serverSocket;

        public Server(int portNumber, string redirectionMatrixPath)
        {
            //TODO: call this.LoadRedirectionRules passing redirectionMatrixPath to it
            this.LoadRedirectionRules(redirectionMatrixPath);
            //TODO: initialize this.serverSocket
            this.serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint hostEndPoint = new IPEndPoint(IPAddress.Any, portNumber);
            this.serverSocket.Bind(hostEndPoint);
        }

        public void StartServer()
        {
            // TODO: Listen to connections, with large backlog.
            serverSocket.Listen(100);
            // TODO: Accept connections in while loop and start a thread for each connection on function "Handle Connection"
            while (true)
            {
                //TODO: accept connections and start thread for each accepted connection.
                Socket clientSocket = this.serverSocket.Accept();
                Console.WriteLine("New Client Accepted: {0}", clientSocket.RemoteEndPoint);
                Thread newThread = new Thread(new ParameterizedThreadStart(HandleConnection));
                newThread.Start(clientSocket);

            }
        }

        public void HandleConnection(object obj)
        {
            // TODO: Create client socket 
            Socket clientSocket = (Socket)obj;
            // set client socket ReceiveTimeout = 0 to indicate an infinite time-out period
            clientSocket.ReceiveTimeout = 0;
            // TODO: receive requests in while true until remote client closes the socket.
            while (true)
            {
                try
                {
                    byte[] Data = new byte[1024];
                    // TODO: Receive request
                    //clientSocket.Receive(Data);
                    int recievedLen = clientSocket.Receive(Data);
                    string message = Encoding.ASCII.GetString(Data);
                    // TODO: break the while loop if receivedLen==0
                    if (recievedLen == 0)
                    {
                        Console.WriteLine("Client: {0} ended the connection", clientSocket.RemoteEndPoint);
                        break;
                    }
                    // TODO: Create a Request object using received request string
                    Request request = new Request(message);
                    // TODO: Call HandleRequest Method that returns the response
                    Response res = HandleRequest(request);
                    // TODO: Send Response back to client
                    clientSocket.Send(Encoding.ASCII.GetBytes(res.ResponseString));

                }
                catch (Exception ex)
                {
                    // TODO: log exception using Logger class
                    Logger.LogException(ex);
                }
            }

            // TODO: close client socket
            clientSocket.Close();
        }

        Response HandleRequest(Request request)
        {
            //throw new NotImplementedException();
            string content;
            try
            {
                //TODO: check for bad request 
                if(!request.ParseRequest())
                {
                    content = LoadDefaultPage(Configuration.BadRequestDefaultPageName);
                    Response res = new Response(StatusCode.BadRequest , "text/html", content , null);
                    return res;
                }
                //TODO: map the relativeURI in request to get the physical path of the resource.
                int found = request.relativeURI.IndexOf("/");
                string relat = request.relativeURI.Substring(found + 1);
                string physPath;
                //TODO: check for redirect
                if (Configuration.RedirectionRules.ContainsKey(relat))
                {
                    physPath = Configuration.RootPath + "/" + Configuration.RedirectionRules[relat];
                    if (!File.Exists(physPath))
                    {
                        content = LoadDefaultPage(Configuration.NotFoundDefaultPageName);
                        Response res = new Response(StatusCode.NotFound, "text/html", content, null);
                        return res;
                    }
                    else
                    {
                        content = LoadDefaultPage(GetRedirectionPagePathIFExist(relat));
                        Response res = new Response(StatusCode.Redirect, "text/html", content ,Configuration.RedirectionRules[relat]);
                        return res;
                    }
                }
                //TODO: check file exists
                physPath = Configuration.RootPath + request.relativeURI;
                if (!File.Exists(physPath))
                {
                    content = LoadDefaultPage(Configuration.NotFoundDefaultPageName);
                    Response res = new Response(StatusCode.NotFound, "text/html", content, null);
                      return res;
                }
                //TODO: read the physical file
                else
                {
                    // Create OK response
                    content = LoadDefaultPage(relat);
                    Response res = new Response(StatusCode.OK, "text/html", content, null);
                    return res;
                }
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                // TODO: in case of exception, return Internal Server Error.
                content = LoadDefaultPage(Configuration.InternalErrorDefaultPageName);
                Response res = new Response(StatusCode.InternalServerError, "text/html", content, null);
                return res;
            }
        }

        private string GetRedirectionPagePathIFExist(string relativePath)
        {
            // using Configuration.RedirectionRules return the redirected page path if exists else returns empty
            foreach (KeyValuePair<string, string> kvp in Configuration.RedirectionRules) {
                if (kvp.Key.Equals(relativePath)){
                    return kvp.Value;
                }
            }
            return string.Empty;
        }

        private string LoadDefaultPage(string defaultPageName)
        {
            string filePath = Path.Combine(Configuration.RootPath, defaultPageName);
            // TODO: check if filepath not exist log exception using Logger class and return empty string
            if (!File.Exists(filePath))
            {
                FileNotFoundException ex = new FileNotFoundException();
                Logger.LogException(ex);
                return string.Empty;
            }

            // else read file and return its content
            else {
                StreamReader streamReader = new StreamReader(filePath);
                string content = streamReader.ReadToEnd();
                streamReader.Close();
                return content;
            }
        }

        private void LoadRedirectionRules(string filePath)
        {
            try
            {
                // TODO: using the filepath paramter read the redirection rules from file 
                string[] lines = File.ReadAllLines(filePath);
                // then fill Configuration.RedirectionRules dictionary
                Configuration.RedirectionRules = new Dictionary<string, string>();

                foreach (string line in lines)
                {
                    string[] separatedRules = line.Split(',');
                    Configuration.RedirectionRules.Add(separatedRules[0], separatedRules[1]);
                }

                /*
                foreach (KeyValuePair<string, string> kvp in Configuration.RedirectionRules)
                {
                Console.WriteLine("Key = {0} , Val ={1}",kvp.Key , kvp.Value);
                }
                //*/                 
            }
            catch (Exception ex)
            {
                // TODO: log exception using Logger class
                Logger.LogException(ex);
                Environment.Exit(1);
            }
        }
    }
}
