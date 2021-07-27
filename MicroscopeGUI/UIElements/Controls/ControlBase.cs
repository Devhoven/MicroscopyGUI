using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace MicroscopeGUI
{
    class Control
    {
        static List<Control> AllControls = new List<Control>();

        public Label Label;

        protected bool _Enable;
        internal bool Serializable;

        public virtual bool Enable
        {
            get { return _Enable; }
            set { _Enable = value; }
        }

        internal Control(Grid Parent)
        {
            // Has to be initialized for every new row
            RowDefinition CheckBoxRowDefinition = new RowDefinition()
            {
                Height = GridLength.Auto
            };
            Parent.RowDefinitions.Add(CheckBoxRowDefinition);

            AllControls.Add(this);
            Serializable = true;
        }

        internal Control(string Name, Grid Parent) : this(Parent)
        {
            Label = new Label();
            Label.Content = Name;
            Label.VerticalAlignment = VerticalAlignment.Top;
            Label.HorizontalAlignment = HorizontalAlignment.Left;
        }

        public virtual string GetName() => 
            "";

        public virtual void SetValue(object Value) { }

        public virtual object GetValue() => 
            null;

        public static void LoadXML(string XML)
        {
            using (XmlReader Reader = XmlReader.Create(new StringReader(XML)))
            {
                foreach (Control Current in AllControls)
                {
                    if (Current.Serializable)
                    {
                        Reader.ReadToFollowing(Current.GetName().Replace(' ', '-'));
                        Reader.Read();
                        Current.SetValue(Reader.ReadContentAsObject());
                    }
                }
            }
        }

        public static string GetXMLString()
        {
            string Result = "<MicroscopeCamConfig>\n";
            string CurrentName;
            foreach (Control Current in AllControls)
            {
                if (Current.Serializable)
                {
                    CurrentName = Current.GetName().Replace(' ', '-');
                    Result += "\t<" + CurrentName + ">" + Current.GetValue().ToString() + "</" + CurrentName + ">\n";
                }
            }

            return Result + "</MicroscopeCamConfig>";
        }
    }
}
