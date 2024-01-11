

namespace OfficeReportInterface.ReadingIniFile
{
     public  class ElementManager
    {
        public string LastErrorMessage { get; set; }

        public bool GetUserNameAndPasswordFromIniFile(string fileName, out string userName, out string password)
        {
            userName = string.Empty;
            password = string.Empty;
            try
            {
                var iniFile = new INIFile(fileName);
                userName = iniFile.ReadString("Login", "UserName");
                password = iniFile.ReadString("Login", "Password");
                return true;
            }
            catch (System.Exception ex)
            {
                LastErrorMessage = ex.Message;
                return false;
            }
        }
    }
}
