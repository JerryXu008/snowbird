import asyncio
import sys
from bleak import BleakScanner

async def scan_for_device(target_mac):
    #for _ in range(20): 
    for _ in range(10):
        scanner = BleakScanner()
        devices = await scanner.discover()
        print(devices)
        for device in devices:
            if device.address.lower() == target_mac.lower():
                return True
        await asyncio.sleep(1)  
    return False

def main(mac_address):
    loop = asyncio.get_event_loop()
    found = loop.run_until_complete(scan_for_device(mac_address))
    print(found)

if __name__ == "__main__":
    if len(sys.argv) != 2:
        print("Usage: python scan_device.py <MAC_ADDRESS>")
    else:
        mac_address = sys.argv[1]
        main(mac_address)
