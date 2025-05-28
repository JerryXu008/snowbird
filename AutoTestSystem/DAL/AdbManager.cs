using AutoTestSystem.BLL;
using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace AutoTestSystem.DAL
{
    public class AdbManager
    {
        public string packageName = "com.eero.android";

        public static ILog logger = Log4NetHelper.GetLogger(typeof(SetNetClickManager));

        public bool SetUpNetWork()
        {
            int retry = 3;
            // Package của ứng dụng

           RETRY_SETUPNETWORK:

            DeleteXmlFile();
            logger.Info("Open apk eero...");
            OpenApplication(packageName);

            System.Threading.Thread.Sleep(3000);

            DumpUiAutomator();

            // Click vào button "Start Setup" navigation_add
            logger.Info("Find and click 'Start Setup'...");
            if (!ClickElementById("com.eero.android:id/button_next"))
            {
                logger.Info("Not found 'com.eero.android:id/button_next'...");
                DeleteXmlFile();

                if (retry > 0)
                {
                    KillApp(packageName);
                    goto RETRY_SETUPNETWORK;
                }

                return false;
            }

            System.Threading.Thread.Sleep(500);
            DumpUiAutomator();
            logger.Info("Find and click 'Start'...");
            if (!ClickElementById("com.eero.android:id/next_button"))
            {
                logger.Info("Not found ID 'com.eero.android:id/next_button'.");
                DeleteXmlFile();

                if (retry > 0)
                {
                    KillApp(packageName);
                    goto RETRY_SETUPNETWORK;
                }

                return false;
            }
            //Add or replace eero devices
            System.Threading.Thread.Sleep(500);
            DumpUiAutomator();

            // Click vào button "Next"
            for(int i = 0; i < 3; i++)
            {
                logger.Info("Find button and click '->'...");
                if (!ClickElementById("com.eero.android:id/button_next"))
                {
                    logger.Info("Not found 'com.eero.android:id/button_next'");
                    DeleteXmlFile();

                    if (retry > 0)
                    {
                        KillApp(packageName);
                        goto RETRY_SETUPNETWORK;
                    }

                    return false;
                }
                System.Threading.Thread.Sleep(500);
            }

            DumpUiAutomator();

            logger.Info("Find and click 'Next'...");

            if (DumpUiAutomatorAndWaitForElement("resource-id", "com.eero.android:id/setup_confirmation_next_button", 30))
            {
                logger.Info("Button 'Next' found. Click it...");
                ClickElementById("com.eero.android:id/setup_confirmation_next_button");
                DeleteXmlFile();
            }
            else
            {
                logger.Info("not found 'Next' button, try to click Basement");
                DeleteXmlFile();
            }
            System.Threading.Thread.Sleep(500);
            DumpUiAutomator();

            if (DumpUiAutomatorAndWaitForElement("text", "Basement", 100))
            {
                logger.Info("Button 'Basement' found. Click it...");
                ClickElementByText("Basement");
                DeleteXmlFile();
            }
            else
            {
                logger.Info("not found 'Basement' button");
                DeleteXmlFile();
                if (retry > 0)
                {
                    KillApp(packageName);
                    goto RETRY_SETUPNETWORK;
                }
            }
            System.Threading.Thread.Sleep(500);
            DumpUiAutomator();

            if (!InputTextById("com.eero.android:id/network_name_edit_text","snowbird"))
            {
                logger.Info("Not found 'com.eero.android:id/network_name_edit_text'");
                DeleteXmlFile();
                if (retry > 0)
                {
                    KillApp(packageName);
                    goto RETRY_SETUPNETWORK;
                }
            }
            Thread.Sleep(100);
            if (!InputTextById("com.eero.android:id/network_password_edit_text", "12345678"))
            {
                logger.Info("Not found 'com.eero.android:id/network_password_edit_text'");
                DeleteXmlFile();
                if (retry > 0)
                {
                    KillApp(packageName);
                    goto RETRY_SETUPNETWORK;
                }
            }
            DumpUiAutomator();
            if (!ClickElementById("com.eero.android:id/button_next"))
            {
                logger.Info("Not found ID 'com.eero.android:id/button_next'.");
                DeleteXmlFile();
                if (retry > 0)
                {
                    KillApp(packageName);
                    goto RETRY_SETUPNETWORK;
                }
                return false;
            }

            System.Threading.Thread.Sleep(500);
            if (DumpUiAutomatorAndWaitForElement("text", "Finish setup", 50))
            {
                logger.Info("Button 'Finish setup' found. Click it...");
                ClickElementByText("Finish setup");
                DeleteXmlFile();
            }
            else
            {
                logger.Info("Not found button 'Finish setup' before waitting.");
                DeleteXmlFile();
                if (retry > 0)
                {
                    KillApp(packageName);
                    goto RETRY_SETUPNETWORK;
                }
            }
            System.Threading.Thread.Sleep(2000);
            
            DumpUiAutomator();

            if (!ClickElementById("com.eero.android:id/next_button"))
            {
                logger.Info("Not found ID 'com.eero.android:id/next_button'.");
                DeleteXmlFile();
                if (retry > 0)
                {
                    KillApp(packageName);
                    goto RETRY_SETUPNETWORK;
                }
                return false;
            }
            System.Threading.Thread.Sleep(2000);

            KillApp(packageName);

            return true;
        }

        public bool CheckConnectCable()
        {
            OpenApplication(packageName);
            // Chờ ứng dụng mở
            System.Threading.Thread.Sleep(5000);
            DumpUiAutomator();

            ////Click vào button "CANCEL"
            //logger.Info("Find and click 'CANCEL'...");
            //if (!ClickElementByText("CANCEL"))
            //{
            //    logger.Info("Not found buuton 'CANCEL'.");
            //    DeleteXmlFile();
                
            //}

            bool result = IsTextWithIconPresent("Bedroom", "com.eero.android:id/connection_icon");

            KillApp(packageName);

            return result;
        }

        public bool RemoveNetWork()
        {
            OpenApplication(packageName);
            // Chờ ứng dụng mở
            System.Threading.Thread.Sleep(2000);

            DumpUiAutomator();
            //Check connection 

            if (DumpUiAutomatorAndWaitForElement("text", "CANCEL", 3))
            {
                logger.Info("Button 'Next' found. Click it...");
                ClickElementById("CANCEL");
                DeleteXmlFile();
            }
            else
            {
                logger.Info("not found 'CANCEL' button, try to click Basement");
                DeleteXmlFile();
            }

            DumpUiAutomator();
            //Check connection /
            logger.Info("Find and click button 'Basement'...");
            if (!ClickElementByText("Basement"))
            {
                logger.Info("Not found button 'Basement'.");
                DeleteXmlFile();
                return false;
            }
            //Menu drop 
            System.Threading.Thread.Sleep(500);
            DumpUiAutomator();

            if (!ClickElementById("eero_device_detail_toolbar_action_button"))
            {
                logger.Info("Not found button 'eero_device_detail_toolbar_action_button'.");
                DeleteXmlFile();
                return false;
            } 
            //Remove Bedroom
            System.Threading.Thread.Sleep(500);
            DumpUiAutomator();
            if (!ClickElementByText("Remove Basement"))
            {
                logger.Info("Not found button 'Remove Basement'.");
                DeleteXmlFile();
                return false;
            }
            //Remove Bedroom eero
            System.Threading.Thread.Sleep(500);
            DumpUiAutomator();
            if (!ClickElementByText("Delete network"))
            {
                logger.Info("Not found button 'Delete network'.");
                DeleteXmlFile();
                return false;
            }

            //OK
            System.Threading.Thread.Sleep(500);
            DumpUiAutomator();
            if (!ClickElementByText("DELETE NETWORK"))
            {
                logger.Info("Not found 'DELETE NETWORK'.");
                DeleteXmlFile();
                return false;
            }

            System.Threading.Thread.Sleep(2000);
            KillApp(packageName);

            return true ;
        }

        private static bool InputTextById(string resourceId, string inputText)
        {
            if (!File.Exists("window_dump.xml"))
            {
                Console.WriteLine("File XML không tồn tại.");
                return false;
            }

            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load("window_dump.xml");

                XmlNodeList nodes = xmlDoc.GetElementsByTagName("node");
                foreach (XmlNode node in nodes)
                {
                    if (node.Attributes != null && node.Attributes["resource-id"]?.Value == resourceId)
                    {
                        string bounds = node.Attributes["bounds"]?.Value;
                        if (!string.IsNullOrEmpty(bounds))
                        {
                            // Lấy tọa độ trung tâm từ bounds
                            string[] coords = bounds.Replace("[", "").Replace("]", ",").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            int x1 = int.Parse(coords[0]);
                            int y1 = int.Parse(coords[1]);
                            int x2 = int.Parse(coords[2]);
                            int y2 = int.Parse(coords[3]);
                            int centerX = (x1 + x2) / 2;
                            int centerY = (y1 + y2) / 2;

                            // Click vào tọa độ trung tâm
                            Console.WriteLine($"Click vào tọa độ ({centerX}, {centerY}) cho resource-id '{resourceId}'.");
                            TapScreen(centerX, centerY);

                            // Chờ một chút để đảm bảo focus
                            System.Threading.Thread.Sleep(500);

                            // Gửi dữ liệu vào textbox
                            ExecuteAdbCommand($"shell input text \"{inputText.Replace(" ", "%s")}\"");
                            Console.WriteLine($"Đã nhập '{inputText}' vào textbox với resource-id '{resourceId}'.");
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi phân tích XML: {ex.Message}");
            }

            return false;
        }

        private static bool ClickElementById(string resourceId)
        {
            if (!File.Exists("window_dump.xml"))
            {
                Console.WriteLine("File XML not exist.");
                return false;
            }

            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load("window_dump.xml");

                XmlNodeList nodes = xmlDoc.GetElementsByTagName("node");
                foreach (XmlNode node in nodes)
                {
                    if (node.Attributes != null && node.Attributes["resource-id"]?.Value == resourceId)
                    {
                        string bounds = node.Attributes["bounds"]?.Value;
                        if (!string.IsNullOrEmpty(bounds))
                        {
                            // Lấy tọa độ trung tâm từ bounds
                            string[] coords = bounds.Replace("[", "").Replace("]", ",").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            int x1 = int.Parse(coords[0]);
                            int y1 = int.Parse(coords[1]);
                            int x2 = int.Parse(coords[2]);
                            int y2 = int.Parse(coords[3]);
                            int centerX = (x1 + x2) / 2;
                            int centerY = (y1 + y2) / 2;

                            // Click vào tọa độ
                            TapScreen(centerX, centerY);
                            Console.WriteLine($"Clicked 'button_next' at ({centerX}, {centerY}).");
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error XML: {ex.Message}");
            }

            return false;
        }

        private static void OpenApplication(string packageName)
        {
            DeleteXmlFile();
            ExecuteAdbCommand($"shell monkey -p {packageName} -c android.intent.category.LAUNCHER 1");
        }

        private static void DumpUiAutomator()
        {
            ExecuteAdbCommand("shell uiautomator dump /sdcard/window_dump.xml");
            ExecuteAdbCommand("pull /sdcard/window_dump.xml window_dump.xml");
        }

        private static bool ClickElementByText(string text)
        {
            if (!File.Exists("window_dump.xml"))
            {
                Console.WriteLine("File XML not exist.");
                return false;
            }

            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load("window_dump.xml");

                XmlNodeList nodes = xmlDoc.GetElementsByTagName("node");
                foreach (XmlNode node in nodes)
                {
                    if (node.Attributes != null && node.Attributes["text"]?.Value == text)
                    {
                        string bounds = node.Attributes["bounds"]?.Value;
                        if (!string.IsNullOrEmpty(bounds))
                        {
                            // Lấy tọa độ trung tâm từ bounds
                            string[] coords = bounds.Replace("[", "").Replace("]", ",").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            int x1 = int.Parse(coords[0]);
                            int y1 = int.Parse(coords[1]);
                            int x2 = int.Parse(coords[2]);
                            int y2 = int.Parse(coords[3]);
                            int centerX = (x1 + x2) / 2;
                            int centerY = (y1 + y2) / 2;

                            // Click vào tọa độ
                            TapScreen(centerX, centerY);
                            Console.WriteLine($"Clicked button '{text}' at ({centerX}, {centerY}).");
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error XML: {ex.Message}");
            }

            return false;
        }

        private static void TapScreen(int x, int y)
        {
            ExecuteAdbCommand($"shell input tap {x} {y}");
        }
        public void AbWakeUpPhone()
        {
            ExecuteAdbCommand("shell input keyevent KEYCODE_WAKEUP");
            Thread.Sleep(2000);
            ExecuteAdbCommand("shell input keyevent KEYCODE_MENU");
        }

        private static void ExecuteAdbCommand(string command)
        {
            try
            {
                Process process = new Process();
                process.StartInfo.FileName = "adb";
                process.StartInfo.Arguments = command;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (!string.IsNullOrEmpty(error))
                {
                    Console.WriteLine($"ADB Error: {error}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error when run ADB: {ex.Message}");
            }
        }

        private static void DeleteXmlFile()
        {
            try
            {
                if (File.Exists("window_dump.xml"))
                {
                    File.Delete("window_dump.xml");
                    Console.WriteLine("File XML deleted.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Delete file XML error: {ex.Message}");
            }
        }

        private static bool DumpUiAutomatorAndWaitForElement(string attribute, string value, int timeoutInSeconds = 10)
        {
            DateTime startTime = DateTime.Now;

            while ((DateTime.Now - startTime).TotalSeconds < timeoutInSeconds)
            {
                // Dump giao diện
                DumpUiAutomator();

                if (File.Exists("window_dump.xml"))
                {
                    try
                    {
                        // Phân tích file XML
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load("window_dump.xml");

                        XmlNodeList nodes = xmlDoc.GetElementsByTagName("node");
                        foreach (XmlNode node in nodes)
                        {
                            if (node.Attributes != null && node.Attributes[attribute]?.Value == value)
                            {
                                // Tìm thấy phần tử
                                Console.WriteLine($"Attribute {attribute}='{value}' found.");
                                return true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error XML: {ex.Message}");
                    }
                }

                // Chờ một chút trước khi dump lại
                System.Threading.Thread.Sleep(500);
            }

            Console.WriteLine($"Timeout when waitting ({timeoutInSeconds} s) not found {attribute}='{value}'.");
            return false;
        }

        private static void KillApp(string packageName)
        {
            try
            {
                Console.WriteLine($"Close app with package: {packageName}...");
                ExecuteAdbCommand($"shell am force-stop {packageName}");
                Console.WriteLine($"App {packageName} closed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Close app error : {ex.Message}");
            }
        }

        private static bool IsTextWithIconPresent(string targetText, string targetIconResourceId)
        {
            if (!File.Exists("window_dump.xml"))
            {
                Console.WriteLine("File XML not exist.");
                return false;
            }

            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load("window_dump.xml");

                // Duyệt qua tất cả các node trong XML
                XmlNodeList nodes = xmlDoc.GetElementsByTagName("node");
                foreach (XmlNode parentNode in nodes)
                {
                    bool hasText = false;
                    bool hasIcon = false;

                    // Kiểm tra tất cả các node con của parentNode
                    foreach (XmlNode childNode in parentNode.ChildNodes)
                    {
                        if (childNode.Attributes != null)
                        {
                            // Kiểm tra nếu có text là targetText
                            if (childNode.Attributes["text"]?.Value == targetText)
                            {
                                hasText = true;
                            }

                            // Kiểm tra nếu có resource-id là targetIconResourceId
                            if (childNode.Attributes["resource-id"]?.Value == targetIconResourceId)
                            {
                                hasIcon = true;
                            }
                        }

                        // Nếu cả text và icon được tìm thấy, trả về true
                        if (hasText && hasIcon)
                        {
                            logger.Info($"Found '{targetText}'with icon '{targetIconResourceId}'.");
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Info($"Error XML: {ex.Message}");
            }

            logger.Info($"Not found text '{targetText}' with icon '{targetIconResourceId}'.");
            return false;
        }


    }
}
