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

    Client ->> HiveServer: 계정 생성 요청 (CreateAccountController/create)
    HiveServer ->> HiveMySQL: 계정 조회
    alt 존재하지 않는 이메일
        HiveServer ->> HiveMySQL: 계정 생성
    end

    HiveServer ->> Client: 계정 생성 응답
    Client ->> HiveServer: 로그인 요청 (LoginController/login)
    HiveServer ->> HiveMySQL: 계정 조회
    alt 로그인 정보 불일치
        HiveServer ->> Client: 로그인 응답
    end

    HiveServer ->> HiveRedis: 토큰 생성 후 보관
    HiveServer ->> Client: 로그인 응답
    Client ->> GameServer: 로그인 요청 (LoginController/login)
    GameServer ->> HiveServer: 검증 요청
    HiveServer ->> GameServer: 검증 응답
    alt 검증 실패
        GameServer ->> Client: 로그인 실패 응답
    end

    GameServer ->> GameRedis: 토큰 보관
    GameServer ->> Client: 로그인 성공
```
