Brian Zhang:
可以用这个指令查询limit：
response = requests.post("http://{factory}:bento@{server IP}:{port}/api/1/limits", data={"station_type": “RTT”, “model”: “FIREFLY”})

Brian Zhang:
response.text()打印
