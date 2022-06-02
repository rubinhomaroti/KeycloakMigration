namespace KeycloakMigration
{
    internal class User
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public User(string email, string name)
        {
            Email = email;
            FirstName = LastName = "";
            SetName(name.Trim());
        }

        private void SetName(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                List<string> nameParts = name.Split(" ").ToList();

                if (nameParts.Count == 1)
                {
                    FirstName = nameParts[0];
                }
                else if (nameParts[0].ToUpper().StartsWith("DR"))
                {
                    FirstName = $"{nameParts[0]} {nameParts[1]}";
                    if (nameParts.Count > 2)
                    {
                        for (int i = 2; i < nameParts.Count; i++)
                        {
                            LastName = nameParts[i] + " ";
                        }
                        LastName = LastName.Trim();
                    }
                }
                else
                {
                    FirstName = $"{nameParts[0]}";

                    if (nameParts.Count > 1)
                    {
                        for (int i = 1; i < nameParts.Count; i++)
                        {
                            LastName = nameParts[i] + " ";
                        }
                        LastName = LastName.Trim();
                    }
                }
            }
        }

        public override string ToString()
        {
            return $"E-mail: {Email}\n" +
                $"First Name: {FirstName}\n" +
                $"Last Name: {LastName}";
        }
    }
}
