using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyUp
{
    internal class AppSettings
    {
        private static AppSettings? instance;

        public static AppSettings GetInstance()
        {
            instance ??= new AppSettings();
            return instance;
        }

        private string? _lockScreenHeader;
        public string LockScreenHeader
        {
            get { return _lockScreenHeader ?? ""; }
            set { _lockScreenHeader = value; }
        }

        private string? _lockScreenParagraph;
        public string LockScreenParagraph
        {
            get { return _lockScreenParagraph ?? ""; }    
            set { _lockScreenParagraph = value; }
        }

        private bool _themeIsDark;
        public bool ThemeIsDark
        {
            get { return _themeIsDark; }    
            set { _themeIsDark = value; }
        }

        private int _breaksIntervalSec;
        public int BreaksIntervalSec
        {
            get { return _breaksIntervalSec; }    
            set { 
                if (value > 0)
                    _breaksIntervalSec = value; 
                else
                    _breaksIntervalSec = 0;
            }
        }

        private int _durationBreakSec;
        public int DurationBreakSec
        {
            get { return _durationBreakSec; }    
            set {
                if (value > 0)
                    _durationBreakSec = value;
                else
                    _durationBreakSec = 0;
            }
        }

        private bool _reviveProgram;
        public bool ReviveProgram
        {
            get { return _reviveProgram; }    
            set { _reviveProgram = value; }
        }

        private bool _lookHiddenRest;
        public bool LookHiddenRest
        {
            get { return _lookHiddenRest; }    
            set { _lookHiddenRest = value; }
        }

        private bool _startupWithSystem;
        public bool StartupWithSystem
        {
            get { return _startupWithSystem; }    
            set { _startupWithSystem = value; }
        }

        private bool _startInTray;
        public bool StartInTray
        {
            get { return _startInTray; }    
            set { _startInTray = value; }
        }

        private bool _closeInTray;
        public bool CloseInTray
        {
            get { return _closeInTray; }    
            set { _closeInTray = value; }
        }

        private bool _hideProgram;
        public bool HideProgram
        {
            get { return _hideProgram; }    
            set { _hideProgram = value; }
        }


        internal AppSettings() {
        }
    }
}
