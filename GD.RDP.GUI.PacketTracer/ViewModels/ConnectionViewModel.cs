namespace GD.RDP.GUI.PacketTracer.ViewModels
{
    using GD.RDP.Contracts;
    using GD.RDP.Core;
    using GD.RDP.Debug;
    using GD.RDP.GUI.PacketTracer.Commands;
    using GD.RDP.GUI.PacketTracer.Models;
    using GD.RDP.Network.Sockets.Managers.TCP;
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Net;
    using System.Windows.Input;

    public class ConnectionViewModel : ObservableObject
    {
        private ConnectionModel _model;

        private ICommand _connectCommand;
        private ICommand _sendCommand;

        private string _dataHexDisplay;
        private string _outputHexDisplay;
        private string _data;
        private string _address;
        private string _port;
        private ICommand _restoreCommand;

        public ConnectionViewModel()
        {
            this._model = new ConnectionModel();
            this.History = new ObservableCollection<HistoryModel>();


        }

        public string Address
        {
            get
            {
                return this._address;
            }
            set
            {
                this._address = value.Trim();
                this.OnPropertyChanged(() => this.Address);
            }
        }

        public string Port
        {
            get
            {
                return this._port;
            }
            set
            {
                this._port = value.Trim();
                this.OnPropertyChanged(() => this.Port);
            }
        }

        public string Data
        {
            get
            {
                return this._data;
            }
            set
            {
                this._data = value;
                this.OnPropertyChanged(() => this.Data);
                this.DataHexDisplay = value;
            }
        }

        public string DataHexDisplay
        {
            get
            {
                return _dataHexDisplay;
            }
            set
            {
                this._dataHexDisplay = Injector.Instance
                      .Get<IDumper>()
                      .Dump(Injector.Instance
                            .Get<InputHelper>()
                            .TextToByteArray(
                                value,
                                new string[] { "\t", "\n", "\r" },
                                NumberStyles.HexNumber));

                this.OnPropertyChanged(() => this.DataHexDisplay);
            }
        }

        public string OutputHexDisplay
        {
            get
            {
                return this._outputHexDisplay;
            }
            set
            {
                this._outputHexDisplay = value;

                this.OnPropertyChanged(() => this.OutputHexDisplay);
            }
        }

        public ObservableCollection<HistoryModel> History { get; set; }

        public HistoryModel SelectedHistoryModel { get; set; }

        public ICommand SendCommand
        {
            get
            {
                if (this._sendCommand == null)
                {
                    this._sendCommand = new RelayCommand((x) =>
                    {
                        //TODO(PPavlov): Create Converter
                        this._model.Data = Injector.Instance
                            .Get<InputHelper>()
                            .TextToByteArray(
                                this.Data,
                                new string[] { "\t", "\n", "\r" },
                                NumberStyles.HexNumber);

                        try
                        {
                            //TODO(PPavlov): Figure out a way to remove duplicate event handlers
                            Injector.Instance
                                  .Get<ISocketManager>().Send(new SocketContext()
                                  {
                                      Buffer = this._model.Data,
                                      EndPoint = this._model.EndPoint
                                  });

                            this.History.Add(new HistoryModel()
                            {
                                Address = this.Address,
                                Port = this.Port,
                                Data = this.Data,
                                DataHexDisplay = this.DataHexDisplay
                            });
                        }
                        catch (Exception ex)
                        {
                            this.OutputHexDisplay = ex.Message;
                        }
                    });
                }

                return this._sendCommand;
            }
        }

        public ICommand ConnectCommand
        {
            get
            {
                if (this._connectCommand == null)
                {
                    this._connectCommand = new RelayCommand((x) =>
                    {
                        //TODO(PPavlov): Create Converter
                        this._model.EndPoint = new IPEndPoint(
                            IPAddress.Parse(Address),
                            Convert.ToInt32(Port));

                        Injector.Instance
                            .Register<ISocketManager>(
                                    new TCPClientSocketManager().Initialize(this._model.EndPoint).Start());

                        Injector.Instance
                           .Get<ISocketManager>()
                              .Subscribe(EventType.RECEIVE, (snd, ctx) =>
                               {
                                   this.OutputHexDisplay = Injector.Instance
                                           .Get<IDumper>()
                                           .Dump(ctx.Buffer);
                               });
                    });
                }

                return this._connectCommand;
            }
        }

        public ICommand RestoreCommand
        {
            get
            {
                if (this._restoreCommand == null)
                {
                    this._restoreCommand = new RelayCommand((param) =>
                    {
                        if (this.SelectedHistoryModel == null)
                        {
                            return;
                        }

                        this.Address = this.SelectedHistoryModel.Address;
                        this.Port = this.SelectedHistoryModel.Port;
                        this.Data = this.SelectedHistoryModel.Data;
                        this.OutputHexDisplay = String.Empty;

                        //TODO(PPavlov): Create Converter
                        this._model.EndPoint = new IPEndPoint(
                            IPAddress.Parse(Address),
                            Convert.ToInt32(Port));
                    });
                }

                return this._restoreCommand;
            }
        }
    }
}
