using DS4Windows;
using System;
using System.IO;

namespace DS4WinWPF.DS4Forms.ViewModels
{
    public class RenameProfileViewModel
    {
        private string profileName;
        public string ProfileName
        {
            get => profileName;
            set
            {
                profileName = value;
                ProfileNameChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public event EventHandler ProfileNameChanged;

        public bool ProfileFileExists()
        {
            string filePath = Path.Combine(Global.appdatapath,
                "Profiles", $"{profileName}.xml");
            return File.Exists(filePath);
        }
    }
}
