using System;
using System.Windows;
using System.Collections.Generic;
using System.Diagnostics;
using peak.core;
using peak.core.nodes;
using System.Windows.Controls;
using System.Text;
using MicroscopeGUI.MainWindowComponents.Controls;
using MicroscopeGUI.MainWindowComponents.Controls.NodeControls;

namespace MicroscopeGUI.MainWindowComponents.Controls
{
    class ControlCon : StackPanel
    {
        readonly static List<ControlNodeInfo> CONTROL_NODES = new List<ControlNodeInfo>()
        {
            new ControlNodeInfo("AcquisitionFrameRate", "Acquisition frame rate", NodeType.Float),
            new ControlNodeInfo("ExposureTime", "Exposure time (µs)", NodeType.Float),
            new ControlNodeInfo("Gain", "Gain", NodeType.Float)
        };
        
        public struct ControlNodeInfo
        {
            public string Name;
            public string DisplayName;
            public NodeType Type;

            public ControlNodeInfo(string name, string displayName, NodeType type)
            {
                Name = name;
                Type = type;
                DisplayName = displayName;
            }
        }

        public ControlCon(NodeMap nodeMap)
        {
            AddControls(nodeMap);
        } 

        void AddControls(NodeMap nodeMap)
        {
            // BoolControl for the ColorCorrectionMatrix
            Children.Add(new BoolControl("Color Correction Matrix", AcquisitionWorker.UseColorCorrection, (val) =>
            {
                AcquisitionWorker.UseColorCorrection = val;
            }));

            foreach (ControlNodeInfo element in CONTROL_NODES)
            {
                switch (element.Type)
                {
                    case NodeType.Float:
                        Children.Add(new FloatNodeControl(nodeMap.FindNode<FloatNode>(element.Name), element));
                        break;

                    case NodeType.Enumeration:
                        Children.Add(new EnumNodeControl(nodeMap.FindNode<EnumerationNode>(element.Name), element));
                        break;
                }
            }
        }
    }
}
