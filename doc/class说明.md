###GameObject  
管理游戏进程的类  

需要实现的方法:  
|--类型--|--名称--|---功能----|
|--------|-------|-----------|
|-private-|-GameLoop-|-游戏循环--|
|-private-|-WaitPlayer-|--等待玩家操作--|
|-private-|-WaitAI--|-等待AI采取行动---|
|-private-|-WaitFight--|-等待即时战斗--|


###MapManager  
管理游戏地图的类

已有的方法和接口:
|--类型--|--名称--|---功能----|
|--------|-------|-----------|
|-public-|-instance-|-该类型的实例-|
|-public-|-LogMapBlock-|-显示对应位置的地图块信息并返回块的地图位置-|
|-public-|-UpdateBlockState-|-更新地图块的可见度信息-|
|-public-|-ShowCameraRect-|-动态显示摄像机可以看到的地图块区域-|


###Map  
主要功能是从map数组中获取MapBlock  

###MapBlock  
地图块  

###Door
门  

接口:  
|--类型--|--名称--|---功能----|
|--------|-------|-----------|
|-public-|-Opened-|-表示门是否开启-|
|-public-|-Open-|-开门-|

###UIManager  
管理UI界面的显示以及存储一些UI模板  

###MainCharactor  
需要实现的接口:  
|--类型--|--名称--|---功能----|
|--------|-------|-----------|
|-public-|-BeginTurn-|-开始回合-|
|-public-|-TurnOver-|-表示回合是否结束-|
