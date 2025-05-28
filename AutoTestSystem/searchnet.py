import time
import pywifi
import subprocess
import sys  # Add this line to import the sys module



def list_interfaces():
    wifi = pywifi.PyWiFi()
    interfaces = wifi.interfaces()
    
    if interfaces:
        print(f"Found {len(interfaces)} wireless interfaces:")
        for i, iface in enumerate(interfaces):
            print(f"Interface {i}: {iface.name()}")
    else:
        print("No wireless interfaces found.")



def scan_networks():
    wifi = pywifi.PyWiFi()
    
    # Retry mechanism
    retries = 15
    for attempt in range(retries):
        interfaces = wifi.interfaces()
        #print("=======",interfaces)
        if interfaces:
            iface = interfaces[0]  # Get the first wireless interface
            #print("================",interfaces[0])
            iface.scan()  # Start scanning
            time.sleep(15)  # Wait for the scan to complete
            results = iface.scan_results()  # Get scan results

            networks = []
            for network in results:
                networks.append(network.ssid)
            if len(networks)==0:
                print("not find networkssid,retry")
                time.sleep(5)
                continue

            return networks
        else:
            print(f"No wireless interfaces found. Retrying in 5 seconds... (Attempt {attempt + 1}/{retries})")
            time.sleep(5)
    
    # Return an empty list if no interfaces are found after retries
    return []

def get_network_details(ssid):
    cmd = f'netsh wlan show networks mode=bssid | findstr /R /C:"^{ssid}" /C:"Channel"'
    result = subprocess.run(cmd, shell=True, stdout=subprocess.PIPE, stderr=subprocess.PIPE, text=True)
    return result.stdout

def check_for_ssid_with_two_bssids(res, target_ssid):
    lines = res.splitlines()
    found_ssid = False
    bssid_count = 0
    
    for line in lines:
        if "SSID" in line and target_ssid in line:
            found_ssid = True
            bssid_count = 0  # Reset BSSID count when a new SSID is found
        elif found_ssid and "BSSID" in line:
            bssid_count += 1
        elif found_ssid and "SSID" in line and target_ssid not in line:
            # We've reached the next SSID, stop checking
            break

    return found_ssid and bssid_count >= 2

def main(target_ssid):
    networks = scan_networks()
    print(networks)
    # If networks is empty, print False and return
    if not networks:
        print(False)
        return

    res = ""
    for ssid in networks:
        details = get_network_details(ssid)
        res += details
    
    print(res)
    # 进一步处理res并检查目标SSID
    result = check_for_ssid_with_two_bssids(res, target_ssid)
    
    # 打印 True 或 False
    print(result)

if __name__ == "__main__":
    if len(sys.argv) != 2:
        print("Usage: python script.py <SSID>")
    else:
        target_ssid = sys.argv[1]
        main(target_ssid)
