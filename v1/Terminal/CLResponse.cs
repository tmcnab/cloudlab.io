namespace Terminal
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class CLResponse
    {
        [DataMember]
        public string Text;
        [DataMember]
        public string JSAction;
        [DataMember]
        public string Error;

        public void Print()
        {
            if (!string.IsNullOrWhiteSpace(Text))
                Console.WriteLine(Text);
            if (!string.IsNullOrWhiteSpace(Error))
                Console.Error.WriteLine(Error);
        }
    }
}
