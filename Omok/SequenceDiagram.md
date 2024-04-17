# 유저의 로그인 관련
```mermaid
sequenceDiagram
    actor Client
    participant HiveServer
    participant HiveMySQL
    participant HiveRedis
    participant GameServer
    participant GameMySQL
    participant GameRedis
    Client ->> HiveServer: 계정 로그인
    HiveServer ->> HiveMySQL: 계정 로그인 확인 요청
    HiveMySQL ->> HiveServer: 계정 로그인 확인 응답
    alt 로그인 정보 불일치
        HiveServer ->> Client: 로그인 실패 알림
    end

    HiveServer ->> HiveRedis: 인증 토큰 생성 후 Redis에 인증 토큰 보관
    HiveServer ->> Client: 인증 토큰 전달
    Client ->> GameServer: 전달 받은 토큰으로 APIServer에 로그인 요청
    GameServer ->> HiveServer: 토큰 검증 요청
    HiveServer ->> GameServer: 토큰 검증 응답
    alt 토큰 불일치
        GameServer ->> Client: 로그인 실패 알림
    end
    
    alt 첫 로그인
        GameServer ->> GameMySQL: UserGameData 추가
    end

    GameServer ->> GameRedis: 토큰 보관
    GameServer ->> Client: 로그인 성공
```
