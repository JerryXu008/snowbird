using System.Collections.Generic;
using static System.String;

namespace AutoTestSystem.Model
{
    public class Station
    {
        public string serial = "";
        public string station_name = "";
        public string station_type = "";
        public string position = "A1";
        public string start_time = "";
        public string finish_time = "";
        public string status = "canceled";
        public string mode = "";
        public string error_code = "";
        public string error_details = "";
        //public string luxshare_qsdk_version = "";
        public string test_software_version = "";
        public List<phase_items> tests = new List<phase_items>();

        public Station(string _sn, string _stationNo, string _station_name, string startTime, string _mode, string _QSDKVER, string swVer)
        {
            serial = _sn;
            station_name = _stationNo;
            station_type = _station_name;
            start_time = startTime;
            mode = _mode;
            //luxshare_qsdk_version = _QSDKVER;
            test_software_version = swVer;  //测试软件版本。
        }

        public void CopyToMES(MesPhases mesPhases)
        {
            mesPhases.serial = serial;
            if (status == "passed")
                mesPhases.status = "PASS";
            else if (status == "failed")
                mesPhases.status = "FAIL";
            mesPhases.start_time = start_time;
            mesPhases.finish_time = finish_time;
            mesPhases.error_code = error_code;
            mesPhases.error_details = error_details;
            mesPhases.test_station = station_name;
            mesPhases.test_software_version = test_software_version;
            mesPhases.mode = mode;
        }
    }

    ///sequences
    public class test_phases
    {
        public string phase_name = "";
        public string status = "canceled";
        public string start_time = "";
        public string finish_time = "";
        public string phase_details = "";
        public string error_code = "";
        public List<phase_items> phase_items = new List<phase_items>();

        public void Copy(Sequence sequence, MainForm mainFrom)
        {
            phase_name = sequence.SeqName;
            status = sequence.TestResult ? "passed" : "failed";
            start_time = sequence.start_time;
            finish_time = sequence.finish_time;
            //error_code = MainForm.error_code;
        }
    }

    //testItem
    public class phase_items
    {
        public string test_name = null;
        public string test_value = null;
        public string units = null;
        public string status = null;
        public string error_code;
        public string start_time;
        public string finish_time;
        public string lower_limit = null;
        public string spec = null;
        public string upper_limit = null;

        public string device = null;
        public string test = null;
        public string speed = null;
        public string voltage = null;
        public string serial = null;
        public string mac = null;
        public string nor_version = null;
        public string board_register_value = null;
        public string version = null;
        public string model = null;
        public string radio = null;
        public string chain = null;
        public string frequency = null;
        public string measured_power = null;
        public string absolute_power = null;
        public string path_loss = null;
        public string rx_power = null;
        public string per = null;
        public string delta_f0_fn_max = null;
        public string delta_f1_f0 = null;
        public string delta_f1_avg = null;
        public string delta_f2_avg = null;
        public string delta_f2_max = null;
        public string delta_fn_fn5_max = null;
        public string fn_max = null;
        public string ini_freq_error = null;
        public string per_test_power = null;
        public string power = null;
        public string power_spec = null;
        public string ratio_of_f2_to_f1 = null;
        public string rx_per = null;
        public string data_rate = null;
        public string evm = null;
        public string freq_error = null;
        public string lo_leakage = null;
        public string power_accuracy = null;
        public string spectral_flatness = null;
        public string spectrum_mask = null;
        public string sym_clk_error = null;
        public string goal_power;
        public string gain;
        public string led;
        public string performance;
        public string iperf_cmd;


        /// <summary>
        ///  复制部分值到目标测试项中 items--》phase_items
        /// </summary>
        public void CopyFrom(bool flag, Items testItem, bool tResult)
        {
            test_name = testItem.EeroName;
            test_value = testItem.testValue;
            units = testItem.Unit == null ? "" : testItem.Unit;
            if (test_name == "LED_W_LUM_CORRECTED" || test_name == "LED_W_LUM")
                status = tResult ? "passed" : "failed";
            else
                status = testItem.tResult ? "passed" : "failed";
            error_code = testItem.error_code;
            start_time = testItem.start_time_json.ToString("yyyy-MM-dd HH:mm:ss");
            if (start_time == "0001-01-01 00:00:00" || start_time=="" || start_time == null) { 
               start_time= System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
            
            finish_time = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            upper_limit = testItem.Limit_max == null ? "" : testItem.Limit_max;
            lower_limit = testItem.Limit_min == null ? "" : testItem.Limit_min;
            
            //这个地方这么写着实有些奇怪...
            if (!flag && IsNullOrEmpty(testItem.Limit_min))
            {
                lower_limit = testItem.Spec;


                if (testItem.EeroName == "ENTER_KERNEL")
                {
                    if (lower_limit == null || lower_limit == "")
                    {
                        lower_limit = "root@OpenWrt:/#";
                    }

                }
            }
        }
    }

    public class MesPhases
    {
        public string serial = "";
        public string start_time = "";
        public string finish_time = "";
        public string status = "";
        public string mode = "";
        public string error_code = "";
        public string error_details = "";
        public string test_station = "";
        public string IP;
        public string NO;
        public string FIRST_FAIL;
        public string test_software_version = "";
        /// <summary>
        /// 测试大项名字和测试时间
        /// </summary>
        public string VerifySFIS = null;

        public string CurrentShortTest = null;
        public string CurrentTest = null;
        public string VoltageTest = null;
        public string EnterUBootTransition = null;
        public string ThermalShutdownTest = null;
        public string ReadBoardRegisterValue = null;
        public string ReadAndUpdateLuxQsdkVersion = null;
        public string SaveIdentityInEnv = null;
        public string CPUVersionTest = null;
        public string CheckBZTBootloaderVersion = null;
        public string SubsystemTest = null;
        public string MMCReadWriteSpeedTest = null;
        public string USBReadWriteTest = null;
        public string ResetButtonTest = null;
        public string LEDFunctionTest = null;
        public string EthernetFunctionTest = null;
        public string CheckArtPartition = null;
        public string SpruceCanCommunicateTest = null;

        public string VerifySFIS_Time = null;
        public string CurrentShortTest_Time = null;
        public string CurrentTest_Time = null;
        public string VoltageTest_Time = null;
        public string EnterUBootTransition_Time = null;
        public string ThermalShutdownTest_Time = null;

        public string ReadBoardRegisterValue_Time = null;
        public string ReadAndUpdateLuxQsdkVersion_Time = null;
        public string SaveIdentityInEnv_Time = null;
        public string CPUVersionTest_Time = null;
        public string CheckBZTBootloaderVersion_Time = null;
        public string SubsystemTest_Time = null;
        public string MMCReadWriteSpeedTest_Time = null;
        public string USBReadWriteTest_Time = null;
        public string ResetButtonTest_Time = null;
        public string LEDFunctionTest_Time = null;
        public string EthernetFunctionTest_Time = null;
        public string CheckArtPartition_Time = null;
        public string SpruceCanCommunicateTest_Time = null;

        public string VerifyDUT = null;
        public string ThermalShutdownCheck = null;
        public string RadioValidation = null;
        public string RadioCalibration = null;
        public string RadioCalibration_5G = null;
        public string RadioValidation_5G = null;
        public string RadioCalibration_2G = null;
        public string RadioValidation_2G = null;

        public string VerifyDUT_Time = null;
        public string ThermalShutdownCheck_Time = null;
        public string RadioValidation_Time = null;
        public string RadioCalibration_Time = null;
        public string RadioCalibration_5G_Time = null;
        public string RadioValidation_5G_Time = null;
        public string RadioCalibration_2G_Time = null;
        public string RadioValidation_2G_Time = null;

        public string OpenShortCurrentTest = null;
        public string ReportChildBoard = null;
        public string LEDIrradianceTest = null;
        public string EthernetSpeedTest = null;

        public string OpenShortCurrentTest_Time = null;
        public string ReportChildBoard_Time = null;
        public string LEDIrradianceTest_Time = null;
        public string EthernetSpeedTest_Time = null;

        public string TemperatureResult_AfterBoot = null;
        public string WiFiTransmitPowerTest = null;
        public string TempSensorTest_AfterWiFiTXPowerTest = null;
        public string DesenseTest_WiFi = null;
        public string TempSensorTest_AfterDesenseWiFiTest = null;
        public string BluetoothValidation = null;
        public string RadioValidation_Zigbee = null;
        public string SetIPAddress = null;

        public string TemperatureResult_AfterBoot_Time = null;
        public string WiFiTransmitPowerTest_Time = null;
        public string TempSensorTest_AfterWiFiTXPowerTest_Time = null;
        public string DesenseTest_WiFi_Time = null;
        public string TempSensorTest_AfterDesenseWiFiTest_Time = null;
        public string BluetoothValidation_Time = null;
        public string RadioValidation_Zigbee_Time = null;
        public string SetIPAddress_Time = null;

        public string TempSensorTest_AfterBoot = null;
        public string LoadBZTFirmware = null;
        public string WiFiSpeedTest = null;
        public string TempSensorTest_AfterWiFiSpeedTest = null;
        public string BluetoothFunctionTest = null;
        public string ZigbeeFunctionTest = null;
        public string SetBootcmdtoDHCP = null;

        public string TempSensorTest_AfterBoot_Time = null;
        public string LoadBZTFirmware_Time = null;
        public string WiFiSpeedTest_Time = null;
        public string TempSensorTest_AfterWiFiSpeedTest_Time = null;
        public string BluetoothFunctionTest_Time = null;
        public string ZigbeeFunctionTest_Time = null;
        public string SetBootcmdtoDHCP_Time = null;

        public string ReadBTZFwVersion = null;
        public string PowerCycleTest = null;
        public string WaitForTelnet = null;
        public string RAMStressTest = null;
        public string CPUStressTest = null;
        public string TempSensorTest_AfterCPUStressTest = null;
        public string MMCStressTest = null;
        public string HardwareReset = null;
        public string TempSensorTest_AfterMMCReadWriteSpeedTest = null;
        public string JSON_UPLOAD = null;

        public string ReadBTZFwVersion_Time = null;
        public string PowerCycleTest_Time = null;
        public string WaitForTelnet_Time = null;
        public string RAMStressTest_Time = null;
        public string CPUStressTest_Time = null;
        public string TempSensorTest_AfterCPUStressTest_Time = null;
        public string MMCStressTest_Time = null;
        public string HardwareReset_Time = null;
        public string TempSensorTest_AfterMMCReadWriteSpeedTest_Time = null;
        public string JSON_UPLOAD_Time = null;

        //MBLT 电压电流
        public string I_USBC_VBUS_OPEN = null;
        public string I_USBC_VBUS_UBOOTIDLE = null;
        public string V_USBC_VBUS = null;
        public string V_USBC_TP1213_DVDD5 = null;
        public string V_POE_POWER_RAIL = null;
        public string V_TP1205_POE_5V = null;
        public string V_TP1213_DVDD5 = null;
        public string V_TP10_DVDD3_3= null;
        public string V_TP14_DVDD1_95= null;
        public string V_TP17_DVDD2_2= null;
        public string V_TP19_DVDD0_912= null;
        public string V_TP26_DVDD1_29_MP_CORE= null;
        public string V_TP37_DVDD1_35_MP_DDR= null;
        public string V_TP55_STBY_VDD3_3= null;
        public string V_TP503_VDD1_8_NAPA= null;
        public string V_TP504_VDD1_05_NAPA= null;
        /// <summary>
        /// 测试参数上传
        /// </summary>
        public string Ledoff = null;

        public string W_x = null;
        public string W_y = null;
        public string W_L = null;
        public string B_x = null;
        public string B_y = null;
        public string B_L = null;
        public string G_x = null;
        public string G_y = null;
        public string G_L = null;
        public string R_x = null;
        public string R_y = null;
        public string R_L = null;

        public string USBC_VBUS = null;
        public string GND = null;
        public string DVDD3_3 = null;
        public string DVDD1_95 = null;
        public string DVDD2_2 = null;
        public string DVDD0_912 = null;
        public string DVDD1_29_MP_CORE = null;
        public string DVDD1_35_MP_DDR = null;

        /// <summary>
        ///  MBLT
        /// </summary>
        public string FW_VERSION = null;

        public string HW_REVISION = null;
        public string SW_VERSION = null;
        public string CURRENT_OPEN = null;
        public string CURRENT_IDLE = null;
        public string EMMC_VENDOR = null;
        public string MMCWrite117 = null;
        public string MMCWrite120 = null;
        public string MMCRead017 = null;
        public string MMCRead020 = null;
        public string LED_R_ON = null;
        public string LED_B_ON = null;
        public string LED_G_ON = null;
        public string LED_W_ON = null;
        public string ETH0_THROUGHPUT = null;
        public string ETH1_THROUGHPUT = null;
        public string CURRENT_SHORT;
        public string LED_ALLOFF;
        public string Temp_AfterBoot;
        public string LoadWiFiDrivers;
        public string Temp_AfterWiFiSpeedTest;
        public string SetBootcmdToDHCP;
        public string USB_WRITE_SPEED;
        public string USB_READ_SPEED;
        public string WIFI2G_THROUGHPUT_SERIAL;
        public string WIFI5G_THROUGHPUT_SERIAL;
        public string WIFI2G_THROUGHPUT_PARALLEL;
        public string WIFI5G_THROUGHPUT_PARALLEL;
        public string MES_UPLOAD;
        public string ETH0_THROUGHPUT_SEND;
        public string ETH0_THROUGHPUT_RECEIVE;
        public string ETH1_THROUGHPUT_SEND;
        public string ETH1_THROUGHPUT_RECEIVE;
        public string LoadBZTFDrivers;
        public string LoadBZTFDrivers_Times;
        public string LED_MODEL;
        public string PHY_STATUS;
        public string FUSB_STATUS;
        public string LED_OFF_LUM;
        public string ChildBoardSN;
        public string WIFI5G_THROUGHPUT_PARALLEL_TX;
        public string WIFI2G_THROUGHPUT_PARALLEL_TX;
        public string WIFI5G_THROUGHPUT_PARALLEL_RX;
        public string WIFI2G_THROUGHPUT_PARALLEL_RX;
        public string ETH0_SPEED_RX;
        public string ETH1_SPEED_RX;
        public string ETH0_SPEED_TX;
        public string ETH1_SPEED_TX;
        public string LED_W_X_CORRECTED;
        public string LED_W_Y_CORRECTED;
        public string LED_W_LUM_CORRECTED;
        public string TEMP_WIFI_SPEED_RADIO_2G;
        public string TEMP_WIFI_SPEED_RADIO_5G;
        public string TEMP_LOAD_WIFI_RADIO_2G;
        public string TEMP_LOAD_WIFI_RADIO_5G;


        public string SW_REVISION;
        public string WriteMAC;
        public string NODEPROPERTY_MAC;
        public string WriteSN;
        public string SETENV_SN;
        public string CPU_VERSION;


        public string BZT_BOOTLOADER_VERSION;
     



        public string LED_R_Set;
        public string LED_R_R;
        public string LED_R_G;
        public string LED_R_B;


        public string LED_G_Set;

        public string LED_G_R;
        public string LED_G_G;
        public string LED_G_B;


        public string LED_B_Set;

        public string LED_B_R;
        public string LED_B_G;
        public string LED_B_B;


        public string LED_W_Set;

        public string LED_W_R;
        public string LED_W_G;
        public string LED_W_B;



    
        public string ENTER_UBOOT;

        
        public string V_TP27_VDD_SOC_CX;
        public string V_TP28_VAA_0P8;
        public string V_TP29_VDD_SOC_MX;
        public string V_TP30_VDD_PCIE_1P8;
        public string V_TP31_VDD_1V8_PX3;
        public string V_TP34_VDD_PCIE_0P925;
        public string V_TP36_VAA_1P2;
        public string V_TP38_USBC_VBUS;
        public string V_TP503_VDD1_8_PHY;
public string V_TP535_USBC_VBUS_SWITCHED; 
public string V_TP569_VDD_XPA_Qorvo;
        public string V_TP573_DVDD3_3;
        public string V_TP574_VDD_DDR;
        public string V_TP576_VDD_LDO_2P5_VPP;
        public string V_TP577_AVDD3_3_2G;
public string V_TP578_VDD1V95_PMU;
        public string V_TP579_VDD_CX;
        public string V_TP581_VDD3_3_PHY;
public string V_TP582_VAA_1P8;
        public string V_TP583_VDD0_95_PHY;
public string V_TP586_TSD_OTN;
        public string V_TP589_DVDD3_3_BZT;
public string V_TP590_LED_5V_Qorvo;
        public string V_TP580_MA_VDD_CX_VCC;


        public string V_TP573_DVDD3_3_TSDOn;





         
        public string FW_REVISION;









        public string ETH0_DATA_RATE;
        public string ETH1_DATA_RATE;






        





        //扫码时间
        public string AT_SCAN_TIME;
        public string AT_SCAN_SATRT_TIME;
        public string AT_SCAN_END_TIME;

        //SRF
        public string WIFI_TX_POWER_F5180_BW20_MCS0_C0;
        public string WIFI_TX_POWER_F5180_BW20_MCS0_C1;
        public string WIFI_TX_POWER_F5540_BW20_MCS0_C0;
        public string WIFI_TX_POWER_F5540_BW20_MCS0_C1;
        public string WIFI_TX_POWER_F5885_BW20_MCS0_C0;
        public string WIFI_TX_POWER_F5885_BW20_MCS0_C1;
        public string WIFI_TX_POWER_F5530_BW80_MCS0_C0;
        public string WIFI_TX_POWER_F5530_BW80_MCS0_C1;
        public string WIFI_TX_POWER_F5570_BW160_MCS0_C0;
        public string WIFI_TX_POWER_F5570_BW160_MCS0_C1;
        public string WIFI_TX_POWER_F2412_BW20_MCS0_C0;
        public string WIFI_TX_POWER_F2412_BW20_MCS0_C1;
        public string WIFI_TX_POWER_F2437_BW20_MCS0_C0;
        public string WIFI_TX_POWER_F2437_BW20_MCS0_C1;
        public string WIFI_TX_POWER_F2472_BW20_MCS0_C0;
        public string WIFI_TX_POWER_F2472_BW20_MCS0_C1;
        public string BLE_TX_POWER_F2402;
        public string BLE_TX_POWER_F2480;
        public string ZB_TX_POWER_F2405;

        //MBFT
      
        public string WIFI_TX_POWER_F5180_BW20_MCS7_C0;
        public string WIFI_TX_POWER_F5180_BW20_MCS7_C1;
        public string WIFI_TX_POWER_F5500_BW20_MCS7_C0;
        public string WIFI_TX_POWER_F5500_BW20_MCS7_C1;
        public string WIFI_TX_POWER_F5885_BW20_MCS7_C0;
        public string WIFI_TX_POWER_F5885_BW20_MCS7_C1;
        public string WIFI_TX_POWER_F5250_BW160_MCS11_C0;
        public string WIFI_TX_POWER_F5250_BW160_MCS11_C1;
        public string WIFI_TX_POWER_F5570_BW160_MCS11_C0;
        public string WIFI_TX_POWER_F5570_BW160_MCS11_C1;
        public string WIFI_TX_POWER_F5815_BW160_MCS11_C0;
        public string WIFI_TX_POWER_F5815_BW160_MCS11_C1;
        public string WIFI_TX_EVM_F5180_BW20_MCS0_C0;
        public string WIFI_TX_EVM_F5180_BW20_MCS0_C1;
        public string WIFI_TX_EVM_F5540_BW20_MCS0_C0;
        public string WIFI_TX_EVM_F5540_BW20_MCS0_C1;
        public string WIFI_TX_EVM_F5885_BW20_MCS0_C0;
        public string WIFI_TX_EVM_F5885_BW20_MCS0_C1;
        public string WIFI_TX_EVM_F5180_BW20_MCS7_C0;
        public string WIFI_TX_EVM_F5180_BW20_MCS7_C1;
        public string WIFI_TX_EVM_F5500_BW20_MCS7_C0;
        public string WIFI_TX_EVM_F5500_BW20_MCS7_C1;
        public string WIFI_TX_EVM_F5885_BW20_MCS7_C0;
        public string WIFI_TX_EVM_F5885_BW20_MCS7_C1;
        public string WIFI_TX_EVM_F5250_BW160_MCS11_C0;
        public string WIFI_TX_EVM_F5250_BW160_MCS11_C1;
        public string WIFI_TX_EVM_F5570_BW160_MCS11_C0;
        public string WIFI_TX_EVM_F5570_BW160_MCS11_C1;
        public string WIFI_TX_EVM_F5815_BW160_MCS11_C0;
        public string WIFI_TX_EVM_F5815_BW160_MCS11_C1;

        
        public string WIFI_TX_POWER_F2472_BW20_MCS7_C0;
        public string WIFI_TX_POWER_F2472_BW20_MCS7_C1;
        public string WIFI_TX_POWER_F2462_BW40_MCS11_C0;
        public string WIFI_TX_POWER_F2462_BW40_MCS11_C1;
        public string WIFI_TX_EVM_F2412_BW20_MCS0_C1;
        public string WIFI_TX_EVM_F2437_BW20_MCS0_C0;
        public string WIFI_TX_EVM_F2437_BW20_MCS0_C1;
        public string WIFI_TX_EVM_F2472_BW20_MCS0_C0;
        public string WIFI_TX_EVM_F2472_BW20_MCS0_C1;
        public string WIFI_TX_EVM_F2472_BW20_MCS7_C0;
        public string WIFI_TX_EVM_F2472_BW20_MCS7_C1;
        public string WIFI_TX_EVM_F2462_BW40_MCS11_C0;
        public string WIFI_TX_EVM_F2462_BW40_MCS11_C1;
        public string WIFI_TX_TEMP_F2462_BW40_MCS11_C0;
        public string WIFI_TX_TEMP_F2412_BW20_MCS0_C0;
        public string WIFI_TX_TEMP_F2437_BW20_MCS0_C0;
        public string WIFI_TX_TEMP_F2472_BW20_MCS0_C0;
        public string WIFI_TX_TEMP_F2472_BW20_MCS7_C0;
        public string TotalTimeExceptScanAndPopFixutre;



        public string WIFI_TX_POWER_F5180_HE_BW20_MCS0_C0;
        public string WIFI_TX_POWER_F5180_HE_BW20_MCS0_C1;
        public string WIFI_TX_POWER_F5500_HE_BW20_MCS0_C0;
        public string WIFI_TX_POWER_F5500_HE_BW20_MCS0_C1;
        public string WIFI_TX_POWER_F5805_HE_BW20_MCS0_C0;
        public string WIFI_TX_POWER_F5805_HE_BW20_MCS0_C1;
        public string WIFI_TX_POWER_F5180_HE_BW20_MCS7_C0;
        public string WIFI_TX_POWER_F5180_HE_BW20_MCS7_C1;
        public string WIFI_TX_POWER_F5500_HE_BW20_MCS7_C0;
        public string WIFI_TX_POWER_F5500_HE_BW20_MCS7_C1;
        public string WIFI_TX_POWER_F5805_HE_BW20_MCS7_C0;
        public string WIFI_TX_POWER_F5805_HE_BW20_MCS7_C1;
        public string WIFI_TX_POWER_F5775_HE_BW80_MCS9_C0;
        public string WIFI_TX_POWER_F5775_HE_BW80_MCS9_C1;
        public string WIFI_TX_POWER_F5570_HE_BW160_MCS11_C0;
        public string WIFI_TX_POWER_F5570_HE_BW160_MCS11_C1;
        public string WIFI_TX_POWER_F5250_EHT_BW160_MCS13_C0;
        public string WIFI_TX_POWER_F5250_EHT_BW160_MCS13_C1;

        public string WIFI_TX_POWER_F5955_HE_BW20_MCS0_C0;
        public string WIFI_TX_POWER_F5955_HE_BW20_MCS0_C1;
        public string WIFI_TX_POWER_F6595_HE_BW20_MCS0_C0;
        public string WIFI_TX_POWER_F6595_HE_BW20_MCS0_C1;
        public string WIFI_TX_POWER_F7115_HE_BW20_MCS0_C0;
        public string WIFI_TX_POWER_F7115_HE_BW20_MCS0_C1;
        public string WIFI_TX_POWER_F6345_HE_BW160_MCS11_C0;
        public string WIFI_TX_POWER_F6345_HE_BW160_MCS11_C1;
        public string WIFI_TX_POWER_F6665_HE_BW160_MCS11_C0;
        public string WIFI_TX_POWER_F6665_HE_BW160_MCS11_C1;
        public string WIFI_TX_POWER_F6985_HE_BW160_MCS11_C0;
        public string WIFI_TX_POWER_F6985_HE_BW160_MCS11_C1;
        public string WIFI_TX_POWER_F6105_EHT_BW320_MCS13_C0;
        public string WIFI_TX_POWER_F6105_EHT_BW320_MCS13_C1;
        public string WIFI_TX_POWER_F6425_EHT_BW320_MCS13_C0;
        public string WIFI_TX_POWER_F6425_EHT_BW320_MCS13_C1;
        public string WIFI_TX_POWER_F6745_EHT_BW320_MCS13_C0;
        public string WIFI_TX_POWER_F6745_EHT_BW320_MCS13_C1;
        public string WIFI_TX_POWER_F2412_HE_BW20_MCS0_C0;
        public string WIFI_TX_POWER_F2412_HE_BW20_MCS0_C1;
        public string WIFI_TX_POWER_F2437_HE_BW20_MCS0_C0;
        public string WIFI_TX_POWER_F2437_HE_BW20_MCS0_C1;
        public string WIFI_TX_POWER_F2472_HE_BW20_MCS0_C0;
        public string WIFI_TX_POWER_F2472_HE_BW20_MCS0_C1;
        public string WIFI_TX_POWER_F2472_HE_BW20_MCS7_C0;
        public string WIFI_TX_POWER_F2472_HE_BW20_MCS7_C1;
        public string WIFI_TX_POWER_F2462_HE_BW40_MCS11_C0;
        public string WIFI_TX_POWER_F2462_HE_BW40_MCS11_C1;



        public string SN;
        public string MAC;
    }

    public class Limits
    {
        public string timestamp;
        public string model;
        public string station_type;
        public string limits_validation;
        public List<Limit> limits = new List<Limit>();
    }

    public class Limit
    {
        public string last_updated_at;
        public string station_type;
        //public string required;
        public string limit_type;
        public string upper_limit;
        public string lower_limit;
        public string units;
        public string test_name;
        public string model;
        public string error_code;
        //public string id;

    }


}