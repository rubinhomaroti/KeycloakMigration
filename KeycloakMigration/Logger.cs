namespace KeycloakMigration
{
    internal static class Logger
    {
        private static List<string> _logs = new List<string>();
        public static void StartLog()
        {
            string log = $"Execution started at {DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()}";
            Console.WriteLine(log);
            _logs.Add(log);
        }

        public static void Log(string message)
        {
            string log = $"# {DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()}: {message}";
            Console.WriteLine(log);
            _logs.Add(log);
        }

        public static void FinishLog()
        {
            string log = $"Execution finisehd {DateTime.Now.ToShortDateString()} {DateTime.Now.ToLongTimeString()}";
            Console.WriteLine(log);
            _logs.Add(log);

            SaveLogs();
        }

        private static void SaveLogs()
        {
            string logFile = Path.Combine(Environment.CurrentDirectory, $"{DateTime.Now.ToString("ddMMyyyyHHmmss")}.log");
            File.WriteAllLines(logFile, _logs.ToArray());
        }
    }
}
