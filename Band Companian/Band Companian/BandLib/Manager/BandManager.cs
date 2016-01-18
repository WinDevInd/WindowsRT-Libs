using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Band;
using BandLibs.Manager;

namespace BandLib
{
    public class BandInfo
    {
        public string HardwareVersion { get; set; }
        public string FirmwareVersion { get; set; }
        public string BandName { get; set; }
        public BandConnectionType ConnectionType { get; set; }
    }

    public enum SensorType
    {
        Heart,
        GSR,
        Acclerometer,
        Gyro,
        Distance,
        UV,
        Barometer,
        Alimeter,
        Contact,
        AmbientLight,
        SkinTempreture
    }
    public class BandManager:IDisposable
    {
        private BandClientManager _client;
        private IBandInfo _ConnectedBands;
        private IBandClient bandClient;
        public bool HandleException { get; set; }
        private ICallbackReceiverInterface _Subscriber;

        public static BandManager Instance { get; private set; }

        static BandManager()
        {
            Instance = new BandManager();
        }

        private BandManager()
        {
        }

        public async Task<BandInfo> Connect(ICallbackReceiverInterface subscriber)
        {
            try
            {
                _ConnectedBands = (await BandClientManager.Instance.GetBandsAsync())[0];
                bandClient = await BandClientManager.Instance.ConnectAsync(_ConnectedBands);
                _Subscriber = subscriber;
                BandInfo bandInfo = new BandInfo();
                bandInfo.BandName = _ConnectedBands.Name;
                bandInfo.ConnectionType = _ConnectedBands.ConnectionType;
                bandInfo.FirmwareVersion = await bandClient.GetFirmwareVersionAsync();
                bandInfo.HardwareVersion = await bandClient.GetHardwareVersionAsync();
                return bandInfo;
            }
            catch (BandException e)
            {
                if (!HandleException)
                {
                    throw e;
                }
                return null;
            }
        }

        public async Task<bool> RequestAndSubscribeSensor(SensorType sensorType)
        {
            try
            {
                switch (sensorType)
                {
                    case SensorType.Heart:
                        return await SubscribeHeartSensor();
                    default:
                        return false;
                }
            }
            catch (BandException e)
            {
                if (!HandleException)
                {
                    throw e;
                }
                return false;
            }
            catch (Exception e)
            {
                if (!HandleException)
                {
                    throw e;
                }
                return false;
            }

        }

        public IEnumerable<TimeSpan> GetInterval(SensorType sensorType)
        {
            switch (sensorType)
            {
                case SensorType.Heart:
                    return bandClient.SensorManager.HeartRate.SupportedReportingIntervals;
                default: return null;
            }
        }

        private async Task<bool> SubscribeHeartSensor()
        {
            if (bandClient.SensorManager.HeartRate.IsSupported)
            {
                bool allowed = bandClient.SensorManager.HeartRate.GetCurrentUserConsent() != UserConsent.Granted ?
                    await bandClient.SensorManager.HeartRate.RequestUserConsentAsync() : true;

                if (_Subscriber != null && bandClient.SensorManager.HeartRate.GetCurrentUserConsent() == UserConsent.Granted)
                {
                    bandClient.SensorManager.HeartRate.ReadingChanged += HeartRate_ReadingChanged;
                    CallbackManager.AssignCallback(SensorType.Heart.ToString(), _Subscriber);
                    await bandClient.SensorManager.HeartRate.StartReadingsAsync();
                    return true;
                }
                return false;
            }
            throw new Exception("Sensor not Supported");
        }

        public async Task SetInterval(SensorType sensorType, TimeSpan interval)
        {
            switch (sensorType)
            {
                case SensorType.Heart:
                    await bandClient.SensorManager.HeartRate.StopReadingsAsync();
                    bandClient.SensorManager.HeartRate.ReportingInterval = interval;
                    await bandClient.SensorManager.HeartRate.StartReadingsAsync();
                    break;
                default:
                    break;
            }
        }

        private void HeartRate_ReadingChanged(object sender, Microsoft.Band.Sensors.BandSensorReadingEventArgs<Microsoft.Band.Sensors.IBandHeartRateReading> e)
        {
            CallbackManager.GiveCallbacks(SensorType.Heart.ToString(), e, new string[] { });            
        }

        public async Task CreateTile()
        {

        }

        public void Dispose()
        {
            bandClient.SensorManager.HeartRate.StopReadingsAsync();
            bandClient.SensorManager.HeartRate.ReadingChanged -= HeartRate_ReadingChanged;
        }


    }
}
