using Godot;
using MicroGodotNet.Messages;
using MicroGodotNet.Network;
using MicroGodotNet.Signal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MicroGodotNet.Signal
{
    public class UIManager : Node, ISignal<NetworkStatus>
    {
        private NetworkPlayer hostReference;

        public Label ConnectedStatus;
        public Label PingStatus;
        public Label ConnectedToIpStatus;

        private Panel menu;

        private Label StatusLabel;
        private LineEdit IPInput;
        private LineEdit PortInput;
        private Button connectButton;
        private Button hostButton;

        private bool isClickable = true;

        public void Execute(NetworkStatus signal)
        {
            switch(signal)
            {
                case NetworkStatus.ConnectionSuccess:
                NetworkStatusMessage(StatusMessageType.Information, "Connected");
                menu.Visible = false;
                break;

                case NetworkStatus.ConnectionFailure:
                NetworkStatusMessage(StatusMessageType.Error, "Failed to connect");
                break;

                case NetworkStatus.Disconnection:
                NetworkStatusMessage(StatusMessageType.Error, "Disconnected");
                break;

                case NetworkStatus.Hosting:
                ConnectedStatus.Text = "Hosting";
                break;

                case NetworkStatus.Ready:
                ConnectedStatus.Text = "Ready";
                break;

                case NetworkStatus.Stopped:
                ConnectedStatus.Text = "Stopped";
                break;
            }

            isClickable = true;
        }


        public override void _Ready()
        {
            SignalManager.Subscribe<NetworkStatus>(this);

            Panel networkPanel = (Panel)GetChild(0).GetChild(1);

            ConnectedStatus = (Label)networkPanel.GetChild(0);
            PingStatus = (Label)networkPanel.GetChild(1);
            ConnectedToIpStatus = (Label)networkPanel.GetChild(2);

            menu = (Panel)GetChild(1);

            StatusLabel = (Label)menu.GetChild(0);
            IPInput = (LineEdit)menu.GetChild(1);
            PortInput = (LineEdit)menu.GetChild(2);
            connectButton = (Button)menu.GetChild(3);
            hostButton = (Button)menu.GetChild(4);

      //      SignalManager.Subscribe("Prepare", this, "Initialize");
      //      SignalManager.Subscribe("ConnectionFailure", this, "ConnectionFailure");


            connectButton.Connect("button_up", this, "ConnectButton");
            hostButton.Connect("button_up", this, "HostButton");
        }


        private void HostButton()
        {
            if (!isClickable)
                return;


            hostReference = new HostPlayer();
            SignalManager.Signal(hostReference);
            menu.Visible = false;

        }



        private void ConnectButton()
        {           
            if (!isClickable)
                return;


            hostReference = new ClientPlayer();

            SignalManager.Signal(hostReference);


            if (IPAddress.TryParse(IPInput.Text, out IPAddress addr) && ushort.TryParse(PortInput.Text, out ushort port))
            {
                isClickable = false;
                NetworkStatusMessage(StatusMessageType.Information, "Attempting to connect");
                hostReference.Connect(addr.ToString(), port);
                return;
            }

            NetworkStatusMessage(StatusMessageType.Error, "Invalid input");

        }
       

        public void NetworkStatusMessage(StatusMessageType status, string messasge)
        {
            switch (status)
            {
                case StatusMessageType.Information:
                StatusLabel.SetModulate(new Color(0.15f, 0.95f, 0.15f));
                break;

                case StatusMessageType.Warning:
                StatusLabel.SetModulate(new Color(0.95f, 0.95f, 0.15f));
                break;

                case StatusMessageType.Error:
                StatusLabel.SetModulate(new Color(0.95f, 0.15f, 0.15f));
                break;

            }

            StatusLabel.Text = messasge;

        }

    }

}
