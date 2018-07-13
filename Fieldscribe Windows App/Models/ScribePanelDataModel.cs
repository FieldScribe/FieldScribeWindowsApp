using System.Collections.Generic;
using System.ComponentModel;


namespace Fieldscribe_Windows_App.Models
{
    class ScribesPanelDataModel : INotifyPropertyChanged
    {
        private static ScribesPanelDataModel _instance = null;
        private static readonly object padlock = new object();
        private string _email;
        private string _firstName;
        private string _lastName;
        private string _password;
        private string _editScribeFirstName;
        private string _editScribeLastName;
        private bool _scribeFormValid = false;
        private bool _editScribeFormValid = false;
        private bool _passwordValid = false;
        private bool _resetPasswordValid = false;
        private IList<User> _assignedScribes;
        private IList<User> _scribes;
        private User _selectedScribe;
        private bool _scribeSelected;

        public bool ScribeFormValid
        {
            get { return _scribeFormValid; }
            set
            {
                _scribeFormValid = value;
                NotifyPropertyChanged();
            }
        }

        public bool EditScribeFormValid
        {
            get { return _editScribeFormValid; }
            set
            {
                _editScribeFormValid = value;
                NotifyPropertyChanged();
            }
        }

        public bool PasswordValid
        {
            get { return _passwordValid; }
            set
            {
                _passwordValid = value;
                CheckScribeForm();
                NotifyPropertyChanged();
            }
        }

        public bool ResetPasswordValid
        {
            get { return _resetPasswordValid; }
            set
            {
                _resetPasswordValid = value;
                NotifyPropertyChanged();
            }
        }

        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                CheckScribeForm();
                NotifyPropertyChanged();
            }
        }

        public string FirstName
        {
            get { return _firstName; }
            set
            {
                _firstName = value;
                CheckScribeForm();
                NotifyPropertyChanged();
            }
        }

        public string LastName
        {
            get { return _lastName; }
            set
            {
                _lastName = value;
                CheckScribeForm();
                NotifyPropertyChanged();
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                NotifyPropertyChanged();
            }
        }

        public string EditScribeFirstName
        {
            get { return _editScribeFirstName; }
            set
            {
                _editScribeFirstName = value;
                CheckEditScribeForm();
                NotifyPropertyChanged();
            }
        }

        public string EditScribeLastName
        {
            get { return _editScribeLastName; }
            set
            {
                _editScribeLastName = value;
                CheckEditScribeForm();
                NotifyPropertyChanged();
            }
        }

        public User SelectedScribe
        {
            get { return _selectedScribe; }
            set
            {
                _selectedScribe = value;
                ScribeSelected = (_selectedScribe != null);
                NotifyPropertyChanged();
            }
        }

        public IList<User> Scribes
        {
            get { return _scribes; }
            set
            {
                _scribes = value;
                NotifyPropertyChanged();
            }
        }

        public IList<User> AssignedScribes
        {
            get { return _assignedScribes; }
            set
            {
                _assignedScribes = value;
                NotifyPropertyChanged();
            }
        }

        public bool ScribeSelected
        {
            get { return _scribeSelected; }
            set
            {
                _scribeSelected = value;
                NotifyPropertyChanged();
            }
        }


        public static ScribesPanelDataModel Instance
        {
            get
            {
                lock (padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new ScribesPanelDataModel();
                    }
                    return _instance;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string property = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        private void CheckScribeForm()
        {
            ScribeFormValid =
                (!string.IsNullOrEmpty(_firstName) &&
                !string.IsNullOrEmpty(_lastName) &&
                !string.IsNullOrEmpty(_email) &&
                _passwordValid);
        }

        private void CheckEditScribeForm()
        {
            EditScribeFormValid =
                (!string.IsNullOrEmpty(_editScribeFirstName) &&
                !string.IsNullOrEmpty(_editScribeLastName));
        }
    }
}
