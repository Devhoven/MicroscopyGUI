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
        readonly static List<ControlNode> CONTROL_NODES = new List<ControlNode>()
        {
            new ControlNode("AcquisitionFrameRate", NodeType.Float),
            new ControlNode("ExposureTime", NodeType.Float),
            new ControlNode("Gain", NodeType.Float)
            //new ControlNode("GainSelector", NodeType.Enumeration),
            //new ControlNode("SensorOperationMode", NodeType.Enumeration),
            //new ControlNode("UserSetSelector", NodeType.Enumeration),
            //new ControlNode("SequencerMode", NodeType.Enumeration),
        };
        
        struct ControlNode
        {
            public string Name;
            public NodeType Type;

            public ControlNode(string name, NodeType type)
            {
                Name = name;
                Type = type;
            }
        }

        public ControlCon(NodeMap nodeMap)
        {
            AddControls(nodeMap);
        } 

        void AddControls(NodeMap nodeMap)
        {
            foreach (ControlNode element in CONTROL_NODES)
            {
                switch (element.Type)
                {
                    case NodeType.Float:
                        Children.Add(new FloatNodeControl(nodeMap.FindNode<FloatNode>(element.Name)));
                        break;

                    case NodeType.Enumeration:
                        Children.Add(new EnumNodeControl(nodeMap.FindNode<EnumerationNode>(element.Name)));
                        break;
                }
            }

            Children.Add(new BoolControl("Color Correction Matrix", AcquisitionWorker.UseColorCorrection, (val) =>
            {
                AcquisitionWorker.UseColorCorrection = val;
            }));
        }
    }
}
