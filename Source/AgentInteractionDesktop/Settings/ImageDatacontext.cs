using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Media.Imaging;

namespace Agent.Interaction.Desktop.Settings
{
    class ImageDatacontext
    {
        #region Single instance

        public static ImageDatacontext _instance = null;

        public static ImageDatacontext GetInstance()
        {
            if (_instance == null)
            {
                _instance = new ImageDatacontext();
                return _instance;
            }
            return _instance;
        }

        #endregion Single instance

        private BitmapImage _imgLoginEnabled;
        private BitmapImage _imgLogoutEnabled;
        private BitmapImage _imgReadyEnabled;
        private BitmapImage _imgNotReadyEnabled;
        private BitmapImage _imgTalkEnabled;
        private BitmapImage _imgConfEnabled;
        private BitmapImage _imgCompConfEnabled;
        private BitmapImage _imgTransEnabled;
        private BitmapImage _imgCompTransEnabled;
        private BitmapImage _imgReconnectEnabled;
        private BitmapImage _imgAltCallEnabled;
        private BitmapImage _imgMergeCallEnabled;
        private BitmapImage _imgDialPadEnabled;
        private BitmapImage _imgCallDataEnabled;
        private BitmapImage _imgHoldEnabled;
        private BitmapImage _imgRetrieveEnabled;
        private BitmapImage _imgReleaseEnabled;
        private BitmapImage _imgDeleteConfEnabled;

        private BitmapImage _imgLoginDisabled;
        private BitmapImage _imgLogoutDisabled;
        private BitmapImage _imgReadyDisabled;
        private BitmapImage _imgNotReadyDisabled;
        private BitmapImage _imgTalkDisabled;
        private BitmapImage _imgConfDisabled;
        private BitmapImage _imgCompConfDisabled;
        private BitmapImage _imgTransDisabled;
        private BitmapImage _imgCompTransDisabled;
        private BitmapImage _imgReconnectDisabled;
        private BitmapImage _imgAltCallDisabled;
        private BitmapImage _imgMergeCallDisabled;
        private BitmapImage _imgDialPadDisabled;
        private BitmapImage _imgCallDataDisabled;
        private BitmapImage _imgHoldDisabled;
        private BitmapImage _imgRetrieveDisabled;
        private BitmapImage _imgReleaseDisabled;
        private BitmapImage _imgDeleteConfDisabled;

        private BitmapImage _imgReadyStatus;
        private BitmapImage _imgNotReadyStatus;
        private BitmapImage _imgLogoutStatus;
        private BitmapImage _imgDNDStatus;
        private BitmapImage _imgOutofServiceStatus;
        private BitmapImage _imgACWStatus;

        public BitmapImage ImgLoginEnabled
        {
            get
            {
                return _imgLoginEnabled;
            }
            set
            {
                if (value != _imgLoginEnabled)
                    _imgLoginEnabled = value;
            }
        }

        public BitmapImage ImgLogoutEnabled
        {
            get
            {
                return _imgLogoutEnabled;
            }
            set
            {
                if (value != _imgLogoutEnabled)
                    _imgLogoutEnabled = value;
            }
        }

        public BitmapImage ImgReadyEnabled
        {
            get
            {
                return _imgReadyEnabled;
            }
            set
            {
                if (value != _imgReadyEnabled)
                    _imgReadyEnabled = value;
            }
        }

        public BitmapImage ImgNotReadyEnabled
        {
            get
            {
                return _imgNotReadyEnabled;
            }
            set
            {
                if (value != _imgNotReadyEnabled)
                    _imgNotReadyEnabled = value;
            }
        }

        public BitmapImage ImgTalkEnabled
        {
            get
            {
                return _imgTalkEnabled;
            }
            set
            {
                if (value != _imgTalkEnabled)
                    _imgTalkEnabled = value;
            }
        }

        public BitmapImage ImgConfEnabled
        {
            get
            {
                return _imgConfEnabled;
            }
            set
            {
                if (value != _imgConfEnabled)
                    _imgConfEnabled = value;
            }
        }

        public BitmapImage ImgCompConfEnabled
        {
            get
            {
                return _imgCompConfEnabled;
            }
            set
            {
                if (value != _imgCompConfEnabled)
                    _imgCompConfEnabled = value;
            }
        }

        public BitmapImage ImgTransEnabled
        {
            get
            {
                return _imgTransEnabled;
            }
            set
            {
                if (value != _imgTransEnabled)
                    _imgTransEnabled = value;
            }
        }

        public BitmapImage ImgCompTransEnabled
        {
            get
            {
                return _imgCompTransEnabled;
            }
            set
            {
                if (value != _imgCompTransEnabled)
                    _imgCompTransEnabled = value;
            }
        }

        public BitmapImage ImgReconnectEnabled
        {
            get
            {
                return _imgReconnectEnabled;
            }
            set
            {
                if (value != _imgReconnectEnabled)
                    _imgReconnectEnabled = value;
            }
        }

        public BitmapImage ImgAltCallEnabled
        {
            get
            {
                return _imgAltCallEnabled;
            }
            set
            {
                if (value != _imgAltCallEnabled)
                    _imgAltCallEnabled = value;
            }
        }

        public BitmapImage ImgMergeCallEnabled
        {
            get
            {
                return _imgMergeCallEnabled;
            }
            set
            {
                if (value != _imgMergeCallEnabled)
                    _imgMergeCallEnabled = value;
            }
        }

        public BitmapImage ImgDialPadEnabled
        {
            get
            {
                return _imgDialPadEnabled;
            }
            set
            {
                if (value != _imgDialPadEnabled)
                    _imgDialPadEnabled = value;
            }
        }

        public BitmapImage ImgCallDataEnabled
        {
            get
            {
                return _imgCallDataEnabled;
            }
            set
            {
                if (value != _imgCallDataEnabled)
                    _imgCallDataEnabled = value;
            }
        }

        public BitmapImage ImgHoldEnabled
        {
            get
            {
                return _imgHoldEnabled;
            }
            set
            {
                if (value != _imgHoldEnabled)
                    _imgHoldEnabled = value;
            }
        }

        public BitmapImage ImgRetrieveEnabled
        {
            get
            {
                return _imgRetrieveEnabled;
            }
            set
            {
                if (value != _imgRetrieveEnabled)
                    _imgRetrieveEnabled = value;
            }
        }

        public BitmapImage ImgReleaseEnabled
        {
            get
            {
                return _imgReleaseEnabled;
            }
            set
            {
                if (value != _imgReleaseEnabled)
                    _imgReleaseEnabled = value;
            }
        }

        public BitmapImage ImgDeleteConfEnabled
        {
            get
            {
                return _imgDeleteConfEnabled;
            }
            set
            {
                if (value != _imgDeleteConfEnabled)
                    _imgDeleteConfEnabled = value;
            }
        }



        public BitmapImage ImgLoginDisabled
        {
            get
            {
                return _imgLoginDisabled;
            }
            set
            {
                if (value != _imgLoginDisabled)
                    _imgLoginDisabled = value;
            }
        }

        public BitmapImage ImgLogoutDisabled
        {
            get
            {
                return _imgLogoutDisabled;
            }
            set
            {
                if (value != _imgLogoutDisabled)
                    _imgLogoutDisabled = value;
            }
        }

        public BitmapImage ImgReadyDisabled
        {
            get
            {
                return _imgReadyDisabled;
            }
            set
            {
                if (value != _imgReadyDisabled)
                    _imgReadyDisabled = value;
            }
        }

        public BitmapImage ImgNotReadyDisabled
        {
            get
            {
                return _imgNotReadyDisabled;
            }
            set
            {
                if (value != _imgNotReadyDisabled)
                    _imgNotReadyDisabled = value;
            }
        }

        public BitmapImage ImgTalkDisabled
        {
            get
            {
                return _imgTalkDisabled;
            }
            set
            {
                if (value != _imgTalkDisabled)
                    _imgTalkDisabled = value;
            }
        }

        public BitmapImage ImgConfDisabled
        {
            get
            {
                return _imgConfDisabled;
            }
            set
            {
                if (value != _imgConfDisabled)
                    _imgConfDisabled = value;
            }
        }

        public BitmapImage ImgCompConfDisabled
        {
            get
            {
                return _imgCompConfDisabled;
            }
            set
            {
                if (value != _imgCompConfDisabled)
                    _imgCompConfDisabled = value;
            }
        }

        public BitmapImage ImgTransDisabled
        {
            get
            {
                return _imgTransDisabled;
            }
            set
            {
                if (value != _imgTransDisabled)
                    _imgTransDisabled = value;
            }
        }

        public BitmapImage ImgCompTransDisabled
        {
            get
            {
                return _imgCompTransDisabled;
            }
            set
            {
                if (value != _imgCompTransDisabled)
                    _imgCompTransDisabled = value;
            }
        }

        public BitmapImage ImgReconnectDisabled
        {
            get
            {
                return _imgReconnectDisabled;
            }
            set
            {
                if (value != _imgReconnectDisabled)
                    _imgReconnectDisabled = value;
            }
        }

        public BitmapImage ImgAltCallDisabled
        {
            get
            {
                return _imgAltCallDisabled;
            }
            set
            {
                if (value != _imgAltCallDisabled)
                    _imgAltCallDisabled = value;
            }
        }

        public BitmapImage ImgMergeCallDisabled
        {
            get
            {
                return _imgMergeCallDisabled;
            }
            set
            {
                if (value != _imgMergeCallDisabled)
                    _imgMergeCallDisabled = value;
            }
        }

        public BitmapImage ImgDialPadDisabled
        {
            get
            {
                return _imgDialPadDisabled;
            }
            set
            {
                if (value != _imgDialPadDisabled)
                    _imgDialPadDisabled = value;
            }
        }

        public BitmapImage ImgCallDataDisabled
        {
            get
            {
                return _imgCallDataDisabled;
            }
            set
            {
                if (value != _imgCallDataDisabled)
                    _imgCallDataDisabled = value;
            }
        }

        public BitmapImage ImgHoldDisabled
        {
            get
            {
                return _imgHoldDisabled;
            }
            set
            {
                if (value != _imgHoldDisabled)
                    _imgHoldDisabled = value;
            }
        }

        public BitmapImage ImgRetrieveDisabled
        {
            get
            {
                return _imgRetrieveDisabled;
            }
            set
            {
                if (value != _imgRetrieveDisabled)
                    _imgRetrieveDisabled = value;
            }
        }

        public BitmapImage ImgReleaseDisabled
        {
            get
            {
                return _imgReleaseDisabled;
            }
            set
            {
                if (value != _imgReleaseDisabled)
                    _imgReleaseDisabled = value;
            }
        }

        public BitmapImage ImgDeleteConfDisabled
        {
            get
            {
                return _imgDeleteConfDisabled;
            }
            set
            {
                if (value != _imgDeleteConfDisabled)
                    _imgDeleteConfDisabled = value;
            }
        }



        public BitmapImage ImgReadyStatus
        {
            get
            {
                return _imgReadyStatus;
            }
            set
            {
                if (value != _imgReadyStatus)
                    _imgReadyStatus = value;
            }
        }

        public BitmapImage ImgNotReadyStatus
        {
            get
            {
                return _imgNotReadyStatus;
            }
            set
            {
                if (value != _imgNotReadyStatus)
                    _imgNotReadyStatus = value;
            }
        }

        public BitmapImage ImgLogoutStatus
        {
            get
            {
                return _imgLogoutStatus;
            }
            set
            {
                if (value != _imgLogoutStatus)
                    _imgLogoutStatus = value;
            }
        }

        public BitmapImage ImgDNDStatus
        {
            get
            {
                return _imgDNDStatus;
            }
            set
            {
                if (value != _imgDNDStatus)
                    _imgDNDStatus = value;
            }
        }

        public BitmapImage ImgOutofServiceStatus
        {
            get
            {
                return _imgOutofServiceStatus;
            }
            set
            {
                if (value != _imgOutofServiceStatus)
                    _imgOutofServiceStatus = value;
            }
        }

        public BitmapImage ImgACWStatus
        {
            get
            {
                return _imgACWStatus;
            }
            set
            {
                if (value != _imgACWStatus)
                    _imgACWStatus = value;
            }
        }
    }
}
