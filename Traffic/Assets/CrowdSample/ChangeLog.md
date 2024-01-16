# ChangeLog範本

所有顯著的更改都將記錄在此文件中。

格式基於[Keep a Changelog](https://keepachangelog.com/en/1.0.0/)
，並遵循[Semantic Versioning](https://semver.org/spec/v2.0.0.html)。

### 新增

- [AgentEntity] 新增 AgentEntity 漸停效果
- [AgentEntity] 新增 agentID, licensePlateNumber... 欄位
- [AgentIDReader] 新增 獲取車輛通行資格資訊的功能
- [CrowdAgentData] 新增 permissionID, angularSpeed, acceleration... 欄位
- [AgentAlarmReceiver] 新增 接收車輛臨停事件的告警功能
- [AgentEntityController] 新增 控制車輛行為的功能
- [UIAgentAlarmListHandler] 新增 顯示車輛車牌號碼的 UI 功能
- [TwoRoadsSample.scene] 新增一些 UI 與另一條路線

### 變更

- [AgentEntity] 修改成員變數使其不直接對外公開
- [TwoRoadsSample.scene] 修改場景重新烘焙 Navigation 的區域

### 修復

- [CrowdWizard] 修復 無法刪除 Instance 的問題
- [CrowdAgentFactory] 修復 重複生成 AgentEntity 的問題