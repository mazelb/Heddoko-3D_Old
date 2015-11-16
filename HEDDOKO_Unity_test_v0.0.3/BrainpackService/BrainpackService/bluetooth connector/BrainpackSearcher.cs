using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BrainpackService.Tools_and_Utilities;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;

namespace BrainpackService.bluetooth_connector
{
    /**
  * BrainpackSearcher class 
  * @brief Represents a bluetooth poller, whose responsiblity is to search for bluetooth devices,  filter them as Brainpacks. 
  * member variable gsFilter
  * @note something 
  */

    public class BrainpackSearcher
    {
        private BluetoothAddress mBTAddress; //the selected bluetooth address
         // public static ConcurrentBag<BluetoothDeviceInfo> sgaFilteredBrainpackDevices = new ConcurrentBag<BluetoothDeviceInfo>(); //the list of brainpack device infos, will be filtered in the constructor
        public static ConcurrentDictionary<BluetoothAddress, BluetoothDeviceInfo> gsFiltedBrainpackMapping = new ConcurrentDictionary<BluetoothAddress, BluetoothDeviceInfo>();
        public static ConcurrentDictionary<BluetoothAddress, BluetoothDeviceInfo> gsPairedBrainpackMapping = new ConcurrentDictionary<BluetoothAddress, BluetoothDeviceInfo>();
        public static BluetoothDeviceInfo[] FilteredBluetoothDevices => gsFiltedBrainpackMapping.Values.ToArray();
        private long mMaxTimer = 5000;
        private short mMaxTries = 20;
        private Timer mTimer;
        BluetoothClient mClient = new BluetoothClient();
        private int mMaxNumberOfDevices = 15;
        static readonly Guid sBrainpackServiceType = BluetoothService.SerialPort;
        const string DEVICE_PIN = "1234";
        const string gsFilter = "Heddoko";

        /**
        * Init() 
        * @brief Initializes the BrainPackBlueToothConnector 
        */
        public void Init()
        {
            AutoResetEvent vAutoEvent = new AutoResetEvent(false);
            TimerCallback vTimerCallback = SearchForDevices;
            mTimer = new Timer(vTimerCallback, vAutoEvent, 0, mMaxTimer);

        }
        /**
        * Stop() 
        * @brief Stops the working timer thread
        */
        public void Stop()
        {
            mTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }
        /**
        *  SearchForDevices(object vStateInfo) 
        * @brief searches for devices and applies a filter to discriminate between non heddoko BT devices and 
        * heddoko bt devices
        * 
        */
        private void SearchForDevices(object vStateInfo)
        {
            List<BluetoothDeviceInfo> vaDeviceinfos = FilterDevices(mClient.DiscoverDevices(15), gsFilter);
            //first try to remove any non existing devices that are present in the filtered list
            //check if already exists in the dictionary
            if (vaDeviceinfos.Count != 0)
            {
                //if the device no longer exists in the filtered list, but still exists in the dictionary remove it
                List<BluetoothAddress> vToBeRemoved = new List<BluetoothAddress>();
                foreach (var vDeviceKey in gsFiltedBrainpackMapping)
                {
                    //check if the key exists in the filtered array
                    if (!vaDeviceinfos.Exists(x => x.DeviceAddress == vDeviceKey.Key))
                    {
                        //add the device in the to be removed list
                        vToBeRemoved.Add(vDeviceKey.Key);

                    }
                }
                //remove items from the filtered device info map
                CleanUp(vToBeRemoved);
                //now that the filteredBrainpackMapping is trimmed, search for any devices that doesn't exist from the list 
                for (int i = 0; i < vaDeviceinfos.Count; i++)
                {
                    BluetoothAddress vKeyAddress = vaDeviceinfos[i].DeviceAddress;
                    if (!gsFiltedBrainpackMapping.ContainsKey(vKeyAddress))
                    {
                        gsFiltedBrainpackMapping.TryAdd(vKeyAddress, vaDeviceinfos[i]);
                    }
                }
            }
        }

        private void CleanUp(List<BluetoothAddress> vaToBeRemoved)
        {
            if (vaToBeRemoved == null || vaToBeRemoved.Count == 0)
            {
                return;
            }
            else
            {
                for (int i = 0; i < vaToBeRemoved.Count; i++)
                {
                    BluetoothAddress vKey = vaToBeRemoved[i];
                    //placeholder for try removes
                    BluetoothDeviceInfo vIngnoredValue1;
                    BluetoothDeviceInfo vIngnoredValue2;
                    gsFiltedBrainpackMapping.TryRemove(vKey, out vIngnoredValue1);
                    gsPairedBrainpackMapping.TryRemove(vKey, out vIngnoredValue2);
                }
            }
        }

        /**
       * FilterDevices(BluetoothDeviceInfo[] vDevicesToBeFiltered, string vFilterCondition) 
       * @brief Filters the devices by the given filtercondition. Will return an array of heddoko brain packs 
       */
        private List<BluetoothDeviceInfo> FilterDevices(BluetoothDeviceInfo[] vDevicesToBeFiltered, string vFilterCondition)
        {
            List<BluetoothDeviceInfo> vBtDevices = new List<BluetoothDeviceInfo>();


            foreach (BluetoothDeviceInfo vBtInfo in vDevicesToBeFiltered)
            {
                if (vBtInfo.DeviceName.Contains(vFilterCondition))
                {
                    vBtDevices.Add(vBtInfo);
                }
            }
            return vBtDevices;
        }
        /**
        * ConnectToDevice(BluetoothDeviceInfo vDeviceInfo)
        * @brief  Attempts to connect to the supplied device and receive stream info 
        * @param object args: the parameters necessary for this
        * function to perform
        * @note Please not that this will throw an exception if
        * y requirements are not met with the given parameter
        * @return returns an arbitrary  
        */
   /*     private static BluetoothDeviceInfo ConnectToDevice(BluetoothDeviceInfo vDeviceInfo)
        {
            BluetoothEndPoint vRemoteEndPoint = new BluetoothEndPoint(vDeviceInfo.DeviceAddress, sBrainpackServiceType);
            //check if device is already paired and within range
            BluetoothAddress vBPDeviceAddress = vDeviceInfo.DeviceAddress; //brainpacks device address
            if (gsPairedBrainpackMapping.ContainsKey(vBPDeviceAddress))
            {
                return gsPairedBrainpackMapping[vBPDeviceAddress];
            }
            else
            {

            }

        }*/
        /**
      * BluetoothDeviceInfo(BluetoothAddress vDeviceAddress)
      * @brief Returns the device info associated with the device address
      * @param  vDeviceAddress:the device address 
      * @returns the device info associated with the address 
      */
        public static BluetoothDeviceInfo GetDeviceInfo(BluetoothAddress vDeviceAddress)
        {
            BluetoothDeviceInfo vDeviceInfo = null;
            gsPairedBrainpackMapping.TryGetValue(vDeviceAddress, out vDeviceInfo);
            return vDeviceInfo;
        }
        /**
        * ContainsKeyInPairedMap(BluetoothAddress vDeviceAddress)
        * @brief Returns if the Brainpack paired map contains the vAddress key
        * @param  vDeviceAddress:the device address 
        * @returns  Returns if the Brainpackmap contains the vAddress key
        */
        public static bool ContainsKeyInPairedMap(BluetoothAddress vAddress)
        {
            return gsPairedBrainpackMapping.ContainsKey(vAddress);
        }
        /**
        * PairDevice(BluetoothDeviceInfo vDeviceAddress)
        * @brief Attempts to pair the device and add it to the list of paired devices
        * @param  vDeviceAddress:the device address 
        * @returns if the device was paired successfully
        * @note : an event log error will be produced if the device failed to connect due to a bad pin

        */
        public static bool PairDevice(BluetoothAddress vDeviceAddress)
        {

            //check if device exists in the gsPairedBrainpackMapping map
            if (gsPairedBrainpackMapping.ContainsKey(vDeviceAddress))
            {
                return true;
            }
            //otherwise add it to the gsPairedBrainpackMapping
            else
            {
                //get the device information from gsFiltedBrainpackMapping
                if (gsFiltedBrainpackMapping.ContainsKey(vDeviceAddress))
                {
                    BluetoothDeviceInfo vDeviceInfo = gsFiltedBrainpackMapping[vDeviceAddress];
                    if (!vDeviceInfo.Authenticated)
                    {
                        if (!BluetoothSecurity.PairRequest(vDeviceAddress, DEVICE_PIN))
                        {
                            BrainpackEventLogManager.InvokeEventLogError("Brainpack " + vDeviceAddress + " failed pairing");
                            return false;
                        }
 
                    }
                    gsPairedBrainpackMapping.TryAdd(vDeviceAddress, vDeviceInfo);
                }

            }
            return true;

        }




    }

}
