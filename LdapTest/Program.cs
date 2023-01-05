using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace LdapTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //using (DirectoryEntry searchRoot = new DirectoryEntry("LDAP://kkdc01.kyklos.local/CN=JEK Users,CN=Users,DC=kyklos,DC=local"))
            //using (DirectorySearcher searcher = new DirectorySearcher(searchRoot))
            //{
            //    searcher.PropertiesToLoad.Add("userprincipalname");

            //    searcher.SearchScope = SearchScope.Subtree;
            //    searcher.Filter = "(objectCategory=user)";
            //    var res = searcher.FindAll();
            //}

            //string username = "testuser";
            //string password = "Password01";
            //string domain = "kkdc01.kyklos.local";
            //string group = "JEK Users";

            Console.WriteLine($"Domain: {args[0]}");
            Console.WriteLine($"User: {args[1]}");
            Console.WriteLine($"Password: {args[2]}");
            Console.WriteLine($"Group: {args[3]}");

            bool isAuth = AuthenticateUserLDAP(args[0], args[1], args[2], args[3]);
            Console.WriteLine(isAuth ? "Authenticated" : "Not authenticated");
        }

        public static bool AuthenticateUserLDAP(string domain, string username, string password, string group)
        {
            using (var context = new PrincipalContext(ContextType.Domain, domain))
            {
                using (UserPrincipal userPrincipal = UserPrincipal.FindByIdentity(context, username))
                {
                    if (userPrincipal == null)
                    {
                        Console.WriteLine("User principal is null");
                        return false;
                    }

                    bool isUserValid = context.ValidateCredentials(username, password);
                    if (!isUserValid)
                    {
                        Console.WriteLine("ValidateCredentials returned false");
                        return false;
                    }

                    //DirectoryEntry deUser = (DirectoryEntry)userPrincipal.GetUnderlyingObject();
                    //ActiveDs.IADsUser nativeDeUser = (ActiveDs.IADsUser)deUser.NativeObject;
                    //var expDate = nativeDeUser.PasswordExpirationDate;
                    //var expDate2 = GetPasswordExpirationDate(username);

                    if (string.IsNullOrWhiteSpace(group))
                    {
                        Console.WriteLine("Group is null");
                        return true;
                    }

                    using (GroupPrincipal groupPrincipal = GroupPrincipal.FindByIdentity(context, group))
                    {
                        if (group != null && userPrincipal.IsMemberOf(groupPrincipal))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        public static DateTime GetPasswordExpirationDate(string userId)
        {
            string forestGc = String.Format("GC://{0}", Forest.GetCurrentForest().Name);
            var searcher = new DirectorySearcher();
            searcher = new DirectorySearcher(new System.DirectoryServices.DirectoryEntry(forestGc));
            searcher.Filter = "(sAMAccountName=" + userId + ")";
            var results = searcher.FindOne().GetDirectoryEntry();
            return (DateTime)results.InvokeGet("PasswordExpirationDate");
        }
    }
}
