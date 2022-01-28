using System;
using System.Windows;
using System.Windows.Controls;

namespace DS4WinWPF.DS4Forms
{
    /// <summary>
    /// Interaction logic for DupBox.xaml
    /// </summary>
    public partial class DupBox : UserControl
    {
        private string oldfilename;
        public string OldFilename { get => oldfilename; set => oldfilename = value; }

        public event EventHandler Cancel;
        public delegate void SaveHandler(DupBox sender, string profilename);
        public event SaveHandler Save;

        public DupBox()
        {
            InitializeComponent();
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            string profile = profileTxt.Text;
            if (!string.IsNullOrWhiteSpace(profile) &&
                profile.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) == -1)
            {
                System.IO.File.Copy(DS4Windows.Global.appdatapath + "\\Profiles\\" + oldfilename + ".xml",
                DS4Windows.Global.appdatapath + "\\Profiles\\" + profile + ".xml", true);
                Save?.Invoke(this, profile);
            }
            else
            {
                MessageBox.Show(Properties.Resources.ValidName, Properties.Resources.NotValid,
                    MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Cancel?.Invoke(this, EventArgs.Empty);
        }
    }
}
