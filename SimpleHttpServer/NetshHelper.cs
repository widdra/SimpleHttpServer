using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SimpleHttpServer
{
    public static class NetshHelper
    {
        public static void RegisterAddressForCurrentUser(string Address)
        {
            ProcessStartInfo psi = new ProcessStartInfo()
            {
                FileName = "netsh",
                Arguments = $"http add urlacl url={Address} user=\"{Environment.UserDomainName}\\{Environment.UserName}\" listen=yes",
                Verb = "runas",
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = true
            };

            Process.Start(psi).WaitForExit();
        }

        private static readonly Regex userRegex = new Regex(@"^\W*Benutzer: ([a-zA-Z0-9.-_\\ ]+)\W*$", RegexOptions.Multiline);
        public static string[] GetUserRegistrationForAddress(string Address)
        {
            ProcessStartInfo psi = new ProcessStartInfo()
            {
                FileName = "netsh",
                Arguments = $"http show urlacl {Address}",
                Verb = "runas",
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                RedirectStandardOutput = true
            };

            Process p = Process.Start(psi);
            p.WaitForExit();
            string output = p.StandardOutput.ReadToEnd();

            MatchCollection matches = userRegex.Matches(output);

            string[] usernames = new string[matches.Count];
            for (int i = 0; i < matches.Count; i++)
            {
                usernames[i] = matches[i].Groups[1].Value;
            }
            return usernames;
        }

        public static bool AddressIsRegisteredForCurrentUser(string Address)
        {
            return GetUserRegistrationForAddress(Address).Contains($"{Environment.UserDomainName}\\{Environment.UserName}");
        }
    }
}
