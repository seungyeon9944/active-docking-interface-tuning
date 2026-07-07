using UnityEngine;

public class VehicleController : MonoBehaviour 
{
    // 차량의 5단계 상태 정의
    public enum VehicleState { Idle, WakeUp, ActiveDocking, Charging, E_Stop }
    public VehicleState currentState = VehicleState.Idle;

    public float distanceToRobot;
    public bool isUwbSignalStable = true;
    public bool isObstacleDetected = false;
    
    // 로봇 측 상태를 확인하기 위한 참조
    public RobotController robot; 

    void Update() 
    {
        CheckFailsafe();
        ExecuteStateMachine();
    }

    void ExecuteStateMachine() 
    {
        switch (currentState) 
        {
            case VehicleState.Idle:
                // 5m 이내 접근 시 Low-Power 상태에서 WakeUp 상태로 전환
                if (distanceToRobot <= 5.0f && !isObstacleDetected) 
                {
                    currentState = VehicleState.WakeUp;
                    Debug.Log("차량: 로봇 접근 감지. UWB 및 보조 센서를 활성화합니다.");
                }
                break;

            case VehicleState.WakeUp:
                // 30cm 이내 초근접 시 제어권 인수 준비
                if (distanceToRobot <= 0.3f) 
                {
                    currentState = VehicleState.ActiveDocking;
                }
                break;

            case VehicleState.ActiveDocking:
                // 능동 결합: 로봇이 미는 것을 멈추고 차량이 서보 액추에이터로 당김
                EngageActuator();
                
                // 로봇이 도킹 완료 상태로 전환되었는지 확인 후 충전 시작
                if (robot.currentState == RobotController.RobotState.Docked) 
                {
                    currentState = VehicleState.Charging;
                }
                break;

            case VehicleState.Charging:
                Debug.Log("차량: 도킹 성공. 배터리 충전을 진행합니다.");
                break;

            case VehicleState.E_Stop:
                StopAllActions();
                break;
        }
    }

    void CheckFailsafe() 
    {
        // 센서 퓨전 로직: 장애물이 감지되거나 통신이 불안정하면 즉시 E-Stop
        if (isObstacleDetected || !isUwbSignalStable) 
        {
            if (currentState != VehicleState.E_Stop)
            {
                currentState = VehicleState.E_Stop;
            }
        }
    }

    void EngageActuator() 
    {
        Debug.Log("차량: 30cm 진입! 액추에이터 가동하여 로봇 커넥터를 강제로 인입합니다.");
    }

    void StopAllActions() 
    {
        Debug.Log("차량: [페일세이프 발동] 통신 단절/장애물 감지로 모든 포트를 폐쇄합니다.");
    }
}