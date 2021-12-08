# API application

## Build and run

```bash
docker-compose up --build
```

## Simulated sensor types:

| Name       | ID  | Interval         |
|:---------- | --- |:---------------- |
| Termometer | 0   | -40 - 50 [°C]    |
| Barometer  | 1   | 960 - 1060 [hPa] |
| Hygrometer | 2   | 0%-100%          |
| Photometer | 3   | 20-11000 lumens  |

## Documentation
[MQTT client](https://github.com/chkr1011/MQTTnet/wiki/Client)
