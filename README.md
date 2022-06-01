# AutomaticFeeding
板材自动上料系统

该系统功能目的是实现开料锯后上料自动化，在缓存站台空闲的时候向WMS请求要料，在推板机构空闲的时候下发推板任务和锯切图，当板件到达开料锯时将对应的锯切图下载到开料锯电脑供开料使用，
当推板机构推板数量缺少时允许人工下发补推任务

该系统包含服务端和客户端两部分

任务管理

操作人员自行选择想生产的批次，如果多个批次生产中，系统会根据批次的优先级进行生产

![image](https://user-images.githubusercontent.com/25633298/171373018-78b5d722-6584-459c-9747-034cfa2a4766.png)

批次生产明细

显示这个批次总共需要多少板件、仍需要多少板件，仍需板件允许更改，因为推板机构有时候会出现误差，或者其他原因导致数量对不上

![image](https://user-images.githubusercontent.com/25633298/171373670-199fe641-9526-4257-bf22-9e4a7e02a6d8.png)

批次垛明细

板件数量、上保护板允许修改，因为存在实际板垛数量、上保护板信息不符的情况

![微信截图_20220601173811](https://user-images.githubusercontent.com/25633298/171375095-f2c7fa95-cb65-4c7e-8089-afee4153e672.png)

6号锯任务管理

因为场地原因，中间有道低矮墙体导致线体无法连通，此锯使用频率较低，此锯生产模式与其他锯生产不同，其他锯是按张上板，此锯按垛上，因为此锯只能通过rgv小车上料，不能通过输送线

操作人员可任意选择一垛分配给该电子锯生产

![image](https://user-images.githubusercontent.com/25633298/171375166-0b064a29-f7fb-4b28-99cc-4f0171891a93.png)

SAW文件

此处可查询上层系统下发的任务完成情况，SAW文件是电子锯特有的文件，根据该文件电子锯就能开出特定的尺寸

状态、开料设备可以更改，因为推板机构有推板失败的情况，但是系统并不知道，此时需要人工更改SAW文件状态为未分配，系统会优先分配手动更改的SAW到指定的机台

![image](https://user-images.githubusercontent.com/25633298/171376088-9324329a-6d5e-4148-bec4-e59cce58799d.png)

设备监控

实时监控线体上的信息，当线体信息出现错误时通过此界面实时反映出来，plc数据有时会出现不准确的情况或者数据丢失，可以在此界面矫正plc数据

![image](https://user-images.githubusercontent.com/25633298/171377558-acb0ae93-5e0b-4122-95db-c5b6ed3b6b28.png)

手动要料/回库

手动向WMS发起要料或者料回库申请，可以指定送到某个站台或者从某个站台回库，有时出料会出现错误需要手动回库，料是从立库出来的

![image](https://user-images.githubusercontent.com/25633298/171378491-80b80970-f540-408f-87f4-faa12fd6e87f.png)

日志

用于记录系统执行的主要信息，主要用于跟踪排查问题，很多时候需要和plc对质，没有日志记录就没有证据

![微信截图_20220601175952](https://user-images.githubusercontent.com/25633298/171379253-3e1abbd8-b8dc-4dcd-bd30-04546679fccd.png)











