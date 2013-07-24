namespace Terminal
{
    using System;
    using System.Net;
    using System.Runtime.Serialization.Json;

    public class Terminal
    {
        public Guid Apikey { get; protected set; }

        public Terminal(Guid apikey)
        {
            this.Apikey = apikey;
        }

        public void REPL()
        {
            string command;
            while (true)
            {
                Console.Write(">> ");
                command = Console.ReadLine();
                switch (command.Trim())
                {
                    case "@clear":
                        Console.Clear();
                        break;

                    case "@quit":
                    case "@exit":
                        System.Environment.Exit(0);
                        break;

                    default:
                        Transact(command.Trim()).Print();
                        break;
                }
            }
        }

        protected CLResponse Transact(string command)
        {
            var requestUrl = string.Format("http://localhost:54317/API/Terminal/?key={0}&cmd={1}", this.Apikey.ToString(), command);
            var request = HttpWebRequest.Create(requestUrl);
            var response = request.GetResponse();
            /*var responseString = string.Empty;
            using (StreamReader reader = new StreamReader(response.GetResponseStream())) {
                responseString = reader.ReadToEnd ();
            }*/

            var serializer = new DataContractJsonSerializer(typeof(CLResponse));
            var clr = serializer.ReadObject(response.GetResponseStream());

            return (CLResponse)clr;
        }
    }
}
