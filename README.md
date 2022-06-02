# Keycloak Migration

.NET Core Console App for user migration from a legacy MySQL database to an existing Keycloak server.

## What is migrating?
- User e-mail as username
- User e-mail itself
- User first name
- User last name

## Setup
1. Restore NuGet packages
2. Setup in your Keycloak server a confidential client, and enable the Service Accounts option to allow Client Credentials Grant flow
3. Replace with your environment variables the _appSettings.json_ properties
4. Build the solution and you're ready to go!

## Next steps
- Create the service to send an e-mail for user verification and reset password
- Parameterize the required actions at the user creation, today is sending _UPDATE_PASSWORD_ and _VERIFY_EMAIL_ as default
