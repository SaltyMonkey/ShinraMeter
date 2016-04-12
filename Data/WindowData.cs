﻿using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Xml;
using System.Xml.Linq;

namespace Data
{
    public class WindowData
    {
        private readonly string _windowFile;
        private readonly XDocument _xml;
        private readonly FileStream _filestream;

        public WindowData(BasicTeraData basicData)
        {
            DefaultValue();
            // Load XML File
            _windowFile = Path.Combine(basicData.ResourceDirectory, "config/window.xml");
            try
            {
                _filestream = new FileStream(_windowFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
                _xml = XDocument.Load(_filestream);
            }
            catch (Exception ex) when (ex is XmlException || ex is InvalidOperationException)
            {
                Save();
                _filestream = new FileStream(_windowFile, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
                return;
            }
            catch
            {
                return;
            }

            ParseLocation();
            ParseLanguage();
            ParseMainWindowOpacity();
            ParseSkillWindowOpacity();
            ParseAutoUpdate();
            ParseRememberPosition();
            ParseWinpcap();
            ParseInvisibleUi();
            ParseAllowTrasparency();
            ParseTopmost();
            ParseTeraDps();
            ParseDebug();
        }


        public Point Location { get; set; }

        public double MainWindowOpacity { get; private set; }
        public double SkillWindowOpacity { get; private set; }

        public bool RememberPosition { get; private set; }
        public string Language { get; private set; }

        public bool AutoUpdate { get; private set; }

        public bool Winpcap { get; private set; }

        public bool InvisibleUI { get; set; }

        public bool AllowTransparency { get; set; }

        public string TeraDpsUser { get; private set; }
        public string TeraDpsToken { get; private set; }

        public bool Topmost { get; set; }

        public bool Debug { get; set; }

        private void DefaultValue()
        {
            Location = new Point(0, 0);
            Language = "Auto";
            MainWindowOpacity = 0.5;
            SkillWindowOpacity = 0.7;
            AutoUpdate = true;
            RememberPosition = true;
            InvisibleUI = false;
            Winpcap = true;
            Topmost = true;
            AllowTransparency = true;
            Debug = true;
            TeraDpsToken = "";
            TeraDpsUser = "";
        }

        private void ParseTeraDps()
        {
            var root = _xml.Root;
            var teradps = root?.Element("teradps.io");
            var user = teradps?.Element("user");
            if (user == null) return;
            var token = teradps?.Element("token");
            if (token == null) return;

            TeraDpsToken = token.Value;
            TeraDpsUser = user.Value;

            if(TeraDpsToken == null || TeraDpsUser == null)
            {
                TeraDpsToken = "";
                TeraDpsUser = "";
            }
        }


        public void ParseRememberPosition()
        {
            var root = _xml.Root;
            var rememberPosition = root?.Element("remember_position");
            if (rememberPosition == null) return;
            bool remember;
            var parseSuccess = bool.TryParse(rememberPosition.Value, out remember);
            if (parseSuccess)
            {
                RememberPosition = remember;
            }
        }

        public void ParseDebug()
        {
            var root = _xml.Root;
            var debug = root?.Element("debug");
            if (debug == null) return;
            bool remember;
            var parseSuccess = bool.TryParse(debug.Value, out remember);
            if (parseSuccess)
            {
                Debug = remember;
            }
        }

        public void ParseTopmost()
        {
            var root = _xml.Root;
            var topmost = root?.Element("topmost");
            if (topmost == null) return;
            bool remember;
            var parseSuccess = bool.TryParse(topmost.Value, out remember);
            if (parseSuccess)
            {
                Topmost = remember;
            }
        }


        public void ParseInvisibleUi()
        {
            var root = _xml.Root;
            var invisibleUI = root?.Element("invisible_ui");
            if (invisibleUI == null) return;
            bool invisibleUi;
            var parseSuccess = bool.TryParse(invisibleUI.Value, out invisibleUi);
            if (parseSuccess)
            {
                InvisibleUI = invisibleUi;
            }
        }

        public void ParseAllowTrasparency()
        {
            var root = _xml.Root;
            var allowTransparency = root?.Element("allow_transparency");
            if (allowTransparency == null) return;
            bool transparency;
            var parseSuccess = bool.TryParse(allowTransparency.Value, out transparency);
            if (parseSuccess)
            {
                AllowTransparency = transparency;
            }
        }

        public void ParseWinpcap()
        {
            var root = _xml.Root;
            var winpcapElement = root?.Element("winpcap");
            if (winpcapElement == null) return;
            bool winpcap;
            var parseSuccess = bool.TryParse(winpcapElement.Value, out winpcap);
            if (parseSuccess)
            {
                Winpcap = winpcap;
            }
        }


        public void ParseAutoUpdate()
        {
            var root = _xml.Root;
            var autoupdate = root?.Element("autoupdate");
            if (autoupdate == null) return;
            bool auto;
            var parseSuccess = bool.TryParse(autoupdate.Value, out auto);
            if (parseSuccess)
            {
                AutoUpdate = auto;
            }
        }

        private void ParseLocation()
        {
            int x, y;
            var root = _xml.Root;

            var location = root?.Element("location");
            if (location == null) return;
            var xElement = location.Element("x");
            var yElement = location.Element("y");
            if (xElement == null || yElement == null) return;

            var xParsed = int.TryParse(xElement.Value, out x);
            var yParsed = int.TryParse(yElement.Value, out y);
            if (xParsed && yParsed)
            {
                Location = new Point(x, y);
            }
        }

        private void ParseLanguage()
        {
            var root = _xml.Root;
            var languageElement = root?.Element("language");
            if (languageElement == null) return;
            Language = languageElement.Value;
            if (!Array.Exists(new[] { "Auto", "EU-EN", "EU-FR", "EU-GER", "NA", "RU", "JP", "TW", "KR" }, s => s.Equals(Language))) Language = "Auto";
        }

        private void ParseMainWindowOpacity()
        {
            int mainWindowOpacity;
            var root = _xml.Root;
            var opacity = root?.Element("opacity");
            var mainWindowElement = opacity?.Element("mainWindow");
            if (mainWindowElement == null) return;

            if (int.TryParse(mainWindowElement.Value, out mainWindowOpacity))
            {
                MainWindowOpacity = (double) mainWindowOpacity/100;
            }
        }

        private void ParseSkillWindowOpacity()
        {
            int skillWindowOpacity;
            var root = _xml.Root;
            var opacity = root?.Element("opacity");
            var mainWindowElement = opacity?.Element("skillWindow");
            if (mainWindowElement == null) return;

            if (int.TryParse(mainWindowElement.Value, out skillWindowOpacity))
            {
                SkillWindowOpacity = (double) skillWindowOpacity/100;
            }
        }

        public void Save()
        {
            var xml = new XDocument(new XElement("window"));
            xml.Root.Add(new XElement("location"));
            xml.Root.Element("location").Add(new XElement("x", Location.X.ToString(CultureInfo.InvariantCulture)));
            xml.Root.Element("location").Add(new XElement("y", Location.Y.ToString(CultureInfo.InvariantCulture)));
            xml.Root.Add(new XElement("language", Language));
            xml.Root.Add(new XElement("opacity"));
            xml.Root.Element("opacity").Add(new XElement("mainWindow", MainWindowOpacity*100));
            xml.Root.Element("opacity").Add(new XElement("skillWindow", SkillWindowOpacity*100));
            xml.Root.Add(new XElement("autoupdate", AutoUpdate));
            xml.Root.Add(new XElement("remember_position", RememberPosition));
            xml.Root.Add(new XElement("winpcap", Winpcap));
            xml.Root.Add(new XElement("invisible_ui", InvisibleUI));
            xml.Root.Add(new XElement("allow_transparency", AllowTransparency));
            xml.Root.Add(new XElement("topmost", Topmost));
            xml.Root.Add(new XElement("teradps.io"));
            xml.Root.Element("teradps.io").Add(new XElement("user", TeraDpsUser));
            xml.Root.Element("teradps.io").Add(new XElement("token", TeraDpsToken));
            xml.Root.Add(new XElement("debug", Debug));

            _filestream.SetLength(0);
            using (StreamWriter sr = new StreamWriter(_filestream))
            {
               
                // File writing as usual
                sr.Write(xml);
            }
            _filestream.Close();
        }
    }
}