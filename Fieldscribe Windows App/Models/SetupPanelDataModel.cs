using System;
using System.Collections.Generic;
using System.ComponentModel;


namespace Fieldscribe_Windows_App.Models
{
    class SetupPanelDataModel : INotifyPropertyChanged
    {
        private static SetupPanelDataModel _instance = null;
        private static readonly object padlock = new object();
        private Meet _meet = null;
        private string _meetName;
        private string _meetLocation;
        private bool _meetSelected;
        private bool _folderSet;
        private bool _meetAndFolderSet;
        private DateTime? _meetDate = null;
        private string _measurementType;
        private bool _createEditMeetFormValid;

        public Meet SelectedMeet
        {
            get { return _meet; }
            set
            {
                _meet = value;
                NotifyPropertyChanged();

                MeetSelected = (_meet != null);
            }
        }

        public bool MeetSelected
        {
            get { return _meetSelected; }
            set
            {
                _meetSelected = value;
                NotifyPropertyChanged();

                MeetAndFolderSet = (_meetSelected && _folderSet);
            }
        }

        public bool FolderSet
        {
            get { return _folderSet; }
            set
            {
                _folderSet = value;
                NotifyPropertyChanged();

                MeetAndFolderSet = (_meetSelected && _folderSet);
            }
        }

        public bool MeetAndFolderSet
        {
            get { return _meetAndFolderSet; }
            set
            {
                _meetAndFolderSet = value;
                NotifyPropertyChanged();
            }
        }

        public string MeetName
        {
            get { return _meetName; }
            set
            {
                _meetName = value;
                NotifyPropertyChanged();
                CheckCreateEditMeetForm();
            }
        }

        public string MeetLocation
        {
            get { return _meetLocation; }
            set
            {
                _meetLocation = value;
                NotifyPropertyChanged();
                CheckCreateEditMeetForm();
            }
        }

        public DateTime? MeetDate
        {
            get { return _meetDate; }
            set
            {
                _meetDate = value;
                NotifyPropertyChanged();
                CheckCreateEditMeetForm();
            }
        }

        public string MeasurementType
        {
            get { return _measurementType; }
            set
            {
                _measurementType = value;
                NotifyPropertyChanged();
                CheckCreateEditMeetForm();
            }
        }

        public bool CreateEditMeetFormValid
        {
            get { return _createEditMeetFormValid; }
            set
            {
                _createEditMeetFormValid = value;
                NotifyPropertyChanged();
            }
        }

        public static SetupPanelDataModel Instance
        {
            get
            {
                lock (padlock)
                {
                    if (_instance == null)
                    {
                        _instance = new SetupPanelDataModel();
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

        private void CheckCreateEditMeetForm()
        {
            CreateEditMeetFormValid =
                (!string.IsNullOrEmpty(_meetName) &&
                !string.IsNullOrEmpty(_meetLocation) &&
                !string.IsNullOrEmpty(_measurementType) &&
                _meetDate != null);
        }
    }
}
