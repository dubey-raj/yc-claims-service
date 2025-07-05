namespace ClaimService.Configurations
{
    public class DbConnectionInfo
    {
        public string Host { get; set; }
        public int? Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Database { get; set; }

        public string ToConnectionString()
        {
            var conn = $"Host={Host};Database={Database};Username={Username};Password={Password}";
            
            if (Port.HasValue)
            {
                conn += $";Port={Port.Value}";
            }

            return conn;
        }
    }
}
