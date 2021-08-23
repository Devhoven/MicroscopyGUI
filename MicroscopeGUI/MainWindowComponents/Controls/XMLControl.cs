using System.Collections.Generic;
using System.Xml;
using System.IO;

namespace MicroscopeGUI
{
    partial class Control
    {
        static List<Control> AllControls = new List<Control>();

        // Accepts an xml string and overwrites the values of the controls with the right name with the values of the xml
        public static void LoadXML(string XML)
        {
            using (XmlReader Reader = XmlReader.Create(new StringReader(XML)))
            {
                foreach (Control Current in AllControls)
                {
                    if (Current.Serializable)
                    {
                        if (Reader.ReadToFollowing(Current.GetName().Replace(' ', '-')))
                        {
                            Reader.Read();
                            Current.SetValue(Reader.ReadContentAsObject());
                        }
                    }
                }
            }
        }

        // Constructs a xml file out of all the controls which have been initialized so far
        // The Content of the Control is set as the name and the value, the value of the given control (Checkbox, Slider)
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
    
        // Removes a control element from the all controls list
        public static void RemoveAllControls() =>
            AllControls.Clear();
    }
}
