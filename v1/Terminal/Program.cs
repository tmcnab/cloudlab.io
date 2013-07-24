namespace Terminal
{
    using System;

    class Program
    {
        static void Main(string[] args)
        {
            (new Terminal(Guid.Parse("37fd9d73-804e-461a-bbbc-a196ab49021d"))).REPL();
        }
    }
}