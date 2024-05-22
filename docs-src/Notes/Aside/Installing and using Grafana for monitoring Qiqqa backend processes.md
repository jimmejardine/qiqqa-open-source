# Installing and using Grafana for monitoring Qiqqa backend processes

## Installing Grafana

How to install this animal on MS Windows:

- https://medium.com/@malvik18/how-to-install-grafana-on-windows-a-step-by-step-guide-380517a71f1d

Before we can use the data sources we want, we need to install a plugin or two. 
This is done most easily by opening Grafana in the browser via http://localhost:3000/connections/add-new-connection -- here we go to the *Home > Connections > Add new connection* page and install the **Infinity** plugin: that one supports all the formats we bother ourselves about. (Alternatively, you can install any of the JSON fetching plugins, but those less actively maintained and Infinity has it all, anyhow.)

Installing via the web page is easier as you don't have to manually restart the Grafana service, etc.

Next, you can add a URL as Data Source via *Add New Data Source* and picking Infinity from the list shown there.

Take it from there.... (you may want to restrict Infinity by configuring to only allow queries to host 'localhost', but YMMV.)



## References

- https://grafana.com/docs/grafana/latest/setup-grafana/configure-grafana/?pg=oss-graf&plcmt=hero-btn-2
- https://grafana.com/grafana/plugins/data-source-plugins/
- https://medium.com/@malvik18/how-to-install-grafana-on-windows-a-step-by-step-guide-380517a71f1d
- https://grafana.com/grafana/plugins/yesoreyeram-infinity-datasource/?tab=overview
- https://grafana.com/docs/plugins/yesoreyeram-infinity-datasource/latest/json/
- https://github.com/grafana/grafana-infinity-datasource
- https://grafana.com/docs/agent/latest/static/set-up/start-agent/
- https://grafana.com/docs/grafana/latest/administration/plugin-management/
- https://github.com/grafana/sqlds
- https://grafana.com/developers/plugin-tools/introduction/grafana-plugin-sdk-for-go
- https://github.com/grafana/grafana-plugin-examples/tree/main/examples/datasource-streaming-backend-websocket
- 