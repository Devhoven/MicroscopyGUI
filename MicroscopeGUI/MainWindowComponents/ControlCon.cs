using System;
using System.Windows;
using System.Collections.Generic;
using System.Diagnostics;
using peak.core;
using peak.core.nodes;
using System.Windows.Controls;
using System.Text;

namespace MicroscopeGUI
{
    class ControlCon : StackPanel
    {
        readonly static List<ControlNode> CONTROL_NODES = new List<ControlNode>()
        {
            new ControlNode("AcquisitionFrameRate", NodeType.Float),
            new ControlNode("ExposureTime", NodeType.Float),
            new ControlNode("Gain", NodeType.Float)
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

        NodeMap NodeMap;
        CommandNode AcquisitionStartNode;
        CommandNode AcquisitionStopNode;

        public ControlCon(StackPanel parent, NodeMap nodeMap)
        {
            NodeMap = nodeMap;

            AcquisitionStartNode = NodeMap.FindNode<CommandNode>("AcquisitionStart");
            AcquisitionStopNode = NodeMap.FindNode<CommandNode>("AcquisitionStop");

            parent.Children.Add(this);
            AddControls(nodeMap);
        }

        void AddControls(NodeMap nodeMap)
        {
            foreach (ControlNode element in CONTROL_NODES)
            {
                switch (element.Type)
                {
                    case NodeType.Float:
                        new FloatSliderControl(nodeMap.FindNode<FloatNode>(element.Name), this);
                        break;

                    case NodeType.Enumeration:
                        new EnumControl(nodeMap.FindNode<EnumerationNode>(element.Name), this);
                        break;
                }
            }
        }

        public void SetNode(Action action)
        {
            AcquisitionStopNode.Execute();
            action();
            AcquisitionStartNode.Execute();
        }
    }
}
